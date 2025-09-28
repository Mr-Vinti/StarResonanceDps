namespace StarResonanceDpsAnalysis.WinForm.Forms
{
    partial class DpsStatisticsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DpsStatisticsForm));
            pageHeader_MainHeader = new AntdUI.PageHeader();
            button_ThemeSwitch = new AntdUI.Button();
            button_Minimum = new AntdUI.Button();
            button_SwitchStatisticsMode = new AntdUI.Button();
            button_AlwaysOnTop = new AntdUI.Button();
            button_RefreshDps = new AntdUI.Button();
            button_Settings = new AntdUI.Button();
            panel_Footer = new AntdUI.Panel();
            label_CurrentDps = new AntdUI.Label();
            label_BattleTimeText = new AntdUI.Label();
            label_CurrentOrder = new AntdUI.Label();
            panel_ModeBox = new AntdUI.Panel();
            button_NpcTakeDamage = new AntdUI.Button();
            button_AlwaysInjured = new AntdUI.Button();
            button_TotalTreatment = new AntdUI.Button();
            button_TotalDamage = new AntdUI.Button();
            sortedProgressBarList_MainList = new StarResonanceDpsAnalysis.WinForm.Control.SortedProgressBarList();
            timer_BattleTimeLabelUpdater = new System.Windows.Forms.Timer(components);
            pageHeader_MainHeader.SuspendLayout();
            panel_Footer.SuspendLayout();
            panel_ModeBox.SuspendLayout();
            SuspendLayout();
            // 
            // pageHeader_MainHeader
            // 
            pageHeader_MainHeader.BackColor = Color.FromArgb(178, 178, 178);
            pageHeader_MainHeader.ColorScheme = AntdUI.TAMode.Dark;
            pageHeader_MainHeader.Controls.Add(button_ThemeSwitch);
            pageHeader_MainHeader.Controls.Add(button_Minimum);
            pageHeader_MainHeader.Controls.Add(button_SwitchStatisticsMode);
            pageHeader_MainHeader.Controls.Add(button_AlwaysOnTop);
            pageHeader_MainHeader.Controls.Add(button_RefreshDps);
            pageHeader_MainHeader.Controls.Add(button_Settings);
            pageHeader_MainHeader.DividerShow = true;
            pageHeader_MainHeader.DividerThickness = 2F;
            pageHeader_MainHeader.Dock = DockStyle.Top;
            pageHeader_MainHeader.Font = new Font("SAO Welcome TT", 8.999999F, FontStyle.Bold);
            pageHeader_MainHeader.ForeColor = Color.White;
            pageHeader_MainHeader.Location = new Point(0, 0);
            pageHeader_MainHeader.Margin = new Padding(2);
            pageHeader_MainHeader.MaximizeBox = false;
            pageHeader_MainHeader.MinimizeBox = false;
            pageHeader_MainHeader.Mode = AntdUI.TAMode.Dark;
            pageHeader_MainHeader.Name = "pageHeader_MainHeader";
            pageHeader_MainHeader.Size = new Size(421, 20);
            pageHeader_MainHeader.SubFont = new Font("HarmonyOS Sans SC Medium", 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            pageHeader_MainHeader.SubGap = 0;
            pageHeader_MainHeader.SubText = "当前 · 伤害";
            pageHeader_MainHeader.TabIndex = 0;
            pageHeader_MainHeader.Text = "DPS Damage Statistics Table  ";
            // 
            // button_ThemeSwitch
            // 
            button_ThemeSwitch.ColorScheme = AntdUI.TAMode.Dark;
            button_ThemeSwitch.Dock = DockStyle.Right;
            button_ThemeSwitch.Ghost = true;
            button_ThemeSwitch.IconRatio = 0.8F;
            button_ThemeSwitch.IconSvg = "SunOutlined";
            button_ThemeSwitch.Location = new Point(323, 0);
            button_ThemeSwitch.Margin = new Padding(2);
            button_ThemeSwitch.Name = "button_ThemeSwitch";
            button_ThemeSwitch.Size = new Size(27, 20);
            button_ThemeSwitch.TabIndex = 1;
            button_ThemeSwitch.ToggleIconSvg = "MoonOutlined";
            button_ThemeSwitch.Click += button_ThemeSwitch_Click;
            // 
            // button_Minimum
            // 
            button_Minimum.ColorScheme = AntdUI.TAMode.Dark;
            button_Minimum.Dock = DockStyle.Right;
            button_Minimum.Ghost = true;
            button_Minimum.IconRatio = 0.8F;
            button_Minimum.IconSvg = resources.GetString("button_Minimum.IconSvg");
            button_Minimum.Location = new Point(350, 0);
            button_Minimum.Name = "button_Minimum";
            button_Minimum.Size = new Size(16, 20);
            button_Minimum.TabIndex = 2;
            button_Minimum.ToggleIconSvg = "";
            button_Minimum.Click += button_Minimum_Click;
            // 
            // button_SwitchStatisticsMode
            // 
            button_SwitchStatisticsMode.Dock = DockStyle.Left;
            button_SwitchStatisticsMode.Ghost = true;
            button_SwitchStatisticsMode.Icon = (Image)resources.GetObject("button_SwitchStatisticsMode.Icon");
            button_SwitchStatisticsMode.IconHover = (Image)resources.GetObject("button_SwitchStatisticsMode.IconHover");
            button_SwitchStatisticsMode.IconRatio = 0.8F;
            button_SwitchStatisticsMode.Location = new Point(184, 0);
            button_SwitchStatisticsMode.Name = "button_SwitchStatisticsMode";
            button_SwitchStatisticsMode.Size = new Size(19, 20);
            button_SwitchStatisticsMode.TabIndex = 0;
            button_SwitchStatisticsMode.Click += button_SwitchStatisticsMode_Click;
            button_SwitchStatisticsMode.MouseEnter += button_SwitchStatisticsMode_MouseEnter;
            // 
            // button_AlwaysOnTop
            // 
            button_AlwaysOnTop.ColorScheme = AntdUI.TAMode.Dark;
            button_AlwaysOnTop.Dock = DockStyle.Right;
            button_AlwaysOnTop.Ghost = true;
            button_AlwaysOnTop.IconRatio = 0.8F;
            button_AlwaysOnTop.IconSvg = resources.GetString("button_AlwaysOnTop.IconSvg");
            button_AlwaysOnTop.Location = new Point(366, 0);
            button_AlwaysOnTop.Name = "button_AlwaysOnTop";
            button_AlwaysOnTop.Size = new Size(18, 20);
            button_AlwaysOnTop.TabIndex = 3;
            button_AlwaysOnTop.ToggleIconSvg = resources.GetString("button_AlwaysOnTop.ToggleIconSvg");
            button_AlwaysOnTop.Click += button_AlwaysOnTop_Click;
            button_AlwaysOnTop.MouseEnter += button_AlwaysOnTop_MouseEnter;
            // 
            // button_RefreshDps
            // 
            button_RefreshDps.ColorScheme = AntdUI.TAMode.Dark;
            button_RefreshDps.Dock = DockStyle.Right;
            button_RefreshDps.Ghost = true;
            button_RefreshDps.IconRatio = 0.8F;
            button_RefreshDps.IconSvg = resources.GetString("button_RefreshDps.IconSvg");
            button_RefreshDps.Location = new Point(384, 0);
            button_RefreshDps.Name = "button_RefreshDps";
            button_RefreshDps.Size = new Size(16, 20);
            button_RefreshDps.TabIndex = 4;
            button_RefreshDps.ToggleIconSvg = "";
            button_RefreshDps.Click += button_RefreshDps_Click;
            button_RefreshDps.MouseEnter += button_RefreshDps_MouseEnter;
            // 
            // button_Settings
            // 
            button_Settings.BackActive = Color.Transparent;
            button_Settings.BackColor = Color.Transparent;
            button_Settings.ColorScheme = AntdUI.TAMode.Dark;
            button_Settings.DefaultBack = Color.Transparent;
            button_Settings.Dock = DockStyle.Right;
            button_Settings.Ghost = true;
            button_Settings.Icon = (Image)resources.GetObject("button_Settings.Icon");
            button_Settings.IconRatio = 1F;
            button_Settings.IconSvg = "";
            button_Settings.Location = new Point(400, 0);
            button_Settings.Name = "button_Settings";
            button_Settings.Size = new Size(21, 20);
            button_Settings.TabIndex = 5;
            button_Settings.ToggleIconSvg = "";
            button_Settings.Click += button_Settings_Click;
            // 
            // panel_Footer
            // 
            panel_Footer.BackColor = Color.Transparent;
            panel_Footer.Controls.Add(label_CurrentDps);
            panel_Footer.Controls.Add(label_BattleTimeText);
            panel_Footer.Controls.Add(label_CurrentOrder);
            panel_Footer.Dock = DockStyle.Bottom;
            panel_Footer.Location = new Point(0, 326);
            panel_Footer.Name = "panel_Footer";
            panel_Footer.Radius = 3;
            panel_Footer.Shadow = 3;
            panel_Footer.ShadowAlign = AntdUI.TAlignMini.Top;
            panel_Footer.Size = new Size(421, 27);
            panel_Footer.TabIndex = 3;
            panel_Footer.Text = "panel1";
            // 
            // label_CurrentDps
            // 
            label_CurrentDps.Dock = DockStyle.Right;
            label_CurrentDps.Font = new Font("阿里妈妈数黑体", 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label_CurrentDps.Location = new Point(314, 3);
            label_CurrentDps.Name = "label_CurrentDps";
            label_CurrentDps.Size = new Size(107, 24);
            label_CurrentDps.TabIndex = 2;
            label_CurrentDps.Text = "";
            label_CurrentDps.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label_BattleTimeText
            // 
            label_BattleTimeText.Dock = DockStyle.Left;
            label_BattleTimeText.Font = new Font("阿里妈妈数黑体", 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label_BattleTimeText.Location = new Point(31, 3);
            label_BattleTimeText.Margin = new Padding(2);
            label_BattleTimeText.Name = "label_BattleTimeText";
            label_BattleTimeText.Size = new Size(191, 24);
            label_BattleTimeText.TabIndex = 1;
            label_BattleTimeText.Text = "请稍等，正在准备监听服务器...";
            // 
            // label_CurrentOrder
            // 
            label_CurrentOrder.Dock = DockStyle.Left;
            label_CurrentOrder.Font = new Font("阿里妈妈数黑体", 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            label_CurrentOrder.Location = new Point(0, 3);
            label_CurrentOrder.Margin = new Padding(2);
            label_CurrentOrder.Name = "label_CurrentOrder";
            label_CurrentOrder.Size = new Size(31, 24);
            label_CurrentOrder.TabIndex = 0;
            label_CurrentOrder.Text = "";
            // 
            // panel_ModeBox
            // 
            panel_ModeBox.BackColor = Color.Transparent;
            panel_ModeBox.Controls.Add(button_NpcTakeDamage);
            panel_ModeBox.Controls.Add(button_AlwaysInjured);
            panel_ModeBox.Controls.Add(button_TotalTreatment);
            panel_ModeBox.Controls.Add(button_TotalDamage);
            panel_ModeBox.Dock = DockStyle.Top;
            panel_ModeBox.Location = new Point(0, 20);
            panel_ModeBox.Name = "panel_ModeBox";
            panel_ModeBox.Shadow = 3;
            panel_ModeBox.ShadowAlign = AntdUI.TAlignMini.Bottom;
            panel_ModeBox.Size = new Size(421, 44);
            panel_ModeBox.TabIndex = 1;
            panel_ModeBox.Text = "panel2";
            // 
            // button_NpcTakeDamage
            // 
            button_NpcTakeDamage.Anchor = AnchorStyles.Top;
            button_NpcTakeDamage.DefaultBack = Color.FromArgb(247, 247, 247);
            button_NpcTakeDamage.DefaultBorderColor = Color.Wheat;
            button_NpcTakeDamage.Font = new Font("HarmonyOS Sans SC Medium", 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            button_NpcTakeDamage.Icon = (Image)resources.GetObject("button_NpcTakeDamage.Icon");
            button_NpcTakeDamage.IconRatio = 0.8F;
            button_NpcTakeDamage.Location = new Point(300, 6);
            button_NpcTakeDamage.Name = "button_NpcTakeDamage";
            button_NpcTakeDamage.Radius = 3;
            button_NpcTakeDamage.Size = new Size(89, 30);
            button_NpcTakeDamage.TabIndex = 3;
            button_NpcTakeDamage.Tag = "3";
            button_NpcTakeDamage.Text = "承伤";
            button_NpcTakeDamage.Click += TypeButtons_Click;
            // 
            // button_AlwaysInjured
            // 
            button_AlwaysInjured.Anchor = AnchorStyles.Top;
            button_AlwaysInjured.DefaultBack = Color.FromArgb(247, 247, 247);
            button_AlwaysInjured.DefaultBorderColor = Color.Wheat;
            button_AlwaysInjured.Font = new Font("HarmonyOS Sans SC Medium", 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            button_AlwaysInjured.Icon = (Image)resources.GetObject("button_AlwaysInjured.Icon");
            button_AlwaysInjured.Location = new Point(211, 6);
            button_AlwaysInjured.Name = "button_AlwaysInjured";
            button_AlwaysInjured.Radius = 3;
            button_AlwaysInjured.Size = new Size(89, 30);
            button_AlwaysInjured.TabIndex = 2;
            button_AlwaysInjured.Tag = "2";
            button_AlwaysInjured.Text = "总承伤";
            button_AlwaysInjured.Click += TypeButtons_Click;
            // 
            // button_TotalTreatment
            // 
            button_TotalTreatment.Anchor = AnchorStyles.Top;
            button_TotalTreatment.DefaultBack = Color.FromArgb(247, 247, 247);
            button_TotalTreatment.Font = new Font("HarmonyOS Sans SC Medium", 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            button_TotalTreatment.Icon = (Image)resources.GetObject("button_TotalTreatment.Icon");
            button_TotalTreatment.Location = new Point(121, 6);
            button_TotalTreatment.Name = "button_TotalTreatment";
            button_TotalTreatment.Radius = 3;
            button_TotalTreatment.Size = new Size(89, 30);
            button_TotalTreatment.TabIndex = 1;
            button_TotalTreatment.Tag = "1";
            button_TotalTreatment.Text = "总治疗";
            button_TotalTreatment.Click += TypeButtons_Click;
            // 
            // button_TotalDamage
            // 
            button_TotalDamage.Anchor = AnchorStyles.Top;
            button_TotalDamage.DefaultBack = Color.FromArgb(223, 223, 223);
            button_TotalDamage.Font = new Font("HarmonyOS Sans SC Medium", 9F, FontStyle.Bold, GraphicsUnit.Point, 134);
            button_TotalDamage.Icon = (Image)resources.GetObject("button_TotalDamage.Icon");
            button_TotalDamage.Location = new Point(32, 6);
            button_TotalDamage.Name = "button_TotalDamage";
            button_TotalDamage.Radius = 3;
            button_TotalDamage.Size = new Size(89, 30);
            button_TotalDamage.TabIndex = 0;
            button_TotalDamage.Tag = "0";
            button_TotalDamage.Text = "总伤害";
            button_TotalDamage.Click += TypeButtons_Click;
            // 
            // sortedProgressBarList_MainList
            // 
            sortedProgressBarList_MainList.Dock = DockStyle.Fill;
            sortedProgressBarList_MainList.Location = new Point(0, 64);
            sortedProgressBarList_MainList.Margin = new Padding(5, 4, 5, 4);
            sortedProgressBarList_MainList.Name = "sortedProgressBarList_MainList";
            sortedProgressBarList_MainList.OrderColor = Color.Black;
            sortedProgressBarList_MainList.OrderFont = new Font("宋体", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            sortedProgressBarList_MainList.OrderImageAlign = Control.GDI.RenderContent.ContentAlign.MiddleLeft;
            sortedProgressBarList_MainList.OrderImageRenderSize = new Size(0, 0);
            sortedProgressBarList_MainList.OrderImages = null;
            sortedProgressBarList_MainList.ScrollBarWidth = 8;
            sortedProgressBarList_MainList.ScrollOffsetY = 0F;
            sortedProgressBarList_MainList.Size = new Size(421, 262);
            sortedProgressBarList_MainList.TabIndex = 2;
            // 
            // timer_BattleTimeLabelUpdater
            // 
            timer_BattleTimeLabelUpdater.Interval = 500;
            timer_BattleTimeLabelUpdater.Tick += timer_BattleTimeLabelUpdater_Tick;
            // 
            // DpsStatisticsForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.White;
            BorderWidth = 0;
            ClientSize = new Size(421, 353);
            Controls.Add(sortedProgressBarList_MainList);
            Controls.Add(panel_ModeBox);
            Controls.Add(panel_Footer);
            Controls.Add(pageHeader_MainHeader);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(2);
            MinimumSize = new Size(370, 150);
            Name = "DpsStatisticsForm";
            Radius = 3;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "别查我DPS";
            FormClosing += DpsStatisticsForm_FormClosing;
            Load += DpsStatistics_Load;
            Shown += DpsStatisticsForm_Shown;
            ForeColorChanged += DpsStatisticsForm_ForeColorChanged;
            pageHeader_MainHeader.ResumeLayout(false);
            panel_Footer.ResumeLayout(false);
            panel_ModeBox.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private AntdUI.PageHeader pageHeader_MainHeader;
        private AntdUI.Button button_Settings;
        private AntdUI.Button button_RefreshDps;
        private AntdUI.Button button_AlwaysOnTop;
        private AntdUI.Button button_SwitchStatisticsMode;
        private AntdUI.Button button_Minimum;
        private AntdUI.Panel panel_Footer;
        private AntdUI.Label label_BattleTimeText;
        private AntdUI.Label label_CurrentOrder;
        private AntdUI.Label label_CurrentDps;
        private AntdUI.Panel panel_ModeBox;
        private AntdUI.Button button_TotalDamage;
        private AntdUI.Button button_TotalTreatment;
        private AntdUI.Button button_AlwaysInjured;
        private AntdUI.Button button_NpcTakeDamage;
        private AntdUI.Button button_ThemeSwitch;
        private Control.SortedProgressBarList sortedProgressBarList_MainList;
        private System.Windows.Forms.Timer timer_BattleTimeLabelUpdater;
    }
}