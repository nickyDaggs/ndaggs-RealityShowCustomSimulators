﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeasonParts;

public class ExileMenu : MonoBehaviour
{
    public InputField skip;
    public Text skipText;
    public List<int> skips = new List<int>();
    public List<Dropdown> PreMerge = new List<Dropdown>();
    public List<Dropdown> Merge = new List<Dropdown>();
    public Dropdown endAt;
    public GameObject curAdv;
    public GameObject AdvantagePrefab;
    public Transform AdvantageParent;
    public RectTransform editorParent;

    public Dropdown islandChoice;
    public Button islandButton;
    public Image islandImage;

    public Sprite EIImage;
    public Sprite GIImage;
    public Sprite JImage;

    public List<Dropdown> PreMergeGI = new List<Dropdown>();
    public List<Dropdown> MergeGI = new List<Dropdown>();
    public Dropdown endAtGI;
    public GameObject curAdvGI;
    public GameObject GIAdvantagePrefab;
    public Transform GIAdvantageParent;

    public GameObject RegularJourney;
    public GameObject curJourney;
    public GameObject JourneyPrefab;
    public Transform SpecialJourneyParent;

    // Start is called before the first frame update
    void Start()
    {
        //skip.onEndEdit.AddListener(skipEnter);
        curAdv = AdvantageParent.GetChild(0).gameObject;
        curAdv.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ConfirmAdv);

