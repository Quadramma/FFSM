using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFStateMachineSolution
{
    public class FFStateMachineState
    {
        public string name { get; set; }
        public Dictionary<string, object> attr = new Dictionary<string, object>();
        public System.Action<FFStateMachineState> onStateInit;
        public System.Action<FFStateMachineState> onStateEnter;
        public System.Action<FFStateMachineState> onStateUpdate;
        public System.Action<FFStateMachineState> onStateExit;
        public FFStateMachineState(string name)
        {
            this.name = name;
        }
        public override bool Equals(object obj)
        {
            FFStateMachineState state = (FFStateMachineState)obj;
            return state.name == this.name;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
