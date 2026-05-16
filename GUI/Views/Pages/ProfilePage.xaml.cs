using FitTrack.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FitTrack.UI.Views.Pages
{
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            Loaded += ProfilePage_Loaded;
        }

        private void ProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            var glow = FindResource("GlowPulse") as Storyboard;
            glow?.Begin(AvatarRing);
        }

        private void PbCurrent_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileViewModel vm) vm.CurrentPassword = PbCurrent.Password;
        }

        private void PbNew_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileViewModel vm) vm.NewPassword = PbNew.Password;
        }

        private void PbConfirm_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileViewModel vm) vm.ConfirmNewPassword = PbConfirm.Password;
        }
    }
}
