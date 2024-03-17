using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MapDesigner.UserControls
{
    public partial class BlockingTileControl : UserControl
    {
        public float BlockX => float.Parse(XTextbox.Text);
        public float BlockY => float.Parse(YTextbox.Text);
        public float BlockWidth => float.Parse(WidthTextbox.Text);
        public float BlockHeight => float.Parse(HeightTextbox.Text);

        private readonly UIElementCollection _from;
        private readonly Action _update;
        private bool _loaded;

        public BlockingTileControl(UIElementCollection from, Action update)
        {
            _from = from;
            _update = update;
            InitializeComponent();
        }

        public BlockingTileControl(UIElementCollection from, Action update, float x, float y, float width, float height) : this(from, update)
        {
            XTextbox.Text = $"{x}";
            YTextbox.Text = $"{y}";
            WidthTextbox.Text = $"{width}";
            HeightTextbox.Text = $"{height}";
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
