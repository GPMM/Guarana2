using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebService : MonoBehaviour
{
    [SerializeField]
    private string GingaCCWSLocation;
    [SerializeField]
    private string WebSocketHandle;
    [SerializeField]
    private string WebSocketURL;

    private Discovery discovery;
    private Register register;
    private WebSocketClient webSocketClient;


    void Start()
    {
        discovery = new MockDiscovery();
    }

    
    void Update()
    {
        // Has discovery phase ended?
        if (discovery != null && !discovery.Running())
        {
            GingaCCWSLocation = discovery.GetLocation();
            discovery = null;
            
            register = new Register(GingaCCWSLocation);
            StartCoroutine(register.SendMessage());
        }


        // Has register phase ended?
        if (register != null && !register.Running())
        {
            WebSocketHandle = register.GetHandle();
            WebSocketURL = register.GetURL();
            register = null;

            webSocketClient = new WebSocketClient(WebSocketURL);
        }
    }
}
