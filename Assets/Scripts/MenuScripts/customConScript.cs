﻿using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SFB;

public class customConScript : MonoBehaviour
{

    public List<GameObject> presets;
    public List<GameObject> customs;
    public bool custom = false;
    public bool customImage = false;
    string path;
    string pathCon = "";
    public string urlCon = "";

    public Text pairText;

    // Start is called before the first frame update
    void Start()
    {
        customs[2].GetComponent<InputField>().onEndEdit.AddListener(delegate { CustomImage(customs[2].GetComponent<InputField>()); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Change()
    {
        foreach(GameObject g in presets)
        {
            g.SetActive(!g.activeSelf);
        }
        foreach (GameObject g in customs)
        {
            g.SetActive(!g.activeSelf);
        }
        custom = !custom;
        if(custom == false)
        {
            urlCon = "";
        }
    }

    public void OpenFile()
    {
        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            
        } else
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Show all images (.png)", "", "png", false);
            path = string.Join(path, paths);
            GetComponentsInChildren<InputField>()[0].text = Path.GetFileNameWithoutExtension(path);
            GetComponentsInChildren<InputField>()[1].text = Path.GetFileNameWithoutExtension(path);
            StartCoroutine(UploadImage());

        }
        
    }

    IEnumerator UploadImage()
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("file:///" + path);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            GetComponentInChildren<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        yield return null;
    }

    // Called from browser
    public void OnFileUpload(string url)
    {
        path = url;
        StartCoroutine(UploadImage());
    }

    public void CustomImage(InputField Input)
    {
        //Debug.Log(Url);
        StartCoroutine(DownloadImage(Input.text));
        IEnumerator DownloadImage(string Url)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(Url);
            //Url = "https://www.google.com";
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                GetComponentInChildren<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                urlCon = Url;
            }
            
        }
    }


}
