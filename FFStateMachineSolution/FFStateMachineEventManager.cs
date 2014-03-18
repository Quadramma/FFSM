using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFStateMachineSolution
{

    public class FFStateMachineEventManager
    {
        public bool debugMode = false;
        public List<FFStateMachineEvent> evts;
        public FFStateMachine machine;
        public FFStateMachineEventManager(FFStateMachine machine)
        {
            evts = new List<FFStateMachineEvent>();
            this.machine = machine;
        }
        public void bind(FFStateMachineEvent evt)
        {
            evts.Add(evt);
        }

        public void trigger(FFStateMachineEventType type, string name, object sender, Dictionary<string, object> args)
        {
            foreach (FFStateMachineEvent evt in evts)
            {
                if (evt.type.Equals(type) && (name.Length < 1 || (evt.name.ToLower().Equals(name.ToLower()))))
                {
                    FFStateMachineDebug.Log(FFStateMachineDebugMessageType.Info, machine.name, "event/ " + type.ToString() + "/" + name, debugMode);
                    evt.action(sender, args);
                }

            }
        }
    }
}
