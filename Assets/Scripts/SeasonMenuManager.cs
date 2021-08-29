using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SeasonParts;
using System.Linq;

public class SeasonMenuManager : MonoBehaviour
{
    [System.Serializable]
    public class Option
    {
        public GameObject button;
        public string option;
        public string optionBool;
    }
    
    public bool cineTribal, absorb;

    [Header("Preset Seasons")]

    public List<SeasonTemplate> seasons;
    public List<Cast> casts;
    List<Transform> buttons;
    public GameObject buttonParent;
    public static SeasonMenuManager instance;
    public static SeasonMenuManager Instance { get { return instance; } }

    public List<Option> options;

    public SeasonTemplate curSeason;
    public Cast curCast;

    [Header("Custom Season")]

    public SeasonTemplate customSeason;
    public Cast customCast;

    bool canSim = false;
    int filledOptions = 0;

    public List<GameObject> editorOptions;
    public GameObject tribeSizer;
    public GameObject tribeSizeParent;
    public Button simButton;
    public RectTransform editorParent;
    public GameObject editorTrueParent;
    float ogY;
    float tribes;
    public int contestants;
    int mergeLimit = 5;
    int minCon = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        customSeason = new SeasonTemplate();
        customCast = Instantiate(customCast);
        ogY = editorParent.sizeDelta.y;
        editorOptions[0].GetComponent<InputField>().onValueChanged.AddListener(SubmitSeasonName);
        editorOptions[1].GetComponent<InputField>().onEndEdit.AddListener(SubmitTribeAmount);
        editorOptions[2].GetComponent<Dropdown>().onValueChanged.AddListener(delegate { SubmitFinalAmount(editorOptions[2].GetComponent<Dropdown>()); }) ;
        editorOptions[3].GetComponent<Dropdown>().onValueChanged.AddListener(delegate { ChooseReturningPlayers(editorOptions[3].GetComponent<Dropdown>()); }) ;
        editorOptions[4].GetComponent<InputField>().onEndEdit.AddListener(SubmitMergeAmount);
        editorOptions[5].GetComponent<InputField>().onEndEdit.AddListener(SubmitJuryAmount);
        customSeason.final = editorOptions[2].GetComponent<Dropdown>().value;
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < buttonParent.transform.childCount; i++)
        {
            int num = i;
            buttonParent.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => StartSeason(num));
            buttonParent.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = seasons[num].nameSeason;
        }
    }

    private void Update()
    {
        //simButton.interactable = canSim;
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            CheckCustomSeason();

            if (tribeSizeParent.transform.childCount > 1)
            {
                List<int> cont = new List<int>();
                foreach (Transform child in tribeSizeParent.transform)
                {
                    if (child.GetChild(3).GetComponent<InputField>().text != "")
                    {
                        cont.Add(int.Parse(child.GetChild(3).GetComponent<InputField>().text));
                    }
                }
                contestants = cont.Sum();

            }

            if (contestants < 8)
            {
                editorOptions[4].GetComponent<InputField>().interactable = false;
                editorOptions[5].GetComponent<InputField>().interactable = false;
            }
            else
            {
                editorOptions[4].GetComponent<InputField>().interactable = true;
                editorOptions[5].GetComponent<InputField>().interactable = true;
            }
        }
        
    }

    // Update is called once per frame
    public void ChangeOption()
    {
        foreach (Option opt in options)
        {
            if(opt.button == UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject)
            {
                string last = opt.button.GetComponentInChildren<Text>().text;
                opt.button.GetComponentInChildren<Text>().text = opt.option;
                opt.option = last;
                if (opt.optionBool == "Custom")
                {
                    buttonParent.SetActive(!buttonParent.activeSelf);
                    //Debug.Log(editorParent.gameObject.activeSelf);
                    editorTrueParent.SetActive(!editorTrueParent.activeSelf);
                }
            }
        }
    }
    public void StartSeason(int season)
    {
        foreach (Option opt in options)
        {
            if (opt.button.GetComponentInChildren<Text>().text.Contains("On") || opt.button.GetComponentInChildren<Text>().text.Contains("Preset"))
            {
                if(opt.optionBool == "CT")
                {
                    cineTribal = true;
                } else if (opt.optionBool == "absorb")
                {
                    absorb = true;
                } else if (opt.optionBool == "Custom")
                {
                    foreach (Transform child in tribeSizeParent.transform)
                    {
                        int num = int.Parse(child.GetChild(3).GetComponent<InputField>().text);

                        Team tribe = new Team() { name = child.GetChild(1).GetComponent<InputField>().text, members = new List<Contestant>(new Contestant[int.Parse(child.GetChild(3).GetComponent<InputField>().text)]), tribeColor = child.GetChild(4).GetComponent<FlexibleColorPicker>().color };
                        //Debug.Log("Tribe:" + tribe.name + ColorUtility.ToHtmlStringRGBA(tribe.tribeColor));
                        customSeason.Tribes.Add(tribe);
                    }
                    customSeason.MergeTribeName = editorOptions[6].transform.GetChild(1).GetComponent<InputField>().text;
                    customSeason.MergeTribeColor = editorOptions[6].transform.GetChild(4).GetComponent<FlexibleColorPicker>().color;
                    seasons[season] = customSeason;

                    for (int i = customCast.cast.Count - 1; i > 0; i--)
                    {
                        int rnd = Random.Range(0, i);

                        Contestant temp = customCast.cast[i];

                        customCast.cast[i] = customCast.cast[rnd];
                        customCast.cast[rnd] = temp;
                    }

                    for (int i = 0; i < 24 - contestants; i++)
                    {
                        customCast.cast.Remove(customCast.cast[customCast.cast.Count - 1]);
                    }
                    casts[season] = customCast;
                    
                }

            } else
            {
                
            }
        }
        curSeason = seasons[season];
        curCast = casts[season];
        SceneManager.LoadScene(1);
    }

    private void SubmitSeasonName(string aa)
    {
        customSeason.nameSeason = aa;
    }

    private void SubmitTribeAmount(string aa)
    {
        if (aa != "")
        {
            float newY = editorParent.sizeDelta.y;
            contestants = 0;
            int num = int.Parse(aa);
            if (num == 0)
            {
                customSeason.Tribes = new List<Team>();
            }
            if (num > 24)
            {
                num = 24;
                editorOptions[1].GetComponent<InputField>().text = "24";
            }
            else if (num < 2)
            {
                num = 2;
                editorOptions[1].GetComponent<InputField>().text = "2";
            }
            float teamSize = Mathf.Round(24 / num);

            tribes = num;
            for (int i = 0; i < num; i++)
            {
                if (i + 1 > tribeSizeParent.transform.childCount)
                {
                    GameObject size = Instantiate(tribeSizer);
                    size.transform.SetParent(tribeSizeParent.transform);

                    float ySize = size.GetComponent<RectTransform>().sizeDelta.y;
                    if (i == num - 1)
                    {
                        //ySize += 10;
                    }
                    editorParent.sizeDelta = new Vector2(editorParent.sizeDelta.x, newY + ySize);
                    newY = editorParent.sizeDelta.y;
                }
            }

            for (int i = 0; i < tribeSizeParent.transform.childCount; i++)
            {
                GameObject size = tribeSizeParent.transform.GetChild(i).gameObject;
                if (i < num)
                {
                    size.transform.GetChild(3).GetComponent<InputField>().text = teamSize.ToString();
                    int s = (int)teamSize;
                    if (0 > 24 - teamSize * (i + 1))
                    {
                        size.transform.GetChild(3).GetComponent<InputField>().text = (24 - teamSize * (i + 1)).ToString();
                        s = 24 - (int)teamSize * (i + 1);
                    }
                    //contestants += s;
                }
                else
                {
                    float ySize = size.GetComponent<RectTransform>().sizeDelta.y;

                    editorParent.sizeDelta = new Vector2(editorParent.sizeDelta.x, newY - ySize);
                    newY = editorParent.sizeDelta.y;
                    Destroy(size);
                }

            }
            if (num - tribeSizeParent.transform.childCount > 0)
                editorParent.sizeDelta = new Vector2(editorParent.sizeDelta.x, newY + 10);
        }
    }

    private void SubmitMergeAmount(string aa)
    {
        if(aa !=  "")
        {
            int num = int.Parse(aa);
            int n = 1;
            if (minCon > 1)
            {
                n = 2;
            }
            if (num >= contestants - n)
            {
                num = contestants - n;
            }
            if (num < mergeLimit)
            {
                num = mergeLimit;
            }
            if (tribeSizeParent.transform.childCount > 1)
            {
                List<int> cont = new List<int>();
                foreach (Transform child in tribeSizeParent.transform)
                {
                    if (child.GetChild(3).GetComponent<InputField>().text != "")
                    {
                        cont.Add(int.Parse(child.GetChild(3).GetComponent<InputField>().text));
                    }
                }
                cont = cont.OrderByDescending(x => x).ToList();
                int pre = contestants - num;
                int loseTeams = 0;
                foreach (int numb in cont)
                {
                    if(pre >= numb)
                    {
                        loseTeams++;
                        pre -= numb;
                    }
                }
                if(loseTeams == cont.Count || (loseTeams == cont.Count - 1 && cont[0] > num))
                {
                    num = cont[0];
                    if(7 - cont[0] > 0)
                    {
                        num += 7 - cont[0];
                    }
                }
            }
            editorOptions[4].GetComponent<InputField>().text = num.ToString();
            customSeason.mergeAt = num;
        } else
        {
            customSeason.mergeAt = 0;
        }
        
    }

    private void SubmitJuryAmount(string aa)
    {
        if (aa != "")
        {
            int num = int.Parse(aa);
            if (num >= contestants - customSeason.final)
            {
                num = contestants - (int)customSeason.final;
            }
            if(customSeason.Outcasts && contestants - (num + customSeason.final) < contestants - customSeason.mergeAt)
            {
                num = (int)customSeason.mergeAt - (int)customSeason.final;
            }
            editorOptions[5].GetComponent<InputField>().text = num.ToString();
            customSeason.jury = num;
        } else
        {
            customSeason.jury = 0;
        }
    }

    private void SubmitFinalAmount(Dropdown change)
    {
        customSeason.final = int.Parse(change.options[change.value].text);
        if(customSeason.jury > contestants - customSeason.final && customSeason.jury > 0)
        {
            customSeason.jury = contestants - customSeason.final;
            editorOptions[5].GetComponent<InputField>().text = customSeason.jury.ToString();
        }
    }

    private void ChooseReturningPlayers(Dropdown change)
    {
        if(change.captionText.text == "None")
        {
            customSeason.EdgeOfExtinction = false;
            customSeason.RedemptionIsland = false;
            customSeason.Outcasts = false;
            mergeLimit = 5;
            minCon = 0;
        }
        else if(change.captionText.text == "EOE")
        {
            customSeason.EdgeOfExtinction = true;
            customSeason.RedemptionIsland = false;
            customSeason.Outcasts = false;
            mergeLimit = 7;
            minCon = 2;
        }
        else if (change.captionText.text == "RI")
        {
            customSeason.EdgeOfExtinction = false;
            customSeason.RedemptionIsland = true;
            customSeason.Outcasts = false;
            mergeLimit = 6;
            minCon = 2;
        }
        else if (change.captionText.text == "Outcasts")
        {
            customSeason.EdgeOfExtinction = false;
            customSeason.RedemptionIsland = false;
            customSeason.Outcasts = true;
            mergeLimit = 6;
            minCon = 2;
            if(contestants - (customSeason.jury + customSeason.final) < contestants - customSeason.mergeAt)
            {
                customSeason.jury = customSeason.mergeAt - customSeason.final;
                editorOptions[5].GetComponent<InputField>().text = customSeason.jury.ToString();
            }
        }
        if(customSeason.mergeAt < mergeLimit)
        {
            customSeason.mergeAt = mergeLimit;
            editorOptions[4].GetComponent<InputField>().text = customSeason.mergeAt.ToString();
        }
        int n = 1;
        if (minCon > 1)
        {
            n = 2;
        }
        if (customSeason.mergeAt >= contestants - n)
        {
            customSeason.mergeAt = contestants - n;
            editorOptions[4].GetComponent<InputField>().text = customSeason.mergeAt.ToString();
        }
    }

    public void CheckCustomSeason()
    {
        if (tribeSizeParent.transform.childCount > 1)
        {
            List<int> cont = new List<int>();
            foreach (Transform child in tribeSizeParent.transform)
            {
                if (child.GetChild(3).GetComponent<InputField>().text != "")
                {
                    cont.Add(int.Parse(child.GetChild(3).GetComponent<InputField>().text));
                }
            }
            cont = cont.OrderByDescending(x => x).ToList();
            int pre = contestants - (int)customSeason.mergeAt;
            int loseTeams = 0;
            foreach (int numb in cont)
            {
                if (pre >= numb)
                {
                    loseTeams++;
                    pre -= numb;
                }
            }
            if (loseTeams == cont.Count || (loseTeams == cont.Count - 1 && cont[0] > customSeason.mergeAt))
            {
                customSeason.mergeAt = 0;
            }
        }
        int n = 1;
        if(minCon > 1)
        {
            n = 2;
        }
        if (customSeason.nameSeason != null && customSeason.nameSeason != "" && customSeason.jury > 0 && customSeason.mergeAt > 0 && customSeason.mergeAt <= contestants - n && tribes > 1 && contestants >= 7 + minCon && contestants < 25 )
        {
            simButton.interactable = true;
        } else
        {
            simButton.interactable = false;
        }
    }

}
