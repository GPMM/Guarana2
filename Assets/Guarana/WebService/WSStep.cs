using UnityEngine;

public class WSStep : MonoBehaviour
{
    protected WSStepIO input = null, output = null;
    protected DTV dtv;
    protected bool running = false;

    public void Initialize(DTV dtv)
    {
        this.dtv = dtv;
        gameObject.SetActive(true);
    }


    public void Initialize(DTV dtv, WSStepIO input)
    {
        this.dtv = dtv;
        this.input = input;
        gameObject.SetActive(true);
    }


    public void Stop()
    {
        if (input != null)
            input = null;

        gameObject.SetActive(false);
    }


    public WSStepIO GetStepOutput()
    {
        return output;
    }
}