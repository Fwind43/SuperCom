using SuperCom.Config;
using SuperCom.Core.Entity;
using SuperCom.Core.Entity.Enums;
using SuperControls.Style;
using SuperControls.Style.Windows;
using SuperUtils.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SuperCom.Windows
{
    /// <summary>
    /// Window_DataFilter.xaml 的交互逻辑
    /// </summary>
    public partial class Window_DataFilter : BaseWindow
    {
        public ObservableCollection<DataFilterRule> FilterRules { get; set; }
        public DataFilterRule SelectedRule { get; set; }
        public bool IsEditMode { get; set; } = false;
        public long EditRuleID { get; set; } = 0;

        public Window_DataFilter()
        {
            InitializeComponent();
            FilterRules = new ObservableCollection<DataFilterRule>();
            this.DataContext = this;
            LoadMatchTypes();
        }

        private void LoadMatchTypes()
        {
            var matchTypes = new ObservableCollection<KeyValuePair<FilterMatchType, string>>
            {
                new KeyValuePair<FilterMatchType, string>(FilterMatchType.Keyword, LangManager.GetValueByKey("MatchTypeKeyword")),
                new KeyValuePair<FilterMatchType, string>(FilterMatchType.Regex, LangManager.GetValueByKey("MatchTypeRegex")),
                new KeyValuePair<FilterMatchType, string>(FilterMatchType.HexPattern, LangManager.GetValueByKey("MatchTypeHexPattern"))
            };
            cmbMatchType.ItemsSource = matchTypes;
            cmbMatchType.SelectedIndex = 0;
        }

        private void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRules();
        }

        private void LoadRules()
        {
            FilterRules.Clear();
            DataFilterRule.LoadAllRules();
            foreach (var rule in DataFilterRule.AllRules)
            {
                FilterRules.Add(rule);
            }
        }

        private void AddRule(object sender, RoutedEventArgs e)
        {
            IsEditMode = false;
            EditRuleID = 0;
            txtRuleName.Text = "";
            cmbMatchType.SelectedIndex = 0;
            txtMatchPattern.Text = "";
            txtDescription.Text = "";
            editPopup.IsOpen = true;
        }

        private void EditRule(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is long ruleID)
            {
                var rule = FilterRules.FirstOrDefault(r => r.RuleID == ruleID);
                if (rule != null)
                {
                    IsEditMode = true;
                    EditRuleID = ruleID;
                    txtRuleName.Text = rule.RuleName;
                    cmbMatchType.SelectedValue = rule.MatchType;
                    txtMatchPattern.Text = rule.MatchPattern;
                    txtDescription.Text = rule.Description;
                    editPopup.IsOpen = true;
                }
            }
        }

        private void SaveRule(object sender, RoutedEventArgs e)
        {
            string ruleName = txtRuleName.Text.Trim();
            string matchPattern = txtMatchPattern.Text.Trim();

            if (string.IsNullOrEmpty(ruleName))
            {
                MessageNotify.Warning(LangManager.GetValueByKey("RuleNameRequired"));
                return;
            }

            if (string.IsNullOrEmpty(matchPattern))
            {
                MessageNotify.Warning(LangManager.GetValueByKey("MatchPatternRequired"));
                return;
            }

            FilterMatchType matchType = (FilterMatchType)cmbMatchType.SelectedValue;

            DataFilterRule rule;
            if (IsEditMode && EditRuleID > 0)
            {
                rule = FilterRules.FirstOrDefault(r => r.RuleID == EditRuleID);
                if (rule != null)
                {
                    rule.RuleName = ruleName;
                    rule.MatchType = matchType;
                    rule.MatchPattern = matchPattern;
                    rule.Description = txtDescription.Text;
                    rule.Save();
                }
            }
            else
            {
                rule = new DataFilterRule(ruleName, matchType, matchPattern)
                {
                    Description = txtDescription.Text,
                    IsEnabled = true
                };
                rule.Save();
                FilterRules.Add(rule);
                DataFilterRule.AllRules.Add(rule);
            }

            editPopup.IsOpen = false;
            MessageNotify.Success(LangManager.GetValueByKey("SaveSuccess"));
            App.Logger.Info($"save filter rule: {ruleName}");
        }

        private void CancelEdit(object sender, RoutedEventArgs e)
        {
            editPopup.IsOpen = false;
        }

        private void DeleteRule(object sender, RoutedEventArgs e)
        {
            if (SelectedRule == null)
            {
                MessageNotify.Warning(LangManager.GetValueByKey("PleaseSelectRule"));
                return;
            }

            if (!(bool)new MsgBox(LangManager.GetValueByKey("ConfirmDelete")).ShowDialog(this))
                return;

            SelectedRule.Delete();
            FilterRules.Remove(SelectedRule);
            DataFilterRule.AllRules.Remove(SelectedRule);
            SelectedRule = null;
            MessageNotify.Success(LangManager.GetValueByKey("DeleteSuccess"));
            App.Logger.Info($"delete filter rule");
        }

        private void TestFilter(object sender, RoutedEventArgs e)
        {
            // 打开测试窗口
            Window_FilterTest testWindow = new Window_FilterTest();
            testWindow.Owner = this;
            testWindow.ShowDialog();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BaseWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            editPopup.IsOpen = false;
        }
    }
}