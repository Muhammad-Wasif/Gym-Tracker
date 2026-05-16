using System.Windows;
using FitTrack.Database;
using FitTrack.Models;
using FitTrack.UI.Views;

namespace FitTrack.UI
{
    public partial class App : Application
    {
        public static Person? CurrentUser { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize database connection and seed data
            string connectionString = "Data Source=localhost;Initial Catalog=FitTrackOOP;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;Command Timeout=2147483647";
            DatabaseHelper.Initialise(connectionString);
            DatabaseSeeder.Seed();

            var login = new LoginWindow();
            login.Show();
        }

        public static void NavigateToMain()
        {
            var main = new MainWindow();
            main.Show();
            foreach (Window w in Current.Windows)
            {
                if (w is LoginWindow)
                {
                    w.Close();
                    break;
                }
            }
        }

        public static void Logout()
        {
            CurrentUser = null;
            var login = new LoginWindow();
            login.Show();
            foreach (Window w in Current.Windows)
            {
                if (w is MainWindow)
                {
                    w.Close();
                    break;
                }
            }
        }
    }
}
