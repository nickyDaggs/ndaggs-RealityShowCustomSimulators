using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using SeasonParts;
using System.Linq;
using UnityEngine.Networking;

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

    public GameObject pairsButton;

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
    public bool pairs = false;

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

    public SpecialEventMenu spEvMenu;
    public ExileMenu ExileMenu;
    public statEditorMenu statMenu;
    public ChallengeEditorMenu challengeMenu;
    public List<GameObject> editorOptions;
    public List<GameObject> twistOptions;
    public List<GameObject> UIParts;
    public List<GameObject> swapUIOptions;
    public List<GameObject> exileUIOptions;
    public List<GameObject> spEvUIOptions;
    public List<GameObject> hidAdvUIOptions;
    public List<GameObject> GIUIOptions;
    public List<GameObject> JourneyUIOptions;
    public List<GameObject> ChallengeUIOptions;
    public GameObject tribeSizePRE;
    public GameObject tribeSizer;
    public GameObject tribeSizeParent;
    public GameObject swapPrefab;
    public GameObject swapParent;
    public Button simButton;
    public Button playSimButton;
    public Button playSimButtonStats;
    public RectTransform editorParent;
    public GameObject editorTrueParent;
    public GameObject castEditor;
    public GameObject contestantPrefab;
    public Button sizeButton;
    public Text editorError;
    public Button menuBackButton;
    public GameObject castBackCustom;
    public InputField sizeToSet;

    public GameObject statPrefab;

    public Transform PMAdvParent;
    public Transform MAdvParent;
    List<HiddenAdvantage> pmAdvantages = new List<HiddenAdvantage>();
    GameObject curPMAdv;
    GameObject curMAdv;
    public GameObject AdvantagePrefab;

    float ogY;
    float tribes;
    public int contestants;
    public int contestantsFull;
    int conLimit = 33;
    int mergeLimit = 5;
    int minCon = 0;
    bool part1 = false;
    public int customStage = 1;

    public List<Advantage> advantages;

    [HideInInspector] public float ogSwapY;

    [HideInInspector] List<string> noPre = new List<string>() { "Mutiny", "Split Tribes (Guatemala)", "Shuffle(Same Tribe Size)" };

    List<Contestant> allContestants = new List<Contestant>();

    public List<Dropdown.OptionData> everyContestant;
    List<Dropdown.OptionData> usedContestants = new List<Dropdown.OptionData>();

    public Text whatever;

    public string json;
    public Text output;
    public Text titleLoad;

    public SavedWithinSim tempLoader;
    public string jsonTemp;
    public Text outputTemp;
    public Text tempTitleLoad;
    bool loadSavedTemp = false;
    bool preset = false;
    int presetSeason = 120;

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
        confirms[0].onClick.AddListener(Confirm1);
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
            if (seasons[num].seasonImage == null)
            {
                buttonParent.transform.GetChild(i).GetChild(1).GetComponent<Image>().enabled = false;
                buttonParent.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = seasons[num].nameSeason;
            }
            else
            {
                buttonParent.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = seasons[num].seasonImage;
            }
        }
        allContestants = GetAllInstances<Contestant>().ToList();

        everyContestant = allContestants.ConvertAll(x => new Dropdown.OptionData { text = x.name, image = x.image });
        statMenu.StartStats();
        curPMAdv = PMAdvParent.GetChild(0).gameObject;
        curPMAdv.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ConfirmPMAdv);
        curMAdv = MAdvParent.GetChild(0).gameObject;
        curMAdv.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ConfirmMAdv);

        //trashpremergeRounds = new List<Dropdown.OptionData> {new Dropdown.OptionData("22"), new Dropdown.OptionData("21"), new Dropdown.OptionData("20"), new Dropdown.OptionData("19"), new Dropdown.OptionData("18") };
        //spEvMenu.StartSpecial();
    }

    public void Load()
    {
        if (output != null && output.text != "")
        {
            json = output.text;
            //Debug.Log(json);
            SceneManager.LoadScene(1);
        }
    }

    public void LoadSavedTemplate()
    {
        if (outputTemp != null && outputTemp.text != "")
        {
            jsonTemp = outputTemp.text;
            SavedWithinSim.SavedTemplate format = JsonUtility.FromJson<SavedWithinSim.SavedTemplate>(jsonTemp);
            SeasonTemplate loadedTemp = tempLoader.LoadSeason(format);

            curSeason = loadedTemp;
            customSeason = loadedTemp;
            loadSavedTemp = true;

            List<int> cont = new List<int>();
            foreach(Team team in curSeason.Tribes)
            {
                cont.Add(team.members.Count);
            }

            contestants = cont.Sum();

            CastEditor();
            //Debug.Log(json);
            //SceneManager.LoadScene(1);
        }
    }

    private void Update()
    {
        //simButton.interactable = canSim;
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            CheckCustomSeason();

            if (tribeSizeParent.transform.childCount > 1 && tribeSizeParent.activeSelf)
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
        //customSeason = new SeasonTemplate();
        //customSeason.final = editorOptions[2].GetComponent<Dropdown>().value;
        Cast cust = new Cast();
        foreach (Dropdown dropdown in castEditor.transform.GetComponentsInChildren<Dropdown>())
        {
            if(dropdown.transform.parent.GetComponent<customConScript>().custom)
            {
                
                cust.cast.Add(CreateContestant(dropdown.transform.parent.GetComponent<customConScript>()));
                if(cust.cast[cust.cast.Count - 1].fullname == "" || cust.cast[cust.cast.Count - 1].nickname == "")
                {
                    editorError.text = "CONTESTANT NAMES MUST BE FILLED OUT FULLY";
                    return;
                }
            } else
            {
                cust.cast.Add(Instantiate(allContestants[dropdown.value]));
            }
        }
        if (cust.cast.Count != cust.cast.Distinct().Count())
        {
            editorError.text = "NO DUPLICATE CONTESTANTS";
            return;
        }

        if(statMenu.mainStats.activeSelf)
        {
            for(int i = 0; i < cust.cast.Count; i++)
            {
                cust.cast[i].stats.Physical = statMenu.statParent.GetChild(i).GetChild(1).GetChild(0).GetComponent<Dropdown>().value + 1;
                cust.cast[i].stats.Endurance = statMenu.statParent.GetChild(i).GetChild(1).GetChild(1).GetComponent<Dropdown>().value + 1;
                cust.cast[i].stats.Mental = statMenu.statParent.GetChild(i).GetChild(1).GetChild(2).GetComponent<Dropdown>().value + 1;
                cust.cast[i].stats.Stamina = statMenu.statParent.GetChild(i).GetChild(1).GetChild(3).GetComponent<Dropdown>().value + 1;
                cust.cast[i].stats.SocialSkills = statMenu.statParent.GetChild(i).GetChild(1).GetChild(4).GetComponent<Dropdown>().value + 1;
                cust.cast[i].stats.Temperament = statMenu.statParent.GetChild(i).GetChild(1).GetChild(5).GetComponent<Dropdown>().value + 1;
                cust.cast[i].stats.Forgivingness = statMenu.statParent.GetChild(i).GetChild(1).GetChild(6).GetComponent<Dropdown>().value + 1;
                cust.cast[i].stats.Boldness = statMenu.statParent.GetChild(i).GetChild(1).GetChild(7).GetComponent<Dropdown>().value + 1;
                cust.cast[i].stats.Strategic = statMenu.statParent.GetChild(i).GetChild(1).GetChild(8).GetComponent<Dropdown>().value + 1;
                cust.cast[i].stats.Loyalty = statMenu.statParent.GetChild(i).GetChild(1).GetChild(9).GetComponent<Dropdown>().value + 1;
                cust.cast[i].stats.Influence = statMenu.statParent.GetChild(i).GetChild(1).GetChild(10).GetComponent<Dropdown>().value + 1;
                cust.cast[i].stats.Intuition = statMenu.statParent.GetChild(i).GetChild(1).GetChild(11).GetComponent<Dropdown>().value + 1;
            }
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
                    TribeHidAdvs();
                    
                    foreach (Team tribe in customSeason.Tribes)
                    {
                        foreach(HiddenAdvantage hid in pmAdvantages)
                        {
                            tribe.hiddenAdvantages.Add((HiddenAdvantage)hid.Clone());
                        }
                        foreach(HiddenAdvantage hid in tribe.hiddenAdvantages)
                        {
                            hid.name = tribe.name + " " + hid.advantage.nickname;
                        }
                    }

                    CreateChallenges();
                    CreateSwaps();
                    CreateSpEv();

                    customSeason.MergeTribeName = editorOptions[10].transform.GetChild(1).GetComponent<InputField>().text;
                    customSeason.MergeTribeColor = editorOptions[10].transform.GetChild(4).GetComponent<FlexibleColorPicker>().color;
                    foreach(HiddenAdvantage hid in customSeason.mergeHiddenAdvantages)
                    {
                        if (hid.advantage.type == "HiddenImmunityIdol")
                        {
                            hid.name = customSeason.MergeTribeName + " Hidden Immunity Idol";
                        }
                    }
                    foreach (TribeSwap swap in customSeason.swaps)
                    {
                        if (swap.ResizeTribes)
                        {
                            foreach (Team tribe in swap.newTribes)
                            {
                                foreach (HiddenAdvantage hid in pmAdvantages)
                                {
                                    tribe.hiddenAdvantages.Add((HiddenAdvantage)hid.Clone());
                                }
                                foreach (HiddenAdvantage hid in tribe.hiddenAdvantages)
                                {
                                    if (hid.advantage.type == "HiddenImmunityIdol")
                                    {

                                        hid.name = tribe.name + " Hidden Immunity Idol";
                                    }
                                }
                            }

                        }
                    }
                    seasons[season] = customSeason;
                    
                    for (int i = customCast.cast.Count - 1; i > 0; i--)
                    {
                        int rnd = Random.Range(0, i);

                        Contestant temp = customCast.cast[i];

                        customCast.cast[i] = customCast.cast[rnd];
                        customCast.cast[rnd] = temp;
                    }

                    for (int i = 0; i < conLimit - contestants; i++)
                    {
                        customCast.cast.Remove(customCast.cast[customCast.cast.Count - 1]);
                    }
                    casts[season] = cust;
                    
                }
                else if(opt.optionBool == "Random")
                {
                    randomStat = true;
                }
                else if (opt.optionBool == "SpecialIsland")
                {
                    if (custom == true)
                    {
                        customSeason.ExileIslandd = true;
                        if(ExileMenu.islandChoice.value == 0)
                        {
                            CreateExile();
                        } else if(ExileMenu.islandChoice.value == 1)
                        {
                            CreateGhost();
                        } else if (ExileMenu.islandChoice.value == 2)
                        {
                            CreateJourneys();
                        }
                    }
                }
                else if (opt.optionBool == "Fire")
                {
                    if (custom == true)
                    {
                        customSeason.forcedFireMaking = true;
                    }
                }
            } else
            {
                
            }
        }
        if(custom)
        {

            List<string> names = new List<string>();
            foreach(Team tribe in customSeason.Tribes)
            {
                if(!names.Contains(tribe.name))
                {
                    names.Add(tribe.name);
                } else
                {
                    editorError.text = "NO DUPLICATE TRIBE NAMES";
                    return;
                }
            }
            foreach(TribeSwap swap in customSeason.swaps)
            {
                if(swap.newTribes.Count > 0)
                {
                    names = new List<string>();
                    foreach (Team tribe in swap.newTribes)
                    {
                        if (!names.Contains(tribe.name) || tribe.name == "same" || tribe.name == "Same")
                        {
                            names.Add(tribe.name);
                        }
                        else
                        {
                            editorError.text = "NO DUPLICATE SWAP TRIBE NAMES";
                            return;
                        }
                    }
                }
                
            }
        }

        if(loadSavedTemp)
        {
            //curSeason = seasons[season];
        }
        else
        {
            curSeason = seasons[season];
        }

        curCast = cust;
        SceneManager.LoadScene(1);


        Contestant CreateContestant(customConScript contestant)
        {
            Contestant con = new Contestant();
            con.stats = new Stats();
            con.fullname = contestant.customs[0].GetComponent<InputField>().text;
            con.nickname = contestant.customs[1].GetComponent<InputField>().text;
            con.image = contestant.gameObject.GetComponentInChildren<Image>().sprite;
            con.gender = contestant.customs[3].GetComponent<Dropdown>().options[contestant.customs[3].GetComponent<Dropdown>().value].text;
            con.imageUrl = contestant.urlCon;
            //Debug.Log(contestant.customs[0].GetComponent<InputField>().text);
            return con;
            
        }
        
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
            if (num > conLimit)
            {
                num = conLimit;
                editorOptions[1].GetComponent<InputField>().text = conLimit.ToString();
            }
            else if (num < 2)
            {
                num = 2;
                editorOptions[1].GetComponent<InputField>().text = "2";
            }
            float teamSize = Mathf.Round(conLimit / num);

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
                    //editorParent.sizeDelta = new Vector2(editorParent.sizeDelta.x, newY + ySize);
                    //newY = editorParent.sizeDelta.y;
                }
            }

            for (int i = 0; i < tribeSizeParent.transform.childCount; i++)
            {
                GameObject size = tribeSizeParent.transform.GetChild(i).gameObject;
                size.transform.GetChild(1).GetComponent<InputField>().text = "Tribe " + (i + 1);
                if (i < num)
                {
                    size.transform.GetChild(3).GetComponent<InputField>().text = teamSize.ToString();
                    int s = (int)teamSize;
                    if (0 > conLimit - teamSize * (i + 1))
                    {
                        size.transform.GetChild(3).GetComponent<InputField>().text = (conLimit - teamSize * (i + 1)).ToString();
                        s = conLimit - (int)teamSize * (i + 1);
                    }
                    //contestants += s;
                }
                else
                {
                    float ySize = size.GetComponent<RectTransform>().sizeDelta.y;

                    //editorParent.sizeDelta = new Vector2(editorParent.sizeDelta.x, newY - ySize);
                    //newY = editorParent.sizeDelta.y;
                    Destroy(size);
                }

            }
            if (num - tribeSizeParent.transform.childCount > 0)
            {
                //editorParent.sizeDelta = new Vector2(editorParent.sizeDelta.x, newY + 10);
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
                /*List<int> cont = new List<int>();
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
                }*/
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
        //ogSwapY = editorParent.sizeDelta.y;
        //Debug.Log(ogSwapY);
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
                if(SwapsMenu.Instance.exileSwap.gameObject.activeSelf && SwapsMenu.Instance.exileSwap.isOn)
                {
                    con--;
                }
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
                        if (inputFieldd.gameObject.name != "InputFieldTribe")
                        {
                            inputFieldd.interactable = false;
                        }
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
            //swap.transform.GetChild(0).GetComponent<Dropdown>().options.Add(new Dropdown.OptionData {text=i.ToString()});
            //Debug.Log(i);
        }

        swap.transform.GetChild(0).GetComponent<Dropdown>().options = new List<Dropdown.OptionData>(premergeRounds);
        swap.transform.GetChild(0).GetComponent<Dropdown>().onValueChanged.AddListener(delegate { SwapsMenu.Instance.SubmitSwapAt(swap.transform.GetChild(0).GetComponent<Dropdown>()); });
        swap.transform.GetChild(1).GetComponent<Dropdown>().onValueChanged.AddListener(delegate { SubmitSwapType(swap.transform.GetChild(1).GetComponent<Dropdown>()); });

        List<Dropdown.OptionData> swapOpt = new List<Dropdown.OptionData>(swapOptions);

        List<Team> t = SwapsMenu.Instance.GetTribesAt(swap.transform.GetChild(0).GetComponent<Dropdown>());

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
        //editorParent.sizeDelta = new Vector2(editorParent.sizeDelta.x, editorParent.sizeDelta.y + 150);
        //ogSwapY = editorParent.sizeDelta.y;
        swap.transform.GetChild(1).GetComponent<Dropdown>().options = swapOpt;
        SwapsMenu.Instance.CurSwap = swap;
        SwapsMenu.Instance.swapAt = swap.transform.GetChild(0).GetComponent<Dropdown>();
        SwapsMenu.Instance.swapType = swap.transform.GetChild(1).GetComponent<Dropdown>();
        SwapsMenu.Instance.exileSwap = swap.transform.GetChild(5).GetComponent<Toggle>();
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
        //editorParent.sizeDelta = new Vector2(editorParent.sizeDelta.x, editorParent.sizeDelta.y + 100);
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
                    parent.transform.GetChild(5).gameObject.SetActive(false);
                    StartCoroutine(ABC());
                    break;
                case "Schoolyard Pick":
                    SwapsMenu.Instance.tribeSizeParent = Instantiate(tribeSizePRE, parent.transform.GetChild(4));
                    parent.GetComponent<LayoutElement>().preferredHeight += 75;
                    List<Team> t = SwapsMenu.Instance.GetTribesAt(swapAt);
                    parent.transform.GetChild(5).gameObject.SetActive(true);
                    SwapsMenu.Instance.SubmitSwapTA(t.Count.ToString());
                    s.transform.GetChild(0).GetComponent<InputField>().interactable = false;
                    break;
                case "Shuffle":
                    s.transform.GetChild(0).GetComponent<InputField>().onEndEdit.AddListener(SwapsMenu.Instance.SubmitSwapTA);
                    parent.transform.GetChild(5).gameObject.SetActive(true);
                    SwapsMenu.Instance.tribeSizeParent = Instantiate(tribeSizePRE, parent.transform.GetChild(4));
                    parent.GetComponent<LayoutElement>().preferredHeight += 75;
                    StartCoroutine(ABC());
                    break;
                case "Dissolve Least Members":
                case "Challenge Dissolve":
                    SwapsMenu.Instance.tribeSizeParent = Instantiate(tribeSizePRE, parent.transform.GetChild(4));
                    parent.GetComponent<LayoutElement>().preferredHeight += 75;
                    List<Team> tt = SwapsMenu.Instance.GetTribesAt(swapAt);
                    parent.transform.GetChild(5).gameObject.SetActive(false);
                    SwapsMenu.Instance.SubmitSwapTA((tt.Count -1).ToString());
                    s.transform.GetChild(0).GetComponent<InputField>().interactable = false;
                    break;
                default:
                    parent.transform.GetChild(5).gameObject.SetActive(false);
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
                //customSeason.mergeAt = 0;
            }
        }
        int n = 1;
        if(minCon > 1)
        {
            n = 2;
        }
        if (customSeason.nameSeason != null && customSeason.nameSeason != "" && customSeason.jury > 0 && customSeason.mergeAt > 0 && customSeason.mergeAt <= contestants - n && tribes > 1 && contestants >= 7 + minCon && contestants < conLimit+1 )
        {
            simButton.interactable = true;
        } else
        {
            simButton.interactable = false;
        }

    }

    public void CastEditor()
    {
        presetSeason = 120;
        editorTrueParent.SetActive(false);
        buttonParent.SetActive(false);
        options[2].button.SetActive(false);
        backButton.SetActive(true);
        //Debug.Log(castEditor.transform.parent.parent.gameObject.name);
        castEditor.transform.parent.parent.gameObject.SetActive(true);
        castBackCustom.SetActive(false);
        if(customSeason.Tribes.Count != 2)
        {
            pairsButton.SetActive(false);
        }
        else
        {
            pairsButton.SetActive(true);
        }
        int curTribe = 0;
        int curCont = 0;
        for (int i = 0; i < contestants; i++)
        {
            GameObject obj = Instantiate(contestantPrefab, castEditor.transform);
            //Debug.Log(obj.name);

            obj.transform.GetChild(7).gameObject.name = "Upload Button " + i;
            obj.GetComponentInChildren<Dropdown>().options = everyContestant;
            obj.GetComponentInChildren<Dropdown>().value = i;
            obj.transform.GetChild(3).GetComponent<Image>().color = customSeason.Tribes[curTribe].tribeColor;
            curCont++;
            if(curCont >= customSeason.Tribes[curTribe].members.Count)
            {
                curTribe++;
                curCont = 0;
            }
        } 
    }

    public void SavedCastLoad(string castToLoad)
    {
        SaveCastFile.SavedCast savedCast = JsonUtility.FromJson<SaveCastFile.SavedCast>(castToLoad);
        List<SimLoader.LoadContestant> loadContestants = new List<SimLoader.LoadContestant>();
        foreach(SimLoader.SavedContestant con in savedCast.cons)
        {
            loadContestants.Add(new SimLoader.LoadContestant(con));
        }

        int fullCount;

        if(castEditor.transform.childCount > loadContestants.Count)
        {
            fullCount = loadContestants.Count;
        } else
        {
            fullCount = castEditor.transform.childCount;
        }

        for(int i = 0; i < fullCount; i++)
        {
            castEditor.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = loadContestants[i].sprite;
            castEditor.transform.GetChild(i).GetChild(4).GetComponent<InputField>().text = loadContestants[i].fullname;
            castEditor.transform.GetChild(i).GetChild(5).GetComponent<InputField>().text = loadContestants[i].nickname;
            castEditor.transform.GetChild(i).GetChild(6).GetComponent<InputField>().text = loadContestants[i].spriteUrl;
            if(loadContestants[i].spriteUrl != "")
            {
                castEditor.transform.GetChild(i).GetComponent<customConScript>().CustomImage(castEditor.transform.GetChild(i).GetChild(6).GetComponent<InputField>());

            }
            if(loadContestants[i].gender == "M")
            {
                castEditor.transform.GetChild(i).GetChild(8).GetComponent<Dropdown>().value = 0;

            } else
            {
                castEditor.transform.GetChild(i).GetChild(8).GetComponent<Dropdown>().value = 1;
            }

            MakeAllCustom();
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
            if(child.GetChild(5).gameObject.activeSelf && child.GetChild(5).GetComponent<Toggle>().isOn)
            {
                swap.exile = true;
                swap.exileIsland = new Exile();
                swap.exileIsland.on = true;
                swap.exileIsland.skipTribal = true;
                swap.exileIsland.exileEvent = "Nothing";
                swap.exileIsland.challenge = "Immunity";
            }
            customSeason.swaps.Add(swap);
        }
    }

    void CreateSpEv()
    {
        customSeason.oneTimeEvents = new List<OneTimeEvent>();
        List<Transform> OTEs = new List<Transform>(spEvMenu.eventsParent.GetComponentsInChildren<Transform>());
        //OTEs = OTEs.OrderBy(x => x.GetChild(0).GetComponentInChildren<Dropdown>());
        foreach (Transform child in spEvMenu.eventsParent)
        {
            if (child.GetChild(2).GetComponent<Button>().interactable == false)
            {
                Dropdown Round = child.GetChild(0).GetComponentInChildren<Dropdown>();
                Dropdown Events = child.GetChild(1).GetComponentInChildren<Dropdown>();
                int num = int.Parse(Round.options[Round.value].text);

                OneTimeEvent ev = new OneTimeEvent();
                switch (Events.options[Events.value].text)
                {
                    case "Multi-Tribal":
                        ev.type = "MultiTribalEveryTeamImm";
                        break;
                    case "Multi-Tribal(Win Reward for immunity)":
                        ev.type = "MultiTribalOneImm";
                        break;
                    case "Multi-Tribal(One Tribe Immunity)":
                        ev.type = "MultiTribalMultiTeam";
                        //ev.elim = 1;
                        ev.elim = SwapsMenu.Instance.GetTribesAt(child.GetChild(0).GetComponentInChildren<Dropdown>()).Count - 1;

                        break;
                    case "Merge Split":
                        ev.elim = 2;
                        ev.type = "MergeSplit";
                        break;
                    case "Fake Merge":
                        ev.type = "FakeMerge";
                        break;
                    case "Multi-Tribal(Reward Only)":
                        ev.type = "MultiTribalReward";
                        break;
                    case "Do Or Die":
                        ev.type = "DoOrDie";
                        break;
                    case "Joint Tribal":
                        ev.type = "JointTribal";
                        ev.elim = 1;//SwapsMenu.Instance.GetTribesAt(child.GetChild(0).GetComponentInChildren<Dropdown>()).Count - 1;
                        break;
                }
                if(ev.type != "MultiTribalMultiTeam" && ev.type.Contains("MultiTribal"))
                {
                    ev.elim = SwapsMenu.Instance.GetTribesAt(child.GetChild(0).GetComponentInChildren<Dropdown>()).Count;
                }
                else
                {
                    if(ev.type == "MultiTribalMultiTeam")
                    {
                        ev.elim = SwapsMenu.Instance.GetTribesAt(child.GetChild(0).GetComponentInChildren<Dropdown>()).Count - 1;
                    }
                }
                //Debug.Log("Round:" + ev.round);
                ev.round = int.Parse(child.GetChild(0).GetComponentInChildren<Dropdown>().options[child.GetChild(0).GetComponentInChildren<Dropdown>().value].text);
                
                //customSeason.skip
                customSeason.oneTimeEvents.Add(ev);
            }
        }
    }

    void CreateExile()
    {
        customSeason.ExileIslandd = true;
        customSeason.Twists = new Twist();
        customSeason.Twists.preMergeEIsland = new Exile();
        customSeason.Twists.preMergeEIsland = ExileMenu.GetPMExile();
        customSeason.Twists.MergeEIsland = new Exile();
        customSeason.Twists.MergeEIsland = ExileMenu.GetMExile();
        customSeason.IslandType = "Exile";
        customSeason.Twists.expireAt = spEvMenu.RoundsClone.Count + 2;
        //Debug.Log(customSeason.Twists.expireAt);
        if (ExileMenu.endAt.options[ExileMenu.endAt.value].text == "Merge")
        {
            customSeason.Twists.expires = "Merge";
            //customSeason.Twists.expireAt = spEvMenu.RoundsClone.IndexOf(spEvMenu.RoundsClone.Find(x => x.text == (customSeason.mergeAt).ToString())) + 2;
            //Debug.Log(customSeason.Twists.expireAt);
        } else
        {
            //customSeason.Twists.expireAt = spEvMenu.RoundsClone.Count + 2;
        }
        customSeason.islandHiddenAdvantages = new List<HiddenAdvantage>();
        foreach (Transform child in ExileMenu.AdvantageParent)
        {
            if(child.GetChild(1).GetComponent<Button>().interactable == false)
            {
                customSeason.islandHiddenAdvantages.Add(GenerateAdv(child));
                //Debug.Log("w");
            }
        }
        
        //Debug.Log(customSeason.Twists.preMergeEIsland.on);
        //Debug.Log(customSeason.Twists.expireAt);
    }

    void CreateGhost()
    {
        customSeason.ExileIslandd = true;
        customSeason.Twists = new Twist();
        customSeason.Twists.preMergeEIsland = new Exile();
        customSeason.Twists.preMergeEIsland = ExileMenu.GetPMGhost();
        customSeason.Twists.MergeEIsland = new Exile();
        customSeason.Twists.MergeEIsland = ExileMenu.GetMGhost();
        customSeason.IslandType = "Ghost";
        customSeason.Twists.expireAt = spEvMenu.RoundsClone.Count + 2;
        //Debug.Log(customSeason.Twists.expireAt);
        if (ExileMenu.endAtGI.options[ExileMenu.endAtGI.value].text == "Merge")
        {
            customSeason.Twists.expires = "Merge";
            //customSeason.Twists.expireAt = spEvMenu.RoundsClone.IndexOf(spEvMenu.RoundsClone.Find(x => x.text == (customSeason.mergeAt).ToString())) + 2;
            //Debug.Log(customSeason.Twists.expireAt);
        }
        else
        {
            //customSeason.Twists.expireAt = spEvMenu.RoundsClone.Count + 2;
        }
        customSeason.islandHiddenAdvantages = new List<HiddenAdvantage>();
        foreach (Transform child in ExileMenu.GIAdvantageParent)
        {
            if (child.GetChild(1).GetComponent<Button>().interactable == false)
            {
                customSeason.islandHiddenAdvantages.Add(GenerateAdvGI(child, false));
                //Debug.Log("w");
            }
        }
    }

    void CreateJourneys()
    {
        customSeason.ExileIslandd = true;
        customSeason.Twists = new Twist();
        
        foreach (Transform child in ExileMenu.SpecialJourneyParent)
        {
            customSeason.Twists.epsSpecialE.Add(int.Parse(child.GetComponentInChildren<InputField>().text));
            Exile spec = new Exile();
            spec = ExileMenu.GetJourney(child);
            customSeason.Twists.SpecialEx.Add(spec);
            if (child.GetChild(1).GetComponent<Button>().interactable == false)
            {
                customSeason.islandHiddenAdvantages.Add(GenerateAdvGI(child, true));
                //Debug.Log("w");
            }
        }
        customSeason.Twists.preMergeEIsland = new Exile();
        customSeason.Twists.preMergeEIsland = ExileMenu.RegJourney(ExileMenu.RegularJourney.transform);
        customSeason.Twists.expires = "Merge";
    }

    void CreateChallenges()
    {
        foreach(Transform obj in challengeMenu.immunityParent)
        {
            if(!obj.GetChild(1).GetComponent<Button>().interactable)
            {
                List<string> statNames = new List<string>();

                Challenge newChall = new Challenge();
                newChall.challengeName = "Custom Challenge";
                for (int i = 0; i < obj.GetChild(4).childCount; i++)
                {
                    if (obj.GetChild(4).GetChild(i).GetComponent<Toggle>().isOn)
                    {
                        newChall.stats.Add((StatChoice)i);

                        statNames.Add(System.Enum.GetName(typeof(StatChoice), i));
                    }
                }

                newChall.description = "This challenge tests these stats: " + string.Join(",", statNames) + ".";
                customSeason.ImmunityChallenges.Add(newChall);
            }
            
        }

        foreach (Transform obj in challengeMenu.rewardParent)
        {
            if (!obj.GetChild(1).GetComponent<Button>().interactable)
            {
                List<string> statNames = new List<string>();

                Challenge newChall = new Challenge();
                newChall.challengeName = "Custom Challenge";
                for (int i = 0; i < obj.GetChild(4).childCount; i++)
                {
                    if (obj.GetChild(4).GetChild(i).GetComponent<Toggle>().isOn)
                    {
                        newChall.stats.Add((StatChoice)i);

                        statNames.Add(System.Enum.GetName(typeof(StatChoice), i));
                    }
                }

                newChall.description = "This challenge tests these stats: " + string.Join(", ", statNames) + ".";
                customSeason.RewardChallenges.Add(newChall);
            }
        }
    }

    void TribeHidAdvs()
    {
        foreach (Transform child in PMAdvParent)
        {
            if (child.GetChild(1).GetComponent<Button>().interactable == false)
            {
                pmAdvantages.Add(GenerateAdv(child));
                //Debug.Log("w");
            }
        }
        customSeason.mergeHiddenAdvantages = new List<HiddenAdvantage>();
        foreach (Transform child in PMAdvParent)
        {
            if (child.GetChild(1).GetComponent<Button>().interactable == false)
            {
                
                customSeason.mergeHiddenAdvantages.Add(GenerateAdv(child));
                //Debug.Log("w");
            }
        }
    }

    HiddenAdvantage GenerateAdv(Transform prefab)
    {
        HiddenAdvantage adv = new HiddenAdvantage();
        adv.advantage = advantages[prefab.GetChild(0).GetComponentInChildren<Dropdown>().value];
        adv.hidden = true;
        if (prefab.GetChild(2).GetComponentInChildren<Dropdown>().options[prefab.GetChild(2).GetComponentInChildren<Dropdown>().value].text == "Yes")
        {
            adv.reHidden = true;
            //Debug.Log("real");
        } else
        {
            adv.reHidden = false;
        }
        adv.name = adv.advantage.nickname;
        if(adv.advantage.type == "HiddenImmunityIdol")
        {
            customSeason.idolLimit++;
        }
        return adv;
    }

    HiddenAdvantage GenerateAdvGI(Transform prefab, bool journey)
    {
        HiddenAdvantage adv = new HiddenAdvantage();
        adv.advantage = advantages[prefab.GetChild(0).GetComponentInChildren<Dropdown>().value];
        adv.hidden = true;
        adv.reHidden = false;
        /*if (prefab.GetChild(2).GetComponentInChildren<Dropdown>().options[prefab.GetChild(2).GetComponentInChildren<Dropdown>().value].text == "Yes")
        {
            adv.reHidden = true;
            //Debug.Log("real");
        }
        else
        {
            adv.reHidden = false;
        }*/
        adv.hideAt = int.Parse(prefab.GetComponentInChildren<InputField>().text);
        adv.name = adv.advantage.nickname;

        if (adv.advantage.type == "HiddenImmunityIdol")
        {
            customSeason.idolLimit++;
        }

        if(journey)
        {
            switch (prefab.GetChild(5).GetComponentInChildren<Dropdown>().value)
            {
                case 0:
                    adv.IOILesson = "Tribal";
                    break;
                case 1:
                    adv.IOILesson = "Camp";
                    break;
            }
        }

        return adv;
    }

    public void MakeAllCustom()
    {
        foreach (customConScript con in castEditor.transform.GetComponentsInChildren<customConScript>())
        {
            foreach (GameObject g in con.presets)
            {
                g.SetActive(false);
            }
            foreach (GameObject g in con.customs)
            {
                g.SetActive(true);
            }
            con.custom = true;
        }
    }

    public void EnablePairs()
    {
        pairs = !pairs;
        int curTribe = 0;
        int curCont = 1;
        customConScript[] conCustomList = castEditor.transform.GetComponentsInChildren<customConScript>();
        SeasonTemplate gamer;
        if(presetSeason != 120)
        {
            gamer = seasons[presetSeason];
        }  else
        {
            gamer = customSeason;
        }
        for (int i = 0; i < conCustomList.Length; i++)
        {
            conCustomList[i].pairText.text = "Pair #" + curCont;
            curCont++;
            if (curCont > gamer.Tribes[curTribe].members.Count)
            {
                curTribe++;
                curCont = 1;
            }
        }
        foreach (customConScript con in castEditor.transform.GetComponentsInChildren<customConScript>())
        {
            con.pairText.gameObject.SetActive(!con.pairText.gameObject.activeSelf);
        }

    }

    public void PresetCastEdit(int season)
    {
        editorTrueParent.SetActive(false);
        buttonParent.SetActive(false);
        options[2].button.SetActive(false);
        backButton.SetActive(true);
        castBackCustom.gameObject.SetActive(false);
        if (seasons[season].Tribes.Count != 2 || seasons[season].oneTimeEvents.Find(x => x.type == "FijiStart") != null || seasons[season].oneTimeEvents.Find(x => x.type == "PalauStart") != null)
        {
            pairsButton.SetActive(false);
        } else
        {
            pairsButton.SetActive(true);
        }
        castEditor.transform.parent.parent.gameObject.SetActive(true);
        presetSeason = season;
        int curTribe = 0;
        int curCont = 0;
        bool Colors = true;
        foreach(OneTimeEvent one in seasons[season].oneTimeEvents)
        {
            if(one.type == "PalauStart" || one.type == "FijiStart" || one.type == "SchoolyardPick" )
            {
                Colors = false;
            }
        }
        for (int i = 0; i < casts[season].cast.Count; i++)
        {
            GameObject obj = Instantiate(contestantPrefab, castEditor.transform);
            obj.GetComponentInChildren<Dropdown>().options = everyContestant;
            obj.GetComponentInChildren<Dropdown>().value = allContestants.IndexOf(casts[season].cast[i]);
            obj.transform.GetChild(7).gameObject.name = "Upload Button " + i;
            //Debug.Log(obj.transform.GetChild(0).gameObject.name);
            //seasons[season]
            if (Colors)
            {
                obj.transform.GetChild(3).GetComponent<Image>().color = seasons[season].Tribes[curTribe].tribeColor;
                curCont++;
                if (curCont >= seasons[season].Tribes[curTribe].members.Count)
                {
                    curTribe++;
                    curCont = 0;
                }
            }
            
        }
        playSimButton.onClick.AddListener(() => StartSeason(season));
        playSimButtonStats.onClick.AddListener(() => StartSeason(season));
    }

    public void EnableSwaps()
    {
        foreach(GameObject obj in editorOptions)
        {
            //obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in UIParts)
        {
            //obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in swapUIOptions)
        {
            obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in twistOptions)
        {
            obj.SetActive(!obj.activeSelf);
        }
        swapUIOptions[2].GetComponent<Button>().interactable = !swapUIOptions[2].GetComponent<Button>().interactable;
        swapUIOptions[1].GetComponentInChildren<Button>().interactable = !swapUIOptions[1].GetComponentInChildren<Button>().interactable;
        confirms[0].gameObject.SetActive(!confirms[0].gameObject.activeSelf);
        editorTrueParent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
        menuBackButton.gameObject.SetActive(!menuBackButton.gameObject.activeSelf);
        StartCoroutine(ABC());
    }

    public void EnableExile()
    {
        foreach (GameObject obj in editorOptions)
        {
            //obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in UIParts)
        {
            //obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in exileUIOptions)
        {
            obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in twistOptions)
        {
            obj.SetActive(!obj.activeSelf);
        }
        //swapUIOptions[2].GetComponent<Button>().interactable = !swapUIOptions[2].GetComponent<Button>().interactable;
        //swapUIOptions[1].GetComponentInChildren<Button>().interactable = !swapUIOptions[1].GetComponentInChildren<Button>().interactable;
        menuBackButton.gameObject.SetActive(!menuBackButton.gameObject.activeSelf);
        confirms[0].gameObject.SetActive(!confirms[0].gameObject.activeSelf);

        editorTrueParent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;

        StartCoroutine(ABC());
    }

    public void EnableGhost()
    {
        foreach (GameObject obj in editorOptions)
        {
            //obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in UIParts)
        {
            //obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in GIUIOptions)
        {
            obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in twistOptions)
        {
            obj.SetActive(!obj.activeSelf);
        }
        //swapUIOptions[2].GetComponent<Button>().interactable = !swapUIOptions[2].GetComponent<Button>().interactable;
        //swapUIOptions[1].GetComponentInChildren<Button>().interactable = !swapUIOptions[1].GetComponentInChildren<Button>().interactable;
        menuBackButton.gameObject.SetActive(!menuBackButton.gameObject.activeSelf);
        confirms[0].gameObject.SetActive(!confirms[0].gameObject.activeSelf);

        editorTrueParent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;

        StartCoroutine(ABC());
    }

    public void EnableJourneys()
    {
        foreach (GameObject obj in JourneyUIOptions)
        {
            obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in twistOptions)
        {
            obj.SetActive(!obj.activeSelf);
        }
        menuBackButton.gameObject.SetActive(!menuBackButton.gameObject.activeSelf);
        confirms[0].gameObject.SetActive(!confirms[0].gameObject.activeSelf);

        editorTrueParent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;

        StartCoroutine(ABC());
    }

    public void HiddenAdvantageEnable()
    {
        foreach (GameObject obj in editorOptions)
        {
            //obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in UIParts)
        {
            //obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in hidAdvUIOptions)
        {
            obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in twistOptions)
        {
            obj.SetActive(!obj.activeSelf);
        }
        menuBackButton.gameObject.SetActive(!menuBackButton.gameObject.activeSelf);
        editorTrueParent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
        confirms[0].gameObject.SetActive(!confirms[0].gameObject.activeSelf);
        StartCoroutine(ABC());
    }

    public void EnableSpEvent()
    {
        //confirms[0].gameObject.SetActive(false);
        foreach (GameObject obj in editorOptions)
        {
            //obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in UIParts)
        {
            //obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in spEvUIOptions)
        {
            obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in twistOptions)
        {
            obj.SetActive(!obj.activeSelf);
        }
        menuBackButton.gameObject.SetActive(!menuBackButton.gameObject.activeSelf);
        //swapUIOptions[2].GetComponent<Button>().interactable = !swapUIOptions[2].GetComponent<Button>().interactable;
        //swapUIOptions[1].GetComponentInChildren<Button>().interactable = !swapUIOptions[1].GetComponentInChildren<Button>().interactable;
        confirms[0].gameObject.SetActive(!confirms[0].gameObject.activeSelf);
        editorTrueParent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;

        StartCoroutine(ABC());
    }

    public void EnableStatEditor()
    {
        backButton.SetActive(false);
        //Debug.Log(castEditor.transform.parent.parent.gameObject.name);
        castEditor.transform.parent.parent.gameObject.SetActive(false);
        castBackCustom.SetActive(false);
        foreach (Dropdown dropdown in castEditor.transform.GetComponentsInChildren<Dropdown>())
        {
            GameObject newStat = Instantiate(statPrefab, statMenu.statParent);
            
            if (dropdown.transform.parent.GetComponent<customConScript>().custom)
            {
                newStat.transform.GetChild(0).GetComponent<Text>().text = dropdown.transform.parent.GetComponent<customConScript>().customs[0].GetComponent<InputField>().text;
            }
            else
            {
                newStat.transform.GetChild(0).GetComponent<Text>().text = allContestants[dropdown.value].fullname;//dropdown.transform.parent.GetComponent<customConScript>().customs[0].GetComponent<InputField>().text;
            }
            int ind = statMenu.statParent.childCount - 1;
            newStat.transform.GetChild(1).GetChild(12).GetComponent<Button>().onClick.AddListener(delegate { randomizeContestant(ind); });

        }
        statMenu.mainStats.SetActive(true);
    }

    public void EnableChallengeEditor()
    {
        //confirms[0].gameObject.SetActive(false);
        foreach (GameObject obj in editorOptions)
        {
            //obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in UIParts)
        {
            //obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in ChallengeUIOptions)
        {
            obj.SetActive(!obj.activeSelf);
        }
        foreach (GameObject obj in twistOptions)
        {
            obj.SetActive(!obj.activeSelf);
        }
        menuBackButton.gameObject.SetActive(!menuBackButton.gameObject.activeSelf);
        //swapUIOptions[2].GetComponent<Button>().interactable = !swapUIOptions[2].GetComponent<Button>().interactable;
        //swapUIOptions[1].GetComponentInChildren<Button>().interactable = !swapUIOptions[1].GetComponentInChildren<Button>().interactable;
        confirms[0].gameObject.SetActive(!confirms[0].gameObject.activeSelf);
        editorTrueParent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;

        StartCoroutine(ABC());
    }

    public void randomizeContestant(int index)
    {
        //Debug.Log(index);
        for (int i = 0; i < 12; i++)
        {
            
            statMenu.statParent.GetChild(index).GetChild(1).GetChild(i).GetComponent<Dropdown>().value = Random.Range(statMenu.curMin, statMenu.curMax + 1);
            //Debug.Log(statMenu.statParent.GetChild(index).GetChild(1).GetChild(i).GetComponent<Dropdown>());
            //Debug.Log(statParent.GetChild(index).GetChild(1).GetChild(i).GetComponent<Dropdown>().value);
        }
        //Debug.Log((curMin + 1) + " " + (curMax + 1));
    }

    public void SetAllSizes()
    {
        int setSize = int.Parse(sizeToSet.text);
        for (int i = 0; i < tribeSizeParent.transform.childCount; i++)
        {
            GameObject size = tribeSizeParent.transform.GetChild(i).gameObject;
            //size.transform.GetChild(1).GetComponent<InputField>().text = "Tribe " + (i + 1);
            int s = (int)setSize;
            size.transform.GetChild(3).GetComponent<InputField>().text = setSize.ToString();
            if (0 > conLimit - setSize * (i + 1))
            {
                size.transform.GetChild(3).GetComponent<InputField>().text = (conLimit - setSize * (i + 1)).ToString();
                s = conLimit - (int)setSize * (i + 1);
            }
            
        }
    }

    public void BackCast()
    {
        editorError.text = "";
        editorTrueParent.SetActive(false);
        buttonParent.SetActive(true);
        options[2].button.SetActive(true);
        backButton.SetActive(false);
        castEditor.transform.parent.parent.gameObject.SetActive(false);
        foreach (Transform child in castEditor.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void BackCastCustom()
    {
        editorError.text = "";
        editorTrueParent.SetActive(true);
        buttonParent.SetActive(false);
        options[2].button.SetActive(true);
        backButton.SetActive(false);
        castEditor.transform.parent.parent.gameObject.SetActive(false);
        foreach (Transform child in castEditor.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Confirm1()
    {
        
        if(contestants > 8)
        {
            
            //editorOptions[0].GetComponent<InputField>().interactable = false;
            editorOptions[1].GetComponent<InputField>().interactable = false;
            editorOptions[2].GetComponent<Dropdown>().interactable = false;
            editorOptions[3].GetComponent<Dropdown>().interactable = false;
            sizeButton.interactable = false;
            foreach (InputField input in tribeSizeParent.GetComponentsInChildren<InputField>())
            {
                if(input.gameObject.name != "InputFieldTribe")
                {
                    input.interactable = false;
                }
            }
            
            editorOptions[4].GetComponent<InputField>().interactable = true;
            editorOptions[5].GetComponent<InputField>().interactable = true;
            //confirms[1].interactable = true;
            foreach (Transform child in tribeSizeParent.transform)
            {
                int num = int.Parse(child.GetChild(3).GetComponent<InputField>().text);

                Team tribe = new Team() {members = new List<Contestant>(new Contestant[int.Parse(child.GetChild(3).GetComponent<InputField>().text)]), tribeColor = child.GetChild(4).GetComponent<FlexibleColorPicker>().color };
                //Debug.Log("Tribe:" + tribe.name + ColorUtility.ToHtmlStringRGBA(tribe.tribeColor));
                customSeason.Tribes.Add(tribe);
            }
            contestantsFull = contestants;
            customStage++;
            menuBackButton.interactable = true;
            confirms[0].onClick.RemoveAllListeners();
            confirms[0].onClick.AddListener(Confirm2);
            confirms[0].GetComponentsInChildren<Text>()[1].text = "Click confirm to lock in your current choices and be able to edit/add custom swaps and twists.";
        }
        
    }
    public void Confirm2()
    {
        if (customSeason.mergeAt != 0 && customSeason.jury != 0)
        {
            //confirms[1].interactable = false;
            //confirms[2].interactable = true;
            //editorOptions[1].GetComponent<InputField>().interactable = false;
            //editorOptions[2].GetComponent<Dropdown>().interactable = false;
            //editorOptions[3].GetComponent<Dropdown>().interactable = false;
            //editorOptions[4].GetComponent<InputField>().interactable = false;
            //editorOptions[5].GetComponent<InputField>().interactable = false;
            foreach (GameObject opt in UIParts)
            {
                if (opt != confirms[0].gameObject)
                {
                    opt.SetActive(false);
                }
            }
            twistOptions[0].GetComponent<Button>().interactable = true;
            foreach (GameObject obj in twistOptions)
            {
                obj.SetActive(true);
                
            }
            
            //editorOptions[9].GetComponent<Button>().interactable = true;
            for (int i = (int)customSeason.mergeAt + 1; i < contestantsFull; i++)
            {
                premergeRounds.Add(new Dropdown.OptionData { text = i.ToString() });
                //Debug.Log(i);
            }
            
            if (premergeRounds.Count < 1)
            {
                editorOptions[7].GetComponent<Button>().interactable = false;
            }
            customStage++;
            confirms[0].onClick.RemoveAllListeners();
            confirms[0].onClick.AddListener(Confirm3);
            confirms[0].gameObject.SetActive(true);
            confirms[0].GetComponentsInChildren<Text>()[1].text = "Click confirm to lock in your swaps and be able to edit the special events.\nHit the cast button to finish editing twists and set up your cast for the season.";
            menuBackButton.GetComponentsInChildren<Text>()[1].text = "If you go back, your swaps and custom advantages will be DELETED.";
        }
    }
    public void Confirm3()
    {
        //confirms[2].interactable = false;
        //confirms[3].interactable = true;
        editorOptions[6].GetComponent<Button>().interactable = false;
        editorOptions[7].GetComponent<Button>().interactable = true;
        spEvMenu.mergeRound = contestants - customSeason.mergeAt;
        
        for (int i = contestants; i > 5; i--)
        {
            spEvMenu.Rounds.Add(new Dropdown.OptionData { text = i.ToString() });
            //Debug.Log(i);
        }
        spEvMenu.RoundsClone = new List<Dropdown.OptionData>(spEvMenu.Rounds);
        foreach (Transform child in swapParent.transform)
        {
            int swapAt = int.Parse(child.GetChild(0).GetComponent<Dropdown>().options[child.GetChild(0).GetComponent<Dropdown>().value].text);
            
            List<Team> yo = SwapsMenu.Instance.GetTribesAt(child.GetChild(0).GetComponent<Dropdown>());

            for(int i = yo.Count - 1; i > 0; i--)
            {
                spEvMenu.Rounds.Remove(spEvMenu.Rounds.Find(x => x.text == (swapAt + i).ToString()));
            }
        }
        customStage++;
        spEvMenu.StartSpecial();
        confirms[0].onClick.RemoveAllListeners();
        confirms[0].onClick.AddListener(Confirm4);
        twistOptions[4].GetComponent<Button>().interactable = true;
        confirms[0].GetComponentsInChildren<Text>()[1].text = "Click confirm to lock in your special events and be able to edit the other twists.\nHit the cast button to finish editing twists and set up your cast for the season.";
        menuBackButton.GetComponentsInChildren<Text>()[1].text = "If you go back, your special events will be DELETED.";
    }
    public void Confirm4()
    {
        //confirms[3].interactable = false;
        editorOptions[7].GetComponent<Button>().interactable = false;
        editorOptions[8].GetComponent<Button>().interactable = true;
        editorOptions[9].GetComponent<Button>().interactable = true;
        editorOptions[12].GetComponent<Button>().interactable = true;
        //editorOptions[10].GetComponent<Button>().interactable = true;
        //spEvMenu.mergeRound = contestants - customSeason.mergeAt;
        confirms[0].interactable = false;
        confirms[0].GetComponentsInChildren<Text>()[1].text = "Hit the cast button to finish editing twists and set up your cast for the season.";
        menuBackButton.GetComponentsInChildren<Text>()[1].text = "";
        customStage++;
    }

    public void customBack()
    {
        customStage--;
        switch (customStage + 1)
        {
            case 2:
                customSeason.Tribes = new List<Team>();
                editorOptions[1].GetComponent<InputField>().interactable = true;
                editorOptions[2].GetComponent<Dropdown>().interactable = true;
                editorOptions[3].GetComponent<Dropdown>().interactable = true;
                sizeButton.interactable = true;
                foreach (InputField input in tribeSizeParent.GetComponentsInChildren<InputField>())
                {
                    if (input.gameObject.name != "InputFieldTribe")
                    {
                        input.interactable = true;
                    }
                }

                editorOptions[4].GetComponent<InputField>().interactable = false;
                editorOptions[5].GetComponent<InputField>().interactable = false;
                confirms[0].onClick.RemoveAllListeners();
                confirms[0].onClick.AddListener(Confirm1);
                confirms[0].GetComponentsInChildren<Text>()[1].text = "Click confirm to lock in your current choices and be able to set the merge and jury size.";
                menuBackButton.interactable = false;
                menuBackButton.GetComponentsInChildren<Text>()[1].text = "";
                break;
            case 3:
                premergeRounds = new List<Dropdown.OptionData>();
                foreach (GameObject opt in UIParts)
                {
                    if (opt != confirms[0].gameObject)
                    {
                        opt.SetActive(true);
                    }
                }
                twistOptions[0].GetComponent<Button>().interactable = true;
                foreach (GameObject obj in twistOptions)
                {
                    obj.SetActive(false);

                }
                foreach (Transform child in swapParent.transform)
                {
                    Destroy(child.gameObject);
                }
                foreach (Transform child in spEvMenu.eventsParent.transform)
                {
                    Destroy(child.gameObject);
                }
                foreach (Transform child in MAdvParent)
                {
                    Destroy(child.gameObject);
                }
                curMAdv = Instantiate(AdvantagePrefab, MAdvParent);
                curMAdv.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ConfirmMAdv);
                foreach (Transform child in PMAdvParent)
                {
                    Destroy(child.gameObject);
                }
                curPMAdv = Instantiate(AdvantagePrefab, PMAdvParent);
                curPMAdv.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ConfirmPMAdv);
                editorOptions[4].GetComponent<InputField>().interactable = true;
                editorOptions[5].GetComponent<InputField>().interactable = true;
                confirms[0].onClick.RemoveAllListeners();
                confirms[0].onClick.AddListener(Confirm2);
                confirms[0].GetComponentsInChildren<Text>()[1].text = "Click confirm to lock in your current choices and be able to edit/add custom swaps and twists.";
                
                menuBackButton.GetComponentsInChildren<Text>()[1].text = "";
                break;
            case 4:
                spEvMenu.Rounds = new List<Dropdown.OptionData>();
                foreach (Transform child in spEvMenu.eventsParent.transform)
                {
                    Destroy(child.gameObject);
                }
                editorOptions[6].GetComponent<Button>().interactable = true;
                editorOptions[7].GetComponent<Button>().interactable = false;
                confirms[0].onClick.RemoveAllListeners();
                confirms[0].onClick.AddListener(Confirm3);
                confirms[0].GetComponentsInChildren<Text>()[1].text = "Click confirm to lock in your swaps and be able to edit the special events.";
                menuBackButton.GetComponentsInChildren<Text>()[1].text = "If you go back, your swaps and custom advantages will be DELETED.";
                break;
            case 5:
                menuBackButton.GetComponentsInChildren<Text>()[1].text = "If you go back, your special events will be DELETED.";
                editorOptions[7].GetComponent<Button>().interactable = true;
                editorOptions[8].GetComponent<Button>().interactable = false;
                editorOptions[9].GetComponent<Button>().interactable = false;
                editorOptions[12].GetComponent<Button>().interactable = false;
                confirms[0].onClick.RemoveAllListeners();
                confirms[0].onClick.AddListener(Confirm4);
                confirms[0].interactable = true;
                confirms[0].GetComponentsInChildren<Text>()[1].text = "Click confirm to lock in your special events and be able to edit the other twists.\nHit the cast button to finish editing twists and set up your cast for the season.";
                break;
        }
        
    }

    void ConfirmPMAdv()
    {
        foreach (Transform child in PMAdvParent)
        {
            if (child.GetChild(0).GetComponentInChildren<Dropdown>().value == curPMAdv.transform.GetChild(0).GetComponentInChildren<Dropdown>().value && child.gameObject.name != curPMAdv.name)
            {
                return;
            }
        }
        curPMAdv.transform.GetChild(1).GetComponent<Button>().interactable = false;
        curPMAdv.transform.GetChild(0).GetComponentInChildren<Dropdown>().interactable = false;
        curPMAdv.transform.GetChild(2).GetComponentInChildren<Dropdown>().interactable = false;
        curPMAdv = Instantiate(AdvantagePrefab, PMAdvParent);
        curPMAdv.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ConfirmPMAdv);
        StartCoroutine(ABC());
    }

    void ConfirmMAdv()
    {
        foreach (Transform child in MAdvParent)
        {
            if (child.GetChild(0).GetComponentInChildren<Dropdown>().value == curMAdv.transform.GetChild(0).GetComponentInChildren<Dropdown>().value && child.gameObject.name != curMAdv.name)
            {
                return;
            }
        }
        curMAdv.transform.GetChild(1).GetComponent<Button>().interactable = false;
        curMAdv.transform.GetChild(0).GetComponentInChildren<Dropdown>().interactable = false;
        curMAdv.transform.GetChild(2).GetComponentInChildren<Dropdown>().interactable = false;
        curMAdv = Instantiate(AdvantagePrefab, MAdvParent);
        curMAdv.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ConfirmMAdv);
        StartCoroutine(ABC());
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

    public void ResetCustom()
    {
        confirms[0].interactable = true;
        //editorOptions[0].GetComponent<InputField>().interactable = false;
        editorOptions[1].GetComponent<InputField>().interactable = true;
        editorOptions[2].GetComponent<Dropdown>().interactable = true;
        editorOptions[3].GetComponent<Dropdown>().interactable = true;
        foreach (GameObject input in tribeSizeParent.transform)
        {
            Destroy(input);
        }
        editorOptions[4].GetComponent<InputField>().interactable = false;
        editorOptions[5].GetComponent<InputField>().interactable = false;
        confirms[1].interactable = false;
        confirms[2].interactable = false;
        editorOptions[7].GetComponent<Button>().interactable = false;
    }

    

    public static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        object[] guids = Resources.LoadAll("Contestants"); //System.Array.ConvertAll(, typeof(T)), x => x.name); //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
        {
            //string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = guids[i] as T;
        }
        a = a.OrderBy(x => int.Parse(new string(x.name.Where(char.IsDigit).ToArray()))).ToArray();
        //a = Resources.LoadAll("Contestants", typeof(T)) as T[];
        return a;

    }
}
