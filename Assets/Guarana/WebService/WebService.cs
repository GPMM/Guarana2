using UnityEngine;

public class WebService : MonoBehaviour
{
    private string GingaCCWSLocation;
    private string WebSocketHandle;
    private string WebSocketURL;

    private Discovery discovery;
    private Register register;
    private WebSocketClient webSocketClient;
    private GuaranaManager manager;


    void Start()
    {
        manager = transform.parent.gameObject.GetComponent<GuaranaManager>();
        transform.Find("DebugConsole").gameObject.SetActive(manager.DebugMode());

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

            webSocketClient = new WebSocketClient(WebSocketURL, WebSocketHandle, GingaCCWSLocation);
        }


        // Check if socket is connected
        if (webSocketClient != null && webSocketClient.Running())
        {
            WebServiceUI.Clear();
            manager.HasConnected(webSocketClient, GingaCCWSLocation);
        }
    }
}
