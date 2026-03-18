using SuperCom.Core.Entity.Enums;
using SuperCom.Core.Utils;
using SuperControls.Style;
using SuperControls.Style.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace SuperCom.Windows
{
    /// <summary>
    /// Window_HexConverter.xaml 的交互逻辑
    /// </summary>
    public partial class Window_HexConverter : BaseWindow
    {
        public ObservableCollection<KeyValuePair<HexConvertType, string>> ConvertTypes { get; set; }

        public Window_HexConverter()
        {
            InitializeComponent();
            InitConvertTypes();
            cmbConvertType.ItemsSource = ConvertTypes;
        }

        private void InitConvertTypes()
        {
            ConvertTypes = new ObservableCollection<KeyValuePair<HexConvertType, string>>
            {
                new KeyValuePair<HexConvertType, string>(HexConvertType.HexToString, LangManager.GetValueByKey("ConvertHexToString")),
                new KeyValuePair<HexConvertType, string>(HexConvertType.StringToHex, LangManager.GetValueByKey("ConvertStringToHex")),
                new KeyValuePair<HexConvertType, string>(HexConvertType.HexToDec, LangManager.GetValueByKey("ConvertHexToDec")),
                new KeyValuePair<HexConvertType, string>(HexConvertType.DecToHex, LangManager.GetValueByKey("ConvertDecToHex")),
                new KeyValuePair<HexConvertType, string>(HexConvertType.HexToFloatBE, LangManager.GetValueByKey("ConvertHexToFloatBE")),
                new KeyValuePair<HexConvertType, string>(HexConvertType.HexToFloatLE, LangManager.GetValueByKey("ConvertHexToFloatLE")),
                new KeyValuePair<HexConvertType, string>(HexConvertType.FloatToHexBE, LangManager.GetValueByKey("ConvertFloatToHexBE")),
                new KeyValuePair<HexConvertType, string>(HexConvertType.FloatToHexLE, LangManager.GetValueByKey("ConvertFloatToHexLE")),
                new KeyValuePair<HexConvertType, string>(HexConvertType.HexToDoubleBE, LangManager.GetValueByKey("ConvertHexToDoubleBE")),
                new KeyValuePair<HexConvertType, string>(HexConvertType.HexToDoubleLE, LangManager.GetValueByKey("ConvertHexToDoubleLE")),
                new KeyValuePair<HexConvertType, string>(HexConvertType.DoubleToHexBE, LangManager.GetValueByKey("ConvertDoubleToHexBE")),
                new KeyValuePair<HexConvertType, string>(HexConvertType.DoubleToHexLE, LangManager.GetValueByKey("ConvertDoubleToHexLE")),
                new KeyValuePair<HexConvertType, string>(HexConvertType.HexToAscii, LangManager.GetValueByKey("ConvertHexToAscii")),
                new KeyValuePair<HexConvertType, string>(HexConvertType.AsciiToHex, LangManager.GetValueByKey("ConvertAsciiToHex")),
            };
        }

        private void cmbConvertType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // 转换类型改变时自动转换
            if (!string.IsNullOrEmpty(txtInput.Text))
            {
                DoConvertClick(null, null);
            }
        }

        private void txtInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // 输入改变时自动转换
            if (cmbConvertType.SelectedItem != null && !string.IsNullOrEmpty(txtInput.Text))
            {
                try
                {
                    DoConvert();
                }
                catch
                {
                    // 忽略转换错误
                }
            }
        }

        private void DoConvertClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtInput.Text))
            {
                txtOutput.Text = "";
                return;
            }

            try
            {
                DoConvert();
            }
            catch (Exception ex)
            {
                txtOutput.Text = $"{LangManager.GetValueByKey("ConvertError")}: {ex.Message}";
            }
        }

        private void DoConvert()
        {
            if (!(cmbConvertType.SelectedItem is KeyValuePair<HexConvertType, string> selected))
                return;

            HexConvertType convertType = selected.Key;
            string input = txtInput.Text.Trim();

            string result = "";
            switch (convertType)
            {
                case HexConvertType.HexToString:
                    result = HexToString(input);
                    break;
                case HexConvertType.StringToHex:
                    result = StringToHex(input);
                    break;
                case HexConvertType.HexToDec:
                    result = HexToDec(input);
                    break;
                case HexConvertType.DecToHex:
                    result = DecToHex(input);
                    break;
                case HexConvertType.HexToFloatBE:
                    result = HexToFloat(input, true);
                    break;
                case HexConvertType.HexToFloatLE:
                    result = HexToFloat(input, false);
                    break;
                case HexConvertType.FloatToHexBE:
                    result = FloatToHex(input, true);
                    break;
                case HexConvertType.FloatToHexLE:
                    result = FloatToHex(input, false);
                    break;
                case HexConvertType.HexToDoubleBE:
                    result = HexToDouble(input, true);
                    break;
                case HexConvertType.HexToDoubleLE:
                    result = HexToDouble(input, false);
                    break;
                case HexConvertType.DoubleToHexBE:
                    result = DoubleToHex(input, true);
                    break;
                case HexConvertType.DoubleToHexLE:
                    result = DoubleToHex(input, false);
                    break;
                case HexConvertType.HexToAscii:
                    result = HexToAscii(input);
                    break;
                case HexConvertType.AsciiToHex:
                    result = AsciiToHex(input);
                    break;
            }

            txtOutput.Text = result;
        }

        #region 转换方法

        private string HexToString(string hex)
        {
            byte[] bytes = DataConverter.HexStringToBytes(hex);
            return Encoding.UTF8.GetString(bytes);
        }

        private string StringToHex(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return DataConverter.BytesToHexString(bytes, " ");
        }

        private string HexToDec(string hex)
        {
            hex = hex.Replace(" ", "").Replace("-", "");
            if (hex.Length == 0)
                return "0";

            // 尝试解析为不同长度的整数
            if (hex.Length <= 8)
            {
                int value = Convert.ToInt32(hex, 16);
                return value.ToString();
            }
            else
            {
                long value = Convert.ToInt64(hex, 16);
                return value.ToString();
            }
        }

        private string DecToHex(string dec)
        {
            if (long.TryParse(dec, out long value))
            {
                return value.ToString("X").ToUpper();
            }
            throw new ArgumentException("Invalid decimal number");
        }

        private string HexToFloat(string hex, bool bigEndian)
        {
            byte[] bytes = DataConverter.HexStringToBytes(hex);
            if (bytes.Length < 4)
                throw new ArgumentException("Hex data must be at least 4 bytes");

            if (!bigEndian && bytes.Length >= 4)
                Array.Reverse(bytes, 0, 4);

            float result = BitConverter.ToSingle(bytes, 0);
            return result.ToString("G");
        }

        private string FloatToHex(string floatStr, bool bigEndian)
        {
            if (!float.TryParse(floatStr, out float value))
                throw new ArgumentException("Invalid float number");

            byte[] bytes = BitConverter.GetBytes(value);
            if (bigEndian)
                Array.Reverse(bytes);

            return DataConverter.BytesToHexString(bytes, " ");
        }

        private string HexToDouble(string hex, bool bigEndian)
        {
            byte[] bytes = DataConverter.HexStringToBytes(hex);
            if (bytes.Length < 8)
                throw new ArgumentException("Hex data must be at least 8 bytes");

            if (!bigEndian && bytes.Length >= 8)
                Array.Reverse(bytes, 0, 8);

            double result = BitConverter.ToDouble(bytes, 0);
            return result.ToString("G");
        }

        private string DoubleToHex(string doubleStr, bool bigEndian)
        {
            if (!double.TryParse(doubleStr, out double value))
                throw new ArgumentException("Invalid double number");

            byte[] bytes = BitConverter.GetBytes(value);
            if (bigEndian)
                Array.Reverse(bytes);

            return DataConverter.BytesToHexString(bytes, " ");
        }

        private string HexToAscii(string hex)
        {
            byte[] bytes = DataConverter.HexStringToBytes(hex);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                if (b >= 32 && b <= 126)
                    sb.Append((char)b);
                else
                    sb.Append($".");
            }
            return sb.ToString();
        }

        private string AsciiToHex(string ascii)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(ascii);
            return DataConverter.BytesToHexString(bytes, " ");
        }

        #endregion

        private void SwapInputOutput(object sender, RoutedEventArgs e)
        {
            string temp = txtInput.Text;
            txtInput.Text = txtOutput.Text;
            txtOutput.Text = temp;
        }

        private void ClearAll(object sender, RoutedEventArgs e)
        {
            txtInput.Text = "";
            txtOutput.Text = "";
        }

        private void CopyResult(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtOutput.Text))
            {
                Clipboard.SetText(txtOutput.Text);
                MessageNotify.Success(LangManager.GetValueByKey("CopySuccess"));
            }
        }
    }

    /// <summary>
    /// 进制转换类型
    /// </summary>
    public enum HexConvertType
    {
        HexToString = 0,
        StringToHex = 1,
        HexToDec = 2,
        DecToHex = 3,
        HexToFloatBE = 4,
        HexToFloatLE = 5,
        FloatToHexBE = 6,
        FloatToHexLE = 7,
        HexToDoubleBE = 8,
        HexToDoubleLE = 9,
        DoubleToHexBE = 10,
        DoubleToHexLE = 11,
        HexToAscii = 12,
        AsciiToHex = 13,
    }
}