using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
    
    public bool cineTribal, absorb, randomStat;

    [Header("Preset Seasons")]

    public List<SeasonTemplate> seasons;
    public List<Cast> casts;
    List<Transform> buttons;
    public List<Button> confirms;
    public GameObject buttonParent;
    public GameObject backButton;
    public static SeasonMenuManager instance;
    public static SeasonMenuManager Instance { get { return instance; } }

    public List<Option> options;
    public Dropdown Enviro;

    public List<Dropdown.OptionData> swapOptions;
    public List<Dropdown.OptionData> premergeRounds;
    public List<GameObject> swapPrefabs;

    public SeasonTemplate curSeason;
    public Cast curCast;

    [Header("Custom Season")]

    public SeasonTemplate customSeason;
    public Cast customCast;

    bool canSim = false;
    int filledOptions = 0;


    public Advantage idol;

    public List<GameObject> editorOptions;
    public GameObject tribeSizePRE;
    public GameObject tribeSizer;
    public GameObject tribeSizeParent;
    public GameObject swapPrefab;
    public GameObject swapParent;
    public Button simButton;
    public Button playSimButton;
    public RectTransform editorParent;
    public GameObject editorTrueParent;
    public GameObject castEditor;
    public GameObject contestantPrefab;
    float ogY;
    float tribes;
    public int contestants;
    int mergeLimit = 5;
    int minCon = 0;
    bool part1 = false;
    [HideInInspector] public float ogSwapY;

    [HideInInspector] List<string> noPre = new List<string>() { "Mutiny", "Split Tribes (Guatemala)", "Shuffle(Same Tribe Size)" };

    List<Contestant> allContestants = new List<Contestant>();

    public List<Dropdown.OptionData> everyContestant;
    List<Dropdown.OptionData> usedContestants = new List<Dropdown.OptionData>();

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
        //editorOptions[7].GetComponent<InputField>().onEndEdit.AddListener(SubmitSwapAmount);
        //editorOptions[4].GetComponent<InputField>().interactable = false;
        //editorOptions[5].GetComponent<InputField>().interactable = false;
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
            buttonParent.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => PresetCastEdit(num));
            buttonParent.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = seasons[num].nameSeason;
        }
        allContestants = GetAllInstances<Contestant>().ToList();

        everyContestant = allContestants.ConvertAll(x => new Dropdown.OptionData { text = x.name, image = x.image });
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
            /*
            if (contestants < 8)
            {
                editorOptions[4].GetComponent<InputField>().interactable = false;
                editorOptions[5].GetComponent<InputField>().interactable = false;
            }
            else
            {
                editorOptions[4].GetComponent<InputField>().interactable = true;
                editorOptions[5].GetComponent<InputField>().interactable = true;
            }*/
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
        Cast cust = new Cast();
        foreach (Dropdown dropdown in castEditor.transform.GetComponentsInChildren<Dropdown>())
        {
            cust.cast.Add(allContestants[dropdown.value]);
        }
        if (cust.cast.Count != cust.cast.Distinct().Count())
        {
            return;
        }
        bool custom = false;
        foreach (Option opt in options)
        {
            if (opt.button.GetComponentInChildren<Text>().text.Contains("On") || opt.button.GetComponentInChildren<Text>().text.Contains("Preset"))
            {
                if(opt.optionBool == "CT")
                {
                    cineTribal = true;
                }
                else if (opt.optionBool == "absorb")
                {
                    absorb = true;
                }
                else if (opt.optionBool == "Custom")
                {
                    
                    custom = true;
                    customSeason.Tribes = new List<Team>();
                    foreach (Transform child in tribeSizeParent.transform)
                    {
                        int num = int.Parse(child.GetChild(3).GetComponent<InputField>().text);

                        Team tribe = new Team() { name = child.GetChild(1).GetComponent<InputField>().text, members = new List<Contestant>(new Contestant[int.Parse(child.GetChild(3).GetComponent<InputField>().text)]), tribeColor = child.GetChild(4).GetComponent<FlexibleColorPicker>().color, environment = (Environment)Enviro.value + 1 };
                        //Debug.Log("Tribe:" + tribe.name + ColorUtility.ToHtmlStringRGBA(tribe.tribeColor));
                        customSeason.Tribes.Add(tribe);
                    }
                    CreateSwaps();

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
                    casts[season] = cust;
                    
                }
                else if(opt.optionBool == "Random")
                {
                    randomStat = true;
                }
                else if(opt.optionBool == "Idols")
                {
                    if(custom == true)
                    {
                        foreach (Team tribe in customSeason.Tribes)
                        {
                            tribe.hiddenAdvantages = new List<HiddenAdvantage>() { new HiddenAdvantage {name=tribe.name + " Hidden Immunity Idol", advantage=idol, hideAt=0, reHidden=true, hidden=true } };
                        }
                        //List<Team> lastTribes = customSeason.Tribes;
                        //customSeason.swaps = customSeason.swaps.OrderByDescending(x => x.swapAt).ToList();
                        foreach (TribeSwap swap in customSeason.swaps)
                        {
                            if(swap.ResizeTribes)
                            {
                                foreach(Team tribe in swap.newTribes)
                                {
                                    tribe.hiddenAdvantages = new List<HiddenAdvantage>() { new HiddenAdvantage { name = tribe.name + " Hidden Immunity Idol", advantage = idol, hideAt = 0, reHidden = true, hidden = true } };
                                }
                            }
                        }
                    }
                }

                

            } else
            {
                
            }
        }
        curSeason = seasons[season];
        curCast = cust;
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
            {
                editorParent.sizeDelta = new Vector2(editorParent.sizeDelta.x, newY + 10);
            }
            StartCoroutine(ABC());

            
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

    public void AddSwap()
    {
        
        if (SwapsMenu.Instance.CurSwap != null)
        {
            string type = SwapsMenu.Instance.swapType.options[SwapsMenu.Instance.swapType.value].text;

            if (SwapsMenu.Instance.types.Contains(type))
            {
                float sum = 0;
                foreach (Transform childd in SwapsMenu.Instance.CurSwap.transform.GetChild(4).GetChild(1))
                {
                    int num = int.Parse(childd.GetChild(3).GetComponent<InputField>().text);
                    sum += num;
                }
                int con = int.Parse(SwapsMenu.Instance.swapAt.options[SwapsMenu.Instance.swapAt.value].text);
                if (con != sum)
                {
                    return;
                }
            }

            premergeRounds.Remove(SwapsMenu.Instance.swapAt.options[SwapsMenu.Instance.swapAt.value]);
            SwapsMenu.Instance.swapAt.interactable = false;
            SwapsMenu.Instance.swapType.interactable = false;

            foreach(InputField inputField in SwapsMenu.Instance.CurSwap.transform.GetChild(4).GetComponentsInChildren<InputField>())
            {
                if(inputField.transform.childCount > 0)
                {
                    foreach (InputField inputFieldd in inputField.GetComponentsInChildren<InputField>())
                    {
                        inputFieldd.interactable = false;
                    }
                }
                inputField.interactable = false;
            }
            if (premergeRounds.Count < 2)
            {
                editorOptions[7].GetComponent<Button>().interactable = false;
            }
            //SwapsMenu.Instance.CurSwap.transform.GetChild(4).GetComponentsInChildren<InputField>().All(x => x.interactable = false);
        }
        

        GameObject swap = Instantiate(swapPrefab, swapParent.transform);
        swap.transform.GetChild(0).GetComponent<Dropdown>().options.Clear();
        for (int i = (int)customSeason.mergeAt + 1; i < contestants; i++)
        {
            swap.transform.GetChild(0).GetComponent<Dropdown>().options.Add(new Dropdown.OptionData {text=i.ToString()});
            //Debug.Log(i);
        }
        swap.transform.GetChild(0).GetComponent<Dropdown>().options = new List<Dropdown.OptionData>(premergeRounds);
        swap.transform.GetChild(0).GetComponent<Dropdown>().onValueChanged.AddListener(delegate { SwapsMenu.Instance.SubmitSwapAt(swap.transform.GetChild(0).GetComponent<Dropdown>()); });
        swap.transform.GetChild(1).GetComponent<Dropdown>().onValueChanged.AddListener(delegate { SubmitSwapType(swap.transform.GetChild(1).GetComponent<Dropdown>()); });

        List<Dropdown.OptionData> swapOpt = new List<Dropdown.OptionData>(swapOptions);

        List<Team> t = SwapsMenu.Instance.GetTribesAt(swap.transform.GetChild(0).GetComponent<Dropdown>(), swap.transform.GetChild(1).GetComponent<Dropdown>());

        if (t.Count < 3)
        {
            swapOpt.RemoveAt(3);
            swapOpt.RemoveAt(3);
        } else
        {
            swapOpt.RemoveAt(6);
        }

        

        /*if (t.Count > 2)
        {
            swapOpt.RemoveAt(6);
        }*/
        ogSwapY = editorParent.sizeDelta.y;
        swap.transform.GetChild(1).GetComponent<Dropdown>().options = swapOpt;
        SwapsMenu.Instance.CurSwap = swap;
        SwapsMenu.Instance.swapAt = swap.transform.GetChild(0).GetComponent<Dropdown>();
        SwapsMenu.Instance.swapType = swap.transform.GetChild(1).GetComponent<Dropdown>();
        SwapsMenu.Instance.parent = swap.GetComponent<LayoutElement>();
        SwapsMenu.Instance.SubmitSwapAt(swap.transform.GetChild(0).GetComponent<Dropdown>());

        

        StartCoroutine(ABC());
    }

    public void SubmitSwapType(Dropdown change)
    {
        RectTransform parent = change.transform.parent.GetComponent<RectTransform>();

        Dropdown swapAt = parent.transform.GetChild(0).GetComponent<Dropdown>();

        string type = change.options[change.value].text;

        parent.GetComponent<LayoutElement>().preferredHeight = 100;
        editorParent.sizeDelta = new Vector2(editorParent.sizeDelta.x, ogSwapY + 100);
        foreach (Transform child in parent.transform.GetChild(4))
        {
            Destroy(child.gameObject);
        }
        if(!noPre.Contains(type))
        {
            GameObject s = Instantiate(swapPrefabs[change.value], parent.transform.GetChild(4));

            switch (change.captionText.text)
            {
                case "Swap":
                    s.transform.GetChild(1).GetComponent<InputField>().onEndEdit.AddListener(SwapsMenu.Instance.SubmitSwapContestants);
                    s.transform.GetChild(1).GetComponent<InputField>().text = "1";
                   parent.GetComponent<LayoutElement>().preferredHeight += 50;
                    StartCoroutine(ABC());
                    break;
                case "Schoolyard Pick":
                    SwapsMenu.Instance.tribeSizeParent = Instantiate(tribeSizePRE, parent.transform.GetChild(4));
                    parent.GetComponent<LayoutElement>().preferredHeight += 75;
                    List<Team> t = SwapsMenu.Instance.GetTribesAt(swapAt, change);

                    SwapsMenu.Instance.SubmitSwapTA(t.Count.ToString());
                    s.transform.GetChild(0).GetComponent<InputField>().interactable = false;
                    break;
                case "Shuffle":
                    s.transform.GetChild(0).GetComponent<InputField>().onEndEdit.AddListener(SwapsMenu.Instance.SubmitSwapTA);
                    SwapsMenu.Instance.tribeSizeParent = Instantiate(tribeSizePRE, parent.transform.GetChild(4));
                    parent.GetComponent<LayoutElement>().preferredHeight += 75;
                    StartCoroutine(ABC());
                    break;
                case "Dissolve Least Members":
                case "Challenge Dissolve":
                    SwapsMenu.Instance.tribeSizeParent = Instantiate(tribeSizePRE, parent.transform.GetChild(4));
                    parent.GetComponent<LayoutElement>().preferredHeight += 75;
                    List<Team> tt = SwapsMenu.Instance.GetTribesAt(swapAt, change);

                    SwapsMenu.Instance.SubmitSwapTA((tt.Count -1).ToString());
                    s.transform.GetChild(0).GetComponent<InputField>().interactable = false;
                    break;
            }
            
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

    public void CastEditor()
    {
        editorTrueParent.SetActive(false);
        options[2].button.SetActive(false);
        backButton.SetActive(true);
        castEditor.transform.parent.gameObject.SetActive(true);
        for(int i = 0; i < contestants; i++)
        {
            GameObject obj = Instantiate(contestantPrefab, castEditor.transform);
            obj.GetComponentInChildren<Dropdown>().options = everyContestant;
            obj.GetComponentInChildren<Dropdown>().value = i;
        } 
    }

    public void RandomizeCast()
    {
        List<Dropdown.OptionData> clone = new List<Dropdown.OptionData>(everyContestant);
        for (int i = clone.Count - 1; i > 0; i--)
        {
            int swapIndex = Random.Range(0, i + 1);
            Dropdown.OptionData currentCon = clone[i];
            Dropdown.OptionData conToSwap = clone[swapIndex];
            clone[i] = conToSwap;
            clone[swapIndex] = currentCon;
        }
        int g = 0;
        foreach (Dropdown dropdown in castEditor.transform.GetComponentsInChildren<Dropdown>())
        {
            dropdown.value = everyContestant.IndexOf(clone[g]);
            g++;
        }
    }

    void CreateSwaps()
    {
        customSeason.swaps = new List<TribeSwap>();
        foreach (Transform child in swapParent.transform)
        {

            string type = child.GetChild(1).GetComponent<Dropdown>().options[child.GetChild(1).GetComponent<Dropdown>().value].text;

            TribeSwap swap = new TribeSwap();

            swap.swapAt = int.Parse(child.GetChild(0).GetComponent<Dropdown>().options[child.GetChild(0).GetComponent<Dropdown>().value].text);
            swap.newTribes = new List<Team>();
            if (SwapsMenu.Instance.types.Contains(type))
            {
                foreach (Transform childd in child.GetChild(4).GetChild(1))
                {
                    int num = int.Parse(childd.GetChild(3).GetComponent<InputField>().text);

                    Team tribe = new Team() { name = childd.GetChild(1).GetComponent<InputField>().text, members = new List<Contestant>(new Contestant[int.Parse(childd.GetChild(3).GetComponent<InputField>().text)]), tribeColor = childd.GetChild(4).GetComponent<FlexibleColorPicker>().color, environment=(Environment)Enviro.value };
                    //Debug.Log("Tribe:" + tribe.name + ColorUtility.ToHtmlStringRGBA(tribe.tribeColor));
                    swap.newTribes.Add(tribe);
                }
            }
            switch (type)
            {
                case "Swap":
                    swap.type = SwapType.RegularSwap;
                    swap.numberSwap = int.Parse(child.GetChild(4).GetChild(0).GetChild(1).GetComponent<InputField>().text);
                    break;
                case "Mutiny":
                    swap.type = SwapType.Mutiny;
                    break;
                case "Schoolyard Pick":
                    swap.type = SwapType.SchoolyardPick;
                    swap.pickingRules = "altTribes";
                    break;
                case "Shuffle":
                    swap.type = SwapType.RegularShuffle;
                    swap.ResizeTribes = true;
                    break;
                case "Shuffle(Same Tribe Size)":
                    swap.type = SwapType.RegularShuffle;
                    break;
                case "Split Tribes (Guatemala)":
                    swap.type = SwapType.SplitTribes;
                    break;
                case "Dissolve Least Members":
                    swap.type = SwapType.DissolveLeastMembers;
                    break;
                case "Challenge Dissolve":
                    swap.type = SwapType.ChallengeDissolve;
                    break;
            }
            customSeason.swaps.Add(swap);
        }
    }

    public void PresetCastEdit(int season)
    {
        editorTrueParent.SetActive(false);
        buttonParent.SetActive(false);
        options[2].button.SetActive(false);
        backButton.SetActive(true);
        castEditor.transform.parent.gameObject.SetActive(true);
        for (int i = 0; i < casts[season].cast.Count; i++)
        {
            GameObject obj = Instantiate(contestantPrefab, castEditor.transform);
            obj.GetComponentInChildren<Dropdown>().options = everyContestant;
            obj.GetComponentInChildren<Dropdown>().value = allContestants.IndexOf(casts[season].cast[i]);
        }
        playSimButton.onClick.AddListener(() => StartSeason(season));
    }

    public void BackCast()
    {
        editorTrueParent.SetActive(false);
        buttonParent.SetActive(true);
        options[2].button.SetActive(true);
        backButton.SetActive(false);
        castEditor.transform.parent.gameObject.SetActive(false);
        foreach (Transform child in castEditor.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Confirm1()
    {
        if(contestants > 8)
        {
            confirms[0].interactable = false;
            //editorOptions[0].GetComponent<InputField>().interactable = false;
            editorOptions[1].GetComponent<InputField>().interactable = false;
            editorOptions[2].GetComponent<Dropdown>().interactable = false;
            editorOptions[3].GetComponent<Dropdown>().interactable = false;
            foreach (InputField input in tribeSizeParent.GetComponentsInChildren<InputField>())
            {
                input.interactable = false;
            }
            editorOptions[4].GetComponent<InputField>().interactable = true;
            editorOptions[5].GetComponent<InputField>().interactable = true;
            confirms[1].interactable = true;
            foreach (Transform child in tribeSizeParent.transform)
            {
                int num = int.Parse(child.GetChild(3).GetComponent<InputField>().text);

                Team tribe = new Team() {members = new List<Contestant>(new Contestant[int.Parse(child.GetChild(3).GetComponent<InputField>().text)])};
                //Debug.Log("Tribe:" + tribe.name + ColorUtility.ToHtmlStringRGBA(tribe.tribeColor));
                customSeason.Tribes.Add(tribe);
            }
            
        }
        
    }
    public void Confirm2()
    {
        if (customSeason.mergeAt != 0 && customSeason.jury != 0)
        {
            confirms[1].interactable = false;
            editorOptions[4].GetComponent<InputField>().interactable = false;
            editorOptions[5].GetComponent<InputField>().interactable = false;
            editorOptions[7].GetComponent<Button>().interactable = true;
            for (int i = (int)customSeason.mergeAt + 1; i < contestants; i++)
            {
                premergeRounds.Add(new Dropdown.OptionData { text = i.ToString() });
                //Debug.Log(i);
            }
            if(premergeRounds.Count < 1)
            {
                editorOptions[7].GetComponent<Button>().interactable = false;
            }
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
        editorParent.gameObject.SetActive(!editorParent.gameObject.activeSelf);
    }

    public static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return a;

    }
}
