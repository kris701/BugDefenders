using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugDefender.Core.Campain.Models
{
    public class ConversationDefinition
    {
        public string From { get; set; }
        public string Text { get; set; }

        public ConversationDefinition(string from, string text)
        {
            From = from;
            Text = text;
        }
    }
}
