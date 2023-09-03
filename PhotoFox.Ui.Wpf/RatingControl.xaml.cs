using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PhotoFox.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for RatingControl.xaml
    /// </summary>
    public partial class RatingControl : UserControl
    {
        public RatingControl()
        {
            InitializeComponent();
        }

        public event Action<int>? ValueChanged;

        public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register(
                "Value",
                typeof(int),
                typeof(RatingControl),
                new FrameworkPropertyMetadata(
                    0, 
                    new PropertyChangedCallback(OnValueChanged), 
                    new CoerceValueCallback(CoerceValueValue)));

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnValueChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var ratingControl = (RatingControl)d;
            SetupStars(ratingControl);
        }

        private static object CoerceValueValue(DependencyObject d, object value)
        {
            var current = (int)value;
            if (current < 0) current = 0;
            if (current > 5) current = 5;
            return current;
        }

        private static void SetupStars(RatingControl ratingsControl)
        {
            var off = (ImageSource)ratingsControl.Resources["off"];
            var on = (ImageSource)ratingsControl.Resources["on"];

            ratingsControl.star1.Source = ratingsControl.Value >= 1 ? on : off;
            ratingsControl.star2.Source = ratingsControl.Value >= 2 ? on : off;
            ratingsControl.star3.Source = ratingsControl.Value >= 3 ? on : off;
            ratingsControl.star4.Source = ratingsControl.Value >= 4 ? on : off;
            ratingsControl.star5.Source = ratingsControl.Value >= 5 ? on : off;
        }

        private void StarMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var value = Convert.ToInt32(((Image)sender).Tag);

            this.SetValue(ValueProperty, value);

            ValueChanged?.Invoke(value);
        }
    }
}
