using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitTrack.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FitTrack.UI.ViewModels
{
    public partial class ProgressViewModel : BaseViewModel
    {
        [ObservableProperty] private IEnumerable<(DateTime date, double value)> _weightDataPoints = Enumerable.Empty<(DateTime, double)>();
        
        public ObservableCollection<ProgressSnapshot> Snapshots { get; } = new ObservableCollection<ProgressSnapshot>();

        public ProgressViewModel()
        {
            if (App.CurrentUser != null) LoadData();
        }

        private void LoadData()
        {
            var user = App.CurrentUser!;
            var allSnaps = _progress.GetSnapshots(user.PersonId, 100);
            
            Snapshots.Clear();
            foreach (var s in allSnaps) Snapshots.Add(s);

            if (allSnaps.Any())
            {
                var points = allSnaps.Select(s => (s.SnapshotDate, s.WeightKg)).OrderBy(p => p.SnapshotDate).ToList();
                WeightDataPoints = points;
            }
            else
            {
                // Add dummy data for visual if empty
                WeightDataPoints = new List<(DateTime, double)>
                {
                    (DateTime.Now.AddDays(-30), user.WeightKg + 2),
                    (DateTime.Now.AddDays(-20), user.WeightKg + 1),
                    (DateTime.Now.AddDays(-10), user.WeightKg + 0.5),
                    (DateTime.Now, user.WeightKg)
                };
            }
        }

        [RelayCommand]
        private void LogProgress()
        {
            var dlg = new FitTrack.UI.Dialogs.LogProgressDialog();
            if (dlg.ShowDialog() == true) LoadData();
        }
    }
}
