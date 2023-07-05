using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterMessage
{
    public string deviceClass;
    public List<string> supportedFormats;
    public List<string> recognizableEvents;

    public RegisterMessage()
    {
        deviceClass = "Guarana";

        supportedFormats = new List<string>();
        supportedFormats.Add("application/x-ncl360");

        recognizableEvents = new List<string>();
        recognizableEvents.Add("selection");
        recognizableEvents.Add("look");
    }
}


public class RegisterResponse
{
    public string handle;
    public string url;
}

//public class MessageTemplates : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }
//}


//msgBody = "{" +
//        "\"deviceClass\": \"Guarana\"," +
//        "\"supportedFormats\": [\"application/x-ncl360\"]," +
//        "\"recognizableEvents\": [\"selection\",\"look\"]" +
//        "}";
