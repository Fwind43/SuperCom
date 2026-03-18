namespace SuperCom.Core.Entity.Enums
{
    /// <summary>
    /// 数据转换类型
    /// </summary>
    public enum DataConvertType
    {
        /// <summary>
        /// 无转换
        /// </summary>
        None = 0,

        /// <summary>
        /// 十六进制转浮点数（大端）
        /// </summary>
        HexToFloatBigEndian = 1,

        /// <summary>
        /// 十六进制转浮点数（小端）
        /// </summary>
        HexToFloatLittleEndian = 2,

        /// <summary>
        /// 浮点数转十六进制（大端）
        /// </summary>
        FloatToHexBigEndian = 3,

        /// <summary>
        /// 浮点数转十六进制（小端）
        /// </summary>
        FloatToHexLittleEndian = 4,

        /// <summary>
        /// 十六进制转整数（大端）
        /// </summary>
        HexToIntBigEndian = 5,

        /// <summary>
        /// 十六进制转整数（小端）
        /// </summary>
        HexToIntLittleEndian = 6,

        /// <summary>
        /// 整数转十六进制（大端）
        /// </summary>
        IntToHexBigEndian = 7,

        /// <summary>
        /// 整数转十六进制（小端）
        /// </summary>
        IntToHexLittleEndian = 8,

        /// <summary>
        /// 十六进制转双精度浮点（大端）
        /// </summary>
        HexToDoubleBigEndian = 9,

        /// <summary>
        /// 十六进制转双精度浮点（小端）
        /// </summary>
        HexToDoubleLittleEndian = 10,

        /// <summary>
        /// 双精度浮点转十六进制（大端）
        /// </summary>
        DoubleToHexBigEndian = 11,

        /// <summary>
        /// 双精度浮点转十六进制（小端）
        /// </summary>
        DoubleToHexLittleEndian = 12
    }
}