        curAdvGI = GIAdvantageParent.GetChild(0).gameObject;
        curAdvGI.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ConfirmAdvGI);
        curJourney = SpecialJourneyParent.GetChild(0).gameObject;
        curJourney.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ConfirmJourney);
        //Debug.Log());
        StartCoroutine(WaitBro());

        IEnumerator WaitBro()
        {

            yield return new WaitForSeconds(2f);
            islandButton.onClick.AddListener(SeasonMenuManager.Instance.EnableExile);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Exile GetPMExile()
    {
        Exile exile = new Exile();
        exile.on = true;
        if (PreMerge[0].options[PreMerge[0].value].text == "Winner")
        {
            exile.reason = "Winner";
        } else
        {
            exile.reason = "Loser";
        }
        if (PreMerge[1].options[PreMerge[1].value].text == "Reward Challenge")
        {
            exile.challenge = "Reward";
        }
        else
        {
            exile.challenge = "Immunity";
        }
        if (PreMerge[2].options[PreMerge[2].value].text == "Own Tribe")
        {
            exile.ownTribe = true;
        }
        if (PreMerge[3].options[PreMerge[3].value].text == "Two")
        {
            exile.two = true;
        }
        switch(PreMerge[4].options[PreMerge[4].value].text)
        {
            case "None":
                exile.exileEvent = "Nothing";
                break;
            case "Mutiny Urn":
                exile.exileEvent = "UrnMutiny";
                break;
            case "Safety":
                exile.exileEvent = "Safety";
                break;
        } 

        if (PreMerge[5].options[PreMerge[5].value].text == "Yes")
        {
            exile.skipTribal = true;
        } 

        

        return exile;
    }
    public Exile GetMExile()
    {
        Exile exile = new Exile();
        exile.on = true;
        exile.reason = "Winner";
        
        if (Merge[0].options[Merge[0].value].text == "Reward Challenge")
        {
            exile.challenge = "Reward";
        }
        else
        {
            exile.challenge = "Immunity";
        }

        switch (Merge[1].options[Merge[1].value].text)
        {
            case "None":
                exile.exileEvent = "Nothing";
                break;
            case "Safety":
                exile.exileEvent = "Safety";
                break;
        }

        if (Merge[2].options[Merge[2].value].text == "Yes")
        {
            exile.skipTribal = true;
        }


        return exile;
    }

    void ConfirmAdv()
    {
        foreach(Transform child in AdvantageParent)
        {
            if(child.GetChild(0).GetComponentInChildren<Dropdown>().value ==  curAdv.transform.GetChild(0).GetComponentInChildren<Dropdown>().value && child.gameObject.name != curAdv.name)
            {
                return;
            }
        }
        curAdv.transform.GetChild(1).GetComponent<Button>().interactable = false;
        curAdv.transform.GetChild(0).GetComponentInChildren<Dropdown>().interactable = false;
        curAdv.transform.GetChild(2).GetComponentInChildren<Dropdown>().interactable = false;
        curAdv = Instantiate(AdvantagePrefab, AdvantageParent);
        curAdv.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ConfirmAdv);
        StartCoroutine(ABC());
    }

    public Exile GetPMGhost()
    {
        Exile exile = new Exile();
        exile.on = true;
        if (PreMergeGI[0].options[PreMergeGI[0].value].text == "Winner")
        {
            exile.reason = "Winner";
        }
        else
        {
            exile.reason = "Loser";
        }
        if (PreMergeGI[1].options[PreMergeGI[1].value].text == "Reward Challenge")
        {
            exile.challenge = "Reward";
        }
        else
        {
            exile.challenge = "Immunity";
        }
        if (PreMergeGI[2].options[PreMergeGI[2].value].text == "Own Tribe")
        {
            exile.ownTribe = true;
        }
        switch (PreMergeGI[4].options[PreMergeGI[4].value].text)
        {
            case "None":
                exile.exileEvent = "Nothing";
                break;
            case "Mutiny Urn":
                exile.exileEvent = "UrnMutiny";
                break;
            case "Safety":
                exile.exileEvent = "Safety";
                break;
        }

        if (PreMergeGI[5].options[PreMergeGI[5].value].text == "Yes")
        {
            exile.skipTribal = true;
        }



        return exile;
    }
    public Exile GetMGhost()
    {
        Exile exile = new Exile();
        exile.on = true;
        exile.reason = "Winner";

        if (MergeGI[0].options[MergeGI[0].value].text == "Reward Challenge")
        {
            exile.challenge = "Reward";
        }
        else
        {
            exile.challenge = "Immunity";
        }

        switch (MergeGI[1].options[MergeGI[1].value].text)
        {
            case "None":
                exile.exileEvent = "Nothing";
                break;
            case "Safety":
                exile.exileEvent = "Safety";
                break;
        }

        if (MergeGI[2].options[MergeGI[2].value].text == "Yes")
        {
            exile.skipTribal = true;
        }


        return exile;
    }

    public Exile GetJourney(Transform prefab)
    {
        Exile exile = new Exile();


        switch(prefab.GetChild(2).GetComponent<Dropdown>().value)
        {
            case 0:
                exile.exileEvent = "Shipwheel";
                break;
            case 1:
                exile.exileEvent = "Tarp";
                break;
            case 2:
                exile.exileEvent = "PublicRisk";
                break;
            case 3:
                exile.exileEvent = "ForcedBag";
                break;
            case 4:
                exile.exileEvent = "IndividualChallenge";
                break;
        }

        switch (prefab.GetChild(4).GetComponentInChildren<Dropdown>().value)
        {
            case 0:
                exile.reason = "Else";
                exile.challenge = "Immunity";
                exile.two = true;
                break;
            case 1:
                exile.reason = "Else";
                exile.challenge = "Immunity";
                exile.two = false;
                break;
            case 2:
                exile.reason = "Random";
                exile.challenge = "Random";
                break;
        }

        

        return exile;
    }

    public Exile RegJourney(Transform prefab)
    {
        Exile exile = new Exile();


        switch (prefab.GetChild(0).GetComponent<Dropdown>().value)
        {
            case 0:
                exile.exileEvent = "Shipwheel";
                break;
            case 1:
                exile.exileEvent = "Tarp";
                break;
            case 2:
                exile.exileEvent = "PublicRisk";
                break;
            case 3:
                exile.exileEvent = "ForcedBag";
                break;
            case 4:
                exile.exileEvent = "IndividualChallenge";
                break;
        }

        switch (prefab.GetChild(1).GetComponentInChildren<Dropdown>().value)
        {
            case 0:
                exile.reason = "Else";
                exile.challenge = "Immunity";
                exile.two = true;
                break;
            case 1:
                exile.reason = "Else";
                exile.challenge = "Immunity";
                exile.two = false;
                break;
            case 2:
                exile.reason = "Random";
                exile.challenge = "Random";
                break;
        }
        int contest = SeasonMenuManager.Instance.contestants;
        for(int i = contest; contest < SeasonMenuManager.Instance.customSeason.mergeAt; i--)
        {
            if(!SeasonMenuManager.Instance.customSeason.Twists.epsSpecialE.Contains(i))
            {
                HiddenAdvantage adv = new HiddenAdvantage();
                adv.advantage = SeasonMenuManager.Instance.advantages[prefab.GetChild(2).GetComponentInChildren<Dropdown>().value];
                adv.hidden = true;
                adv.reHidden = false;
                
                adv.hideAt = i;
                adv.name = adv.advantage.nickname;

                if (adv.advantage.type == "HiddenImmunityIdol")
                {
                    SeasonMenuManager.Instance.customSeason.idolLimit++;
                }
                switch (prefab.GetChild(5).GetComponentInChildren<Dropdown>().value)
                {
                    case 0:
                        adv.IOILesson = "Tribal";
                        break;
                    case 1:
                        adv.IOILesson = "Camp";
                        break;
                }
                SeasonMenuManager.Instance.customSeason.islandHiddenAdvantages.Add(adv);
            }
        }

        return exile;
    }

    void ConfirmAdvGI()
    {
        if(int.Parse(curAdvGI.transform.GetChild(2).GetComponentInChildren<InputField>().text) > SeasonMenuManager.instance.contestantsFull)
        {
            return;
        }
        foreach (Transform child in GIAdvantageParent)
        {
            if (child.gameObject.name != curAdvGI.name)
            {
                //return;
            }
        }
        curAdvGI.transform.GetChild(1).GetComponent<Button>().interactable = false;
        curAdvGI.transform.GetChild(0).GetComponentInChildren<Dropdown>().interactable = false;
        curAdvGI.transform.GetChild(2).GetComponentInChildren<InputField>().interactable = false;
        curAdvGI = Instantiate(GIAdvantagePrefab, GIAdvantageParent);
        curAdvGI.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ConfirmAdvGI);
        StartCoroutine(ABC());
    }

    void ConfirmJourney()
    {
        if (int.Parse(curJourney.transform.GetChild(2).GetComponentInChildren<InputField>().text) > SeasonMenuManager.instance.contestantsFull)
        {
            return;
        }
        curJourney.transform.GetChild(0).GetComponentInChildren<Dropdown>().interactable = false;
        curJourney.transform.GetChild(1).GetComponent<Button>().interactable = false;
        curJourney.transform.GetChild(2).GetComponentInChildren<InputField>().interactable = false;
        curJourney.transform.GetChild(3).GetComponentInChildren<Dropdown>().interactable = false;
        curJourney.transform.GetChild(4).GetComponentInChildren<Dropdown>().interactable = false;
        curJourney.transform.GetChild(5).GetComponentInChildren<Dropdown>().interactable = false;
        curJourney = Instantiate(JourneyPrefab, SpecialJourneyParent);
        curJourney.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ConfirmJourney);
        curJourney.transform.GetChild(0).GetComponentInChildren<Dropdown>().interactable = true;
        curJourney.transform.GetChild(1).GetComponent<Button>().interactable = true;
        curJourney.transform.GetChild(2).GetComponentInChildren<InputField>().interactable = true;
        curJourney.transform.GetChild(3).GetComponentInChildren<Dropdown>().interactable = true;
        curJourney.transform.GetChild(4).GetComponentInChildren<Dropdown>().interactable = true;
        curJourney.transform.GetChild(5).GetComponentInChildren<Dropdown>().interactable = true;
        Debug.Log("fdafesd");
        StartCoroutine(ABC());
    }

    public void skipEnter()
    {
        
        int num = int.Parse(skip.text);
        skipText.text += " ";
        if(skips.Contains(num))
        {
            skips.Remove(num);
            //skipText.text.Replace(" F" + num + " ", "");
        }
        else
        {
            skips.Add(num);
        }

        skipText.text = "Skipping at:";
        for(int i = 0; i < skips.Count; i++)
        {
            skipText.text += " F" + skips[i] + " ";

        }
    }

    public void ChooseIsland()
    {
        if(islandChoice.value == 0)
        {
            islandImage.sprite = EIImage;
            islandButton.GetComponentInChildren<Text>().text = "Exile Island";
            islandButton.onClick.RemoveAllListeners();
            islandButton.onClick.AddListener(SeasonMenuManager.instance.EnableExile);
        } else if(islandChoice.value == 1)
        {
            islandImage.sprite = GIImage;
            islandButton.GetComponentInChildren<Text>().text = "Ghost Island";
            islandButton.onClick.RemoveAllListeners();
            islandButton.onClick.AddListener(SeasonMenuManager.instance.EnableGhost);
        } else if(islandChoice.value == 2)
        {
            islandImage.sprite = JImage;
            islandButton.GetComponentInChildren<Text>().text = "Journeys";
            islandButton.onClick.RemoveAllListeners();
            islandButton.onClick.AddListener(SeasonMenuManager.instance.EnableJourneys);
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
