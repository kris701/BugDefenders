namespace BugDefender.Core.Campaign.Models
{
    public class CutsceneDefinition
    {
        public Guid ID { get; set; }
        public List<ConversationDefinition> Conversation { get; set; }

        public CutsceneDefinition(Guid iD, List<ConversationDefinition> conversation)
        {
            ID = iD;
            Conversation = conversation;
        }
    }
}
