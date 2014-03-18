using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FFStateMachineManager : MonoBehaviour {

    //unity editor
    public bool debugEnabled = false;

    public static bool debugMode = true;
    public static FFStateMachine playerMachine = null;
    public static List<FFStateMachine> machines = new List<FFStateMachine>();
    private static FFStateMachine managerRepresentation = new FFStateMachine("FFStateMachineManager");
    private static FFStateMachineEventManager events = new FFStateMachineEventManager(managerRepresentation);

    

    public static string getMachineNameFromGameObject(GameObject g){
        return g.name+"_FFStateMachine";
    }
    public static void bind(FFStateMachineEvent evt)
    {
        events.bind(evt);
    }
    public static void trigger(FFStateMachineEventType type, string name)
    {
        events.trigger(type, name,null,null);
    }


    public static void StartCorountine(float seconds,MonoBehaviour monoBehaviour, System.Action callback)
    {
        monoBehaviour.StartCoroutine(corountine(seconds, callback));
    }
    private static IEnumerator corountine(float seconds, System.Action callback)
    {
        yield return new WaitForSeconds(seconds);
        if (callback != null)
        {
            callback();
        }
        else
        {
            Debug.Log("[FFStateMachineManager][Warning][StartCorountine][callback null]");
        }
    }

    public static FFStateMachine getCurrentMachine(Transform t)
    {
        FFStateMachineScript script = ((FFStateMachineScript)t.GetComponent<FFStateMachineScript>());
        if (script != null)
        {
            return script.ffsm;
        }
        else
        {
            throw new System.Exception("[State Script][No puedo encontrar una FFSM asociada al gameobject(" + t.gameObject.name + ")]");
        }
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
            if(machine.debugMode==false) machine.debugMode = debugMode;
            if (debugMode) Debug.Log("[FFStateMachineManager][add][" + machine.name + " agregado]");
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
                trigger(FFStateMachineEventType.managerBeforeInit,"");
                m.init();
                m.initialized = true;
                trigger(FFStateMachineEventType.managerAfterInit, "");
            }
            
            m.Update();
        }
    }

    void Awake()
    {
        FFStateMachineManager.debugMode = debugEnabled;
    }
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        FFStateMachineManager.FFSMMUpdate();
	}
}



//-------------------------------------
//FASO FINITE STATE MACHINE
//-------------------------------------
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

public enum FFStateMachineEventType
{
    beforeInit,
    afterInit,
    beginUpdate,
    afterUpdate,

    click,

    normal,

    action,
    actionConsequence,

    changeState,
    triggerEnter,
    triggerExit,

    managerBeforeInit, 
    managerAfterInit,

}

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
    
    public void trigger(FFStateMachineEventType type,string name, object sender, Dictionary<string,object> args)
    {
        foreach (FFStateMachineEvent evt in evts)
        {
            if (evt.type.Equals(type) && (name.Length<1 || (evt.name.ToLower().Equals(name.ToLower()))))
            {
                if(debugMode) Debug.Log("["+machine.name+"][Event][" + type.ToString() + "][" + name + "]");
                evt.action(sender,args);
            }
            
        }
    } 
}

//Faso Finite State Machine
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
    public void trigger(FFStateMachineEventType type,string name, object sender, Dictionary<string,object> args)
    {
        events.trigger(type,name,sender,args);
    }
    public void Update()
    {
        if (states.Count == 0)
        {
            if(debugMode) Debug.Log("["+this.name+"][Update][Sin estados!][omite]");
            return;
        }
        if (currentState == null)
        {
            throw new System.Exception("["+this.name+"][State default requerido!!]["+states.Count+"]");
        }

        if (currentState.onStateUpdate != null)
        {
            events.trigger(FFStateMachineEventType.beginUpdate, "",this,null);
            currentState.onStateUpdate(currentState);
            events.trigger(FFStateMachineEventType.afterUpdate, "",this,null);
        }
    }
    //
    public void init(){
        events.trigger(FFStateMachineEventType.beforeInit, "",this,null);
        foreach (FFStateMachineState s in states)
        {
            if(debugMode) Debug.Log("["+this.name+"]["+s.name+"][onStateInit]");
            s.onStateInit(s);
        }
        events.trigger(FFStateMachineEventType.afterInit, "",this,null);
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
            if(debugMode) Debug.Log("[" + this.name + "][" + newState.name + "][added]");
            states.Add(newState);
        }
        else
        {
            if (debugMode) Debug.Log("[NinjaStateMachine][add][state existente -> " + newState.name + "][omitido]");
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
            Debug.Log("[WARNING][" + this.name + "][" + currentState.name + " -> " + toName + "][advance Omitido]");
            //throw new System.Exception("[" + this.name + "][advance][" + toName + " -> " + toName + " provocaria un loop infinito, estas loco ?]");
        }

        if (!exists(toName))
        {
            throw new System.Exception("["+this.name+"][advance][state no encontrado -> " + toName + "]");
        }
        else
        {
            if (currentState != null) { 
                if (currentState.onStateExit !=null) currentState.onStateExit(currentState);
                if (currentState.onStateEnter != null) currentState.onStateEnter(currentState);
            }
            FFStateMachineState to = get(toName);
            foreach (FFStateMachineTransition t in transitions)
            {
                if ((t.from == null || t.from == currentState) && t.to == to)
                {
                    if (this.debugMode) Debug.Log("[" + this.name + "][transition]["+t.name+"][" + (currentState==null?"null":currentState.name) + " -> " + toName + "]");
                    t.execute();
                }
            }
            currentState = to;
        }
    }
}

public class FFStateMachineState
{
    //Agregar atributos aqui->
    public float speed { get; set; }
    public bool avanzarEstado { get; set; }
    public Vector2 dir { get; set; }
    //
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

//-------------------------------------


