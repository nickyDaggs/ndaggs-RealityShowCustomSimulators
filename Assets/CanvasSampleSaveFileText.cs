using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;

[RequireComponent(typeof(Button))]
public class CanvasSampleSaveFileText : MonoBehaviour, IPointerDownHandler {
    public Text output;

    // Sample text data
    private string _data = "Example text created by StandaloneFileBrowser";

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    // Broser plugin should be called in OnPointerDown.
    public void OnPointerDown(PointerEventData eventData) {
        var bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(GameManager.Instance.saveThisSeason));
        DownloadFile(gameObject.name, "OnFileDownload", GameManager.Instance.seasonTemp.nameSeason.Replace(" ", ""), bytes, bytes.Length);
    }

    // Called from browser
    public void OnFileDownload() {
        output.text = "File Successfully Downloaded";
    }
#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData) { }

    // Listen OnClick event in standlone builds
    void Start() {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick() {
        string nameNoSpaces = GameManager.Instance.seasonTemp.nameSeason.Replace(" ", "");
        var path = StandaloneFileBrowser.SaveFilePanel("Title", "", nameNoSpaces, "txt");
        if (!string.IsNullOrEmpty(path)) {
            File.WriteAllText(path, JsonUtility.ToJson(GameManager.Instance.saveThisSeason));
        }
    }
#endif
}