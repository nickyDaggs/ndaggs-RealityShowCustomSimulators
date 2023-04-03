using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using SFB;
using System.IO;

[RequireComponent(typeof(Button))]
public class CanvasSampleOpenFileImage : MonoBehaviour, IPointerDownHandler {
    public Image output;

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //

    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData)  {
        UploadFile(gameObject.name, "OnFileUpload", ".png, .jpg", false);
    }

    // Called from browser
    public void OnFileUpload(string url) {
        StartCoroutine(OutputRoutine(url));
    }
#else
    //
    // Standalone platforms & editor
    //

    public void OnPointerDown(PointerEventData eventData) {
        //Debug.Log(gameObject.name);
    }
    
    /*void Start() {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick() {
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "png", false);
        if (paths.Length > 0) {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }*/
#endif

    private IEnumerator OutputRoutine(string url) {
        var loader = new WWW(url);
        yield return loader;

        //transform.parent.GetComponentsInChildren<InputField>()[0].text = Path.GetFileName(url);
        //transform.parent.GetComponentsInChildren<InputField>()[1].text = Path.GetFileName(url);
        transform.parent.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(loader.texture, new Rect(0, 0, loader.texture.width, loader.texture.height), Vector2.zero);
    }
}