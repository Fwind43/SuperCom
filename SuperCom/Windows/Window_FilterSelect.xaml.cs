using SuperCom.Core.Entity;
using System.Collections.ObjectModel;
using System.Windows;

namespace SuperCom.Windows
{
    /// <summary>
    /// Window_FilterSelect.xaml 的交互逻辑
    /// </summary>
    public partial class Window_FilterSelect
    {
        public ObservableCollection<DataFilterRule> FilterRules { get; set; }
        public DataFilterRule SelectedRule { get; set; }

        public Window_FilterSelect()
        {
            InitializeComponent();
            FilterRules = new ObservableCollection<DataFilterRule>();
            this.DataContext = this;
            LoadRules();
        }

        private void LoadRules()
        {
            FilterRules.Clear();
            foreach (var rule in DataFilterRule.AllRules)
            {
                if (rule.IsEnabled)
                {
                    FilterRules.Add(rule);
                }
            }
            if (FilterRules.Count > 0)
            {
                listBox.SelectedIndex = 0;
            }
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            if (SelectedRule == null)
            {
                return;
            }
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}