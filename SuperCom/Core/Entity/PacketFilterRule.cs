using SuperCom.Config;
using SuperCom.Core.Entity.Enums;
using SuperUtils.Common;
using SuperUtils.Framework.ORM.Attributes;
using SuperUtils.Framework.ORM.Enums;
using SuperUtils.Framework.ORM.Mapper;
using SuperUtils.WPF.VieModel;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SuperCom.Core.Entity
{
    /// <summary>
    /// 数据包过滤规则，用于过滤接收的数据包
    /// </summary>
    [Table(tableName: "packet_filter_rule")]
    public class PacketFilterRule : ViewModelBase
    {
        #region "静态属性"

        public static List<PacketFilterRule> AllRules { get; set; } = new List<PacketFilterRule>();

        #endregion

        #region "属性"

        [TableId(IdType.AUTO)]
        public long RuleID { get; set; }

        private string _RuleName;
        public string RuleName {
            get { return _RuleName; }
            set { _RuleName = value; RaisePropertyChanged(); }
        }

        private FilterMatchType _MatchType;
        public FilterMatchType MatchType {
            get { return _MatchType; }
            set { _MatchType = value; RaisePropertyChanged(); }
        }

        private string _MatchPattern;
        /// <summary>
        /// 匹配模式（如 "AA 55 07"）
        /// </summary>
        public string MatchPattern {
            get { return _MatchPattern; }
            set { _MatchPattern = value; RaisePropertyChanged(); }
        }

        private FilterAction _Action;
        /// <summary>
        /// 过滤动作（保留或丢弃）
        /// </summary>
        public FilterAction Action {
            get { return _Action; }
            set { _Action = value; RaisePropertyChanged(); }
        }

        private int _Enabled = 1;
        public int Enabled {
            get { return _Enabled; }
            set { _Enabled = value; RaisePropertyChanged(); }
        }

        [TableField(exist: false)]
        public bool IsEnabled {
            get { return _Enabled == 1; }
            set { _Enabled = value ? 1 : 0; RaisePropertyChanged(); }
        }

        private string _Description;
        public string Description {
            get { return _Description; }
            set { _Description = value; RaisePropertyChanged(); }
        }

        #endregion

        public PacketFilterRule()
        {
        }

        public PacketFilterRule(string ruleName, FilterMatchType matchType, string matchPattern, FilterAction action = FilterAction.Keep)
        {
            RuleName = ruleName;
            MatchType = matchType;
            MatchPattern = matchPattern;
            Action = action;
        }

        /// <summary>
        /// 检查数据是否匹配规则
        /// </summary>
        public bool IsMatch(byte[] data)
        {
            if (data == null || data.Length == 0 || string.IsNullOrEmpty(MatchPattern))
                return false;

            try
            {
                if (MatchType == FilterMatchType.HexPattern)
                {
                    // 十六进制模式匹配
                    string hexData = BitConverter.ToString(data).Replace("-", " ").ToUpper();
                    string pattern = MatchPattern.Replace(" ", "").ToUpper();
                    string normalizedData = hexData.Replace(" ", "");
                    return normalizedData.Contains(pattern);
                }
                else if (MatchType == FilterMatchType.Keyword)
                {
                    // 关键字匹配（将数据转为字符串）
                    string text = System.Text.Encoding.UTF8.GetString(data);
                    return text.Contains(MatchPattern);
                }
                else if (MatchType == FilterMatchType.Regex)
                {
                    // 正则表达式匹配
                    string text = System.Text.Encoding.UTF8.GetString(data);
                    return Regex.IsMatch(text, MatchPattern);
                }
            }
            catch (Exception ex)
            {
                App.Logger.Error($"Packet filter rule match error: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// 检查数据是否应该被过滤（丢弃）
        /// </summary>
        public bool ShouldFilter(byte[] data)
        {
            if (!IsEnabled)
                return false;

            bool isMatch = IsMatch(data);
            
            // 如果动作是丢弃且匹配成功，则过滤
            if (Action == FilterAction.Discard && isMatch)
                return true;

            // 如果动作是保留且匹配失败，则过滤
            if (Action == FilterAction.Keep && !isMatch)
                return true;

            return false;
        }

        public override void Init()
        {
        }

        #region "数据库操作"

        public static class SqliteTable
        {
            public static Dictionary<string, string> Table = new Dictionary<string, string>()
            {
                {
                    "packet_filter_rule",
                    "create table if not exists packet_filter_rule( " +
                        "RuleID INTEGER PRIMARY KEY autoincrement, " +
                        "RuleName VARCHAR(200), " +
                        "MatchType INTEGER DEFAULT 0, " +
                        "MatchPattern TEXT, " +
                        "Action INTEGER DEFAULT 0, " +
                        "Enabled INTEGER DEFAULT 1, " +
                        "Description TEXT, " +
                        "CreateDate VARCHAR(30) DEFAULT(STRFTIME('%Y-%m-%d %H:%M:%S', 'NOW', 'localtime')), " +
                        "UpdateDate VARCHAR(30) DEFAULT(STRFTIME('%Y-%m-%d %H:%M:%S', 'NOW', 'localtime')) " +
                    ");"
                }
            };
        }

        public static void InitSqlite()
        {
            SqliteMapper<PacketFilterRule> mapper = new SqliteMapper<PacketFilterRule>(ConfigManager.SQLITE_DATA_PATH);
            foreach (var item in SqliteTable.Table.Keys)
            {
                if (!mapper.IsTableExists(item))
                {
                    mapper.CreateTable(item, SqliteTable.Table[item]);
                }
            }
        }

        public static void LoadAllRules()
        {
            SqliteMapper<PacketFilterRule> mapper = new SqliteMapper<PacketFilterRule>(ConfigManager.SQLITE_DATA_PATH);
            AllRules = mapper.SelectList();
        }

        public bool Save()
        {
            SqliteMapper<PacketFilterRule> mapper = new SqliteMapper<PacketFilterRule>(ConfigManager.SQLITE_DATA_PATH);
            if (RuleID > 0)
            {
                return mapper.UpdateById(this) > 0;
            }
            else
            {
                return mapper.Insert(this);
            }
        }

        public bool Delete()
        {
            SqliteMapper<PacketFilterRule> mapper = new SqliteMapper<PacketFilterRule>(ConfigManager.SQLITE_DATA_PATH);
            return mapper.DeleteById(RuleID) > 0;
        }

        #endregion
    }
}