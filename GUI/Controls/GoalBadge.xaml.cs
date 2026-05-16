using System.Windows;
using System.Windows.Controls;

namespace FitTrack.UI.Controls
{
    public partial class GoalBadge : UserControl
    {
        public static readonly DependencyProperty GoalNameProperty =
            DependencyProperty.Register("GoalName", typeof(string), typeof(GoalBadge), new PropertyMetadata(string.Empty));

        public string GoalName
        {
            get { return (string)GetValue(GoalNameProperty); }
            set { SetValue(GoalNameProperty, value); }
        }

        public GoalBadge()
        {
            InitializeComponent();
        }
    }
}
