using EasyBluetooth.Aida64Helper.Services;
using EasyBluetooth.DisplayExport;

namespace EasyBluetooth.Aida64Helper;

internal sealed class MainForm : Form
{
    private readonly HelperConfigStore _configStore = new();
    private readonly UnifiedApiClient _apiClient = new();
    private readonly Aida64RegistryWriter _registryWriter = new();
    private readonly System.Windows.Forms.Timer _pollTimer = new();

    private Aida64HelperConfig _config;
    private bool _isOutputEnabled;
    private bool _isRefreshing;

    private Label _headerLabel = null!;
    private Label _descriptionLabel = null!;
    private Label _apiUrlLabel = null!;
    private TextBox _apiUrlTextBox = null!;
    private Label _tokenLabel = null!;
    private TextBox _tokenTextBox = null!;
    private Label _pollIntervalLabel = null!;
    private NumericUpDown _pollIntervalUpDown = null!;
    private Label _pollIntervalUnitLabel = null!;
    private Label _languageLabel = null!;
    private ComboBox _languageComboBox = null!;
    private Button _saveButton = null!;
    private Button _startStopButton = null!;
    private Label _statusTitleLabel = null!;
    private Label _statusValueLabel = null!;

    public MainForm()
    {
        _config = _configStore.Load();
        _isOutputEnabled = _config.IsOutputEnabled;
        HelperLocalizationService.Initialize(_config.LanguagePreference);

        InitializeLayout();
        ApplyLocalizedText();
        LoadConfigToControls();

        _pollTimer.Tick += PollTimer_Tick;
        RestartPolling();

        Shown += async (_, _) =>
        {
            if (_isOutputEnabled)
            {
                await RefreshNowAsync();
            }
        };
        FormClosing += MainForm_FormClosing;
    }

