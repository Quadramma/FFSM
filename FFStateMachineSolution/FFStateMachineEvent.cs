using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFStateMachineSolution
{
    public class FFStateMachineEvent
    {
        public FFStateMachineEventType type;
        public string name;
        public System.Action<object, Dictionary<string, object>> action;
        public FFStateMachineEvent(FFStateMachineEventType type, string name, System.Action<object, Dictionary<string, object>> action)
        {
            this.type = type;
            this.action = action;
            this.name = name;
        }
    }
}
