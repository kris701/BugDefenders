using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MapDesigner.UserControls
{
    public partial class PathControl : UserControl
    {
        public float WaypointX => float.Parse($"0{XTextbox.Text}");
        public float WaypointY => float.Parse($"0{YTextbox.Text}");

        private readonly UIElementCollection _from;
        private readonly Action _update;
        private bool _loaded;

        public PathControl(UIElementCollection from, Action update)
        {
            _from = from;
            _update = update;
            InitializeComponent();
        }

        public PathControl(UIElementCollection from, Action update, float x, float y) : this(from, update)
        {
            XTextbox.Text = $"{x}";
            YTextbox.Text = $"{y}";
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _from.Remove(this);
        }

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_loaded)
                _update.Invoke();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;
        }
    }
}
