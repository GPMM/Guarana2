using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identifying : MonoBehaviour
{
    private DTV dtv;
    private bool reload = false;
    private bool getUserAtt, getUserImg, activateSelector, running;

    private List<UserData> userList;
    private GameObject selector;
    private UserData activeUser;


    public void Initialize(DTV dtv, bool reload)
    {
        this.dtv = dtv;
        this.reload = reload;

        selector = transform.Find("Selector").gameObject;
        selector.SetActive(false);

        gameObject.SetActive(true);
    }

    void Start()
    {
        running = true;
        getUserAtt = false;
        getUserImg = false;
        activateSelector = false;

        if (reload || userList == null)
        {
            userList = new List<UserData>();
            GetUSerList();
        }
        else
        {
            activateSelector = true;
        }
    }

    
    void Update()
    {
        if (getUserAtt)
        {
            StartCoroutine(GetUsersAttribute());
        }

        if (getUserImg)
        {
            StartCoroutine(GetUsersImages());
        }

        if (activateSelector)
        {
            activateSelector = false;
            selector.GetComponent<Selector>().SetInfo(userList);
            selector.SetActive(true);
            running = false;
        }

        if (!running && !selector.activeInHierarchy)
        {
            int user = selector.GetComponent<Selector>().GetCurrentIndex();
            activeUser = userList[user];
            gameObject.SetActive(false);
        }
    }

    public void Stop()
    {
        gameObject.SetActive(false);
    }


    private void GetUSerList()
    {
        StartCoroutine(dtv.CurrentService.UserAPI.PostUserList(null, SaveUsers));
    }


    private void SaveUsers(DTVCSUserList list)
    {
        foreach (User user in list.users)
        {
            UserData u = new UserData();
            u.id = user.id;
            userList.Add(u);
        }

        getUserAtt = true;
    }


    private IEnumerator GetUsersAttribute()
    {
        getUserAtt = false;

        foreach (UserData user in userList)
        {
            yield return dtv.CurrentService.UserAPI.GetUserAttribute(user.id, null, SaveUserAttribute);
        }

        getUserImg = true;
    }


    private void SaveUserAttribute(string id, DTVCSUserAttribute att)
    {
        foreach (UserData user in userList)
        {
            if (user.id == id)
            {
                user.name = att.name;
                user.path = att.avatar;
                break;
            }
        }
    }


    private IEnumerator GetUsersImages()
    {
        getUserImg = false;

        foreach (UserData user in userList)
        {
            yield return dtv.CurrentService.UserAPI.GetUserFile(user.id, user.path, SaveUserImage);
        }

        activateSelector = true;
    }


    private void SaveUserImage(string id, Texture2D texture)
    {
        foreach (UserData user in userList)
        {
            if (user.id == id)
            {
                user.texture = texture;
                break;
            }
        }
    }


}
