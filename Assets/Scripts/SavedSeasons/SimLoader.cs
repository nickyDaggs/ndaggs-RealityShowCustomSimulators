using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeasonParts;
using UnityEngine.Networking;
using UnityEditor;
using UnityEngine.UI.Extensions;
using System.Linq;

public class SimLoader : MonoBehaviour
{
    public static SimLoader instance;
    public static SimLoader Instance { get { return instance; } }

    public GameObject Loading;

    public Season loadedSeason;
    public SavedSeason load;

    public LoadContestant test;

    public List<LoadContestant> fullCast = new List<LoadContestant>();
    public List<LoadContestant> Eliminated = new List<LoadContestant>();

    public List<GameObject> Prefabs;
    public GameObject GroupPrefab, ContestantPrefab, imagePrefab;

    public GameObject Canvas;
    GameObject lastThing;

    public GameObject VoteButton;
    public GameObject Vote;
    public GameObject VotedOffCine;
    public GameObject lastVoteOff;
    public List<GameObject> Torches;
    bool what = false;

    Material grayScale;
    public List<LoadEpisode> episodes;

    bool cineTribal;
    int curEp = 0;
    int curEv = 0;
    int curTribal = 0;
    int curGroup = 0;
    int curVot;
    float tri = 0;

    bool showVL = true;

    int currentContestants = 0;
    int finaleAt = 0;

    List<LoadContestant> votes, votesRead, Idols, tie;
    Dictionary<LoadContestant, int> dicVotes = new Dictionary<LoadContestant, int>(), dicVR = new Dictionary<LoadContestant, int>();
    string actualElim, finalVotes;

    public GameObject nextButton;
    public Dropdown episodeList;

    bool e;

