using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitTrack.UI.Views;
using System.Windows;

namespace FitTrack.UI.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        [RelayCommand]
        private void Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                SetError("Please enter both username and password.");
                return;
            }

            IsLoading = true;
            ClearMessages();

            var result = _auth.Login(Username, Password);
            IsLoading = false;

            if (result.success && result.person != null)
            {
                App.CurrentUser = result.person;
                App.NavigateToMain();
            }
            else
            {
                SetError(result.message ?? "Login failed. Please try again.");
            }
        }

        [RelayCommand]
        private void NavigateToRegister()
        {
            var register = new RegisterWindow();
            register.Show();
            
            foreach (Window w in Application.Current.Windows)
            {
                if (w is LoginWindow loginWindow)
                {
                    loginWindow.Close();
                    break;
                }
            }
        }
    }
}
