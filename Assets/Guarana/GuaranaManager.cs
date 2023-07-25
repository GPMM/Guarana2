using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuaranaManager : MonoBehaviour
{

    [SerializeField]
    private GameObject xrcamera;
    [SerializeField]
    private GameObject audioPlayerPrefab;
    [SerializeField]
    private GameObject image2DPlayerPrefab;
    [SerializeField]
    private GameObject textPlayerPrefab;
    [SerializeField]
    private GameObject video2DPlayerPrefab;
    [SerializeField]
    private GameObject video360PlayerPrefab;

    private GameObject webservice;
    private GameObject userSelector;
    private DownloadManager downloadManager;
    private GameObject scheduler;
    private GameObject scene;

    private WebSocketClient GingaCCWSClient;
    private string GingaCCWSLocation;

    private bool debugModeOn;
    private ReceiveScene receivedScene;
    private List<EventType> notifyEvents;
    private List<ReceiveAction> storedActionMsg;


    void Awake()
    {
        webservice = transform.Find("WebService").gameObject;
        userSelector = transform.Find("UserSelector").gameObject;
        downloadManager = transform.Find("Downloader").gameObject.GetComponent<DownloadManager>();
        scheduler = transform.Find("Scheduler").gameObject;
        scene = transform.Find("Scene").gameObject;

        //TODO: create a first step to define debug mode?
        debugModeOn = true;
        notifyEvents = new List<EventType>();
        storedActionMsg = new List<ReceiveAction>();

        userSelector.SetActive(false);
        scheduler.SetActive(false);
    }


    void Update()
    {
        if (userSelector.activeInHierarchy && !userSelector.GetComponent<UserSelector>().Running())
        {
            // User identification is over, send to Ginga
            NotifyUser msg = new NotifyUser();
            msg.user = userSelector.GetComponent<UserSelector>().GetSelectedUser();
            
            GingaCCWSClient.SendMessage(msg);
            userSelector.SetActive(false);
        }

        if (receivedScene != null)
        {
            downloadManager.SetBaseLocation(GingaCCWSLocation, receivedScene.appId);
            StartCoroutine(downloadManager.DownloadDocument(receivedScene.nodeSrc, this));
            foreach (string e in receivedScene.notifyEvents)
            {
                try
                {
                    notifyEvents.Add((EventType)Enum.Parse(typeof(EventType), e, true));
                }
                catch (Exception ex) { Debug.Log(ex); }
            }
            receivedScene = null;
        }

        if (storedActionMsg.Count > 0)
        {
            // Forward messages to scheduler
            List<ReceiveAction> temp = new List<ReceiveAction>();

            foreach (ReceiveAction msg in storedActionMsg)
            {
                EventType type = (EventType)Enum.Parse(typeof(EventType), msg.eventType, true);

                // If user identification is ongoing, only preparation messages can go through
                if (userSelector.activeInHierarchy && type != EventType.PREPARATION)
                {
                    temp.Add(msg);
                    continue;
                }

                EventTransition action = (EventTransition)Enum.Parse(typeof(EventTransition), msg.action, true);

                // Check if is an action over the document as a whole
                if (msg.node == "DOC" && action == EventTransition.STOP)
                {
                    RemoveDocument();
                    continue;
                }

                
                if (msg.delay > 0)
                {
                    scheduler.GetComponent<Scheduler>().AddDelayedAction(msg.node, type, action, msg.delay);
                }
                else
                {
                    scheduler.GetComponent<Scheduler>().AddAction(msg.node, type, action);
                }

            }
            storedActionMsg = temp;
        }
    }


    public bool DebugMode()
    {
        return debugModeOn;
    }


    public void HasConnected(WebSocketClient GingaCCWSClient, string GingaCCWSLocation)
    {
        this.GingaCCWSClient = GingaCCWSClient;
        this.GingaCCWSLocation = GingaCCWSLocation;
        
        // Ask for user identification
        userSelector.GetComponent<UserSelector>().SetBaseLocation(GingaCCWSLocation);
        
        webservice.SetActive(false);
        userSelector.SetActive(true);
    }


    public void ReceivedDocument(ReceiveScene msg)
    {
        receivedScene = msg;
    }


    public void SetDocument(string xmldoc)
    {
        Debug.Log("Received document");
        // No problem to parse and start scheduler if user identification is ongoing
        XMLParser parser = new XMLParser();
        Document doc = parser.Parse(xmldoc);
        doc.SetManager(this);

        scheduler.GetComponent<Scheduler>().SetDocument(doc);
        scheduler.SetActive(true);
    }


    public void RemoveDocument()
    {
        scheduler.SetActive(false);
        for (int i = scene.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(scene.transform.GetChild(i).gameObject);
        }
        for (int i = xrcamera.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(xrcamera.transform.GetChild(i).gameObject);
        }
        Debug.Log("Received document termination");
    }


    public void ReceiveAction(ReceiveAction msg)
    {
        storedActionMsg.Add(msg);
    }


    public void NotifyEventTransition(string nodeid, EventType evt, EventTransition trans)
    {
        if (!notifyEvents.Contains(evt))
            return;

        NotifyTransition msg = new NotifyTransition();
        msg.node = nodeid;
        msg.eventType = Enum.GetName(typeof(EventType), evt).ToLower();
        msg.transition = Enum.GetName(typeof(EventTransition), trans).ToLower() + "s";
        
        GingaCCWSClient.SendMessage(msg);
    }


    public GameObject GeneratePlayer(BaseMimeType mime, ScenePin pin)
    {
        GameObject player;

        if (mime == BaseMimeType.audio)
        {
            player = Instantiate(audioPlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else if (mime == BaseMimeType.image)
        {
            player = Instantiate(image2DPlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
        //else if (mime == BaseMimeType.model)
        //{
        //    //TODO
        //}
        else if (mime == BaseMimeType.text)
        {
            player = Instantiate(textPlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else if (mime == BaseMimeType.video)
        {
            player = Instantiate(video2DPlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else
        {
            player = new GameObject();
        }

        player.transform.localPosition = new Vector3(0, 0, 0);
        player.transform.LookAt(Vector3.zero);

        player.GetComponent<Player>().SetDownloadManager(downloadManager.GetComponent<DownloadManager>());

        if (pin == ScenePin.ENVIRONMENT)
            player.transform.parent = scene.transform;
        else if (pin == ScenePin.CAMERA)
            player.transform.parent = xrcamera.transform;

        return player;
    }


    public GameObject GenerateSkyPlayer(BaseMimeType mime)
    {
        GameObject player;

        //if (mime == BaseMimeType.image)
        //{
        //    //TODO
        //}
        //else if (mime == BaseMimeType.video)
        //{
            player = Instantiate(video360PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //}
        //else
        //{
        //    player = new GameObject();
        //}

        player.transform.localPosition = new Vector3(0, 0, 0);
        
        player.GetComponent<Player>().SetDownloadManager(downloadManager.GetComponent<DownloadManager>());
        player.transform.parent = scene.transform;

        return player;
    }
}
