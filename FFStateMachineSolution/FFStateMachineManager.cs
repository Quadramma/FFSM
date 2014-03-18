using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFStateMachineSolution
{
    public class FFStateMachineManager
    {
        public bool debugEnabled = false;
        public static bool debugMode = true;
        public static FFStateMachine playerMachine = null;
        public static List<FFStateMachine> machines = new List<FFStateMachine>();
        private static FFStateMachine managerRepresentation = new FFStateMachine("FFStateMachineManager");
        private static FFStateMachineEventManager events = new FFStateMachineEventManager(managerRepresentation);

        
        public static void bind(FFStateMachineEvent evt)
        {
            events.bind(evt);
        }
        public static void trigger(FFStateMachineEventType type, string name)
        {
            events.trigger(type, name, null, null);
        }


    

        public void initMachine(string machineName)
        {
            FFStateMachine m = get(machineName);
            foreach (FFStateMachineState s in m.states)
            {
                s.onStateInit(s);
            }
        }
        public static void add(FFStateMachine machine)
        {
            if (!exists(machine.name))
            {
                machines.Add(machine);
                if (machine.debugMode == false) machine.debugMode = debugMode;
                FFStateMachineDebug.Log(FFStateMachineDebugMessageType.Info, "FFStateMachineManager", "add/ " + machine.name + "/agregado", debugMode);
            }
            else
            {
                throw new System.Exception("[FFStateMachineManager][add][" + machine.name + " ya existe]");
            }
        }
        public static FFStateMachine get(string name)
        {
            foreach (FFStateMachine machine in machines)
            {
                if (name.ToLower().Equals(machine.name.ToLower()))
                {
                    return machine;
                }
            }
            throw new System.Exception("[FFStateMachineManager][get][" + name + " not found]");
        }
        public static bool exists(string name)
        {
            foreach (FFStateMachine machine in machines)
            {
                if (name.ToLower().Equals(machine.name.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }
        private static void FFSMMUpdate()
        {
            foreach (FFStateMachine m in machines)
            {
                if (!m.initialized)
                {
                    trigger(FFStateMachineEventType.managerBeforeInit, "");
                    m.init();
                    m.initialized = true;
                    trigger(FFStateMachineEventType.managerAfterInit, "");
                }

                m.Update();
            }
        }
    }
}
