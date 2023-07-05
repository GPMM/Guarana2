using UnityEngine;

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
        WebServiceUI.Log("Starting Web Service...\n");

        discovery = new MockDiscovery();
    }

    
    void Update()
    {
        // Has discovery phase ended?
        if (discovery != null && !discovery.Running())
        {
            GingaCCWSLocation = discovery.GetLocation();
            discovery = null;
            WebServiceUI.Log("Ginga at "+ GingaCCWSLocation + "\n");
            
            register = new Register(GingaCCWSLocation);
            StartCoroutine(register.SendMessage());
        }


        // Has register phase ended?
        if (register != null && !register.Running())
        {
            WebSocketHandle = register.GetHandle();
            WebSocketURL = register.GetURL();
            register = null;
            WebServiceUI.Log("Ginga at " + WebSocketURL + " my handle: " + WebSocketHandle + "\n");

            webSocketClient = new WebSocketClient(WebSocketURL);
        }


        // Check if socket is connected
        if (webSocketClient != null && webSocketClient.Running())
        {
            WebServiceUI.Clear();
        }
    }
}
