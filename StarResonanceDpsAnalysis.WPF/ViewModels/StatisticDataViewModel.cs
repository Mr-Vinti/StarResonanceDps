using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StarResonanceDpsAnalysis.WPF.ViewModels;

[DebuggerDisplay("Name:{Player.Name};Value:{Value}")]
public partial class StatisticDataViewModel(DebugFunctions debug) : BaseViewModel, IComparable<StatisticDataViewModel>
{
    [ObservableProperty] private ulong _duration;
    [ObservableProperty] private long _index;
    [ObservableProperty] private double _percent;
    [ObservableProperty] private double _percentOfMax;
    [ObservableProperty] private PlayerInfoViewModel _player = new();

    [ObservableProperty] private ulong _value;

    // [ObservableProperty] private ObservableCollection<SkillItemViewModel> _skillList = new();
    public Func<PlayerInfoViewModel, List<SkillItemViewModel>>? GetSkillList { get; set; }
    public List<SkillItemViewModel> SkillList => GetSkillList?.Invoke(Player) ?? new List<SkillItemViewModel>();
    public DebugFunctions Debug { get; } = debug;

    public int CompareTo(StatisticDataViewModel? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        return Value.CompareTo(other.Value);
    }
}