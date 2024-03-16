using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class Selector : MonoBehaviour
{
    private InputDevice control;

    [SerializeField]
    private GameObject canvas;

    [Header("Update time")]
    [SerializeField]
    private float deltaMin;
    [SerializeField]
    private float deltaMax;
    private float delta;

    [Header("Tile settings")]
    [SerializeField]
    private Vector2 tileSize;
    [SerializeField]
    private Vector2 thumbSize;
    [SerializeField]
    private Vector2 textSize;
    [SerializeField]
    private Sprite tileBackground;
    [SerializeField]
    [Range(0, 1)]
    private float alpha;
    [SerializeField]
    private float sepDist;
    [SerializeField]
    private float fontSize;

    private List<UserData> infos;
    private List<GameObject> tiles;
    private int currentIndex;


    public void SetInfo(List<UserData> infos)
    {
        this.infos = infos;
    }


    public int GetCurrentIndex()
    {
        return currentIndex;
    }


    void Start()
    {
        control = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        delta = 0;

        // set the canvas size and position
        float xSize = tileSize.x * infos.Count + sepDist * (infos.Count - 1);
        float ySize = tileSize.y + textSize.y;

        RectTransform _rt = canvas.GetComponent<RectTransform>();
        _rt.sizeDelta = new Vector2(xSize, ySize);
        _rt.anchorMin = new Vector2(0f, 0.5f);
        _rt.anchorMax = new Vector2(0f, 0.5f);
        _rt.pivot = new Vector2(0f, 0.5f);
        _rt.anchoredPosition = new Vector2(-tileSize.x/2, 0);

        // create the tiles
        tiles = new List<GameObject>();
        float xPos = 0;
        foreach (UserData info in infos)
        {
            tiles.Add(CreateTile(xPos, info.texture, info.name));

            // increase the X position for the next tile
            xPos += tileSize.x + sepDist;
        }

        currentIndex = 0;
        FocusOnElementAt(currentIndex);
    }

    
    void Update()
    {
        if (control.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue) && triggerValue)
        {
            // select user, stops selector
            gameObject.SetActive(false);
            // remove tiles
            foreach (GameObject tile in tiles)
            {
                Destroy(tile);
            }
            tiles.Clear();
        }

        if (delta > 0)
        {
            delta -= Time.deltaTime;
            return;
        }

        if (control.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axisValue) && axisValue != Vector2.zero)
        {
            delta = deltaMin + (1 - Math.Abs(axisValue.x)) * (deltaMax - deltaMin);

            if (axisValue.x > 0)
            {
                // move right
                currentIndex++;
                if (currentIndex >= tiles.Count)
                    currentIndex = tiles.Count - 1;
            }
            else
            {
                // move left
                currentIndex--;
                if (currentIndex < 0)
                    currentIndex = 0;
            }

            FocusOnElementAt(currentIndex);
        }
    }


    private void FocusOnElementAt(int index)
    {
        Image _img;

        // set focus to current
        _img = tiles[index].transform.Find("background").gameObject.GetComponent<Image>();
        _img.color = new Color(1f, 1f, 1f, 1f);

        // remove the focus from neighbors
        if (index - 1 >= 0)
        {
            _img = tiles[index - 1].transform.Find("background").GetComponent<Image>();
            _img.color = new Color(1f, 1f, 1f, alpha);
        }
        
        if (index + 1 < tiles.Count)
        {
            _img = tiles[index + 1].transform.Find("background").GetComponent<Image>();
            _img.color = new Color(1f, 1f, 1f, alpha);
        }

        // move the canvas accordingly
        float xPos = index * (tileSize.x + sepDist) + tileSize.x / 2;
        canvas.GetComponent<RectTransform>().anchoredPosition = new Vector2(-xPos, 0);
    }


    private GameObject CreateTile(float xPos, Texture2D texture, string text)
    {
        GameObject _tile = new GameObject("tile" + text, typeof(RectTransform));
        _tile.transform.SetParent(canvas.transform);

        RectTransform _rt = _tile.GetComponent<RectTransform>();
        _rt.sizeDelta = new Vector2(tileSize.x, tileSize.y);
        _rt.anchorMin = new Vector2(0f, 1f);
        _rt.anchorMax = new Vector2(0f, 1f);
        _rt.pivot = new Vector2(0f, 1f);
        _rt.anchoredPosition = new Vector2(xPos, 0);
        _rt.localPosition = new Vector3(_rt.localPosition.x, _rt.localPosition.y, 0);

        GameObject _background = new GameObject("background", typeof(Image));
        _background.transform.SetParent(_tile.transform);

        Image _img = _background.GetComponent<Image>();
        _img.sprite = tileBackground;
        _img.color = new Color(1f, 1f, 1f, alpha);

        _rt = _background.GetComponent<RectTransform>();
        _rt.sizeDelta = new Vector2(tileSize.x, tileSize.y);
        _rt.anchorMin = new Vector2(0f, 1f);
        _rt.anchorMax = new Vector2(0f, 1f);
        _rt.pivot = new Vector2(0f, 1f);
        _rt.anchoredPosition = new Vector2(0, 0);
        _rt.localPosition = new Vector3(_rt.localPosition.x, _rt.localPosition.y, 0);

        // Create the user thumbnail
        CreateThumb(_tile.transform, texture);

        // Create the user name
        CreateName(_tile.transform, text);

        return _tile;
    }


    private void CreateThumb(Transform parent, Texture2D texture)
    {
        GameObject _thumb = new GameObject("thumb", typeof(Image));
        _thumb.transform.SetParent(parent, false);

        RectTransform _rt = _thumb.GetComponent<RectTransform>();
        _rt.sizeDelta = new Vector2(thumbSize.x, thumbSize.y);
        _rt.anchorMin = new Vector2(0.5f, 1f);
        _rt.anchorMax = new Vector2(0.5f, 1f);
        _rt.pivot = new Vector2(0.5f, 1f);
        _rt.anchoredPosition = new Vector2(0, -(tileSize.y - textSize.y - thumbSize.y)/2);
        _rt.localPosition = new Vector3(_rt.localPosition.x, _rt.localPosition.y, 0);

        Sprite _sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        _thumb.GetComponent<Image>().sprite = _sprite;
    }


    private void CreateName(Transform parent, string text)
    {
        GameObject _name = new GameObject("name", typeof(TextMeshPro));
        _name.transform.SetParent(parent, false);

        TMP_Text _tmptext = _name.GetComponent<TMP_Text>();
        _tmptext.text = text;
        _tmptext.fontSize = fontSize;
        _tmptext.alignment = TextAlignmentOptions.Center;

        RectTransform _rt = _name.GetComponent<RectTransform>();
        _rt.sizeDelta = new Vector2(textSize.x, textSize.y);
        _rt.anchorMin = new Vector2(0.5f, 0f);
        _rt.anchorMax = new Vector2(0.5f, 0f);
        _rt.pivot = new Vector2(0.5f, 0f);
        _rt.anchoredPosition = new Vector2(0, 0);
        _rt.localPosition = new Vector3(_rt.localPosition.x, _rt.localPosition.y, 0);

        _name.GetComponent<MeshRenderer>().sortingOrder = 100;
    }
}
