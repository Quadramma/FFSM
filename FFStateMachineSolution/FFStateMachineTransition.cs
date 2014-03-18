using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFStateMachineSolution
{
    public class FFStateMachineTransition
    {
        public string name { get; private set; }
        public FFStateMachineState from;
        public FFStateMachineState to;
        public FFStateMachineStateAction action;
        public FFStateMachineTransition(string transitionName, FFStateMachineState from, FFStateMachineState to, FFStateMachineStateAction action)
        {
            this.name = transitionName;
            this.from = from;
            this.to = to;
            this.action = action;
        }
        public void execute()
        {
            this.action.execute(this.from, this.to);
        }
    }
}
