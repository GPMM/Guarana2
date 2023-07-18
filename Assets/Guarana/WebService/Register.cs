using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Register
{
    private string baseLocation, msgType, msgBody;
	private RegisterResponse response;
	private bool running;


	public Register(string loc)
    {
		WebServiceUI.Log("Register ");
        baseLocation = loc + GingaURLTemplates.RegisterSuffix();
        msgType = "application/json";
		msgBody = JsonUtility.ToJson(new RegisterMessage());

		running = true;
    }


	public IEnumerator SendMessage()
	{
		WebServiceUI.Log(". ");
		using (UnityWebRequest wr = UnityWebRequest.Post(baseLocation, msgBody, msgType))
		{
			WebServiceUI.Log(". ");
			yield return wr.SendWebRequest();
			if (wr.result != UnityWebRequest.Result.Success)
			{
				WebServiceUI.Log(wr.error);
			}
			else
			{
				WebServiceUI.Log(". ");
				var text = wr.downloadHandler.text;
				response = JsonUtility.FromJson<RegisterResponse>(text);
				WebServiceUI.Log("got handle " + response.handle + " and url " + response.url + "\n");
				running = false;
			}
		}
	}


	public bool Running()
	{
		return running;
	}


	public string GetHandle()
	{
		return response.handle;
	}


	public string GetURL()
	{
		return response.url;
	}
}
