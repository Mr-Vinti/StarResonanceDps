using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using StarResonanceDpsAnalysis.Core;
using StarResonanceDpsAnalysis.Core.Data.Models;
using StarResonanceDpsAnalysis.Core.Extends.Data;
using StarResonanceDpsAnalysis.Core.Models;
using StarResonanceDpsAnalysis.WPF.Data;
using StarResonanceDpsAnalysis.WPF.Extensions;
using StarResonanceDpsAnalysis.WPF.Models;

namespace StarResonanceDpsAnalysis.WPF.ViewModels;

/// <summary>
/// Helper struct for pre-processed DPS data to avoid redundant calculations
/// Immutable by design for thread-safety and performance
/// </summary>
public readonly record struct DpsDataProcessed(
    DpsData OriginalData,
    ulong Value,
    ulong Duration,
    List<SkillItemViewModel> SkillList,
    string PlayerName,
    Classes PlayerClass,
    ClassSpec PlayerSpec,
    int PowerLevel);

public partial class DpsStatisticsSubViewModel : BaseViewModel
{
    private readonly DebugFunctions _debugFunctions;
    private readonly Dispatcher _dispatcher;
    private readonly ILogger<DpsStatisticsViewModel> _logger;
    private readonly IDataStorage _storage;
    private readonly StatisticType _type;
    [ObservableProperty] private StatisticDataViewModel? _currentPlayerSlot;
    [ObservableProperty] private BulkObservableCollection<StatisticDataViewModel> _data = new();
    [ObservableProperty] private ScopeTime _scopeTime;
    [ObservableProperty] private StatisticDataViewModel? _selectedSlot;
    [ObservableProperty] private int _skillDisplayLimit = 8;
    [ObservableProperty] private SortDirectionEnum _sortDirection = SortDirectionEnum.Descending;
    [ObservableProperty] private string _sortMemberPath = "Value";

