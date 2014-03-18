using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFStateMachineSolution
{
    public class FFStateMachineStateAction
    {
        public string name { get; private set; }
        public System.Action<FFStateMachineStateAction, FFStateMachineState, FFStateMachineState> action { get; private set; }
        public FFStateMachineStateAction(string name, System.Action<FFStateMachineStateAction, FFStateMachineState, FFStateMachineState> action)
        {
            this.name = name;
            this.action = action;
        }
        public void execute(FFStateMachineState from, FFStateMachineState to)
        {
            action(this, from, to);
        }
    }
}
