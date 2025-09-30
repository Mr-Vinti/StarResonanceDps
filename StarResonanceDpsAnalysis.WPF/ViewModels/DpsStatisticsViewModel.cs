using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using StarResonanceDpsAnalysis.Core.Analyze.Exceptions;
using StarResonanceDpsAnalysis.Core.Data;
using StarResonanceDpsAnalysis.Core.Data.Models;
using StarResonanceDpsAnalysis.Core.Extends.Data;
using StarResonanceDpsAnalysis.Core.Models;
using StarResonanceDpsAnalysis.WPF.Config;
using StarResonanceDpsAnalysis.WPF.Converters;
using StarResonanceDpsAnalysis.WPF.Data;
using StarResonanceDpsAnalysis.WPF.Extensions;
using StarResonanceDpsAnalysis.WPF.Models;
using StarResonanceDpsAnalysis.WPF.Services;

namespace StarResonanceDpsAnalysis.WPF.ViewModels;

public partial class DpsStatisticsOptions : BaseViewModel
{
    [ObservableProperty] private int _minimalDurationInSeconds;

    [RelayCommand]
    private void SetMinimalDuration(int duration)
    {
        MinimalDurationInSeconds = duration;
    }
}

public partial class DpsStatisticsViewModel : BaseViewModel, IDisposable
{
    private readonly IApplicationControlService _appControlService;
    private readonly Stopwatch _battleTimer = new();
    private readonly IConfigManager _configManager;
    private readonly Dispatcher _dispatcher;

    private readonly Stopwatch _fullBattleTimer = new();
    private readonly ILogger<DpsStatisticsViewModel> _logger;
    private readonly Random _rd = new();
    private readonly Dictionary<long, StatisticDataViewModel> _slotsDictionary = new();
    private readonly IDataStorage _storage;
    private readonly IWindowManagementService _windowManagement;
    private DispatcherTimer? _durationTimer;
    private bool _isInitialized;

    [ObservableProperty] private DateTime _battleDuration;
    [ObservableProperty] private NumberDisplayMode _numberDisplayMode = NumberDisplayMode.Wan;
    [ObservableProperty] private ScopeTime _scopeTime = ScopeTime.Current;
    [ObservableProperty] private StatisticDataViewModel? _selectedSlot;
    [ObservableProperty] private bool _showContextMenu;
    [ObservableProperty] private BulkObservableCollection<StatisticDataViewModel> _slots = new();
    [ObservableProperty] private SortDirectionEnum _sortDirection = SortDirectionEnum.Descending;
    [ObservableProperty] private string _sortMemberPath = "Value";
    [ObservableProperty] private StatisticType _statisticIndex;
    [ObservableProperty] private StatisticDataViewModel? _currentPlayerSlot;

    [ObservableProperty] private AppConfig _appConfig = null!;

