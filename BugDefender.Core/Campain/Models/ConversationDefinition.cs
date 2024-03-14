using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.Core.Campain.Models
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
