using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Register
{
    private string baseLocation, msgType, msgBody;
	private bool running;

	private class RegisterResponse
	{
		public string handle;
		public string url;
	}
	private RegisterResponse response;


	public Register(string loc)
    {
        baseLocation = loc + "/remote-device";
        msgType = "application/json";
        msgBody = "{" +
                "\"deviceClass\": \"Guarana\"," +
                "\"supportedFormats\": [\"application/x-ncl360\"]," +
                "\"recognizableEvents\": [\"selection\",\"look\"]" +
                "}";

		running = true;
    }


	public IEnumerator SendMessage()
	{
		using (UnityWebRequest wr = UnityWebRequest.Post(baseLocation, msgBody, msgType))
		{
			yield return wr.SendWebRequest();
			if (wr.result != UnityWebRequest.Result.Success)
			{
				Debug.Log(wr.error);
			}
			else
			{
				Debug.Log("Register upload complete!");
				var text = wr.downloadHandler.text;
				Debug.Log(text);
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
