using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDGame.Core.Resources.Integrity
{
    public class IntegrityError
    {
        public enum SeverityLevel { None, Message, Critical }
        public string Error { get; set; }
        public SeverityLevel Severity { get; set; }

        public IntegrityError(string error, SeverityLevel severity)
        {
            Error = error;
            Severity = severity;
        }

        public override string? ToString()
        {
            return $"[{Enum.GetName(typeof(SeverityLevel), Severity)}]\t{Error}";
        }
    }
}
