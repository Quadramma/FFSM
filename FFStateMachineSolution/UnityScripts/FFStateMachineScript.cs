using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FFStateMachineScript : MonoBehaviour
{
    public string ffsmName = "";
    public bool useGameObjectName = false;
    public FFStateMachine ffsm { get; private set; }
    public List<FFStateMachineState> states = new List<FFStateMachineState>();
    void Awake()
    {
        if (ffsmName == "" && !useGameObjectName)
        {
            throw new System.Exception("[FFStateMachine][Name required !]");
        }
        if (useGameObjectName)
        {
            FFStateMachineManager.add(ffsm = new FFStateMachine(gameObject.name + "_FFStateMachine"));
        }
        else
        {
            FFStateMachineManager.add(ffsm = new FFStateMachine(ffsmName));
        }
        
        
        
    }
	void Start () {
    }
}
