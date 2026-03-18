namespace SuperCom.Core.Entity.Enums
{
    /// <summary>
    /// 筛选匹配类型
    /// </summary>
    public enum FilterMatchType
    {
        /// <summary>
        /// 关键字匹配
        /// </summary>
        Keyword = 0,

        /// <summary>
        /// 正则表达式匹配
        /// </summary>
        Regex = 1,

        /// <summary>
        /// 十六进制模式匹配
        /// </summary>
        HexPattern = 2
    }
}