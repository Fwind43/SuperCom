using SuperCom.Core.Entity.Enums;
using SuperControls.Style;
using System;
using System.Text;

namespace SuperCom.Core.Utils
{
    /// <summary>
    /// 数据转换工具类，支持各种进制转换
    /// </summary>
    public static class DataConverter
    {
        /// <summary>
        /// 根据转换类型转换数据
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="convertType">转换类型</param>
        /// <returns>转换后的字符串</returns>
        public static string ConvertData(string input, DataConvertType convertType)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            try
            {
                switch (convertType)
                {
                    case DataConvertType.None:
                        return input;

                    case DataConvertType.HexToFloatBigEndian:
                        return HexToFloat(input, true);

                    case DataConvertType.HexToFloatLittleEndian:
                        return HexToFloat(input, false);

                    case DataConvertType.FloatToHexBigEndian:
                        return FloatToHex(input, true);

                    case DataConvertType.FloatToHexLittleEndian:
                        return FloatToHex(input, false);

                    case DataConvertType.HexToIntBigEndian:
                        return HexToInt(input, true);

                    case DataConvertType.HexToIntLittleEndian:
                        return HexToInt(input, false);

                    case DataConvertType.IntToHexBigEndian:
                        return IntToHex(input, true);

                    case DataConvertType.IntToHexLittleEndian:
                        return IntToHex(input, false);

                    case DataConvertType.HexToDoubleBigEndian:
                        return HexToDouble(input, true);

                    case DataConvertType.HexToDoubleLittleEndian:
                        return HexToDouble(input, false);

                    case DataConvertType.DoubleToHexBigEndian:
                        return DoubleToHex(input, true);

                    case DataConvertType.DoubleToHexLittleEndian:
                        return DoubleToHex(input, false);

                    default:
                        return input;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"转换失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 十六进制字符串转字节数组
        /// </summary>
        public static byte[] HexStringToBytes(string hex)
        {
            hex = hex.Replace(" ", "").Replace("-", "");
            if (hex.Length % 2 != 0)
                hex = "0" + hex;

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

        /// <summary>
        /// 字节数组转十六进制字符串
        /// </summary>
        public static string BytesToHexString(byte[] bytes, string separator = " ")
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("X2"));
                if (i < bytes.Length - 1 && !string.IsNullOrEmpty(separator))
                    sb.Append(separator);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 十六进制转浮点数
        /// </summary>
        private static string HexToFloat(string hex, bool bigEndian)
        {
            byte[] bytes = HexStringToBytes(hex);
            if (bytes.Length < 4)
                throw new ArgumentException("十六进制数据长度不足4字节");

            if (!bigEndian)
                Array.Reverse(bytes, 0, 4);

            float result = BitConverter.ToSingle(bytes, 0);
            return result.ToString("G");
        }

        /// <summary>
        /// 浮点数转十六进制
        /// </summary>
        private static string FloatToHex(string floatStr, bool bigEndian)
        {
            if (!float.TryParse(floatStr, out float value))
                throw new ArgumentException("无效的浮点数格式");

            byte[] bytes = BitConverter.GetBytes(value);
            if (bigEndian)
                Array.Reverse(bytes);

            return BytesToHexString(bytes, " ");
        }

        /// <summary>
        /// 十六进制转整数
        /// </summary>
        private static string HexToInt(string hex, bool bigEndian)
        {
            byte[] bytes = HexStringToBytes(hex);
            if (bytes.Length < 4)
                throw new ArgumentException("十六进制数据长度不足4字节");

            if (!bigEndian)
                Array.Reverse(bytes, 0, Math.Min(4, bytes.Length));

            // 根据字节数选择合适的转换方式
            if (bytes.Length >= 8)
            {
                long result = BitConverter.ToInt64(bytes, 0);
                return result.ToString();
            }
            else if (bytes.Length >= 4)
            {
                int result = BitConverter.ToInt32(bytes, 0);
                return result.ToString();
            }
            else if (bytes.Length >= 2)
            {
                short result = BitConverter.ToInt16(bytes, 0);
                return result.ToString();
            }
            else
            {
                return bytes[0].ToString();
            }
        }

        /// <summary>
        /// 整数转十六进制
        /// </summary>
        private static string IntToHex(string intStr, bool bigEndian)
        {
            if (!long.TryParse(intStr, out long value))
                throw new ArgumentException("无效的整数格式");

            byte[] bytes;
            if (value >= int.MinValue && value <= int.MaxValue)
            {
                bytes = BitConverter.GetBytes((int)value);
            }
            else
            {
                bytes = BitConverter.GetBytes(value);
            }

            if (bigEndian)
                Array.Reverse(bytes);

            return BytesToHexString(bytes, " ");
        }

        /// <summary>
        /// 十六进制转双精度浮点
        /// </summary>
        private static string HexToDouble(string hex, bool bigEndian)
        {
            byte[] bytes = HexStringToBytes(hex);
            if (bytes.Length < 8)
                throw new ArgumentException("十六进制数据长度不足8字节");

            if (!bigEndian)
                Array.Reverse(bytes, 0, 8);

            double result = BitConverter.ToDouble(bytes, 0);
            return result.ToString("G");
        }

        /// <summary>
        /// 双精度浮点转十六进制
        /// </summary>
        private static string DoubleToHex(string doubleStr, bool bigEndian)
        {
            if (!double.TryParse(doubleStr, out double value))
                throw new ArgumentException("无效的双精度浮点数格式");

            byte[] bytes = BitConverter.GetBytes(value);
            if (bigEndian)
                Array.Reverse(bytes);

            return BytesToHexString(bytes, " ");
        }

        /// <summary>
        /// 获取转换类型的显示名称
        /// </summary>
        public static string GetConvertTypeName(DataConvertType convertType)
        {
            switch (convertType)
            {
                case DataConvertType.None:
                    return LangManager.GetValueByKey("ConvertNone");
                case DataConvertType.HexToFloatBigEndian:
                    return LangManager.GetValueByKey("ConvertHexToFloatBE");
                case DataConvertType.HexToFloatLittleEndian:
                    return LangManager.GetValueByKey("ConvertHexToFloatLE");
                case DataConvertType.FloatToHexBigEndian:
                    return LangManager.GetValueByKey("ConvertFloatToHexBE");
                case DataConvertType.FloatToHexLittleEndian:
                    return LangManager.GetValueByKey("ConvertFloatToHexLE");
                case DataConvertType.HexToIntBigEndian:
                    return LangManager.GetValueByKey("ConvertHexToIntBE");
                case DataConvertType.HexToIntLittleEndian:
                    return LangManager.GetValueByKey("ConvertHexToIntLE");
                case DataConvertType.IntToHexBigEndian:
                    return LangManager.GetValueByKey("ConvertIntToHexBE");
                case DataConvertType.IntToHexLittleEndian:
                    return LangManager.GetValueByKey("ConvertIntToHexLE");
                case DataConvertType.HexToDoubleBigEndian:
                    return LangManager.GetValueByKey("ConvertHexToDoubleBE");
                case DataConvertType.HexToDoubleLittleEndian:
                    return LangManager.GetValueByKey("ConvertHexToDoubleLE");
                case DataConvertType.DoubleToHexBigEndian:
                    return LangManager.GetValueByKey("ConvertDoubleToHexBE");
                case DataConvertType.DoubleToHexLittleEndian:
                    return LangManager.GetValueByKey("ConvertDoubleToHexLE");
                default:
                    return convertType.ToString();
            }
        }
    }
}