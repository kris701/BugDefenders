using BugDefender.Core.Campaign.Models;
using System.Windows;
using System.Windows.Controls;

namespace CampaignDesigner.UserControls
{
    public partial class SpeakersControl : UserControl
    {
        public CampaignDefinition Campaign { get; }

        public SpeakersControl(CampaignDefinition campaign)
        {
            Campaign = campaign;
            InitializeComponent();

            ReloadSpeakersList();
        }

        private void AddSpeakerButton_Click(object sender, RoutedEventArgs e)
        {
            SpeakersPanel.Children.Add(new SpeakerControl(SpeakersPanel.Children, UpdateSpeakerDict, Guid.NewGuid(), "New Speaker"));
        }

        private void UpdateSpeakerDict()
        {
            var allIDs = new List<Guid>();
            foreach (var child in SpeakersPanel.Children)
                if (child is SpeakerControl speaker)
                    allIDs.Add(speaker.SpeakerID);
            foreach (var used in GetAllUsedSpeakers())
            {
                if (!allIDs.Contains(used))
                {
                    MessageBox.Show("Cannot remove speakers that are used elsewhere!");
                    ReloadSpeakersList();
                    return;
                }
            }

            Campaign.Speakers.Clear();
            foreach (var child in SpeakersPanel.Children)
            {
                if (child is SpeakerControl speaker)
                {
                    Campaign.Speakers.Add(speaker.SpeakerID, speaker.SpeakerName);
                }
            }
        }

        private void ReloadSpeakersList()
        {
            SpeakersPanel.Children.Clear();
            foreach (var speaker in Campaign.Speakers.Keys)
                SpeakersPanel.Children.Add(new SpeakerControl(SpeakersPanel.Children, UpdateSpeakerDict, speaker, Campaign.Speakers[speaker]));
        }

        private List<Guid> GetAllUsedSpeakers()
        {
            var retList = new HashSet<Guid>();
            foreach (var convo in Campaign.CampaignOver.Conversation)
                retList.Add(convo.SpeakerID);
            foreach (var chapter in Campaign.Chapters)
                foreach (var convo in chapter.Intro.Conversation)
                    retList.Add(convo.SpeakerID);
            return retList.ToList();
        }
    }
}
