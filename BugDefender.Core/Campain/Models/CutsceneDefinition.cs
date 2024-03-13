using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.Core.Campain.Models
{
    public class CutsceneDefinition
    {
        public List<ConversationDefinition> Conversation { get; set; }

        public CutsceneDefinition(List<ConversationDefinition> conversation)
        {
            Conversation = conversation;
        }
    }
}
