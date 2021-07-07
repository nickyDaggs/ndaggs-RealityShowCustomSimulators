using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadingAnimation : MonoBehaviour
{
    public void Animate()
    {
        switch(GetComponent<Text>().text)
        {
            case "Loading.":
                GetComponent<Text>().text = "Loading..";
                break;
            case "Loading..":
                GetComponent<Text>().text = "Loading...";
                break;
            case "Loading...":
                GetComponent<Text>().text = "Loading.";
                break;
        }
    }
}
