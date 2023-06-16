using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sharedScrollbar : MonoBehaviour
{
    public Scrollbar share;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void sameValue()
    {
        GetComponent<Scrollbar>().value = share.value;
    }
}
