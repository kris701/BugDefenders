using System.Windows;
using System.Windows.Controls;

namespace CampaignDesigner.UserControls
{
    public partial class SpeakerControl : UserControl
    {
        public Guid SpeakerID => Guid.Parse(IDText.Text);
        public string SpeakerName => SpeakerLabel.Text;

        private readonly UIElementCollection _from;
        private bool _isLoaded;
        private readonly Action _save;
        public SpeakerControl(UIElementCollection from, Action save, Guid speaker, string name)
        {
            _from = from;
            _save = save;
            InitializeComponent();
            SpeakerLabel.Text = name;
            IDText.Text = speaker.ToString();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _from.Remove(this);
            _save.Invoke();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
        }

        private void IDText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded)
                return;
            _save.Invoke();
        }

        private void SpeakerLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded)
                return;
            _save.Invoke();
        }
    }
}