    public List<Texture> textures = new List<Texture>();

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        textures = GetAllInstances<Texture>().ToList();
    }

    public void Start()
    {
        StartCoroutine(SetUp());
        IEnumerator SetUp()
        {
            yield return new WaitForSeconds(.01f);
            LoadSeason(SeasonMenuManager.Instance.json);
        }
    }

    // Update is called once per frame
    public void LoadSeason(string json)
    {
        load = JsonUtility.FromJson<SavedSeason>(json);
        cineTribal = load.cineTribal;
        List<LoadContestant> urlContestants = new List<LoadContestant>();
        foreach(SavedContestant con in load.contestants)
        {
            LoadContestant newCon = new LoadContestant(con);
            if(newCon.spriteUrl != "")
            {
                urlContestants.Add(newCon);
            }

            fullCast.Add(newCon);
        }

        StartCoroutine(DownloadImage());

        IEnumerator DownloadImage()
        {
            foreach(LoadContestant contestant in urlContestants)
            {
                UnityWebRequest request = UnityWebRequestTexture.GetTexture(contestant.spriteUrl);
                yield return request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    Texture2D temp = ((DownloadHandlerTexture)request.downloadHandler).texture;

                    contestant.sprite = Sprite.Create(temp, new Rect(0, 0, temp.width, temp.height), Vector2.zero);
                }
            }
            LoadMore();
        }
    }

    public void LoadMore()
    {
        test = fullCast[0];

        foreach (SavedEpisode episode in load.episodes)
        {
            LoadEpisode epi = new LoadEpisode();
            epi.episodeName = episode.episodeName;
            foreach (SavedPageTrue page in episode.pages)
            {
                LoadPage newPage = new LoadPage(page);
                newPage.obj = MakePage(newPage);
                StartCoroutine(ABC(newPage.obj));
                epi.pages.Add(newPage);
            }
            episodes.Add(epi);
        }
        episodeList.options = episodes.ConvertAll(x => new Dropdown.OptionData { text = x.episodeName }).ToList();

        curEp = 0;
        curEv = 0;

        StartCoroutine(cancelLoad());
    }

    IEnumerator cancelLoad()
    {
        yield return new WaitForSeconds(3f);
        Loading.SetActive(false);

    }

    public void NextGM()
    {
        

        if (curEp > episodes.Count - 1)
        {
            nextButton.SetActive(false);
            return;
        }
        episodes[curEp].pages[curEv].obj.SetActive(false);
        curEv++;
        if (curEv >= episodes[curEp].pages.Count)
        {
            curEp++;
            curEv = 0;
            if (curEp > episodes.Count - 1)
            {
                nextButton.SetActive(false);
                return;
            }
        }
        episodes[curEp].pages[curEv].obj.SetActive(true);

        if (episodes[curEp].pages[curEv].obj.name == "Placements")
        {
            nextButton.SetActive(false);
        }
        if (episodes[curEp].pages[curEv].obj.name.Contains("Tribal Council") && cineTribal)
        {
            dicVotes = new Dictionary<LoadContestant, int>();
            what = true;
            int numb = episodes[curEp].pages.Count - 1;
            if (VotedOffCine.transform.GetChild(0).childCount > 0)
            {
                for (int i = 0; i < VotedOffCine.transform.GetChild(0).childCount; i++)
                {
                    //VotedOffCine.transform.GetChild(0).GetChild(i).transform.SetParent(null);
                    Destroy(VotedOffCine.transform.GetChild(0).GetChild(i).transform.gameObject);
                }
            }
            votes = ConvertToCons(episodes[curEp].pages[curEv].Votes);
            votesRead = ConvertToCons(episodes[curEp].pages[curEv].VotesRead);
            Idols = ConvertToCons(episodes[curEp].pages[curEv].Idols);
            actualElim = episodes[curEp].pages[curEv].elim;
            finalVotes = episodes[curEp].pages[curEv].voteCount;

            foreach (GameObject torch in Torches)
            {
                torch.SetActive(true);
            }

            VoteButton.SetActive(true);
            lastVoteOff = episodes[curEp].pages[curEv].obj.transform.GetChild(0).GetChild(0).gameObject;

            if (episodes[curEp].pages[curEv].VoteObjs.Count > 0)
            {
                foreach (GroupObject vot in episodes[curEp].pages[curEv].VoteObjs)
                {
                    MakeGroup(vot, VotedOffCine.transform.GetChild(0));
                    //vot.transform.parent = VotedOffCine.transform.GetChild(0);
                }
            }
            curVot = 0;

            if (votes.Count > 0)
            {
                Dictionary<LoadContestant, int> check = new Dictionary<LoadContestant, int>();
                LoadContestant votedOff = votes[0];
                check.Add(votes[0], 1);
                dicVR = new Dictionary<LoadContestant, int>();
                if (!Idols.Contains(votesRead[0]))
                {
                    dicVR.Add(votesRead[0], 1);
                }
                for (int i = 1; i < votes.Count; i++)
                {
                    if (check.ContainsKey(votes[i]))
                    {
                        check[votes[i]] += 1;
                        if (check[votes[i]] > check[votedOff])
                        {
                            votedOff = votes[i];
                        }
                    }
                    else if (!check.ContainsKey(votes[i]))
                    {
                        check.Add(votes[i], 1);
                    }
                }
                tie = new List<LoadContestant>();
                float maxValue = check.Values.Max();
                foreach (KeyValuePair<LoadContestant, int> num in check)
                {
                    if (num.Value == maxValue)
                    {
                        tie.Add(num.Key);
                    }
                    else
                    {

                    }
                }

            }
        } else
        {
            if (Vote.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("voteRevealed"))
            {
                Vote.GetComponent<Animator>().SetTrigger("Reveal");
                
                //what = true;
            }
            for (int i = 0; i < VotedOffCine.transform.GetChild(0).childCount; i++)
            {
                VotedOffCine.transform.GetChild(0).GetChild(i).transform.SetParent(null);
            }
            foreach (GameObject torch in Torches)
            {
                torch.SetActive(false);
            }
            VotedOffCine.SetActive(false);
            VoteButton.SetActive(false);
            VotedOffCine.transform.parent.gameObject.SetActive(false);
        }


    }

    public void LastGM()
    {
        nextButton.SetActive(true);
        episodes[curEp].pages[curEv].obj.SetActive(false);

        curEv--;
        if (curEv < 0)
        {
            curEp--;
            
            if(curEp < 0)
            {
                curEp = 0;
                curEv = 0;
            } else
            {
                curEv = episodes[curEp].pages.Count - 1;
            }
        }
        episodes[curEp].pages[curEv].obj.SetActive(true);

        if (episodes[curEp].pages[curEv].obj.name.Contains("Tribal Council") && cineTribal)
        {
            dicVotes = new Dictionary<LoadContestant, int>();
            what = true;
            int numb = episodes[curEp].pages.Count - 1;
            if (VotedOffCine.transform.GetChild(0).childCount > 0)
            {
                for (int i = 0; i < VotedOffCine.transform.GetChild(0).childCount; i++)
                {
                    //VotedOffCine.transform.GetChild(0).GetChild(i).transform.SetParent(null);
                    Destroy(VotedOffCine.transform.GetChild(0).GetChild(i).transform.gameObject);
                }
            }
            votes = ConvertToCons(episodes[curEp].pages[curEv].Votes);
            votesRead = ConvertToCons(episodes[curEp].pages[curEv].VotesRead);
            Idols = ConvertToCons(episodes[curEp].pages[curEv].Idols);
            actualElim = episodes[curEp].pages[curEv].elim;
            finalVotes = episodes[curEp].pages[curEv].voteCount;

            foreach (GameObject torch in Torches)
            {
                torch.SetActive(true);
            }

            VoteButton.SetActive(true);
            lastVoteOff = episodes[curEp].pages[curEv].obj.transform.GetChild(0).GetChild(0).gameObject;

            if (episodes[curEp].pages[curEv].VoteObjs.Count > 0)
            {
                foreach (GroupObject vot in episodes[curEp].pages[curEv].VoteObjs)
                {
                    MakeGroup(vot, VotedOffCine.transform.GetChild(0));
                    //vot.transform.parent = VotedOffCine.transform.GetChild(0);
                }
            }
            curVot = 0;

            if (votes.Count > 0)
            {
                Dictionary<LoadContestant, int> check = new Dictionary<LoadContestant, int>();
                LoadContestant votedOff = votes[0];
                check.Add(votes[0], 1);
                dicVR = new Dictionary<LoadContestant, int>();
                if (!Idols.Contains(votesRead[0]))
                {
                    dicVR.Add(votesRead[0], 1);
                }
                for (int i = 1; i < votes.Count; i++)
                {
                    if (check.ContainsKey(votes[i]))
                    {
                        check[votes[i]] += 1;
                        if (check[votes[i]] > check[votedOff])
                        {
                            votedOff = votes[i];
                        }
                    }
                    else if (!check.ContainsKey(votes[i]))
                    {
                        check.Add(votes[i], 1);
                    }
                }
                tie = new List<LoadContestant>();
                float maxValue = check.Values.Max();
                foreach (KeyValuePair<LoadContestant, int> num in check)
                {
                    if (num.Value == maxValue)
                    {
                        tie.Add(num.Key);
                    }
                    else
                    {

                    }
                }

            }
        }
        else
        {
            if (Vote.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("voteRevealed"))
            {
                Vote.GetComponent<Animator>().SetTrigger("Reveal");

                //what = true;
            }
            for (int i = 0; i < VotedOffCine.transform.GetChild(0).childCount; i++)
            {
                VotedOffCine.transform.GetChild(0).GetChild(i).transform.SetParent(null);
            }
            foreach (GameObject torch in Torches)
            {
                torch.SetActive(false);
            }
            VotedOffCine.SetActive(false);
            VoteButton.SetActive(false);
            VotedOffCine.transform.parent.gameObject.SetActive(false);
        }
    }

    public void VoteReveal()
    {

        what = true;
        if(lastVoteOff != null)
        {
            lastVoteOff.SetActive(false);
        }
        if (votesRead.Count > 1)
        {
            if (Vote.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("voteIdle"))
            {
                string votesLeft = "";
                string votess = "";
                if (curVot == 0 && currentContestants != 3)
                {
                    Vote.transform.GetChild(2).GetComponent<Text>().text = "First vote...";
                    votess = " vote ";
                    if (showVL == true)
                    {
                        float vl = votes.Count - 1;
                        votesLeft = ". " + vl + " Votes Left";
                    }
                    else
                    {
                        votesLeft = "";
                    }
                    Vote.transform.GetChild(0).GetComponent<Text>().text = votesRead[0].nickname;

                    if (Idols.Contains(votesRead[0]))
                    {
                        Vote.transform.GetChild(1).GetComponent<Text>().text = "<color=red>DOES NOT COUNT</color>";
                    }
                    else
                    {
                        Vote.transform.GetChild(1).GetComponent<Text>().text = dicVR[votesRead[0]] + votess + votesRead[0].nickname + votesLeft;
                    }
                    curVot++;
                }
                else if (curVot == 0 && currentContestants == 3)
                {
                    

                    Vote.transform.GetChild(2).GetComponent<Text>().text = actualElim;
                    votess = " vote ";
                    votesLeft = "";
                    Vote.transform.GetChild(0).GetComponent<Text>().text = votes[0].nickname;
                    Vote.transform.GetChild(1).GetComponent<Text>().text = "Final votes count was 1 vote " + votes[0].nickname;

                    VotedOffCine.transform.parent.gameObject.SetActive(true);
                    VotedOffCine.SetActive(true);
                    nextButton.gameObject.SetActive(true);
                    VoteButton.SetActive(false);
                    curTribal++;
                }
                else
                {
                    Vote.transform.GetChild(2).GetComponent<Text>().text = "";
                    if (dicVR.ContainsKey(votesRead[curVot]))
                    {
                        dicVR[votesRead[curVot]] += 1;
                    }
                    else if (!dicVR.ContainsKey(votesRead[curVot]))
                    {
                        if (!Idols.Contains(votesRead[curVot]))
                        {
                            dicVR.Add(votesRead[curVot], 1);
                        }
                    }
                    votess = "";
                    votesLeft = "";
                    if (showVL == true)
                    {
                        float vl = votes.Count - curVot - 1 + Idols.Count;

                        if (vl > 0)
                        {
                            if (vl > 1)
                            {
                                votesLeft = ". " + vl + " Votes Left";
                            }
                            else if (vl < 1)
                            {
                                votesLeft = "";
                            }
                            else if (vl == 1)
                            {
                                votesLeft = ". " + vl + " Vote Left";
                            }

                        }
                    }
                    else
                    {
                        votesLeft = "";
                    }
                    List<string> votesSoFar = new List<string>();
                    foreach (KeyValuePair<LoadContestant, int> num in dicVR)
                    {
                        if (num.Value > 1)
                        {
                            votess = " votes ";
                        }
                        else
                        {
                            votess = " vote ";
                        }
                        string v = dicVR[num.Key] + votess + num.Key.nickname;
                        votesSoFar.Add(v);
                    }
                    votesSoFar = votesSoFar.OrderByDescending(go => go[0]).ToList();
                    Vote.transform.GetChild(0).GetComponent<Text>().text = votesRead[curVot].nickname;
                    Vote.transform.GetChild(1).GetComponent<Text>().text = string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + votesLeft;
                    if (Idols.Contains(votesRead[curVot]))
                    {
                        Vote.transform.GetChild(1).GetComponent<Text>().text = "<color=red>DOES NOT COUNT</color>";
                    }
                    if (curVot == votesRead.Count - 1 && tie.Count < 2)
                    {
                        nextButton.gameObject.SetActive(true);
                        VoteButton.SetActive(false);

                        Vote.transform.GetChild(2).GetComponent<Text>().text = actualElim;

                        //elimed++;
                        foreach (UIGroup group in VotedOffCine.GetComponentsInChildren<UIGroup>())
                        {
                            if (group.eventText.text.Contains("returns to the game"))
                            {
                                Vote.transform.GetChild(2).GetComponent<Text>().text = "";
                            }
                        }
                        votesSoFar = new List<string>();
                        foreach (KeyValuePair<LoadContestant, int> num in dicVotes)
                        {
                            if (num.Value > 1)
                            {
                                votess = " votes ";
                            }
                            else
                            {
                                votess = " vote ";
                            }
                            string v = dicVotes[num.Key] + votess + num.Key.nickname;
                            Debug.Log(v);
                            votesSoFar.Add(v);
                        }
                        votesSoFar = votesSoFar.OrderByDescending(go => go[0]).ToList();
                        //finalVotes = "Final vote count was " + string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + ".";

                        Vote.transform.GetChild(1).GetComponent<Text>().text = finalVotes;
                        VotedOffCine.transform.parent.gameObject.SetActive(true);
                        VotedOffCine.SetActive(true);
                        curTribal++;
                    }
                    else if (curVot == votesRead.Count - 1 && tie.Count > 1)
                    {
                        nextButton.gameObject.SetActive(true);
                        VoteButton.SetActive(false);
                        VotedOffCine.SetActive(true);
                        if (e == false && curEp < episodes.Count - 1)
                        {
                            string firstline = "There is a tie and a revote. Those in in the tie will not revote, unless no one received votes on the original vote.";
                            string secondline = finalVotes;// "Final vote count was " + string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + ".";
                            GroupObject group = new GroupObject {tNameEnabled=false, eText= firstline + "\n" + "\n" + "\n" + secondline, cons=tie.ConvertAll(x => x.id), spacing=20 };
                            //MakeGroup(group, VotedOffCine.transform.GetChild(0));
                        }

                        curTribal++;
                        VotedOffCine.transform.parent.gameObject.SetActive(true);
                    }
                    curVot++;
                }
            }

        }
        else
        {
            nextButton.gameObject.SetActive(true);
            VoteButton.SetActive(false);

            //elimed++;
            Vote.transform.GetChild(0).GetComponent<Text>().text = votes[0].nickname;
            Vote.transform.GetChild(1).GetComponent<Text>().text = "Final vote count was 1 vote " + votes[0].nickname;
            Vote.transform.GetChild(2).GetComponent<Text>().text = actualElim;
            VotedOffCine.transform.parent.gameObject.SetActive(true);
            VotedOffCine.SetActive(true);
            curTribal++;
        }
        if (Vote.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("voteIdle") || Vote.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("voteRevealed"))
        {
            Vote.GetComponent<Animator>().SetTrigger("Reveal");
        }
    }

    GameObject MakePage(LoadPage page)
    {
        GameObject real = Instantiate(Prefabs[page.type]);

        real.transform.parent = Canvas.transform;
        real.GetComponent<RectTransform>().offsetMax = new Vector2(0, real.GetComponent<RectTransform>().offsetMax.y);
        real.GetComponent<RectTransform>().offsetMax = new Vector2(real.GetComponent<RectTransform>().offsetMin.x, 0);
        real.name = page.namePage;

        foreach(GroupObject group in page.groups)
        {
            Transform parent = real.transform.GetChild(0).GetChild(0);
            if(page.type == 2)
            {
                parent = real.transform.GetChild(0);
            }
            MakeGroup(group, parent);
        }

        real.SetActive(false);
        return real;
    }

    List<LoadContestant> ConvertToCons(List<int> input)
    {
        List<LoadContestant> cons = new List<LoadContestant>();
        for(int i = 0; i < input.Count; i++)
        {
            cons.Add(fullCast.Find(x => x.id == input[i]));
        }
        return cons;
    }

    public float ConListWidth(float contestants, bool placement)
    {

        if (contestants % 9 == 0 && placement && contestants / 9 > 2)
        {
            return 1100f;
        }
        else if (contestants % 8 == 0 && placement && contestants / 8 > 2)
        {
            return 1000f;
        }
        else if (contestants % 7 == 0 && placement && contestants / 7 > 2)
        {
            return 900f;
        }
        else if (contestants % 6 == 0)
        {
            return 800f;
        }
        else if (contestants % 5 == 0)
        {
            return 700f;
        }
        else if (contestants % 4 == 0)
        {
            return 550f;
        }
        else
        {
            contestants++;
            if (contestants % 6 == 0)
            {
                return 800f;
            }
            else if (contestants % 5 == 0)
            {
                return 700f;
            }
            else if (contestants % 4 == 0)
            {
                return 550f;
            }
            else
            {
                if (contestants > 10)
                {
                    contestants = Mathf.Round(contestants / 10) * 10;
                    if (contestants % 6 == 0)
                    {
                        return 800f;
                    }
                    else if (contestants % 5 == 0)
                    {
                        return 700f;
                    }
                    else if (contestants % 4 == 0)
                    {
                        return 550f;
                    }
                    else
                    {
                        return 800f;
                    }
                }
                else
                {
                    return 800f;
                }
            }
        }
    }

    IEnumerator ABC(GameObject game)
    {
        //returning 0 will make it wait 1 frame
        game.SetActive(!game.activeSelf);
        yield return 0;
        game.SetActive(!game.activeSelf);
        yield return 0;
        game.SetActive(!game.activeSelf);
        yield return 0;
        game.SetActive(!game.activeSelf);
        //code goes here



        if (game.name == "Placements")
        {
            //Loading.SetActive(false);
            episodes[0].pages[0].obj.SetActive(true);
        }
    }

    void MakeGroup(GroupObject g, Transform ep)
    {
        float spacing = g.spacing;
        bool nameEnabled = g.tNameEnabled;
        string teamName = g.teamName;
        string teamColor = g.teamColor;
        string conText = g.conText;
        string aText = g.aText;
        string eText = g.eText;
        bool placement = g.placement;
        bool owStatus = g.owStatus;
        bool all = g.all;
        List<LoadContestant> cons = ConvertToCons(g.cons);
        if(placement)
        {
            Eliminated = cons;
        }

        GameObject team = Instantiate(GroupPrefab);
        team.GetComponent<UIGroup>().tribeName.enabled = nameEnabled;

        if (nameEnabled)
        {
            team.GetComponent<UIGroup>().tribeName.text = teamName;
            Color newCol;

            //Debug.Log(ColorUtility.TryParseHtmlString(teamColor, out newCol));
            if (ColorUtility.TryParseHtmlString("#" + teamColor, out newCol))
            {
                team.GetComponent<UIGroup>().tribeName.color = newCol;
            }
        }
        else
        {
            team.GetComponent<UIGroup>().tribeName.gameObject.SetActive(false);
        }
        int highestLength = 0;
        int er = Eliminated.Count;
        if (placement)
        {
            //team.GetComponent<SetupLayout>()._ContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            if (Eliminated.Count % 9 == 0 && Eliminated.Count / 9 > 2)
            {
                er = 8;
            }
            else if (Eliminated.Count % 8 == 0 && Eliminated.Count / 8 > 2)
            {
                er = 7;
            }
            else if (Eliminated.Count % 7 == 0 && Eliminated.Count / 7 > 2)
            {
                er = 6;
            }
            else if (Eliminated.Count % 6 == 0)
            {
                er = 5;
            }
            else if (Eliminated.Count % 5 == 0)
            {
                er = 4;
            }
            else if (Eliminated.Count % 4 == 0)
            {
                er = 3;
            }
            else
            {
                er++;
                if (er % 6 == 0)
                {
                    er = 5;
                }
                else if (er % 5 == 0)
                {
                    er = 4;
                }
                else if (er % 4 == 0)
                {
                    er = 3;
                }
                else
                {
                    er = 5;
                }
            }
        }
        if (cons.Count > 0)
        {
            foreach (LoadContestant num in cons)
            {
                GameObject mem = Instantiate(ContestantPrefab);
                mem.GetComponentInChildren<Image>().sprite = num.sprite;
                if (conText == "name")
                {
                    mem.GetComponentInChildren<Text>().text = num.fullname;
                }
                else if (conText == "nname")
                {
                    mem.GetComponentInChildren<Text>().text = num.nickname;
                }
                else if (conText == "placement")
                {
                    string place = Oridinal(Eliminated.Count - Eliminated.IndexOf(num)) + " Place";
                    if (Eliminated.Count - Eliminated.IndexOf(num) == 1)
                    {
                        place = "Winner";
                    }
                    num.placement = place + "\n" + num.placement;
                    mem.GetComponentInChildren<Text>().text = load.placementTexts[cons.IndexOf(num)];
                }
                else
                {
                    mem.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
                    mem.GetComponentInChildren<Text>().text = conText;
                }
                if(cons.IndexOf(num) < g.grayCon.Count)
                {
                    if(g.grayCon[cons.IndexOf(num)])
                    {
                        mem.GetComponentInChildren<Image>().material = grayScale;
                    }
                }
                if (all)
                {
                    mem.GetComponentInChildren<Text>().text += g.allLoyalty[cons.IndexOf(num)];
                }
                mem.transform.parent = team.transform.GetChild(2);
                List<string> conTeams = new List<string>();
                if(cons.IndexOf(num) < g.conColors.Count)
                {
                    conTeams = g.conColors[cons.IndexOf(num)].colors;
                }
                if ((nameEnabled || all) && conTeams.Count > 1)
                {
                    mem.transform.GetChild(1).gameObject.SetActive(true);
                    for (int j = 0; j < conTeams.Count - 1; j++)
                    {
                        GameObject image = Instantiate(imagePrefab);
                        Color newCol;
                        if (ColorUtility.TryParseHtmlString("#" + conTeams[j], out newCol))
                        {
                            image.GetComponent<Image>().color = newCol;
                        }
                        image.transform.parent = mem.transform.GetChild(1);
                    }
                }
                if (owStatus)
                {
                    mem.transform.GetChild(1).gameObject.SetActive(true);
                    for (int j = 0; j < conTeams.Count; j++)
                    {
                        GameObject image = Instantiate(imagePrefab);
                        Color newCol;
                        if (ColorUtility.TryParseHtmlString("#" + conTeams[j], out newCol))
                        {
                            image.GetComponent<Image>().color = newCol;
                        }
                        image.transform.parent = mem.transform.GetChild(1);
                    }
                }
                if (placement)
                {
                    mem.transform.GetChild(1).gameObject.SetActive(true);
                    for (int j = 0; j < conTeams.Count; j++)
                    {
                        GameObject image = Instantiate(imagePrefab);
                        Color newCol;
                        if (ColorUtility.TryParseHtmlString("#" + conTeams[j], out newCol))
                        {
                            image.GetComponent<Image>().color = newCol;
                        }
                        image.transform.parent = mem.transform.GetChild(1);
                    }
                    int ee = mem.GetComponentInChildren<Text>().text.Split('\n').Length - 4;
                    if (ee > highestLength && Eliminated.IndexOf(num) > er)
                    {
                        highestLength = ee;
                    }
                    if (ee > 0)
                    {
                        mem.GetComponentInChildren<VerticalLayoutGroup>().padding.bottom -= 16 * ee;
                    }
                }
            }
        }
        if (ep != null)
        {
            team.transform.parent = ep;
            //Debug.Log(savedPages.Find(x => x.connected == real));
        }
        team.transform.parent = ep;


        float teamWidth = ConListWidth(team.transform.GetChild(2).childCount, placement);
        if (placement)
        {
            team.GetComponent<UIGroup>().List.GetComponent<FlowLayoutGroup>().SpacingY = (16 * highestLength) + 2;
            team.GetComponent<UIGroup>().List.GetComponent<FlowLayoutGroup>().SpacingX = 40;
            team.GetComponent<VerticalLayoutGroup>().padding.top = 0;
        }
        team.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(teamWidth, team.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.y);
        team.GetComponent<RectTransform>().ForceUpdateRectTransforms();

        if (team.GetComponent<RectTransform>().sizeDelta.y < 1 && placement)
        {
            team.GetComponent<SetupLayout>()._ContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            team.GetComponent<SetupLayout>()._ContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        team.GetComponent<UIGroup>().allianceText.text = aText;

        team.GetComponent<UIGroup>().eventText.text = eText;
        if (spacing != 0)
        {
            team.GetComponent<VerticalLayoutGroup>().spacing = spacing;
        }
    }

    public string Oridinal(float num)
    {
        if (num % 100 == 11 || num % 100 == 12 || num % 100 == 13)
        {
            return num + "th";
        }
        else
        {
            if (num % 10 == 1)
            {
                return num + "st";
            }
            else if (num % 10 == 2)
            {
                return num + "nd";
            }
            else if (num % 10 == 3)
            {
                return num + "rd";
            }
            else
            {
                return num + "th";
            }
        }
    }

    public void OnEpisodeChange()
    {
        nextButton.gameObject.SetActive(true);
        if(lastThing != null)
        {
            lastThing.SetActive(false);

        }
        episodes[episodeList.value].pages[0].obj.SetActive(true);
        curEp = episodeList.value;
        curEv = 0;
        lastThing = episodes[episodeList.value].pages[0].obj;
    }

    public static T[] GetAllInstances<T>() where T : Texture
    {
        object[] guids = Resources.LoadAll("Sprites"); //System.Array.ConvertAll(, typeof(T)), x => x.name); //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
        {
            //string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = guids[i] as T;
        }
        //a = a.OrderBy(x => int.Parse(new string(x.name.Where(char.IsDigit).ToArray()))).ToArray();
        //a = Resources.LoadAll("Contestants", typeof(T)) as T[];
        return a;

    }

    [System.Serializable]
    public class SavedSeason
    {
        public string seasonName;
        public List<SavedEpisode> episodes = new List<SavedEpisode>();
        public List<SavedContestant> contestants = new List<SavedContestant>();
        public List<string> placementTexts = new List<string>();
        public bool cineTribal = false;
    }
    [System.Serializable]
    public class SavedEpisode
    {
        public string episodeName;
        public int episodeNum;
        public List<SavedPageTrue> pages = new List<SavedPageTrue>();
    }
    [System.Serializable]
    public class LoadEpisode
    {
        public string episodeName;
        public int episodeNum;
        public List<LoadPage> pages = new List<LoadPage>();
    }
    [System.Serializable]
    public class SavedPage
    {
        public List<GroupObject> groups = new List<GroupObject>();
        public GameObject connected;

        public List<GroupObject> VoteObjs = new List<GroupObject>();
        public int type;
        public int episode;
        public int eventNum;
    }
    [System.Serializable]
    public class SavedPageTrue
    {
        public string namePage;
        public List<GroupObject> groups = new List<GroupObject>();
        public List<GroupObject> VoteObjs = new List<GroupObject>();
        public List<int> Votes = new List<int>();
        public List<int> VotesRead = new List<int>();
        public List<int> Idols = new List<int>();
        public string elim;
        public string voteCount;
        public int episode;
        public int type;
    }
    [System.Serializable]
    public class LoadPage : SavedPageTrue
    {
        public GameObject obj;

        public LoadPage(SavedPageTrue copy)
        {
            namePage = copy.namePage;
            groups = copy.groups;
            VoteObjs = copy.VoteObjs;
            Votes = copy.Votes;
            VotesRead = copy.VotesRead;
            Idols = copy.Idols;
            elim = copy.elim;
            voteCount = copy.voteCount;
            episode = copy.episode;
            type = copy.type;
        }
    }
    [System.Serializable]
    public class SavedContestant
    {
        public int id;
        public string fullname;
        public string nickname;
        public string spritePath;
        public string spriteUrl = "";
        public string placement;
        public string gender;
    }
    [System.Serializable]
    public class LoadContestant : SavedContestant
    {
        public Sprite sprite;
        public List<string> teams;

        public LoadContestant(SavedContestant copy)
        {
            id = copy.id;
            fullname = copy.fullname;
            nickname = copy.nickname;
            spritePath = copy.spritePath;
            spriteUrl = copy.spriteUrl;
            gender = copy.gender;
            if(copy.spriteUrl != "")
            {
                
            } else
            {
                if(copy.spritePath != "")
                {
                    //Debug.Log(Resources.Load("Sprites/China/aaron"));// + copy.spritePath));
                    sprite = Resources.Load<Sprite>("Sprites/" + copy.spritePath);//"Sprites/" + copy.spritePath);
                } else
                {
                    sprite = (Sprite)Resources.Load("Sprites/perfectsurvivor", typeof(Sprite));
                }
            }
        }

        
    }
    [System.Serializable]
    public class GroupObject
    {
        public bool tNameEnabled;
        public string teamName;
        public string teamColor;
        public string conText;
        public string aText;
        public string eText;
        public List<int> cons = new List<int>();
        public float spacing;
        public bool owStatus = false;
        public bool placement = false;
        public bool all = false;
        public List<ColorsList> conColors = new List<ColorsList>();
        public List<string> allLoyalty = new List<string>();
        public List<bool> grayCon = new List<bool>();
    }
    [System.Serializable]
    public class ColorsList
    {

        public List<string> colors = new List<string>();
        
    }




}