    private void InitializeLayout()
    {
        AutoScaleMode = AutoScaleMode.Font;
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(720, 430);
        ClientSize = new Size(760, 430);
        MaximizeBox = false;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            AutoSize = true,
            ColumnCount = 1,
            RowCount = 5
        };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _headerLabel = new Label
        {
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold)
        };
        _descriptionLabel = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(700, 0)
        };

        var formTable = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 3,
            RowCount = 4,
            Margin = new Padding(0, 14, 0, 14)
        };
        formTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
        formTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        formTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        _apiUrlLabel = CreateCaptionLabel();
        _apiUrlTextBox = new TextBox { Dock = DockStyle.Fill };
        formTable.Controls.Add(_apiUrlLabel, 0, 0);
        formTable.Controls.Add(_apiUrlTextBox, 1, 0);
        formTable.SetColumnSpan(_apiUrlTextBox, 2);

        _tokenLabel = CreateCaptionLabel();
        _tokenTextBox = new TextBox { Dock = DockStyle.Fill };
        formTable.Controls.Add(_tokenLabel, 0, 1);
        formTable.Controls.Add(_tokenTextBox, 1, 1);
        formTable.SetColumnSpan(_tokenTextBox, 2);

        _pollIntervalLabel = CreateCaptionLabel();
        _pollIntervalUpDown = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 3600,
            Width = 120
        };
        _pollIntervalUnitLabel = new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(8, 6, 0, 0)
        };
        formTable.Controls.Add(_pollIntervalLabel, 0, 2);
        formTable.Controls.Add(_pollIntervalUpDown, 1, 2);
        formTable.Controls.Add(_pollIntervalUnitLabel, 2, 2);

        _languageLabel = CreateCaptionLabel();
        _languageComboBox = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 180
        };
        _languageComboBox.Items.Add(new LanguageOption("en-US", "English"));
        _languageComboBox.Items.Add(new LanguageOption("zh-CN", "简体中文"));
        formTable.Controls.Add(_languageLabel, 0, 3);
        formTable.Controls.Add(_languageComboBox, 1, 3);
        formTable.SetColumnSpan(_languageComboBox, 2);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 0, 0, 14)
        };
        _saveButton = new Button { AutoSize = true };
        _saveButton.Click += SaveButton_Click;
        _startStopButton = new Button { AutoSize = true };
        _startStopButton.Click += StartStopButton_Click;
        buttonPanel.Controls.Add(_saveButton);
        buttonPanel.Controls.Add(_startStopButton);

        var statusPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = 1,
            RowCount = 2
        };
        _statusTitleLabel = new Label
        {
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold)
        };
        _statusValueLabel = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(700, 0)
        };
        statusPanel.Controls.Add(_statusTitleLabel, 0, 0);
        statusPanel.Controls.Add(_statusValueLabel, 0, 1);

        root.Controls.Add(_headerLabel, 0, 0);
        root.Controls.Add(_descriptionLabel, 0, 1);
        root.Controls.Add(formTable, 0, 2);
        root.Controls.Add(buttonPanel, 0, 3);
        root.Controls.Add(statusPanel, 0, 4);

        Controls.Add(root);
    }

    private static Label CreateCaptionLabel()
    {
        return new Label
        {
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(0, 6, 8, 0)
        };
    }

    private void ApplyLocalizedText()
    {
        Text = HelperLocalizationService.GetString("MainForm.Title");
        _headerLabel.Text = HelperLocalizationService.GetString("MainForm.Header.Text");
        _descriptionLabel.Text = HelperLocalizationService.GetString("MainForm.Description.Text");
        _apiUrlLabel.Text = HelperLocalizationService.GetString("MainForm.ApiUrlLabel.Text");
        _tokenLabel.Text = HelperLocalizationService.GetString("MainForm.TokenLabel.Text");
        _pollIntervalLabel.Text = HelperLocalizationService.GetString("MainForm.PollIntervalLabel.Text");
        _pollIntervalUnitLabel.Text = HelperLocalizationService.GetString("MainForm.PollIntervalUnit.Text");
        _languageLabel.Text = HelperLocalizationService.GetString("MainForm.LanguageLabel.Text");
        _saveButton.Text = HelperLocalizationService.GetString("MainForm.SaveButton.Text");
        UpdateStartStopButtonText();
        _statusTitleLabel.Text = HelperLocalizationService.GetString("MainForm.StatusTitle.Text");
        _statusValueLabel.Text = GetInitialStatusText();
    }

    private void LoadConfigToControls()
    {
        _apiUrlTextBox.Text = _config.ApiUrl;
        _tokenTextBox.Text = _config.ApiToken;
        _pollIntervalUpDown.Value = _config.PollIntervalSeconds;

        string normalizedLanguage = HelperLocalizationService.NormalizeLanguage(_config.LanguagePreference);
        _languageComboBox.SelectedItem = _languageComboBox.Items
            .OfType<LanguageOption>()
            .FirstOrDefault(item => string.Equals(item.Code, normalizedLanguage, StringComparison.OrdinalIgnoreCase));

        if (_languageComboBox.SelectedItem == null && _languageComboBox.Items.Count > 0)
        {
            _languageComboBox.SelectedIndex = 0;
        }
    }

    private async void SaveButton_Click(object? sender, EventArgs e)
    {
        var newConfig = CreateConfigFromInputs();
        bool languageChanged = !string.Equals(_config.LanguagePreference, newConfig.LanguagePreference, StringComparison.OrdinalIgnoreCase);

        _config = newConfig;
        _isOutputEnabled = _config.IsOutputEnabled;
        _configStore.Save(_config);
        RestartPolling();
        UpdateStartStopButtonText();

        if (languageChanged)
        {
            MessageBox.Show(
                this,
                HelperLocalizationService.GetString("MainForm.Dialog.LanguageRestart.Content"),
                HelperLocalizationService.GetString("MainForm.Dialog.InfoTitle"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        if (_isOutputEnabled)
        {
            await RefreshNowAsync();
            return;
        }

        UpdateStatus(HelperLocalizationService.GetString("MainForm.Status.SettingsSaved"));
    }

    private async void PollTimer_Tick(object? sender, EventArgs e)
    {
        await RefreshNowAsync();
    }

    private async void StartStopButton_Click(object? sender, EventArgs e)
    {
        if (_isOutputEnabled)
        {
            _isOutputEnabled = false;
            _config.IsOutputEnabled = false;
            RestartPolling();
            UpdateStartStopButtonText();
            UpdateStatus(HelperLocalizationService.GetString("MainForm.Status.OutputStopped"));
            return;
        }

        _config = CreateConfigFromInputs();
        _isOutputEnabled = true;
        _config.IsOutputEnabled = true;
        RestartPolling();
        UpdateStartStopButtonText();
        await RefreshNowAsync();
    }

    private async Task RefreshNowAsync()
    {
        if (!_isOutputEnabled || _isRefreshing)
        {
            return;
        }

        _isRefreshing = true;
        _saveButton.Enabled = false;
        _startStopButton.Enabled = false;

        try
        {
            UpdateStatus(HelperLocalizationService.GetString("MainForm.Status.Syncing"));

            var result = await _apiClient.FetchAsync(_config.ApiUrl, _config.ApiToken, CancellationToken.None);

            string statusText;
            switch (result.Status)
            {
                case UnifiedApiFetchStatus.Success:
                    statusText = HandleSuccessfulFetch(result.Devices);
                    break;
                case UnifiedApiFetchStatus.InvalidUrl:
                    statusText = HelperLocalizationService.GetString("MainForm.Status.InvalidApiUrl");
                    break;
                case UnifiedApiFetchStatus.ConnectionFailed:
                    statusText = string.Format(
                        HelperLocalizationService.GetString("MainForm.Status.ConnectionFailedTemplate"),
                        result.Detail);
                    break;
                case UnifiedApiFetchStatus.Unauthorized:
                    statusText = HelperLocalizationService.GetString("MainForm.Status.Unauthorized");
                    break;
                case UnifiedApiFetchStatus.Forbidden:
                    statusText = HelperLocalizationService.GetString("MainForm.Status.Forbidden");
                    break;
                case UnifiedApiFetchStatus.InvalidResponse:
                    statusText = string.Format(
                        HelperLocalizationService.GetString("MainForm.Status.InvalidResponseTemplate"),
                        result.Detail);
                    break;
                default:
                    statusText = string.Format(
                        HelperLocalizationService.GetString("MainForm.Status.ServerErrorTemplate"),
                        result.Detail);
                    break;
            }

            UpdateStatus(statusText);
        }
        catch (Exception ex)
        {
            UpdateStatus(string.Format(
                HelperLocalizationService.GetString("MainForm.Status.ExceptionTemplate"),
                ex.Message));
        }
        finally
        {
            _saveButton.Enabled = true;
            _startStopButton.Enabled = true;
            _isRefreshing = false;
            UpdateStartStopButtonText();
        }
    }

    private string HandleSuccessfulFetch(IReadOnlyList<DisplayDeviceInfo> devices)
    {
        try
        {
            var slots = DisplayExportFormatter.BuildAida64Slots(devices);
            _registryWriter.WriteSlots(slots);

            if (devices.Count == 0)
            {
                return HelperLocalizationService.GetString("MainForm.Status.NoDevices");
            }

            return Aida64RegistryWriter.IsAida64Running()
                ? string.Format(HelperLocalizationService.GetString("MainForm.Status.SuccessTemplate"), devices.Count)
                : string.Format(HelperLocalizationService.GetString("MainForm.Status.SuccessWaitingAida64Template"), devices.Count);
        }
        catch (Exception ex)
        {
            return string.Format(
                HelperLocalizationService.GetString("MainForm.Status.WriteFailedTemplate"),
                ex.Message);
        }
    }

    private void RestartPolling()
    {
        _pollTimer.Stop();
        if (!_isOutputEnabled)
        {
            return;
        }

        _pollTimer.Interval = Math.Max(1, _config.PollIntervalSeconds) * 1000;
        _pollTimer.Start();
    }

    private Aida64HelperConfig CreateConfigFromInputs()
    {
        var config = new Aida64HelperConfig
        {
            ApiUrl = _apiUrlTextBox.Text?.Trim() ?? string.Empty,
            ApiToken = _tokenTextBox.Text?.Trim() ?? string.Empty,
            PollIntervalSeconds = Decimal.ToInt32(_pollIntervalUpDown.Value),
            LanguagePreference = (_languageComboBox.SelectedItem as LanguageOption)?.Code ?? Aida64HelperConfig.DefaultLanguagePreferenceValue,
            IsOutputEnabled = _isOutputEnabled,
            LastConnectionStatus = _statusValueLabel.Text ?? string.Empty
        };
        config.Normalize();
        return config;
    }

    private string GetInitialStatusText()
    {
        if (!_isOutputEnabled)
        {
            return HelperLocalizationService.GetString("MainForm.Status.OutputStopped");
        }

        return string.IsNullOrWhiteSpace(_config.LastConnectionStatus)
            ? HelperLocalizationService.GetString("MainForm.Status.Ready")
            : _config.LastConnectionStatus;
    }

    private void UpdateStartStopButtonText()
    {
        _startStopButton.Text = HelperLocalizationService.GetString(
            _isOutputEnabled ? "MainForm.StartStopButton.Stop.Text" : "MainForm.StartStopButton.Start.Text");
    }

    private void UpdateStatus(string status)
    {
        _statusValueLabel.Text = status;
        _config.LastConnectionStatus = status;
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _config = CreateConfigFromInputs();
        _config.LastConnectionStatus = _statusValueLabel.Text ?? string.Empty;
        _configStore.Save(_config);
    }

    private sealed record LanguageOption(string Code, string DisplayName)
    {
        public override string ToString() => DisplayName;
    }
}
