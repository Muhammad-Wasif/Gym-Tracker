using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitTrack.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace FitTrack.UI.ViewModels
{
    public partial class AdminViewModel : BaseViewModel
    {
        public ObservableCollection<Person> AllUsers { get; } = new ObservableCollection<Person>();
        
        [ObservableProperty] private Person? _selectedUser;
        [ObservableProperty] private string _userDetailsText = string.Empty;

        public AdminViewModel()
        {
            if (App.CurrentUser != null) LoadData();
        }

        private void LoadData()
        {
            var list = _users.GetAll();
            AllUsers.Clear();
            foreach (var u in list) AllUsers.Add(u);
        }

        partial void OnSelectedUserChanged(Person? value)
        {
            if (value != null)
            {
                UserDetailsText = $"Person ID: {value.PersonId}\nName: {value.FullName}\nUsername: {value.Username}\nEmail: {value.Email}\nRole: {value.Role}\nCreated At: {value.CreatedAt}";
            }
            else
            {
                UserDetailsText = string.Empty;
            }
        }

        [RelayCommand]
        private void DeleteUser()
        {
            if (SelectedUser == null)
            {
                SetError("Please select a user first.");
                return;
            }
            if (SelectedUser.PersonId == App.CurrentUser!.PersonId)
            {
                SetError("Cannot delete yourself.");
                return;
            }
            
            var result = _users.DeleteUser(SelectedUser.PersonId);
            if (result.success)
            {
                AllUsers.Remove(SelectedUser);
                SelectedUser = null;
                SetSuccess("User deleted successfully.");
            }
            else
            {
                SetError(result.message);
            }
        }
    }
}
