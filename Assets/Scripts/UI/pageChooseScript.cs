using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pageChooseScript : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEpisodeChange()
    {
        GameManager.instance.nextButton.gameObject.SetActive(true);
        GameManager.instance.lastThing.SetActive(false);
        GameManager.instance.currentSeason.Episodes[GetComponent<Dropdown>().value].events[0].obj.SetActive(true);
        GameManager.instance.curEp = GetComponent<Dropdown>().value;
        GameManager.instance.curEv = 1;
        GameManager.instance.lastThing = GameManager.instance.currentSeason.Episodes[GetComponent<Dropdown>().value].events[0].obj;
    }
}
