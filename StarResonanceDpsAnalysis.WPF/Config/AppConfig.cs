using System.Drawing;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using StarResonanceDpsAnalysis.Core.Models;
using StarResonanceDpsAnalysis.WPF.Converters;
using StarResonanceDpsAnalysis.WPF.Models;
using KeyBinding = StarResonanceDpsAnalysis.WPF.Models.KeyBinding;

namespace StarResonanceDpsAnalysis.WPF.Config;

/// <summary>
/// Ӧ��������
/// ���������ù��������ܣ�֧��INI�ļ��־û������Ա��֪ͨ
/// </summary>
public partial class AppConfig : ObservableObject
{
    /// <summary>
    /// �ǳ�
    /// </summary>
    [ObservableProperty]
    private string _nickname = string.Empty;

    [ObservableProperty]
    private ModifierKeys _testModifier = ModifierKeys.None;

    /// <summary>
    /// ְҵ
    /// </summary>
    [ObservableProperty]
    private Classes _classes;

    /// <summary>
    /// �û�UID
    /// </summary>
    [ObservableProperty]
    private long _uid;

    /// <summary>
    /// DPS�˺�������ʾ
    /// </summary>
    [ObservableProperty]
    private NumberDisplayMode _damageDisplayType;

    /// <summary>
    /// ս����
    /// </summary>
    [ObservableProperty]
    private int _combatPower;

    /// <summary>
    /// ս����ʱ����ӳ٣��룩
    /// </summary>
    [ObservableProperty]
    private int _combatTimeClearDelay;

    /// <summary>
    /// �Ƿ��ͼ���ȫ�̼�¼
    /// </summary>
    [ObservableProperty]
    private int _clearLogAfterTeleport;

    /// <summary>
    /// ��͸���ȣ�0-100��, Ĭ��100, 0Ϊȫ͸��
    /// </summary>
    [ObservableProperty]
    private double _opacity;

    /// <summary>
    /// �Ƿ�ʹ��ǳɫģʽ
    /// </summary>
    [ObservableProperty]
    private string _theme = "Light";

    /// <summary>
    /// ����ʱ�Ĵ���״̬
    /// </summary>
    [ObservableProperty]
    private Rectangle? _startUpState;

    /// <summary>
    /// ��ѡ����������
    /// </summary>
    [ObservableProperty]
    private NetworkAdapterInfo? _preferredNetworkAdapter;

    /// <summary>
    /// ��괩͸��ݼ�����
    /// </summary>
    [ObservableProperty]
    private KeyBinding _mouseThroughShortcut = new(Key.F6, ModifierKeys.None);

    /// <summary>
    /// ������ݿ�ݼ�����
    /// </summary>
    [ObservableProperty]
    private KeyBinding _clearDataShortcut = new(Key.F9, ModifierKeys.None);

    [ObservableProperty]
    private bool _debugEnabled = false;

    public AppConfig Clone()
    {
        // TODO: Add unittest
        var json = JsonConvert.SerializeObject(this);
        return JsonConvert.DeserializeObject<AppConfig>(json)!;
    }
}