namespace BugDefender.Core.Campaign.Models
{
    public class ConversationDefinition
    {
        public Guid SpeakerID { get; set; }
        public string Text { get; set; }

        public ConversationDefinition(Guid speakerID, string text)
        {
            SpeakerID = speakerID;
            Text = text;
        }
    }
}
