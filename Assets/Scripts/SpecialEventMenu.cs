using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeasonParts;

public class SpecialEventMenu : MonoBehaviour
{
    public List<Dropdown.OptionData> Rounds = new List<Dropdown.OptionData>();
    public List<Dropdown.OptionData> RoundsClone = new List<Dropdown.OptionData>();
    public List<Dropdown.OptionData> PMEvents2;
    public List<Dropdown.OptionData> PMEvents;
    public List<Dropdown.OptionData> MEvents;
    public GameObject curEvent;
    public Transform eventsParent;
    public Dropdown curRound;
    public Dropdown curEvents;
    public GameObject prefabEvent;
    [HideInInspector]public float mergeRound;
    public RectTransform editorParent;
    public Button deleteRound;
    // Start is called before the first frame update
    public void StartSpecial()
    {
        //Rounds = new List<Dropdown.OptionData> { new Dropdown.OptionData("22"), new Dropdown.OptionData("21"), new Dropdown.OptionData("20"), new Dropdown.OptionData("19"), new Dropdown.OptionData("18") };

        
        curEvent = Instantiate(prefabEvent, eventsParent);
        curRound = curEvent.transform.GetChild(0).GetComponentInChildren<Dropdown>();
        curEvents = curEvent.transform.GetChild(1).GetComponentInChildren<Dropdown>();
        curRound.onValueChanged.AddListener(delegate { ChangeRound(curRound); });
        curRound.options = new List<Dropdown.OptionData>(Rounds);
        curRound.value = 0;
        curEvent.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(ConfirmEvent);
        ChangeRound(curRound);
        /*trash*/

    }


    // Update is called once per frame
    void Update()
    {
        if(eventsParent.childCount > 1)
        {
            //deleteRound.interactable = true;
        } else
        {
            //deleteRound.interactable = false;
        }
    }

    void ConfirmEvent()
    {
        List<Team> t = new List<Team>();
        float con = int.Parse(curRound.options[curRound.value].text);
        if (con > SeasonMenuManager.Instance.customSeason.mergeAt)
        {
            t = SwapsMenu.Instance.GetTribesAt(curRound);
            //Debug.Log(t.Count);
        }
        if(curRound.value > 0)
        {
            //Rounds.Remove(Rounds[curRound.value - 1]);
        }
        
        switch (curEvents.options[curEvents.value].text)
        {
            case "Multi-Tribal":
            case "Multi-Tribal(Win Reward for immunity)":
            case "Multi-Tribal(Reward Only)":
                for (int i = t.Count - 1; i > 0; i--)
                {
                    if (RoundsClone.IndexOf(Rounds[curRound.value]) + 1 < RoundsClone.Count && RoundsClone.IndexOf(Rounds[curRound.value]) + 1 != -1)
                    {
                        RoundsClone.Remove(RoundsClone[RoundsClone.IndexOf(Rounds[curRound.value]) + 1]);
                    }
                    
                }
                
                for (int i = 0; i < t.Count - 1; i++)
                {
                    Rounds.Remove(Rounds[curRound.value]);
                }
                for (int i = t.Count - 1; i > 0; i--)
                {
                    if (curRound.value - 1 > -1)
                    {
                        //Debug.Log(Rounds[curRound.value - 1].text);
                        //Rounds.Remove(Rounds[curRound.value - 1]);
                    }
                }
                break;
            case "Multi-Tribal(One Tribe Immunity)":
                for (int i = t.Count - 1; i > 0; i--)
                {
                    if(RoundsClone.IndexOf(Rounds[curRound.value]) + 1 < RoundsClone.Count && RoundsClone.IndexOf(Rounds[curRound.value]) + 1 != -1)
                    {
                        RoundsClone.Remove(RoundsClone[RoundsClone.IndexOf(Rounds[curRound.value]) + 1]);
                    }
                }
                
                for (int i = 0; i < t.Count - 2; i++)
                {
                    Rounds.Remove(Rounds[curRound.value]);
                }
                for (int i = t.Count - 1; i > 0; i--)
                {
                    
                    if (curRound.value - 1 > -1)
                    {
                        //Debug.Log(Rounds[curRound.value - 1].text);
                        //Rounds.Remove(Rounds[curRound.value - 1]);
                    }
                }
                break;
            case "Merge Split":
                if (RoundsClone.IndexOf(Rounds[curRound.value]) + 1 < RoundsClone.Count && RoundsClone.IndexOf(Rounds[curRound.value]) + 1 != -1)
                {
                    RoundsClone.Remove(RoundsClone[RoundsClone.IndexOf(Rounds[curRound.value]) + 1]);
                    Debug.Log(RoundsClone.IndexOf(Rounds[curRound.value]));
                    Rounds.Remove(Rounds[curRound.value]);
                }
                
                break;
        }
        if(Rounds.Count > 0)
        {
            Rounds.Remove(Rounds[curRound.value]);
            
        }

        curEvent.transform.GetChild(2).GetComponent<Button>().interactable = false;
        curRound.interactable = false;
        curEvents.interactable = false;
        curEvent = Instantiate(prefabEvent, eventsParent);
        curRound = curEvent.transform.GetChild(0).GetComponentInChildren<Dropdown>();
        curEvents = curEvent.transform.GetChild(1).GetComponentInChildren<Dropdown>();
        curRound.onValueChanged.AddListener(delegate { ChangeRound(curRound); });
        curRound.options = new List<Dropdown.OptionData>(Rounds);
        curEvent.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(ConfirmEvent);
        ChangeRound(curRound);
        StartCoroutine(ABC());

    }

    public void deleteCurRound()
    {

        Destroy(eventsParent.GetChild(eventsParent.childCount - 1));

    }

    void ChangeRound(Dropdown f)
    {
        //Debug.Log(int.Parse(curRound.options[curRound.value].text));
        float con = int.Parse(curRound.options[curRound.value].text);
        if (con > SeasonMenuManager.Instance.customSeason.mergeAt)
        {
            List<Team> t = SwapsMenu.Instance.GetTribesAt(curRound);
            if(t.Count > 2)
            {
                curEvents.options = new List<Dropdown.OptionData>(PMEvents);

            }
            else
            {
                curEvents.options = new List<Dropdown.OptionData>(PMEvents2);

            }
        } else if (con <= SeasonMenuManager.Instance.customSeason.mergeAt)
        {
            curEvents.options = new List<Dropdown.OptionData>(MEvents);
        } 
    }
    IEnumerator ABC()
    {

        //returning 0 will make it wait 1 frame
        editorParent.gameObject.SetActive(!editorParent.gameObject.activeSelf);
        yield return 0;
        editorParent.gameObject.SetActive(!editorParent.gameObject.activeSelf);
        yield return 0;
        editorParent.gameObject.SetActive(!editorParent.gameObject.activeSelf);
        yield return 0;
        editorParent.gameObject.SetActive(true);

    }
}