    /// <inheritdoc/>
    public DpsStatisticsViewModel(IApplicationControlService appControlService,
        IDataStorage storage,
        ILogger<DpsStatisticsViewModel> logger,
        IConfigManager configManager,
        IWindowManagementService windowManagement,
        DebugFunctions debugFunctions,
        Dispatcher dispatcher)
    {
        DebugFunctions = debugFunctions;
        _appControlService = appControlService;
        _storage = storage;
        _logger = logger;
        _configManager = configManager;
        _windowManagement = windowManagement;
        _dispatcher = dispatcher;
        _slots.CollectionChanged += SlotsChanged;

        // Subscribe to DebugFunctions events to handle sample data requests
        DebugFunctions.SampleDataRequested += OnSampleDataRequested;
        _storage.PlayerInfoUpdated += StorageOnPlayerInfoUpdated;

        AppConfig = _configManager.CurrentConfig;
        _configManager.ConfigurationUpdated += ConfigManagerOnConfigurationUpdated;

        return;

        void SlotsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Debug.Assert(e.NewItems != null, "e.NewItems != null");
                    LocalIterate(e.NewItems, item => _slotsDictionary.Add(item.Player.Uid, item));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Debug.Assert(e.OldItems != null, "e.OldItems != null");
                    LocalIterate(e.OldItems, itm => _slotsDictionary.Remove(itm.Player.Uid));
                    LocalIterate(e.OldItems, itm =>
                    {
                        if (ReferenceEquals(CurrentPlayerSlot, itm))
                        {
                            CurrentPlayerSlot = null;
                        }
                    });
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug.Assert(e.OldItems != null, "e.OldItems != null");
                    LocalIterate(e.OldItems, item => _slotsDictionary[item.Player.Uid] = item);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _slotsDictionary.Clear();
                    CurrentPlayerSlot = null;
                    break;
                case NotifyCollectionChangedAction.Move:
                    // just ignore
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return;

            void LocalIterate(IList list, Action<StatisticDataViewModel> action)
            {
                foreach (StatisticDataViewModel item in list)
                {
                    action.Invoke(item);
                }
            }
        }
    }

    public DebugFunctions DebugFunctions { get; }

    public DpsStatisticsOptions Options { get; } = new();
    private Stopwatch InUsingTimer => ScopeTime == ScopeTime.Total ? _fullBattleTimer : _battleTimer;

    public void Dispose()
    {
        // Unsubscribe from DebugFunctions events
        DebugFunctions.SampleDataRequested -= OnSampleDataRequested;
        _configManager.ConfigurationUpdated -= ConfigManagerOnConfigurationUpdated;

        if (_durationTimer != null)
        {
            _durationTimer.Stop();
            _durationTimer.Tick -= DurationTimerOnTick;
        }

        _storage.DpsDataUpdated -= DataStorage_DpsDataUpdated;
        _storage.NewSectionCreated -= StorageOnNewSectionCreated;
        _storage.PlayerInfoUpdated -= StorageOnPlayerInfoUpdated;
        _storage.Dispose();

        _isInitialized = false;
    }

    private void ConfigManagerOnConfigurationUpdated(object? sender, AppConfig newConfig)
    {
        if (_dispatcher.CheckAccess())
        {
            AppConfig = newConfig;
        }
        else
        {
            _dispatcher.Invoke(() => AppConfig = newConfig);
        }
    }

    private void OnSampleDataRequested(object? sender, EventArgs e)
    {
        // Handle the event from DebugFunctions
        AddRandomData();
    }

    /// <summary>
    /// 读取用户缓存
    /// </summary>
    private void LoadPlayerCache()
    {
        try
        {
            _storage.LoadPlayerInfoFromFile();
        }
        catch (FileNotFoundException)
        {
            // 没有缓存
        }
        catch (DataTamperedException)
        {
            _storage.ClearAllPlayerInfos();
            _storage.SavePlayerInfoToFile();
        }
    }

    [RelayCommand]
    private void OnLoaded()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        _logger.LogDebug("VM Loaded");
        LoadPlayerCache();

        EnsureDurationTimerStarted();
        UpdateBattleDuration();

        // 开始监听DPS更新事件
        _storage.DpsDataUpdated += DataStorage_DpsDataUpdated;
        _storage.NewSectionCreated += StorageOnNewSectionCreated;
    }


    private void DataStorage_DpsDataUpdated()
    {
        if (!_fullBattleTimer.IsRunning)
        {
            _fullBattleTimer.Restart();
        }

        if (!_battleTimer.IsRunning)
        {
            _battleTimer.Restart();
        }

        var dpsList = ScopeTime == ScopeTime.Total
            ? DataStorage.ReadOnlyFullDpsDataList
            : DataStorage.ReadOnlySectionedDpsDataList;
        UpdateData(dpsList);
    }


    private void UpdateData(IReadOnlyList<DpsData> data)
    {
        _logger.LogDebug("Enter updatedata");
        // 根据数据更新数据
        foreach (var dpsData in data)
        {
            var value = GetValue(dpsData, ScopeTime, StatisticIndex);
            PlayerInfo? playerInfo;
            if (!_slotsDictionary.TryGetValue(dpsData.UID, out var slot))
            {
                var ret = _storage.ReadOnlyPlayerInfoDatas.TryGetValue(dpsData.UID, out playerInfo);
                var @class = ret ? playerInfo!.ProfessionID?.GetClassNameById() ?? Classes.Unknown : Classes.Unknown;
                slot = new StatisticDataViewModel(DebugFunctions)
                {
                    Index = 999,
                    Value = ConvertToUnsigned(value),
                    Duration = ConvertToUnsigned(dpsData.LastLoggedTick - (dpsData.StartLoggedTick ?? 0)),
                    Player = new PlayerInfoViewModel
                    {
                        Uid = dpsData.UID,
                        Class = @class,
                        Guild = "Unknown",
                        Name = ret ? playerInfo?.Name ?? $"UID: {dpsData.UID}" : $"UID: {dpsData.UID}",
                        Spec = playerInfo?.Spec ?? ClassSpec.Unknown
                    },
                    GetSkillList = player =>
                    {
                        // Try to get real data first, fallback to test data
                        if (_storage.ReadOnlyFullDpsDatas.TryGetValue(player.Uid, out var fullDpsData))
                        {
                            return fullDpsData.ReadOnlySkillDataList.Select(item =>
                            {
                                return new SkillItemViewModel
                                {
                                    SkillName = item.SkillId.ToString(),
                                    AvgDamage = 1000,
                                    CritCount = item.CritTimes,
                                    HitCount = item.UseTimes,
                                    TotalDamage = item.TotalValue
                                };
                            }).ToList();
                        }

                        return [];
                    }
                };
                _dispatcher.Invoke(() =>
                {
                    Slots.Add(slot);
                });
            }

            // Simplified update of existing slot (replaces the selected block)
            var unsignedValue = ConvertToUnsigned(value);
            var durationTicks = dpsData.LastLoggedTick - (dpsData.StartLoggedTick ?? 0);
            var duration = ConvertToUnsigned(durationTicks);

            // use the out variable `slot` from TryGetValue above for in-place update
            slot.Value = unsignedValue;
            slot.Duration = duration;

            if (_storage.ReadOnlyPlayerInfoDatas.TryGetValue(dpsData.UID, out playerInfo))
            {
                slot.Player.Name = playerInfo.Name ?? $"UID: {dpsData.UID}";
                slot.Player.Class = playerInfo.ProfessionID?.GetClassNameById() ?? Classes.Unknown;
                slot.Player.Spec = playerInfo.Spec;
                slot.Player.Uid = playerInfo.UID;
            }
            else
            {
                slot.Player.Name = $"UID: {dpsData.UID}";
                slot.Player.Class = Classes.Unknown;
                slot.Player.Spec = ClassSpec.Unknown;
                slot.Player.Uid = dpsData.UID;
            }

            // Keep the user's own slot selected for quick reference
            if (_storage.CurrentPlayerInfo.UID != 0 && dpsData.UID == _storage.CurrentPlayerInfo.UID)
            {
                SelectedSlot = slot;
                CurrentPlayerSlot = slot;
            }
        }

        // Calculate percentage of max
        double totalValue = 0;

        if (Slots.Count > 0)
        {
            var maxValue = Slots.Max(d => d.Value);
            foreach (var slot in Slots)
            {
                slot.PercentOfMax = maxValue > 0 ? slot.Value / (double)maxValue * 100 : 0;
            }

            totalValue = Slots.Sum(d => Convert.ToDouble(d.Value));
            foreach (var slot in Slots)
            {
                slot.Percent = totalValue > 0 ? slot.Value / totalValue : 0;
            }
        }

        // Sort data in place 
        SortSlotsInPlace();

        UpdateBattleDuration();

        _logger.LogDebug("Exit updatedata");

        return;

        long GetValue(DpsData dpsData, ScopeTime scopeTime, StatisticType statisticType)
        {
            // Debug.Assert(dpsData.IsNpcData == (statisticType == StatisticType.NpcTakenDamage),
            // "dpsData.IsNpcData && statisticType == StatisticType.NpcTakenDamage"); // 保证是NPC承伤
            return (scopeTime, statisticType) switch
            {
                (ScopeTime.Current, StatisticType.Damage) => dpsData.TotalAttackDamage,
                (ScopeTime.Current, StatisticType.Healing) => dpsData.TotalHeal,
                (ScopeTime.Current, StatisticType.TakenDamage) => dpsData.TotalTakenDamage,
                (ScopeTime.Current, StatisticType.NpcTakenDamage) => dpsData.IsNpcData ? dpsData.TotalTakenDamage : 0,
                (ScopeTime.Total, StatisticType.Damage) => dpsData.TotalAttackDamage,
                (ScopeTime.Total, StatisticType.Healing) => dpsData.TotalHeal,
                (ScopeTime.Total, StatisticType.TakenDamage) => dpsData.TotalTakenDamage,
                (ScopeTime.Total, StatisticType.NpcTakenDamage) => dpsData.IsNpcData ? dpsData.TotalTakenDamage : 0,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    /// <summary>
    /// 获取每个统计类别的默认筛选器
    /// </summary>
    /// <param name="list"></param>
    /// <param name="type"></param>
    /// <param name="scopeTime"></param>
    /// <returns></returns>
    private static IEnumerable<DpsData> GetDefaultFilter(IEnumerable<DpsData> list, StatisticType type,
        ScopeTime scopeTime)
    {
        return (scopeTime, type) switch
        {
            (ScopeTime.Total, StatisticType.Damage) => list.Where(e => !e.IsNpcData && e.TotalAttackDamage != 0),
            (ScopeTime.Total, StatisticType.Healing) => list.Where(e => !e.IsNpcData && e.TotalHeal != 0),
            (ScopeTime.Total, StatisticType.TakenDamage) => list.Where(e => !e.IsNpcData && e.TotalTakenDamage != 0),
            (ScopeTime.Total, StatisticType.NpcTakenDamage) => list.Where(e => e.IsNpcData && e.TotalTakenDamage != 0),
            _ => list
        };
    }

    private static (long max, long sum) GetMaxSumValueByType(IEnumerable<DpsData> list, StatisticType type,
        ScopeTime scopeTime)
    {
        return type switch
        {
            StatisticType.Damage => (list.Max(e => e.TotalAttackDamage), list.Sum(e => e.TotalAttackDamage)),
            StatisticType.Healing => (list.Max(e => e.TotalHeal), list.Sum(e => e.TotalHeal)),
            StatisticType.TakenDamage or StatisticType.NpcTakenDamage => (list.Max(e => e.TotalTakenDamage),
                list.Sum(e => e.TotalTakenDamage)),
            _ => (long.MaxValue, long.MaxValue)
        };
    }

    private static long GetValueByType(DpsData data, StatisticType type)
    {
        return type switch
        {
            StatisticType.Damage => data.TotalAttackDamage,
            StatisticType.Healing => data.TotalHeal,
            StatisticType.TakenDamage or StatisticType.NpcTakenDamage => data.TotalTakenDamage,
            _ => long.MaxValue
        };
    }


    [RelayCommand]
    public void AddRandomData()
    {
        UpdateData();
    }

    // Test command to manually add an item
    [RelayCommand]
    public void AddTestItem()
    {
        var newItem = new StatisticDataViewModel(DebugFunctions)
        {
            Index = Slots.Count + 1,
            Value = (ulong)_rd.Next(100, 2000),
            Duration = 60000,
            Player = new PlayerInfoViewModel
            {
                Uid = _rd.Next(100, 999),
                Class = Classes.Marksman,
                Guild = "Test Guild",
                Name = $"Test Player {Slots.Count + 1}",
                Spec = ClassSpec.Unknown
            },
            // Add test skill data
            GetSkillList = player => new List<SkillItemViewModel>
            {
                new()
                {
                    SkillName = "Test Skill A", TotalDamage = 15000, HitCount = 25, CritCount = 8, AvgDamage = 600
                },
                new() { SkillName = "Test Skill B", TotalDamage = 8500, HitCount = 15, CritCount = 4, AvgDamage = 567 },
                new()
                {
                    SkillName = "Test Skill C", TotalDamage = 12300, HitCount = 30, CritCount = 12, AvgDamage = 410
                }
            }
        };

        // Calculate percentages
        if (Slots.Count > 0)
        {
            var maxValue = Math.Max(Slots.Max(d => d.Value), newItem.Value);
            var totalValue = Slots.Sum(d => Convert.ToDouble(d.Value)) + newItem.Value;

            // Update all existing items
            foreach (var slot in Slots)
            {
                slot.PercentOfMax = maxValue > 0 ? slot.Value / (double)maxValue * 100 : 0;
                slot.Percent = totalValue > 0 ? slot.Value / totalValue : 0;
            }

            // Set new item percentages
            newItem.PercentOfMax = maxValue > 0 ? newItem.Value / (double)maxValue * 100 : 0;
            newItem.Percent = totalValue > 0 ? newItem.Value / totalValue : 0;
        }
        else
        {
            newItem.PercentOfMax = 100;
            newItem.Percent = 1;
        }

        Slots.Add(newItem);
        SortSlotsInPlace();
    }

    [RelayCommand]
    private void NextMetricType()
    {
        StatisticIndex = StatisticIndex.Next();
    }

    [RelayCommand]
    private void PreviousMetricType()
    {
        StatisticIndex = StatisticIndex.Previous();
    }

    [RelayCommand]
    private void ToggleScopeTime()
    {
        ScopeTime = ScopeTime.Next();
    }

    protected void UpdateData()
    {
        DataStorage_DpsDataUpdated();
    }

    /// <summary>
    /// Updates the Index property of items to reflect their current position in the collection
    /// </summary>
    private void UpdateItemIndices()
    {
        for (var i = 0; i < Slots.Count; i++)
        {
            Slots[i].Index = i + 1; // 1-based index
        }
    }

    [RelayCommand]
    private void Refresh()
    {
        _logger.LogDebug("Manual refresh requested");

        // Reload cached player details so that recent changes in the on-disk
        // cache are reflected in the UI.
        LoadPlayerCache();

        try
        {
            DataStorage_DpsDataUpdated();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh DPS statistics");
        }
    }


    [RelayCommand]
    private void OpenContextMenu()
    {
        ShowContextMenu = true;
    }

    [RelayCommand]
    private void OpenSettings()
    {
        _windowManagement.SettingsView.Show();
    }

    [RelayCommand]
    private void Shutdown()
    {
        _appControlService.Shutdown();
    }

    #region Sort

    /// <summary>
    /// Changes the sort member path and re-sorts the data
    /// </summary>
    [RelayCommand]
    private void SetSortMemberPath(string memberPath)
    {
        if (SortMemberPath == memberPath)
        {
            // Toggle sort direction if the same property is clicked
            SortDirection = SortDirection == SortDirectionEnum.Ascending
                ? SortDirectionEnum.Descending
                : SortDirectionEnum.Ascending;
        }
        else
        {
            SortMemberPath = memberPath;
            SortDirection = SortDirectionEnum.Descending; // Default to descending for new properties
        }

        // Trigger immediate re-sort
        SortSlotsInPlace();
    }

    /// <summary>
    /// Manually triggers a sort operation
    /// </summary>
    [RelayCommand]
    private void ManualSort()
    {
        SortSlotsInPlace();
    }

    /// <summary>
    /// Sorts by Value in descending order (highest DPS first)
    /// </summary>
    [RelayCommand]
    private void SortByValue()
    {
        SetSortMemberPath("Value");
    }

    /// <summary>
    /// Sorts by Name in ascending order
    /// </summary>
    [RelayCommand]
    private void SortByName()
    {
        SortMemberPath = "Name";
        SortDirection = SortDirectionEnum.Ascending;
        SortSlotsInPlace();
    }

    /// <summary>
    /// Sorts by Classes
    /// </summary>
    [RelayCommand]
    private void SortByClass()
    {
        SetSortMemberPath("Classes");
    }

    /// <summary>
    /// Sorts the slots collection in-place based on the current sort criteria
    /// </summary>
    private void SortSlotsInPlace()
    {
        if (Slots.Count == 0 || string.IsNullOrWhiteSpace(SortMemberPath))
            return;

        try
        {
            // Sort the collection based on the current criteria
            _dispatcher.Invoke(() =>
            {
                switch (SortMemberPath)
                {
                    case "Value":
                        Slots.SortBy(x => x.Value, SortDirection == SortDirectionEnum.Descending);
                        break;
                    case "Name":
                        Slots.SortBy(x => x.Player.Name, SortDirection == SortDirectionEnum.Descending);
                        break;
                    case "Classes":
                        Slots.SortBy(x => (int)x.Player.Class, SortDirection == SortDirectionEnum.Descending);
                        break;
                    case "PercentOfMax":
                        Slots.SortBy(x => x.PercentOfMax, SortDirection == SortDirectionEnum.Descending);
                        break;
                    case "Percent":
                        Slots.SortBy(x => x.Percent, SortDirection == SortDirectionEnum.Descending);
                        break;
                }
            });
            // Update the Index property to reflect the new order (1-based index)
            UpdateItemIndices();
        }
        catch (Exception ex)
        {
            _logger.LogDebug($"Error during sorting: {ex.Message}");
        }
    }

    #endregion

    private void EnsureDurationTimerStarted()
    {
        if (_durationTimer != null) return;

        _durationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _durationTimer.Tick += DurationTimerOnTick;
        _durationTimer.Start();
    }

    private void DurationTimerOnTick(object? sender, EventArgs e)
    {
        UpdateBattleDuration();
    }

    private void UpdateBattleDuration()
    {
        if (!_dispatcher.CheckAccess())
        {
            _dispatcher.BeginInvoke(UpdateBattleDuration);
            return;
        }

        var elapsed = InUsingTimer.Elapsed;
        var displayTime = DateTime.Today.Add(elapsed);

        if (BattleDuration != displayTime)
        {
            BattleDuration = displayTime;
        }
    }

    private void StorageOnNewSectionCreated()
    {
        _dispatcher.BeginInvoke(() =>
        {
            _battleTimer.Reset();
            UpdateBattleDuration();
        });
    }

    private void StorageOnPlayerInfoUpdated(PlayerInfo info)
    {
        if (info == null)
        {
            return;
        }

        if (!_slotsDictionary.TryGetValue(info.UID, out var slot))
        {
            return;
        }

        _dispatcher.BeginInvoke(() =>
        {
            slot.Player.Name = info.Name ?? slot.Player.Name;
            slot.Player.Class = info.ProfessionID?.GetClassNameById() ?? slot.Player.Class;
            slot.Player.Spec = info.Spec;
            slot.Player.Uid = info.UID;

            if (_storage.CurrentPlayerInfo.UID == info.UID)
            {
                CurrentPlayerSlot = slot;
            }
        });
    }

    partial void OnScopeTimeChanged(ScopeTime value)
    {
        UpdateBattleDuration();
        UpdateData();
    }

    partial void OnStatisticIndexChanged(StatisticType value)
    {
        UpdateData();
    }

    private static ulong ConvertToUnsigned(long value)
    {
        return value <= 0 ? 0UL : (ulong)value;
    }
}



