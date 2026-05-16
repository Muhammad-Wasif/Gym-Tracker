using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace FitTrack.UI.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is true ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class InverseBoolToVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is true ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value == null ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class GoalToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var goal = value as string;
            var key = goal switch
            {
                "Weight Loss" => "GoalWeightLoss",
                "Fat Loss" => "GoalFatLoss",
                "Muscle Gain" => "GoalMuscleGain",
                "Weight Gain" => "GoalWeightGain",
                "Maintain" => "GoalMaintain",
                _ => "TextSecBrush"
            };
            return Application.Current.Resources[key] as Brush ?? Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class GoalToLightColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = new GoalToColorConverter().Convert(value, targetType, parameter, culture) as SolidColorBrush;
            if (brush == null) return Brushes.Transparent;
            var c = brush.Color;
            return new SolidColorBrush(Color.FromArgb((byte)(255 * 0.2), c.R, c.G, c.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class RoleToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var role = value as string;
            var key = role switch
            {
                "Admin" => "RedBrush",
                "Trainer" => "AmberBrush",
                _ => "BrandBrush"
            };
            return Application.Current.Resources[key] as Brush ?? Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class DeltaToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || !(values[0] is double delta) || !(values[1] is bool positiveIsGood))
                return Application.Current.Resources["TextSecBrush"];

            if (delta == 0) return Application.Current.Resources["TextSecBrush"];
            
            bool isGood = (delta > 0 && positiveIsGood) || (delta < 0 && !positiveIsGood);
            return Application.Current.Resources[isGood ? "GreenBrush" : "RedBrush"] as Brush ?? Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class PercentageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || !(values[0] is double actual) || !(values[1] is double target))
                return 0.0;
            
            if (target <= 0) return 0.0;
            var pct = actual / target;
            return pct > 1.0 ? 1.0 : pct;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class InitialsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var name = value as string;
            if (string.IsNullOrWhiteSpace(name)) return "";
            var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpper();
            return (parts[0].Substring(0, 1) + parts[parts.Length - 1].Substring(0, 1)).ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class DateToGreetingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
            {
                var hour = dt.Hour;
                if (hour < 12) return "morning";
                if (hour < 18) return "afternoon";
                return "evening";
            }
            return "day";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class MacroToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var macro = value as string;
            var key = macro switch
            {
                "Protein" => "ProteinBrush",
                "Carbs" => "CarbsBrush",
                "Fat" => "FatBrush",
                _ => "TextSecBrush"
            };
            return Application.Current.Resources[key] as Brush ?? Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
