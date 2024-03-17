using BugDefender.Core.Campaign.Models;
using System.Windows;
using System.Windows.Controls;

namespace CampaignDesigner.UserControls
{
    public partial class CutsceneControl : UserControl
    {
        public CutsceneDefinition Cutscene { get; }
        public Dictionary<Guid, string> Speakers { get; }

        private bool _isLoaded;

        public CutsceneControl(CutsceneDefinition from, Dictionary<Guid, string> speakers)
        {
            Cutscene = from;
            Speakers = speakers;
            InitializeComponent();
            SceneIDTextbox.Text = from.ID.ToString();
            foreach (var speaker in Speakers.Keys)
                SpeakerComboBox.Items.Add(new ComboBoxItem()
                {
                    Content = $"{Speakers[speaker]} ({speaker})",
                    Tag = speaker
                });
            UpdateView();
        }

        private void NewSceneIDButton_Click(object sender, RoutedEventArgs e)
        {
            SceneIDTextbox.Text = Guid.NewGuid().ToString();
        }

        private void SceneIDTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded)
                return;
            var newID = new Guid();
            if (Guid.TryParse(SceneIDTextbox.Text, out newID))
                Cutscene.ID = newID;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
        }

        private void NewConversationButton_Click(object sender, RoutedEventArgs e)
        {
            if (SpeakerComboBox.SelectedItem is ComboBoxItem item && item.Tag is Guid guid)
            {
                if (!Speakers.ContainsKey(guid))
                    return;
                Cutscene.Conversation.Add(new ConversationDefinition(guid, "Text"));
                UpdateView();
            }
        }

        private void UpdateView()
        {
            ConversationPanel.Children.Clear();
            var index = 0;
            foreach (var convo in Cutscene.Conversation)
                ConversationPanel.Children.Add(new ConversationControl(Cutscene.Conversation, index++, UpdateView, Speakers));
        }
    }
}
