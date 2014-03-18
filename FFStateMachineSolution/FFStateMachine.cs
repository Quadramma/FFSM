using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFStateMachineSolution
{
    public class FFStateMachine
    {
        public string name { get; set; }
        public FFStateMachineState currentState { get; private set; }
        public Dictionary<string, object> attr = new Dictionary<string, object>();
        public List<FFStateMachineTransition> transitions = new List<FFStateMachineTransition>();
        public List<FFStateMachineState> states = new List<FFStateMachineState>();
        public FFStateMachineEventManager events;
        public bool initialized { get; set; }
        public bool debugMode { get; set; }
        //
        public void bind(FFStateMachineEvent evt)
        {
            events.bind(evt);
        }
        public void trigger(FFStateMachineEventType type, string name, object sender, Dictionary<string, object> args)
        {
            events.trigger(type, name, sender, args);
        }
        public void Update()
        {
            if (states.Count == 0)
            {
                FFStateMachineDebug.Log(FFStateMachineDebugMessageType.Info, this.name, "Update-Sin estados-Omite",debugMode);
                return;
            }
            if (currentState == null)
            {
                throw new System.Exception("[" + this.name + "][State default requerido!!][" + states.Count + "]");
            }

            if (currentState.onStateUpdate != null)
            {
                events.trigger(FFStateMachineEventType.beginUpdate, "", this, null);
                currentState.onStateUpdate(currentState);
                events.trigger(FFStateMachineEventType.afterUpdate, "", this, null);
            }
        }
        //
        public void init()
        {
            events.trigger(FFStateMachineEventType.beforeInit, "", this, null);
            foreach (FFStateMachineState s in states)
            {
                FFStateMachineDebug.Log(FFStateMachineDebugMessageType.Info, this.name, s.name+"/onStateInit", debugMode);
                s.onStateInit(s);
            }
            events.trigger(FFStateMachineEventType.afterInit, "", this, null);
        }
        //
        public void add(FFStateMachineState[] statesList)
        {
            foreach (FFStateMachineState state in statesList)
            {
                this.add(state);
            }
        }
        public void add(FFStateMachineState newState)
        {
            if (!exists(newState.name))
            {
                FFStateMachineDebug.Log(FFStateMachineDebugMessageType.Info, this.name, newState.name + "-added", debugMode);
                states.Add(newState);
            }
            else
            {
                FFStateMachineDebug.Log(FFStateMachineDebugMessageType.Info, this.name,"add-estado existente/"+ newState.name + "--omitido", debugMode);
            }

        }
        //
        public bool exists(string stateName)
        {
            foreach (FFStateMachineState state in states)
            {
                if (stateName.ToLower().Equals(state.name.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }
        public FFStateMachineState getCurrent() { return currentState; }
        public FFStateMachineState get(string name)
        {
            if (name.Length < 1) return null;
            foreach (FFStateMachineState state in states)
            {
                if (name.ToLower().Equals(state.name.ToLower()))
                {
                    return state;
                }
            }
            throw new System.Exception("[NinjaStateMachine][get][State not found -> " + name + "]");
        }
        public FFStateMachine(string name)
        {
            this.name = name;
            this.events = new FFStateMachineEventManager(this);
            this.initialized = false;
        }
        public void addTransition(string transitionName, string fromName, string toName, FFStateMachineStateAction action)
        {
            /*
            if (!exists(fromName))
            {
                throw new System.Exception("[NinjaStateMachine][addTransition][state no encontrado -> " + fromName + "]");
            }
            if (!exists(toName))
            {
                throw new System.Exception("[NinjaStateMachine][addTransition][state no encontrado -> " + toName + "]");
            }
            //*/
            transitions.Add(new FFStateMachineTransition(transitionName, get(fromName), get(toName), action));
        }
        public void advanceSilent(string toName)
        {
            if (!exists(toName))
            {
                throw new System.Exception("[NinjaStateMachine][advance][state no encontrado -> " + toName + "]");
            }
            else
            {
                FFStateMachineState to = get(toName);
                currentState = to;
            }
        }
        public void advance(string toName)
        {
            if (currentState != null && currentState.name.ToLower().Equals(toName.ToLower()))
            {
                FFStateMachineDebug.Log(FFStateMachineDebugMessageType.Warning, this.name, "add-estado existente/" + currentState.name + "-->" + toName + "/advance omitido" , debugMode);
            }

            if (!exists(toName))
            {
                throw new System.Exception("[" + this.name + "][advance][state no encontrado -> " + toName + "]");
            }
            else
            {
                if (currentState != null)
                {
                    if (currentState.onStateExit != null) currentState.onStateExit(currentState);
                    if (currentState.onStateEnter != null) currentState.onStateEnter(currentState);
                }
                FFStateMachineState to = get(toName);
                foreach (FFStateMachineTransition t in transitions)
                {
                    if ((t.from == null || t.from == currentState) && t.to == to)
                    {
                        FFStateMachineDebug.Log(FFStateMachineDebugMessageType.Warning, this.name, "transition/" + t.name + "/ " + (currentState == null ? "null" : currentState.name) + "-->"+ toName, debugMode);
                        t.execute();
                    }
                }
                currentState = to;
            }
        }
    }
}
