namespace BugDefender.Core.Campaign.Models
{
    public class ConversationDefinition
    {
        public Guid SpeakerID { get; set; }
        public string From { get; set; }
        public string Text { get; set; }

        public ConversationDefinition(Guid speakerID, string from, string text)
        {
            SpeakerID = speakerID;
            From = from;
            Text = text;
        }
    }
}
