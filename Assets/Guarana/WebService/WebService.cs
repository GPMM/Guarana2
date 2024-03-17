using System;
using System.Collections.Generic;
using UnityEngine;

public class WebService : MonoBehaviour
{
    private DTV dtv;

    private List<GameObject> state;
    public int[,] next = {
        { 1, 1 }, //Discovering
        { 2, 5 }, //Authorizing
        { 3, 6 }, //Registering
        { 4, 7 }, //Connecting
        { 4, 7 }, //Running

        { 1, 5 }, //Failed
        { 2, 6 }, //Halted
        { 6, 7 }, //Unregistering

        { 3, 8 }, //Paused
    };
    private int current = 0;

    public System.Action<MultidevMetadata> onMessage;
    public System.Action<bool> onRunning;


    void Awake()
    {
        state = new List<GameObject>();
        state.Add(transform.Find("Discovering").gameObject);    //0
        state.Add(transform.Find("Authorizing").gameObject);    //1
        state.Add(transform.Find("Registering").gameObject);    //2
        state.Add(transform.Find("Connecting").gameObject);     //3
        state.Add(transform.Find("Running").gameObject);        //4

        state.Add(transform.Find("Failed").gameObject);         //5
        state.Add(transform.Find("Halted").gameObject);         //6
        state.Add(transform.Find("Unregistering").gameObject);  //7

        state.Add(transform.Find("Paused").gameObject);         //8

        // Just in case
        foreach (GameObject s in state)
            s.SetActive(false);
    }


    void Start()
    {
        state[current].GetComponent<WSStep>().Initialize(dtv);
    }


    void Update()
    {
        // Perform state changes
        if (!state[current].activeInHierarchy)
            StateChange();
    }


    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // Enters the Paused state
            if (state[current].name == "Running")
            {
                PauseWebService();
            }

            // TODO: e se não estiver em running?
        }
    }


    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            if (state[current].name == "Running")
            {
                PauseWebService();
            }

            // TODO: e se não estiver em running?
        }
    }


    private void StateChange()
    {
        WSStepIO output = state[current].GetComponent<WSStep>().GetStepOutput();

        string oldstate = state[current].name;
        if (oldstate == "Discovering")
        {
            dtv = new DTV(((DiscoveryOutput)output).hostIP, ((DiscoveryOutput)output).hostPort);
        }

        // Goes to the next state, check if input is necessary
        current = next[current, ((int)output.status)];
        string currentstate = state[current].name;
        if (currentstate == "Running")
        {
            ((RunningInput)output).messageHandler = onMessage;
        }

        if (currentstate == "Connecting" || currentstate == "Running" || currentstate == "Paused" || currentstate == "Unregistering")
        {
            state[current].GetComponent<WSStep>().Initialize(dtv, output);
        }
        else
        {
            state[current].GetComponent<WSStep>().Initialize(dtv);
        }

        //Notify that is running
        if (currentstate == "Running")
        {
            onRunning(true);
        }
    }


    private void PauseWebService()
    {
        state[current].GetComponent<WSRun>().Pause();

        PausedInput output = (PausedInput)state[current].GetComponent<WSStep>().GetStepOutput();
        output.nodeid = "";

        current = 8;
        state[current].GetComponent<WSStep>().Initialize(dtv, output);

        onRunning(false);
    }
}
