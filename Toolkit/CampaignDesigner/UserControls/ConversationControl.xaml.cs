using BugDefender.Core.Campaign.Models;
using System.Windows;
using System.Windows.Controls;

namespace CampaignDesigner.UserControls
{
    public partial class ConversationControl : UserControl
    {
        public ConversationDefinition Conversation { get; }
        public Dictionary<Guid, string> Speakers { get; }
        private bool _isLoaded;
        private readonly Action _update;
        private readonly List<ConversationDefinition> _conversations;

        public ConversationControl(List<ConversationDefinition> conversations, int index, Action update, Dictionary<Guid, string> speakers)
        {
            _conversations = conversations;
            Conversation = conversations[index];
            Speakers = speakers;
            _update = update;
            InitializeComponent();
            int count = 0;
            foreach (var speaker in Speakers.Keys)
            {
                SpeakerComboBox.Items.Add(new ComboBoxItem()
                {
                    Content = $"{Speakers[speaker]} ({speaker})",
                    Tag = speaker
                });
                if (Conversation.SpeakerID == speaker)
                    SpeakerComboBox.SelectedIndex = count;
                count++;
            }
            TextTextbox.Text = Conversation.Text;
        }

        private void TextTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded)
                return;
            Conversation.Text = TextTextbox.Text;
        }

        private void SpeakerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded)
                return;
            if (sender is ComboBoxItem item && item.Tag is Guid guid)
                Conversation.SpeakerID = guid;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            var indexOf = _conversations.IndexOf(Conversation);
            if (indexOf == 0)
                return;
            _conversations.Remove(Conversation);
            _conversations.Insert(indexOf - 1, Conversation);
            _update.Invoke();
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            var indexOf = _conversations.IndexOf(Conversation);
            if (indexOf == _conversations.Count - 1)
                return;
            _conversations.Remove(Conversation);
            _conversations.Insert(indexOf + 1, Conversation);
            _update.Invoke();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _conversations.Remove(Conversation);
            _update.Invoke();
        }
    }
}
