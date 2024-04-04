using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeEditorMenu : MonoBehaviour
{
    public Transform rewardParent;
    public Transform immunityParent;
    public GameObject challengePrefab;

    public GameObject curRChallenge;
    public GameObject curIChallenge;
    // Start is called before the first frame update
    void Start()
    {
        curIChallenge.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ConfirmIChallenge);

        curRChallenge.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ConfirmRChallenge);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConfirmRChallenge()
    {
        /*foreach (Transform child in parent)
        {
            if (child.GetChild(0).GetComponentInChildren<Dropdown>().value == curPMAdv.transform.GetChild(0).GetComponentInChildren<Dropdown>().value && child.gameObject.name != curPMAdv.name)
            {
                return;
            }
        }*/
        curRChallenge.transform.GetChild(1).GetComponent<Button>().interactable = false;
        Toggle[] toggles = curRChallenge.transform.GetChild(4).GetComponentsInChildren<Toggle>();
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].interactable = false;
        } 
        
        //curRChallenge.transform.GetChild(2).GetComponentInChildren<Dropdown>().interactable = false;
        curRChallenge = Instantiate(challengePrefab, rewardParent);
        curRChallenge.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ConfirmRChallenge);
        curRChallenge.transform.GetChild(2).GetComponent<Text>().text = "Challenge\n" + (rewardParent.childCount);
        StartCoroutine(ABC());
    }

    public void ConfirmIChallenge()
    {
        /*foreach (Transform child in parent)
        {
            if (child.GetChild(0).GetComponentInChildren<Dropdown>().value == curPMAdv.transform.GetChild(0).GetComponentInChildren<Dropdown>().value && child.gameObject.name != curPMAdv.name)
            {
                return;
            }
        }*/
        curIChallenge.transform.GetChild(1).GetComponent<Button>().interactable = false;
        Toggle[] toggles = curIChallenge.transform.GetChild(4).GetComponentsInChildren<Toggle>();
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].interactable = false;
        }

        //curRChallenge.transform.GetChild(2).GetComponentInChildren<Dropdown>().interactable = false;
        curIChallenge = Instantiate(challengePrefab, immunityParent);
        curIChallenge.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ConfirmIChallenge);
        curIChallenge.transform.GetChild(2).GetComponent<Text>().text = "Challenge\n" + (immunityParent.childCount);
        StartCoroutine(ABC());
    }

    IEnumerator ABC()
    {
        //returning 0 will make it wait 1 frame
        SeasonMenuManager.Instance.editorParent.gameObject.SetActive(!SeasonMenuManager.Instance.editorParent.gameObject.activeSelf);
        yield return 0;
        SeasonMenuManager.Instance.editorParent.gameObject.SetActive(!SeasonMenuManager.Instance.editorParent.gameObject.activeSelf);
        yield return 0;
        SeasonMenuManager.Instance.editorParent.gameObject.SetActive(!SeasonMenuManager.Instance.editorParent.gameObject.activeSelf);
        yield return 0;
        SeasonMenuManager.Instance.editorParent.gameObject.SetActive(!SeasonMenuManager.Instance.editorParent.gameObject.activeSelf);
    }
}
