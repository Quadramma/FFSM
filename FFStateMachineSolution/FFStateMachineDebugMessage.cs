using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFStateMachineSolution
{
    public class FFStateMachineDebugMessage
    {
        public DateTime date;
        public FFStateMachineDebugMessageType type;
        public string title;
        public string description;
        public FFStateMachineDebugMessage(FFStateMachineDebugMessageType type, string title, string description) {
            this.type = type;
            this.title = title;
            this.description = description;
            this.date = DateTime.Now;
        }
        public override string ToString()
        {
            return "["+date.ToShortTimeString()+"]["+type.ToString()+"]["+title+"]["+description+"]";
        }
    }
}
