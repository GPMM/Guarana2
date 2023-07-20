using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UserSelector : MonoBehaviour
{
	private string baseLocation;
	private UserListResponse response;
	private bool running;
	private string selectedUser;


	void Start()
    {
		SetUser("ana");

        //running = true;

        //if (HasUsers())
        //{
        //    StartCoroutine(SendMessage());
        //}
    }


    public void SetBaseLocation(string baseLocation)
    {
        this.baseLocation = baseLocation + GingaURLTemplates.UserListSuffix();
	}


	public void SetUser(string user)
    {
		selectedUser = user;
		running = false;
	}


	public IEnumerator SendMessage()
	{
		using (UnityWebRequest wr = UnityWebRequest.Get(baseLocation))
		{
			yield return wr.SendWebRequest();
			if (wr.result != UnityWebRequest.Result.Success)
			{
				WebServiceUI.Log(wr.error);
			}
			else
			{
				var text = wr.downloadHandler.text;
				response = JsonUtility.FromJson<UserListResponse>(text);
			}
		}
	}


	public bool Running()
	{
		return running;
	}


	public bool HasUsers()
	{
		return response != null;
	}


	public List<UserInfo> GetUserList()
	{
		return response.users;
	}


	public string GetSelectedUser()
    {
		return selectedUser;
    }
}
