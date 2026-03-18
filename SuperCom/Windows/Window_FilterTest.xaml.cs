using SuperCom.Core.Entity;
using SuperControls.Style;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace SuperCom.Windows
{
    /// <summary>
    /// Window_FilterTest.xaml 的交互逻辑
    /// </summary>
    public partial class Window_FilterTest : BaseWindow
    {
        public ObservableCollection<DataFilterRule> Rules { get; set; }

        public Window_FilterTest()
        {
            InitializeComponent();
            Rules = new ObservableCollection<DataFilterRule>();
            this.DataContext = this;
            LoadRules();
        }

        private void LoadRules()
        {
            Rules.Clear();
            foreach (var rule in DataFilterRule.AllRules)
            {
                Rules.Add(rule);
            }
            if (Rules.Count > 0)
            {
                cmbRules.SelectedIndex = 0;
            }
        }

        private void RunTest(object sender, RoutedEventArgs e)
        {
            if (cmbRules.SelectedItem is DataFilterRule rule)
            {
                string input = txtInput.Text;
                if (string.IsNullOrEmpty(input))
                {
                    MessageNotify.Warning(LangManager.GetValueByKey("PleaseInputData"));
                    return;
                }

                string[] lines = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                System.Text.StringBuilder result = new System.Text.StringBuilder();

                int matchCount = 0;
                foreach (string line in lines)
                {
                    if (rule.IsMatch(line))
                    {
                        result.AppendLine(line);
                        matchCount++;
                    }
                }

                txtOutput.Text = result.ToString();
                MessageNotify.Info(string.Format(LangManager.GetValueByKey("MatchResultCount"), matchCount));
            }
        }
    }
}