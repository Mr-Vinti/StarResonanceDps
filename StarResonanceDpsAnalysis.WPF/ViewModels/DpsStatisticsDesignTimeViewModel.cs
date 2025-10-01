#if DEBUG
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Serilog.Events;
using StarResonanceDpsAnalysis.Core.Analyze.Models;
using StarResonanceDpsAnalysis.Core.Data;
using StarResonanceDpsAnalysis.Core.Data.Models;
using StarResonanceDpsAnalysis.WPF.Config;
using StarResonanceDpsAnalysis.WPF.Data;
using StarResonanceDpsAnalysis.WPF.Services;
using StarResonanceDpsAnalysis.WPF.Views;

namespace StarResonanceDpsAnalysis.WPF.ViewModels;

public sealed class DpsStatisticsDesignTimeViewModel : DpsStatisticsViewModel
{
    public DpsStatisticsDesignTimeViewModel() : base(
        new DesignAppControlService(),
        new DesignDataStorage(),
        NullLogger<DpsStatisticsViewModel>.Instance,
        new DesignConfigManager(),
        new DesignWindowManagementService(),
        new DebugFunctions(
            Dispatcher.CurrentDispatcher,
            NullLogger<DebugFunctions>.Instance,
            new DesignLogObservable(),
            new DesignOptionsMonitor()),
        Dispatcher.CurrentDispatcher)
    {
        // Populate with a few sample entries so designer shows something.
        try
        {
            for (var i = 0; i < 3; i++)
            {
                AddTestItemCommand.Execute(null);
            }
        }
        catch
        {
            /* swallow design-time exceptions */
        }
    }

    #region Stub Implementations

    private sealed class DesignAppControlService : IApplicationControlService
    {
        public void Shutdown()
        {
        }
    }

    private sealed class DesignWindowManagementService : IWindowManagementService
    {
        public DpsStatisticsView DpsStatisticsView => throw new NotSupportedException();
        public SettingsView SettingsView => throw new NotSupportedException();
        public SkillBreakdownView SkillBreakdownView => throw new NotSupportedException();
        public AboutView AboutView => throw new NotSupportedException();
        public DamageReferenceView DamageReferenceView => throw new NotSupportedException();
        public ModuleSolveView ModuleSolveView => throw new NotSupportedException();
    }

    private sealed class DesignConfigManager : IConfigManager
    {
        public event EventHandler<AppConfig>? ConfigurationUpdated;
        public AppConfig CurrentConfig { get; } = new();

        public Task SaveAsync(AppConfig newConfig)
        {
            ConfigurationUpdated?.Invoke(this, newConfig);
            return Task.CompletedTask;
        }
    }

    private sealed class DesignDataStorage : IDataStorage
    {
        public PlayerInfo CurrentPlayerInfo { get; } = new();

        public ReadOnlyDictionary<long, PlayerInfo> ReadOnlyPlayerInfoDatas { get; } =
            new(new Dictionary<long, PlayerInfo>());

        public ReadOnlyDictionary<long, DpsData> ReadOnlyFullDpsDatas => ReadOnlySectionedDpsDatas;
        public IReadOnlyList<DpsData> ReadOnlyFullDpsDataList { get; } = [];

        public ReadOnlyDictionary<long, DpsData> ReadOnlySectionedDpsDatas { get; } =
            new(new Dictionary<long, DpsData>());

        public IReadOnlyList<DpsData> ReadOnlySectionedDpsDataList { get; } = [];
        public TimeSpan SectionTimeout { get; set; } = TimeSpan.FromSeconds(5);
        public bool IsServerConnected => false;

#pragma warning disable CS0067
        public event DataStorage.ServerConnectionStateChangedEventHandler? ServerConnectionStateChanged;
        public event DataStorage.PlayerInfoUpdatedEventHandler? PlayerInfoUpdated;
        public event DataStorage.NewSectionCreatedEventHandler? NewSectionCreated;
        public event DataStorage.BattleLogCreatedEventHandler? BattleLogCreated;
        public event DataStorage.DpsDataUpdatedEventHandler? DpsDataUpdated;
        public event DataStorage.DataUpdatedEventHandler? DataUpdated;
        public event DataStorage.ServerChangedEventHandler? ServerChanged;
#pragma warning restore

        public void LoadPlayerInfoFromFile()
        {
        }

        public void SavePlayerInfoToFile()
        {
        }

        public Dictionary<long, PlayerInfoFileData> BuildPlayerDicFromBattleLog(List<BattleLog> battleLogs)
        {
            return new Dictionary<long, PlayerInfoFileData>();
        }

        public void ClearAllDpsData()
        {
        }

        public void ClearDpsData()
        {
        }

        public void ClearCurrentPlayerInfo()
        {
        }

        public void ClearPlayerInfos()
        {
        }

        public void ClearAllPlayerInfos()
        {
        }

        public void Dispose()
        {
        }
    }

    private sealed class DesignLogObservable : IObservable<LogEvent>
    {
        public IDisposable Subscribe(IObserver<LogEvent> observer)
        {
            return new DummyDisp();
        }

        private sealed class DummyDisp : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }

    private sealed class DesignOptionsMonitor : IOptionsMonitor<AppConfig>
    {
        public AppConfig CurrentValue { get; } = new() { DebugEnabled = true };

        public AppConfig Get(string? name)
        {
            return CurrentValue;
        }

        public IDisposable OnChange(Action<AppConfig, string?> listener)
        {
            listener(CurrentValue, null);
            return new DummyDisp();
        }

        private sealed class DummyDisp : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }

    #endregion
}
#endif