    public DpsStatisticsSubViewModel(ILogger<DpsStatisticsViewModel> logger, Dispatcher dispatcher, StatisticType type,
        IDataStorage storage,
        DebugFunctions debugFunctions)
    {
        _logger = logger;
        _dispatcher = dispatcher;
        _type = type;
        _storage = storage;
        _debugFunctions = debugFunctions;
        _data.CollectionChanged += DataChanged;
        return;

        void DataChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Debug.Assert(e.NewItems != null, "e.NewItems != null");
                    LocalIterate(e.NewItems, item => DataDictionary[item.Player.Uid] = item);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Debug.Assert(e.OldItems != null, "e.OldItems != null");
                    LocalIterate(e.OldItems, itm => DataDictionary.Remove(itm.Player.Uid));
                    LocalIterate(e.OldItems, itm =>
                    {
                        if (ReferenceEquals(CurrentPlayerSlot, itm))
                        {
                            CurrentPlayerSlot = null;
                        }
                    });
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug.Assert(e.NewItems != null, "e.NewItems != null");
                    LocalIterate(e.NewItems, item => DataDictionary[item.Player.Uid] = item);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    //TODO: Fix reset handling
                    DataDictionary.Clear();
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

    public Dictionary<long, StatisticDataViewModel> DataDictionary { get; } = new();
    public bool Initialized { get; set; }

    /// <summary>
    /// Sorts the slots collection in-place based on the current sort criteria
    /// </summary>
    public void SortSlotsInPlace()
    {
        if (Data.Count == 0 || string.IsNullOrWhiteSpace(SortMemberPath))
            return;

        try
        {
            // Sort the collection based on the current criteria
            _dispatcher.Invoke(() =>
            {
                switch (SortMemberPath)
                {
                    case "Value":
                        Data.SortBy(x => x.Value, SortDirection == SortDirectionEnum.Descending);
                        break;
                    case "Name":
                        Data.SortBy(x => x.Player.Name, SortDirection == SortDirectionEnum.Descending);
                        break;
                    case "Classes":
                        Data.SortBy(x => (int)x.Player.Class, SortDirection == SortDirectionEnum.Descending);
                        break;
                    case "PercentOfMax":
                        Data.SortBy(x => x.PercentOfMax, SortDirection == SortDirectionEnum.Descending);
                        break;
                    case "Percent":
                        Data.SortBy(x => x.Percent, SortDirection == SortDirectionEnum.Descending);
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

    protected StatisticDataViewModel GetOrAddStatisticDataViewModel(DpsData dpsData)
    {
        PlayerInfo? playerInfo;
        if (!DataDictionary.TryGetValue(dpsData.UID, out var slot))
        {
            var ret = _storage.ReadOnlyPlayerInfoDatas.TryGetValue(dpsData.UID, out playerInfo);
            // Debug.Assert(playerInfo != null, nameof(playerInfo) + " != null");
            var @class = Classes.Unknown;
            if (ret)
            {
                Debug.Assert(playerInfo != null, nameof(playerInfo) + " != null");
                @class = playerInfo.Class;
            }

            slot = new StatisticDataViewModel(_debugFunctions)
            {
                Index = 999,
                Value = 0,
                Duration = (dpsData.LastLoggedTick - (dpsData.StartLoggedTick ?? 0)).ConvertToUnsigned(),
                Player = new PlayerInfoViewModel
                {
                    Uid = dpsData.UID,
                    Class = @class,
                    Guild = "Unknown",
                    Name = ret ? playerInfo?.Name ?? $"UID: {dpsData.UID}" : $"UID: {dpsData.UID}",
                    Spec = playerInfo?.Spec ?? ClassSpec.Unknown,
                    IsNpc = dpsData.IsNpcData
                },
                SkillList = BuildSkillListSnapshot(dpsData),
            };
            _dispatcher.Invoke(() => { Data.Add(slot); });
        }

        return slot;
    }

    public void UpdateData(IReadOnlyList<DpsData> data)
    {
        _logger.LogDebug("Enter updatedata");

        var currentPlayerUid = _storage.CurrentPlayerInfo.UID;
        var hasCurrentPlayer = currentPlayerUid != 0;

        // Update all slots with their data
        foreach (var dpsData in data)
        {
            var slot = GetOrAddStatisticDataViewModel(dpsData);
            var value = GetValueForType(dpsData);

            // Calculate duration once
            var duration = (dpsData.LastLoggedTick - (dpsData.StartLoggedTick ?? 0)).ConvertToUnsigned();

            // Update slot values
            slot.Value = value;
            slot.Duration = duration;
            slot.SkillList = BuildSkillListSnapshot(dpsData);

            // Update player info if available
            if (_storage.ReadOnlyPlayerInfoDatas.TryGetValue(dpsData.UID, out var playerInfo))
            {
                slot.Player.Name = playerInfo.Name ?? $"UID: {dpsData.UID}";
                slot.Player.Class = playerInfo.ProfessionID.GetClassNameById();
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

            // Set current player slot if this is the current player
            if (hasCurrentPlayer && dpsData.UID == currentPlayerUid)
            {
                SelectedSlot = slot;
                CurrentPlayerSlot = slot;
            }
        }

        // Batch calculate percentages
        if (Data.Count > 0)
        {
            var maxValue = Data.Max(d => d.Value);
            var totalValue = Data.Sum(d => Convert.ToDouble(d.Value));

            var hasMaxValue = maxValue > 0;
            var hasTotalValue = totalValue > 0;

            foreach (var slot in Data)
            {
                slot.PercentOfMax = hasMaxValue ? slot.Value / (double)maxValue * 100 : 0;
                slot.Percent = hasTotalValue ? slot.Value / totalValue : 0;
            }
        }

        // Sort data in place 
        SortSlotsInPlace();

        _logger.LogDebug("Exit updatedata");
    }

    /// <summary>
    /// Updates data with pre-computed values for efficient batch processing
    /// </summary>
    internal void UpdateDataOptimized(Dictionary<long, DpsDataProcessed> processedData, long currentPlayerUid)
    {
        var hasCurrentPlayer = currentPlayerUid != 0;

        // Update all slots with pre-processed data
        foreach (var (uid, processed) in processedData)
        {
            // Skip if this statistic type has no value
            if (processed.Value == 0)
                continue;

            var slot = GetOrAddStatisticDataViewModel(processed.OriginalData);

            // Update slot values with pre-computed data
            slot.Value = processed.Value;
            slot.Duration = processed.Duration;
            slot.SkillList = processed.SkillList;

            // Update player info
            slot.Player.Name = processed.PlayerName;
            slot.Player.Class = processed.PlayerClass;
            slot.Player.Spec = processed.PlayerSpec;
            slot.Player.Uid = uid;
            slot.Player.PowerLevel = processed.PowerLevel;

            // Set current player slot if this is the current player
            if (hasCurrentPlayer && uid == currentPlayerUid)
            {
                SelectedSlot = slot;
                CurrentPlayerSlot = slot;
            }
        }

        // Batch calculate percentages
        if (Data.Count > 0)
        {
            var maxValue = Data.Max(d => d.Value);
            var totalValue = Data.Sum(d => Convert.ToDouble(d.Value));

            var hasMaxValue = maxValue > 0;
            var hasTotalValue = totalValue > 0;

            foreach (var slot in Data)
            {
                slot.PercentOfMax = hasMaxValue ? slot.Value / (double)maxValue * 100 : 0;
                slot.Percent = hasTotalValue ? slot.Value / totalValue : 0;
            }
        }

        // Sort data in place 
        SortSlotsInPlace();
    }

    private ulong GetValueForType(DpsData dpsData)
    {
        return _type switch
        {
            StatisticType.Damage => dpsData.TotalAttackDamage.ConvertToUnsigned(),
            StatisticType.Healing => dpsData.TotalHeal.ConvertToUnsigned(),
            StatisticType.TakenDamage => dpsData.TotalTakenDamage.ConvertToUnsigned(),
            StatisticType.NpcTakenDamage => dpsData.IsNpcData ? dpsData.TotalTakenDamage.ConvertToUnsigned() : 0UL,
            _ => throw new ArgumentOutOfRangeException(nameof(_type), _type, "Invalid statistic type")
        };
    }

    /// <summary>
    /// Updates the Index property of items to reflect their current position in the collection
    /// </summary>
    private void UpdateItemIndices()
    {
        var data = Data;
        for (var i = 0; i < data.Count; i++)
        {
            data[i].Index = i + 1; // 1-based index
        }
    }

    private List<SkillItemViewModel> BuildSkillListSnapshot(DpsData dpsData)
    {
        var skills = dpsData.ReadOnlySkillDataList;
        if (skills.Count == 0)
        {
            return [];
        }

        var orderedSkills = skills
            .OrderByDescending(static s => s.TotalValue);

        var projected = orderedSkills.Select(skill =>
        {
            var average = skill.UseTimes > 0
                ? Math.Round(skill.TotalValue / (double)skill.UseTimes)
                : 0d;

            var avgDamage = average > int.MaxValue
                ? int.MaxValue
                : (int)average;

            var skillIdText = skill.SkillId.ToString();
            var skillName = EmbeddedSkillConfig.TryGet(skillIdText, out var definition)
                ? definition.Name
                : skillIdText;

            return new SkillItemViewModel
            {
                SkillName = skillName,
                TotalDamage = skill.TotalValue,
                HitCount = skill.UseTimes,
                CritCount = skill.CritTimes,
                AvgDamage = avgDamage
            };
        });

        return ApplySkillDisplayLimit(projected);
    }

    private List<SkillItemViewModel> ApplySkillDisplayLimit(IEnumerable<SkillItemViewModel> skills)
    {
        var limit = SkillDisplayLimit;
        return limit > 0
            ? skills.Take(limit).ToList()
            : skills.ToList();
    }

    partial void OnSkillDisplayLimitChanged(int value)
    {
        if (value < 0)
        {
            SkillDisplayLimit = 0;
            return;
        }

        if (!Initialized) return;
        RefreshSkillSnapshots();
    }

    private void RefreshSkillSnapshots()
    {
        var dpsList = ScopeTime == ScopeTime.Total
            ? _storage.ReadOnlyFullDpsDataList
            : _storage.ReadOnlySectionedDpsDataList;

        UpdateData(dpsList);
    }

    public void AddTestItem()
    {
        var slots = Data;
        var newItem = new StatisticDataViewModel(_debugFunctions)
        {
            Index = slots.Count + 1,
            Value = (ulong)Random.Shared.Next(100, 2000),
            Duration = 60000,
            Player = new PlayerInfoViewModel
            {
                Uid = Random.Shared.Next(100, 999),
                Class = Classes.Marksman,
                Guild = "Test Guild",
                Name = $"Test Player {slots.Count + 1}",
                Spec = ClassSpec.Unknown
            },
            SkillList = ApplySkillDisplayLimit(new[]
            {
                new SkillItemViewModel
                {
                    SkillName = "Test Skill A", TotalDamage = 15000, HitCount = 25, CritCount = 8, AvgDamage = 600
                },
                new SkillItemViewModel
                {
                    SkillName = "Test Skill B", TotalDamage = 8500, HitCount = 15, CritCount = 4, AvgDamage = 567
                },
                new SkillItemViewModel
                {
                    SkillName = "Test Skill C", TotalDamage = 12300, HitCount = 30, CritCount = 12, AvgDamage = 410
                }
            })
        };

        // Calculate percentages
        if (slots.Count > 0)
        {
            var maxValue = Math.Max(slots.Max(d => d.Value), newItem.Value);
            var totalValue = slots.Sum(d => Convert.ToDouble(d.Value)) + newItem.Value;

            // Update all existing items
            foreach (var slot in slots)
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

        slots.Add(newItem);
        SortSlotsInPlace();
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

    #endregion

    public void Reset()
    {
        // Clear items (will also clear DataDictionary via CollectionChanged Reset handler)
        Data.Clear();
        SelectedSlot = null;
        CurrentPlayerSlot = null;
    }
}
