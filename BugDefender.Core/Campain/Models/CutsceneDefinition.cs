using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.Core.Campain.Models
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
