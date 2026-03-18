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
    /// 数据筛选规则，用于筛选导出特定格式的数据
    /// </summary>
    [Table(tableName: "data_filter_rule")]
    public class DataFilterRule : ViewModelBase
    {
        #region "静态属性"

        public static List<DataFilterRule> AllRules { get; set; } = new List<DataFilterRule>();

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
        /// 匹配模式（正则表达式或关键字）
        /// </summary>
        public string MatchPattern {
            get { return _MatchPattern; }
            set { _MatchPattern = value; RaisePropertyChanged(); }
        }

        private int _DataOffset;
        /// <summary>
        /// 数据偏移量（从匹配位置开始计算）
        /// </summary>
        public int DataOffset {
            get { return _DataOffset; }
            set { _DataOffset = value; RaisePropertyChanged(); }
        }

        private int _DataLength;
        /// <summary>
        /// 数据长度（0表示到行尾）
        /// </summary>
        public int DataLength {
            get { return _DataLength; }
            set { _DataLength = value; RaisePropertyChanged(); }
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

        public DataFilterRule()
        {
        }

        public DataFilterRule(string ruleName, FilterMatchType matchType, string matchPattern, int dataOffset = 0, int dataLength = 0)
        {
            RuleName = ruleName;
            MatchType = matchType;
            MatchPattern = matchPattern;
            DataOffset = dataOffset;
            DataLength = dataLength;
        }

        /// <summary>
        /// 检查一行数据是否匹配规则
        /// </summary>
        public bool IsMatch(string line)
        {
            if (string.IsNullOrEmpty(line) || string.IsNullOrEmpty(MatchPattern))
                return false;

            try {
                if (MatchType == FilterMatchType.Regex) {
                    return Regex.IsMatch(line, MatchPattern);
                } else if (MatchType == FilterMatchType.Keyword) {
                    return line.Contains(MatchPattern);
                } else if (MatchType == FilterMatchType.HexPattern) {
                    // 十六进制模式匹配（忽略空格和大小写）
                    string normalizedLine = line.Replace(" ", "").ToUpper();
                    string normalizedPattern = MatchPattern.Replace(" ", "").ToUpper();
                    return normalizedLine.Contains(normalizedPattern);
                }
            } catch (Exception ex) {
                App.Logger.Error($"Filter rule match error: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// 从匹配行中提取数据
        /// </summary>
        public string ExtractData(string line)
        {
            if (!IsMatch(line))
                return null;

            if (DataLength == 0) {
                // 提取到行尾
                return line.Substring(Math.Min(DataOffset, line.Length));
            }

            int startIndex = DataOffset;
            if (startIndex >= line.Length)
                return null;

            int length = Math.Min(DataLength, line.Length - startIndex);
            return line.Substring(startIndex, length);
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
                    "data_filter_rule",
                    "create table if not exists data_filter_rule( " +
                        "RuleID INTEGER PRIMARY KEY autoincrement, " +
                        "RuleName VARCHAR(200), " +
                        "MatchType INTEGER DEFAULT 0, " +
                        "MatchPattern TEXT, " +
                        "DataOffset INTEGER DEFAULT 0, " +
                        "DataLength INTEGER DEFAULT 0, " +
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
            SqliteMapper<DataFilterRule> mapper = new SqliteMapper<DataFilterRule>(ConfigManager.SQLITE_DATA_PATH);
            foreach (var item in SqliteTable.Table.Keys) {
                if (!mapper.IsTableExists(item)) {
                    mapper.CreateTable(item, SqliteTable.Table[item]);
                }
            }
        }

        public static void LoadAllRules()
        {
            SqliteMapper<DataFilterRule> mapper = new SqliteMapper<DataFilterRule>(ConfigManager.SQLITE_DATA_PATH);
            AllRules = mapper.SelectList();
        }

        public bool Save()
        {
            SqliteMapper<DataFilterRule> mapper = new SqliteMapper<DataFilterRule>(ConfigManager.SQLITE_DATA_PATH);
            if (RuleID > 0) {
                return mapper.UpdateById(this) > 0;
            } else {
                return mapper.Insert(this);
            }
        }

        public bool Delete()
        {
            SqliteMapper<DataFilterRule> mapper = new SqliteMapper<DataFilterRule>(ConfigManager.SQLITE_DATA_PATH);
            return mapper.DeleteById(RuleID) > 0;
        }

        #endregion
    }
}