using ICSharpCode.AvalonEdit;
using SuperCom.Config;
using SuperCom.Entity;
using SuperControls.Style;
using SuperControls.Style.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SuperCom.Windows
{
    /// <summary>
    /// Window_SinglePort.xaml 的交互逻辑
    /// </summary>
    public partial class Window_SinglePort : BaseWindow
    {
        private PortTabItem _portTabItem;
        private DispatcherTimer _updateTimer;
        private bool _isClosing = false;

        public string PortName { get; set; }
        public bool Connected { get; set; }
        public long RX { get; set; }
        public long TX { get; set; }
        public string StatusText { get; set; }
        public Brush StatusColor { get; set; }
        public string ConnectButtonText { get; set; }
        public bool RecvShowHex { get; set; }
        public bool AddTimeStamp { get; set; }
        public bool SendHex { get; set; }
        public bool AddNewLineWhenWrite { get; set; }
        public string WriteData { get; set; }
        public int DataConvertIndex { get; set; }
        public bool FixedText { get; set; }
        public ObservableCollection<KeyValuePair<int, string>> DataConvertTypes { get; set; }

        public Window_SinglePort()
        {
            InitializeComponent();
            this.DataContext = this;
            InitDataConvertTypes();
            InitTimer();
        }

        public Window_SinglePort(PortTabItem portTabItem) : this()
        {
            _portTabItem = portTabItem;
            PortName = portTabItem.Name;
            this.Title = PortName;
            SyncFromPortTabItem();
        }

        private void InitDataConvertTypes()
        {
            DataConvertTypes = new ObservableCollection<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(0, LangManager.GetValueByKey("ConvertNone")),
                new KeyValuePair<int, string>(1, LangManager.GetValueByKey("ConvertHexToString")),
                new KeyValuePair<int, string>(2, LangManager.GetValueByKey("ConvertStringToHex")),
                new KeyValuePair<int, string>(3, LangManager.GetValueByKey("ConvertHexToDec")),
                new KeyValuePair<int, string>(4, LangManager.GetValueByKey("ConvertDecToHex")),
                new KeyValuePair<int, string>(5, LangManager.GetValueByKey("ConvertHexToFloatBE")),
                new KeyValuePair<int, string>(6, LangManager.GetValueByKey("ConvertHexToFloatLE")),
                new KeyValuePair<int, string>(7, LangManager.GetValueByKey("ConvertFloatToHexBE")),
                new KeyValuePair<int, string>(8, LangManager.GetValueByKey("ConvertFloatToHexLE")),
                new KeyValuePair<int, string>(9, LangManager.GetValueByKey("ConvertHexToDoubleBE")),
                new KeyValuePair<int, string>(10, LangManager.GetValueByKey("ConvertHexToDoubleLE")),
                new KeyValuePair<int, string>(11, LangManager.GetValueByKey("ConvertDoubleToHexBE")),
                new KeyValuePair<int, string>(12, LangManager.GetValueByKey("ConvertDoubleToHexLE"))
            };
        }

        private void InitTimer()
        {
            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromMilliseconds(100);
            _updateTimer.Tick += UpdateTimer_Tick;
        }

        private void SyncFromPortTabItem()
        {
            if (_portTabItem == null) return;

            Connected = _portTabItem.Connected;
            RX = _portTabItem.RX;
            TX = _portTabItem.TX;
            RecvShowHex = _portTabItem.RecvShowHex;
            AddTimeStamp = _portTabItem.AddTimeStamp;
            SendHex = _portTabItem.SendHex;
            AddNewLineWhenWrite = _portTabItem.AddNewLineWhenWrite;
            WriteData = _portTabItem.WriteData;
            DataConvertIndex = _portTabItem.DataConvertIndex;
            FixedText = _portTabItem.FixedText;

            UpdateStatus();
            OnPropertyChanged(nameof(Connected));
            OnPropertyChanged(nameof(RX));
            OnPropertyChanged(nameof(TX));
            OnPropertyChanged(nameof(RecvShowHex));
            OnPropertyChanged(nameof(AddTimeStamp));
            OnPropertyChanged(nameof(SendHex));
            OnPropertyChanged(nameof(AddNewLineWhenWrite));
            OnPropertyChanged(nameof(WriteData));
            OnPropertyChanged(nameof(DataConvertIndex));
            OnPropertyChanged(nameof(FixedText));
        }

        private void SyncToPortTabItem()
        {
            if (_portTabItem == null) return;

            _portTabItem.RecvShowHex = RecvShowHex;
            _portTabItem.AddTimeStamp = AddTimeStamp;
            _portTabItem.SendHex = SendHex;
            _portTabItem.AddNewLineWhenWrite = AddNewLineWhenWrite;
            _portTabItem.WriteData = WriteData;
            _portTabItem.DataConvertIndex = DataConvertIndex;
            _portTabItem.FixedText = FixedText;
        }

        private void UpdateStatus()
        {
            if (Connected)
            {
                StatusText = LangManager.GetValueByKey("Connected");
                StatusColor = new SolidColorBrush(Colors.Green);
                ConnectButtonText = LangManager.GetValueByKey("Disconnect");
            }
            else
            {
                StatusText = LangManager.GetValueByKey("Disconnected");
                StatusColor = new SolidColorBrush(Colors.Red);
                ConnectButtonText = LangManager.GetValueByKey("Connect");
            }
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(StatusColor));
            OnPropertyChanged(nameof(ConnectButtonText));
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (_isClosing || _portTabItem == null) return;

            // 同步状态
            if (Connected != _portTabItem.Connected)
            {
                Connected = _portTabItem.Connected;
                UpdateStatus();
            }

            RX = _portTabItem.RX;
            TX = _portTabItem.TX;
            OnPropertyChanged(nameof(RX));
            OnPropertyChanged(nameof(TX));

            // 同步文本编辑器内容
            if (_portTabItem.TextEditor != null && textEditor != null)
            {
                string currentText = textEditor.Text;
                string portText = _portTabItem.TextEditor.Text;
                if (currentText != portText)
                {
                    textEditor.Text = portText;
                    textEditor.ScrollToEnd();
                }
            }
        }

        private void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 使用主窗口的TextEditor
            if (_portTabItem?.TextEditor != null)
            {
                textEditor.Text = _portTabItem.TextEditor.Text;
            }

            _updateTimer.Start();
        }

        private void BaseWindow_Closing(object sender, CancelEventArgs e)
        {
            _isClosing = true;
            _updateTimer?.Stop();
            SyncToPortTabItem();
        }

        private void ToggleConnect(object sender, RoutedEventArgs e)
        {
            if (_portTabItem == null) return;

            if (Connected)
            {
                _portTabItem.Close();
            }
            else
            {
                _portTabItem.Open();
            }

            Connected = _portTabItem.Connected;
            UpdateStatus();
        }

        private void ClearData(object sender, RoutedEventArgs e)
        {
            if (_portTabItem != null)
            {
                _portTabItem.ClearData();
                _portTabItem.RX = 0;
                _portTabItem.TX = 0;
                textEditor.Text = "";
            }
        }

        private void SendData(object sender, RoutedEventArgs e)
        {
            if (_portTabItem == null || string.IsNullOrEmpty(WriteData))
                return;

            SyncToPortTabItem();
            _portTabItem.SendCommand(WriteData);
        }

        private void SendTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                SendData(sender, e);
                e.Handled = true;
            }
        }

        private void OpenPath(object sender, RoutedEventArgs e)
        {
            if (_portTabItem == null) return;

            string logDir = _portTabItem.GetLogDir();
            if (!string.IsNullOrEmpty(logDir))
            {
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", logDir);
                }
                catch (Exception ex)
                {
                    MessageNotify.Error(ex.Message);
                }
            }
        }

        private void OpenByDefaultApp(object sender, RoutedEventArgs e)
        {
            if (_portTabItem == null) return;

            string fileName = _portTabItem.SaveFileName;
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(fileName) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageNotify.Error(ex.Message);
                }
            }
        }

        private void SaveToNewFile(object sender, RoutedEventArgs e)
        {
            if (_portTabItem == null) return;

            string fileName = _portTabItem.SaveFileName;
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{fileName}\"");
                }
                catch (Exception ex)
                {
                    MessageNotify.Error(ex.Message);
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            base.RaisePropertyChanged(propertyName);
        }
    }
}