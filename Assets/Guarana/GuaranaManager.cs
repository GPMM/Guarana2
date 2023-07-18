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
    private GameObject downloadManager;
    private GameObject scheduler;
    private GameObject scene;

    private WebSocketClient GingaCCWSClient;
    private string GingaCCWSLocation;

    private bool debugModeOn;
    private List<EventType> notifyEvents;
    private List<ReceiveAction> storedActionMsg;


    void Awake()
    {
        webservice = transform.Find("WebService").gameObject;
        userSelector = transform.Find("UserSelector").gameObject;
        downloadManager = transform.Find("Downloader").gameObject;
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

        if (!userSelector.activeInHierarchy && storedActionMsg.Count > 0)
        {
            foreach(ReceiveAction msg in storedActionMsg)
            {
                ReceiveAction(msg);
            }
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
        // No problem to receive document if user identification is ongoing
        downloadManager.GetComponent<DownloadManager>().SetBaseLocation(GingaCCWSLocation, msg.appId);

        StartCoroutine(downloadManager.GetComponent<DownloadManager>().DownloadDocument(msg.nodeSrc, this));

        foreach (string e in msg.notifyEvents)
        {
            notifyEvents.Add((EventType) Enum.Parse(typeof(EventType), e, true));
        }
    }


    public void SetDocument(string xmldoc)
    {
        // No problem to parse and start scheduler if user identification is ongoing
        XMLParser parser = new XMLParser();
        Document doc = parser.Parse(xmldoc);
        doc.SetManager(this);

        scheduler.GetComponent<Scheduler>().SetDocument(doc);
        scheduler.SetActive(true);
    }


    public void ReceiveAction(ReceiveAction msg)
    {
        EventType type = (EventType)Enum.Parse(typeof(EventType), msg.eventType, true);

        // If user identification is ongoing, only preparation messages can go through
        if (userSelector.activeInHierarchy && type != EventType.PREPARATION)
        {
            storedActionMsg.Add(msg);
            return;
        }

        EventTransition action = (EventTransition)Enum.Parse(typeof(EventTransition), msg.action, true);

        if (msg.delay > 0)
        {
            scheduler.GetComponent<Scheduler>().AddDelayedAction(msg.node, type, action, msg.delay);
        }
        else
        {
            scheduler.GetComponent<Scheduler>().AddAction(msg.node, type, action);
        }
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
