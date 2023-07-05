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
        baseLocation = loc + "/remote-device";
        msgType = "application/json";
		msgBody = JsonUtility.ToJson(new RegisterMessage());

		running = true;
    }


	public IEnumerator SendMessage()
	{
		using (UnityWebRequest wr = UnityWebRequest.Post(baseLocation, msgBody, msgType))
		{
			yield return wr.SendWebRequest();
			if (wr.result != UnityWebRequest.Result.Success)
			{
				WebServiceUI.Log(wr.error);
			}
			else
			{
				WebServiceUI.Log("Register upload complete!\n");
				var text = wr.downloadHandler.text;
				WebServiceUI.Log(text + "\n");
				response = JsonUtility.FromJson<RegisterResponse>(text);
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
