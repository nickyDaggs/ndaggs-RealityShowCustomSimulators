using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupLayout : MonoBehaviour
{
    public RectTransform Content;
    public ContentSizeFitter _ContentSizeFitter;
    // Start is called before the first frame update
    public void Start()
    {
        StartCoroutine(RefreshLayout(gameObject));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator RefreshLayout(GameObject layout)
    {
        yield return new WaitForFixedUpdate();
        _ContentSizeFitter.enabled = true;
        _ContentSizeFitter.SetLayoutVertical();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Content.transform);
        //_ContentSizeFitter.enabled = false;
    }
}
