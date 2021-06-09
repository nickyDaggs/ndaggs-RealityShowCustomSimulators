using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using SeasonParts;
using UnityEngine.UI.Extensions;

public class GameManager : MonoBehaviour
{
    public SeasonTemplate seasonTemp;
    public List<Team> Tribes = new List<Team>();
    public List<Alliance> Alliances = new List<Alliance>();
    float nextEvent = 1;
    Contestant votedOff;
    List<Contestant> voters = new List<Contestant>();
    public List<Contestant> Exiled = new List<Contestant>();
    public float mergeAt, finaleAt, juryAt;
    public Button nextButton;
    public List<Contestant> immune =  new List<Contestant>();
    List<Contestant> targets;
    public List<Contestant> Eliminated;
    public float currentContestants;
    float currentContestantsOG;
    public bool merged, cineTribal, showVL, genderEqual;
    public List<EpisodeSetting> Episodes;
    int curEpp = 0;
    int curEvv = 0;
    int curT = 0;
    public Cast cast;
    Contestant Winner;
    public SeasonTemplate sea;
    public List<GameObject> Prefabs;
    public GameObject GroupPrefab, ContestantPrefab, imagePrefab, Canvas, Vote, VoteButton, VotedOffCine;
    GameObject lastThing;
    GameObject lastVoteOff;
    public List<GameObject> Torches;
    int curVot;
    float tri;
    float jurVotesRemoved;
    public Season baseSeason;
    Season currentSeason;
    public List<Episode> eps;
    public Material grayScale;
    //Episode epi;
    int curTribal;
    bool h = true;
    float ere;
    public TribeSwap curSwap;
    public static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    public Swaps swapper;
    public Exile curExile;
    public OneTimeEvent curEvent = new OneTimeEvent();
    string MOP;
    bool MOPExpired;
    List<Team> Remove = new List<Team>();
    int curTTT = 0;
    int curGroup = 0;
    bool revoteNext = false;
    public RejoiningTwists reTwists;
    public OneTimeEvents oneTimeEvents;
    public int re;
    int elimed = 1;
    int jurt;
    [HideInInspector] public bool RIExpired = false, OCExpired = false, e = false;
    [HideInInspector] public Contestant lastEOE = null, kidnapped = null;
    [HideInInspector] public Team Outcasts = new Team();
    [HideInInspector] public List<Contestant> jury = new List<Contestant>(), RIsland = new List<Contestant>(), EOE = new List<Contestant>(), tie = new List<Contestant>(), votesRead = new List<Contestant>(), votes = new List<Contestant>();
    [HideInInspector] public int curEv = 0, curEp = 0;
    [HideInInspector] public Dictionary<Contestant, int> dic = new Dictionary<Contestant, int>(), dicVR = new Dictionary<Contestant, int>();
    [HideInInspector] public Team MergedTribe = new Team();
    [HideInInspector] public Team LosingTribe = new Team();
    [HideInInspector] public List<Team> LosingTribes = new List<Team>();
    [HideInInspector] public string finalVotes;
    // Start is called before the first frame update
    public void Start()
    {
        //print("<b>Gaming</b>");
        curSwap.on = false;
        curExile.on = false;
        int male = 0;
        int female = 0;
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        curTribal = 0;
        currentSeason = Instantiate(baseSeason);
        foreach(GameObject torch in Torches)
        {
            torch.SetActive(false);
        }
        sea = Instantiate(seasonTemp);
        Tribes = new List<Team>(sea.Tribes);
        mergeAt = sea.mergeAt; juryAt = sea.jury; finaleAt = sea.final;
        SetSeason();
        nextButton.onClick.AddListener(NextGM);
        

        foreach (Team tribe in Tribes)
        {
            MakeAlliances(tribe);
            currentContestants += tribe.members.Count;
            foreach (Contestant num in tribe.members)
            {
                num.teams.Add(tribe.tribeColor);
                if(num.gender == "M")
                {
                    male++;
                } else if(num.gender == "F")
                {
                    female++;
                }
            }
        }
        if(!sea.Outcasts)
        {
            OCExpired = true;
        }
        if(male == female)
        {
            genderEqual = true;
        }
        //lastThing = EpisodeStart;
        //TieGame();
        nextEvent = 1;
        immune = new List<Contestant>();
        MergedTribe.name = "Merge Tribe";
        CreateEpisodeSettings();
        curEp = 0;
        curEv = 0;
        currentContestantsOG = currentContestants;
        NextEvent();
    }
    // Update is called once per frame
    void Update()
    {
        eps = currentSeason.Episodes;
        if(h == false)
        {
            TurnOff();
            h = true;
            tri = 0;
            jurt = jury.Count - 1;
        }
    }
    void TurnOff()
    {
        currentContestants = currentContestantsOG;
        foreach (Episode epp in currentSeason.Episodes)
        {
            foreach (Page em in epp.events)
            {
                em.obj.SetActive(false);
            }
        }
        curEp = 0;
        curEv = 1;
        currentSeason.Episodes[0].events[0].obj.SetActive(true);
        tri = 0;
    }
    void SetSeason()
    {
        int con = 0;
        for(int i = 0; i < Tribes.Count; i++)
        {
            for(int j = 0; j < Tribes[i].members.Count; j++)
            {
                Tribes[i].members[j] = Instantiate(cast.cast[con]);
                con++;
            }
        }
    }
    void CreateEpisodeSettings()
    {
        float episodeCount = currentContestants - finaleAt;
        foreach (OneTimeEvent timeEvent in sea.oneTimeEvents)
        {
            if (timeEvent.type.Contains("MultiTribal") || timeEvent.type.Contains("DoubleElim") || timeEvent.type.Contains("MergeSplit"))
            {
                episodeCount--;
            }
            if(timeEvent.type == "FirstImpressions" && timeEvent.context == "RI")
            {
                episodeCount -= 2;
            }
        }
        if (sea.RedemptionIsland || sea.EdgeOfExtinction)
        {
            episodeCount += 2;
        }
        if(sea.Outcasts)
        {
            episodeCount++;
        }
        float curCon = currentContestants;
        float mergeRound = 0;
        float curTeams = sea.Tribes.Count;
        int curSE = 0;
        bool riE = false;
        bool oc = false;
        for(int i = 0; i < episodeCount; i++)
        {
            EpisodeSetting ep = new EpisodeSetting();
            ep.name = "Episode " + (i + 1);
            ep.swap.on = false;
            ep.exileIsland.on = false;
            if (curCon == currentContestants)
            {
                foreach (OneTimeEvent timeEvent in sea.oneTimeEvents)
                {
                    if (i + 1 == timeEvent.round)
                    {
                        ep.Event = timeEvent;
                    }
                }
                if(ep.Event.type != "")
                {
                    if(ep.Event.type == "PalauStart")
                    {
                        EpisodeSetting episode = new EpisodeSetting();
                        episode.events.Add("PalauStart");
                        Episodes.Add(episode);
                    } else 
                    {
                        ep.events.Add("BeginningTwist");
                    }
                }
                ep.events.Add("NextEp");
                ep.events.Add("TribeStatus");
                if(sea.MedallionOfPower)
                {
                    ep.events.Add("MOPChallenge");
                }
                ep.events.Add("TribeImmunity");
                ep.events.Add("TribalCouncil");
                ep.events.Add("ShowVotes");
            }
            else if (curCon > mergeAt)
            {
                if(sea.ExileIslandd)
                {
                    bool skip = false;
                    foreach(int num in sea.Twists.epsSkip)
                    {
                        if(num == i+1)
                        {
                            skip = true;
                        }
                    }
                    if(i+1 < sea.Twists.expireAt && skip == false)
                    {
                        
                        ep.exileIsland = sea.Twists.preMergeEIsland;
                        ep.exileIsland.on = true;
                    }
                    foreach (int num in sea.Twists.epsSpecialE)
                    {
                        if (num == i + 1 && skip == false)
                        {
                            Debug.Log(i + 1);
                            ep.exileIsland = sea.Twists.SpecialEx[curSE];
                            curSE++;
                            ep.exileIsland.on = true;
                        }
                    }
                }
                foreach (TribeSwap swap in sea.swaps)
                {
                    if(curCon == swap.swapAt)
                    {
                        ep.events.Add("NextEp");
                        ep.events.Add("Swap");
                        ep.swap = swap;
                        ep.swap.on = true;
                        if (swap.exile == true)
                        {
                            ep.exileIsland = swap.exileIsland;
                            ep.exileIsland.on = true;
                        }
                        if(ep.swap.type != "Mutiny" || ep.swap.type != "RegularSwap" || ep.swap.type != "SplitTribes" || ep.swap.type != "RegularSwap" || (!ep.swap.ResizeTribes &&  ep.swap.type == "RegularShuffle"))
                        curTeams = ep.swap.newTribes.Count;
                    }
                }
                foreach (OneTimeEvent timeEvent in sea.oneTimeEvents)
                {
                    if (i+1 == timeEvent.round)
                    {
                        ep.Event = timeEvent;
                    }
                }
                ep.events.Add("NextEp");
                ep.events.Add("TribeStatus");
                if(sea.RedemptionIsland && i + 1 > 2)
                {
                    bool skip = false;
                    foreach (int num in sea.Twists.epsSkipRI)
                    {
                        if (i + 1 == num)
                        {
                            skip = true;
                        }
                    }
                    if (!skip)
                    {
                        foreach (int num in sea.Twists.epsSpecialRI)
                        {
                            if (i + 1 == num)
                            {
                                ep.elimAllButTwo = true;
                            }
                        }
                        ep.events.Add("RedemptionIsland");
                    }
                }
                if(sea.EdgeOfExtinction)
                {
                    ep.events.Add("EOEStatus");
                }
                if(ep.Event.type.Contains("MultiTribal") || ep.Event.type.Contains("JointTribal"))
                {
                    ep.events.Add("STribeImmunity");
                } else
                {
                    ep.events.Add("TribeImmunity");
                }
                
                if (ep.exileIsland.on)
                {
                    ep.events.Add("ExileI");
                }
                if(ep.Event.type.Contains("MultiTribal"))
                {
                    float a = 0;
                    if(ep.Event.elim > 0)
                    {
                        a = curTeams - ep.Event.elim;
                    }
                    curCon -= curTeams - 1 - ep.Event.elim;
                } else
                {
                    ep.events.Add("TribalCouncil");
                    ep.events.Add("ShowVotes");
                }
                
                if(ep.Event.type.Contains("DoubleElim"))
                {
                    ep.events.Add("TribalCouncil");
                    ep.events.Add("ShowVotes");
                    curCon--;
                }
            }
            else if (curCon == mergeAt)
            {
                mergeRound = i + 1;
                ep.merged = true;
                if (sea.ExileIslandd)
                {
                    bool skip = false;
                    foreach (int num in sea.Twists.epsSkip)
                    {
                        if (num == i + 1)
                        {
                            skip = true;
                        }
                    }
                    if (i + 1 < sea.Twists.expireAt && skip == false)
                    {

                        ep.exileIsland = sea.Twists.MergeEIsland;
                        ep.exileIsland.on = true;
                    }
                    foreach(int num in sea.Twists.epsSpecialE)
                    {
                        if(num == i + 1 && skip == false)
                        {
                            Debug.Log(i + 1);
                            ep.exileIsland = sea.Twists.SpecialEx[curSE];
                            curSE++;
                            ep.exileIsland.on = true;
                        }
                    }
                }
                foreach (OneTimeEvent timeEvent in sea.oneTimeEvents)
                {
                    if (i + 1 == timeEvent.round)
                    {
                        ep.Event = timeEvent;
                    }
                }
                ep.events.Add("NextEp");
                if (sea.RedemptionIsland)
                {
                    ep.events.Add("RedemptionIsland");
                }
                if(sea.EdgeOfExtinction)
                {
                    ep.events.Add("EOEReturnChallenge");
                }
                ep.events.Add("MergeTribes");
                ep.events.Add("MergeStatus");
                if(ep.Event.type == "MergeSplit")
                {
                    ep.events.Add("STribeImmunity");
                }
                else
                {
                    ep.events.Add("MergeImmunity");
                }
                if (ep.exileIsland.on)
                {
                    ep.events.Add("ExileI");
                }
                ep.events.Add("TribalCouncil");
                ep.events.Add("ShowVotes");
                if (ep.Event.type.Contains("DoubleElim"))
                {
                    if (ep.Event.type.Contains("Immunity"))
                    {
                        ep.events.Add("MergeImmunity");
                    }
                    ep.events.Add("TribalCouncil");
                    ep.events.Add("ShowVotes");
                    curCon--;
                }
                if (sea.Outcasts && !oc)
                {
                    curCon++;
                    ep = new EpisodeSetting();
                    ep.events.Add("NextEp");
                    ep.events.Add("TribeStatus");
                    ep.events.Add("OutcastsImmunity");
                    oc = true;
                }

                
            }
            else if (curCon < mergeAt)
            {
                ep.merged = true;
                if (sea.ExileIslandd)
                {
                    bool skip = false;
                    foreach (int num in sea.Twists.epsSkip)
                    {
                        if (num == i + 1)
                        {
                            skip = true;
                        }
                    }
                    if (i + 1 < sea.Twists.expireAt && skip == false)
                    {

                        ep.exileIsland = sea.Twists.MergeEIsland;
                        ep.exileIsland.on = true;
                    }
                    foreach (int num in sea.Twists.epsSpecialE)
                    {
                        if (num == i + 1 && skip == false)
                        {
                            ep.exileIsland = sea.Twists.SpecialEx[curSE];
                            curSE++;
                            ep.exileIsland.on = true;
                        }
                    }
                }
                foreach (OneTimeEvent timeEvent in sea.oneTimeEvents)
                {
                    if (i + 1 == timeEvent.round)
                    {
                        ep.Event = timeEvent;
                    }
                }
                ep.events.Add("NextEpM");
                if(sea.RedemptionIsland && i + 1 > mergeRound + 1 && curCon == 3)
                {
                    if (!riE)
                    {
                        ep.events.Add("RedemptionIsland");
                        ep.events.Add("NextEpM");
                        riE = true;
                    }
                }
                if (sea.EdgeOfExtinction && i + 1 > mergeRound && curCon == 4)
                {
                    if (!riE)
                    {
                        ep.events.Add("EOEReturnChallenge");
                        ep.events.Add("NextEpM");
                        riE = true;
                    }
                }
                ep.events.Add("MergeStatus");
                if (sea.RedemptionIsland && i + 1 > mergeRound + 1 && curCon > 3)
                {
                    bool skip = false;
                    foreach (int num in sea.Twists.epsSkipRI)
                    {
                        if (i + 1 == num)
                        {
                            skip = true;
                        }
                    }
                    if (!skip)
                    {
                        foreach (int num in sea.Twists.epsSpecialRI)
                        {
                            if (i + 1 == num)
                            {
                                ep.elimAllButTwo = true;
                            }
                        }
                        ep.events.Add("RedemptionIsland");
                    }
                }
                if(sea.EdgeOfExtinction && i + 1 > mergeRound && curCon > 4)
                {
                    ep.events.Add("EOEStatus");
                }
                if (ep.Event.type == "MergeSplit")
                {
                    ep.events.Add("STribeImmunity");
                }
                else
                {
                    ep.events.Add("MergeImmunity");
                }
                if (ep.exileIsland.on)
                {
                    ep.events.Add("ExileI");
                }
                ep.events.Add("TribalCouncil");
                ep.events.Add("ShowVotes");
                if (ep.Event.type.Contains("DoubleElim"))
                {
                    if (ep.Event.context == "Immunity")
                    {
                        ep.events.Add("MergeImmunity");
                    }
                    ep.events.Add("TribalCouncil");
                    ep.events.Add("ShowVotes");
                    curCon--;
                }
            }
            Episodes.Add(ep);
            Episode epi = new Episode();
            currentSeason.Episodes.Add(epi);
            curCon--;
        }
        EpisodeSetting epp = new EpisodeSetting();
        Episode epii = new Episode();
        currentSeason.Episodes.Add(epii);
        epp.name = "The Reunion";
        epp.events.Add("NextEpM");
        //epp.events.Add("Reunion");
        epp.events.Add("WinnerReveal");
        epp.events.Add("ShowVotes");
        epp.events.Add("FanFavorite");
        epp.events.Add("Placements");
        Episodes.Add(epp);
    }
    public void NextEvent()
    {
        if (curEp >= Episodes.Count)
        {
            return;
        }
        else
        {
            //NextEventt();
        }
        if (curEv == 0)
        {
            if(Episodes[curEp].swap.on)
            {
                curSwap = Episodes[curEp].swap;
            } else 
            {
                curSwap = new TribeSwap();
            }
            if (Episodes[curEp].exileIsland.on)
            {
                curExile = Episodes[curEp].exileIsland;
                
            } else 
            {
                curExile = new Exile();
            } 
            if(Episodes[curEp].Event.type != "")
            {
                curEvent = Episodes[curEp].Event;
            } else
            {
                curEvent = new OneTimeEvent();
            }
            if (Exiled.Count > 0)
            {
                foreach (Contestant num in Exiled)
                {
                    if(Episodes[curEp].merged && !Episodes[curEp].events.Contains("MergeTribes"))
                    {
                        if (MergedTribe.name == num.team)
                        {
                            if(!num.teams.Contains(MergedTribe.tribeColor))
                            {
                                num.teams.Add(MergedTribe.tribeColor);
                            }
                            num.team = "";
                            MergedTribe.members.Add(num);
                        }
                    } else
                    {
                        foreach (Team tribe in Tribes)
                        {
                            if (tribe.name == num.team)
                            {
                                if(Episodes[curEp-1].swap.exile)
                                {
                                    num.teams.Add(tribe.tribeColor);
                                }
                                num.team = "";
                                tribe.members.Add(num);
                            }
                        }
                    }
                }
                Exiled = new List<Contestant>();
            }
            tie = new List<Contestant>();
            LosingTribes = new List<Team>();
            tri = 0;
            curTTT = 0;
        }

        Invoke(Episodes[curEp].events[curEv], 0);
        curEv++;
        if (curEv >= Episodes[curEp].events.Count)
        {
            curEp++;
            curEv = 0;
        }
    }
    void NextEventt()
    {
        NextEvent();
    }
    void MOPChallenge()
    {
        GameObject EpisodeImm = Instantiate(Prefabs[2]);
        EpisodeImm.transform.parent = Canvas.transform;
        EpisodeImm.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeImm.GetComponent<RectTransform>().offsetMax.y);
        EpisodeImm.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeImm.GetComponent<RectTransform>().offsetMin.x, 0);
        AddGM(EpisodeImm, true);
        int ran = Random.Range(0, Tribes.Count);
        MOP = Tribes[ran].name;
        int ran2 = Random.Range(0, Tribes[ran].members.Count);
        GameObject group = Instantiate(GroupPrefab);
        GameObject mem = Instantiate(ContestantPrefab);
        mem.GetComponentInChildren<Image>().sprite = Tribes[ran].members[ran2].image;
        mem.GetComponentInChildren<Text>().text = "";
        mem.transform.parent = group.transform.GetChild(2);
        group.GetComponent<UIGroup>().eventText.text = Tribes[ran].members[ran2].nickname + " finds the Medallion of Power for \n \n" + Tribes[ran].name + " will have a challenge advantage if they keep the Medallion. \n \n They can give up the Medallion for fire and fishing gear.";
        int ran3 = Random.Range(0, 2);
        if(ran == 1)
        {
            List<Team> TribesV = new List<Team>(Tribes);
            TribesV.Remove(Tribes[ran]);
            int ran4 = Random.Range(0, TribesV.Count);
            MOP = Tribes[ran4].name;
            group.GetComponent<UIGroup>().eventText.text += "\n \n " + Tribes[ran].name + " gives up the Medallion and gets the fire and fishing gear. \n \n " + TribesV[ran4].name + " gets the Medallion of Power.";
        } else
        {
            List<Team> TribesV = new List<Team>(Tribes);
            TribesV.Remove(Tribes[ran]);
            int ran4 = Random.Range(0, TribesV.Count);
            group.GetComponent<UIGroup>().eventText.text += "\n \n " + Tribes[ran].name + " keeps the Medallion. \n \n " + TribesV[ran4].name + " gets the fire and fishing gear.";
        }
        group.transform.parent = EpisodeImm.transform.GetChild(0);
        NextEvent();
    }
    void TribeStatus()
    {
        for (int i = 0; i < Tribes.Count; i++)
        {
            TribeStatuss();
        }
        NextEvent();
    }
    public void NextGM()
    {
        if(curEv == 0)
        {
            curTribal = 0;
            curGroup = 0;
            currentContestants--;
        }
        if (Vote.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("voteRevealed") || tri > 0)
        {
            Vote.GetComponent<Animator>().SetTrigger("Reveal");
            for (int i = 0; i < VotedOffCine.transform.GetChild(0).childCount; i++)
            {
                Destroy(VotedOffCine.transform.GetChild(0).GetChild(i).transform.gameObject);
            }
            foreach (GameObject torch in Torches)
            {
                torch.SetActive(false);
            }
            VotedOffCine.SetActive(false);
            VotedOffCine.transform.parent.gameObject.SetActive(false);
        }
        if(curEv - 1 != -1)
        {
            currentSeason.Episodes[curEp].events[curEv-1].obj.SetActive(false);
        } else
        {
            if(curEp != 0)
            currentSeason.Episodes[curEp - 1].events[currentSeason.Episodes[curEp - 1].events.Count - 1].obj.SetActive(false);
        }
        if(currentSeason.Episodes[curEp].events[curEv].obj.name.Contains("Returning Duel") || currentSeason.Episodes[curEp].events[curEv].obj.name.Contains("Return Challenge"))
        {
            currentContestants++;
        }
        if (currentSeason.Episodes[curEp].events[curEv].obj.name.Contains("OutcastsImmunity"))
        {
            currentContestants += re;
        }
        currentSeason.Episodes[curEp].events[curEv].obj.SetActive(true);
        if(currentSeason.Episodes[curEp].events[curEv].obj.name.Contains("Tribal Council") && cineTribal)
        {
            int numb = currentSeason.Episodes[curEp].events.Count - 1;
            if (VotedOffCine.transform.GetChild(0).childCount > 0)
            {
                for (int i = 0; i < VotedOffCine.transform.GetChild(0).childCount; i++)
                {
                    Destroy(VotedOffCine.transform.GetChild(0).GetChild(i).transform.gameObject);
                }
            }
            votes = currentSeason.Episodes[curEp].events[curEv].Vote;
            votesRead = currentSeason.Episodes[curEp].events[curEv].VotesRead;
            foreach (GameObject torch in Torches)
            {
                torch.SetActive(true);
            }
            nextButton.gameObject.SetActive(false);
            VoteButton.SetActive(true);
            lastVoteOff = currentSeason.Episodes[curEp].events[curEv].obj.transform.GetChild(0).GetChild(0).gameObject;
            //Destroy(VotedOffCine.transform.GetChild(0).gameObject);
            /*
            if(curTribal == currentSeason.Episodes[curEp].votes.Count - 1)
            {
                for (int i = 0; i < currentSeason.Episodes[curEp].finalVote.Count; i++)
                {
                    //GameObject gas = Instantiate();
                    currentSeason.Episodes[curEp].finalVote[i].transform.parent = VotedOffCine.transform.GetChild(0);
                }
                e = true;
            } else
            {
                e = false;
            } */
            if(currentSeason.Episodes[curEp].events[curEv].VoteObjs.Count > 0)
            {
                foreach(GameObject vot in currentSeason.Episodes[curEp].events[curEv].VoteObjs)
                {
                    vot.transform.parent = VotedOffCine.transform.GetChild(0);
                }
            }
            curVot = 0;
            
            if (votes.Count > 0)
            {
                dic = new Dictionary<Contestant, int>();
                votedOff = votes[0];
                dic.Add(votes[0], 1);
                dicVR = new Dictionary<Contestant, int>();
                dicVR.Add(votesRead[0], 1);
                for (int i = 1; i < votes.Count; i++)
                {
                    if (dic.ContainsKey(votes[i]))
                    {
                        dic[votes[i]] += 1;
                        if (dic[votes[i]] > dic[votedOff])
                        {
                            votedOff = votes[i];
                        }
                    }
                    else if (!dic.ContainsKey(votes[i]))
                    {
                        dic.Add(votes[i], 1);
                    }
                }
                tie = new List<Contestant>();
                float maxValue = dic.Values.Max();
                foreach (KeyValuePair<Contestant, int> num in dic)
                {
                    if (num.Value == maxValue)
                    {
                        tie.Add(num.Key);
                    }
                    else
                    {

                    }
                }
                int re = 0;
                if(curTribal < currentSeason.Episodes[curEp].votes.Count - 1)
                {
                    foreach (Contestant num in currentSeason.Episodes[curEp].votes[curTribal + 1])
                    {
                        if(tie.Contains(num))
                        {
                            re++;
                        }
                    }
                    if(re == currentSeason.Episodes[curEp].votes[curTribal + 1].Count)
                    {
                        revoteNext = true;
                    }
                }
            }
        }
        curEv++;
        if (curEv >= currentSeason.Episodes[curEp].events.Count)
        {
            curEp++;
            curEv = 0;
        }
    }
    public void AddGM(GameObject gm, bool add)
    {
        Page page = new Page();
        page.obj = gm;
        currentSeason.Episodes[curEpp].events.Add(page);
        if(add)
        {
            curEvv++;
        }
        if (curEvv >= Episodes[curEpp].events.Count)
        {            
            curEpp++;
            curEvv = 0;
        }
        if(curEpp >= currentSeason.Episodes.Count)
        {
            curEpp = currentSeason.Episodes.Count - 1;
        }
    }
    public void AddVote(List<Contestant> gm, List<Contestant> gmm)
    {
        //Debug.Log(curEpp);
        int num = currentSeason.Episodes[curEpp].events.Count - 1;
        currentSeason.Episodes[curEpp].events[num].Vote = gm;
        currentSeason.Episodes[curEpp].events[num].VotesRead = gmm;
    }
    public void AddFinalVote(GameObject og)
    {
        int num = currentSeason.Episodes[curEpp].events.Count - 1;
        currentSeason.Episodes[curEpp].events[num].VoteObjs.Add(og);
    }
    void NextEp()
    {
        List<Alliance> remove = new List<Alliance>();
        GameObject EpisodeStart = Instantiate(Prefabs[0]);
        EpisodeStart.transform.parent = Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        AddGM(EpisodeStart, true);
        for(int i = 0; i < Tribes.Count; i++)
        {
            //Debug.Log(tribe.name + ":" + string.Join(", ", tribe.members.ConvertAll(i => i.nickname)));
            MakeGroup(true, Tribes[i], "name", "", "", Tribes[i].members, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            foreach (Contestant num in Tribes[i].members)
            {
                num.altVotes = new List<Contestant>();
            }
        }
        remove = new List<Alliance>();
        foreach (Alliance alliance in Alliances)
        {
            if (alliance.members.Count < 2)
            {
                remove.Add(alliance);
            }
        }
        foreach (Alliance alliance in remove)
        {
            Alliances.Remove(alliance);
        }
        immune = new List<Contestant>();
        nextEvent = 1;
        NextEvent();
    }
    void TribeStatuss()
    {
        GameObject EpisodeStatus = Instantiate(Prefabs[1]);
        EpisodeStatus.transform.parent = Canvas.transform;
        EpisodeStatus.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStatus.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStatus.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStatus.GetComponent<RectTransform>().offsetMin.x, 0);
        if(curT == 0)
        {
            AddGM(EpisodeStatus, true);
        } else
        {
            AddGM(EpisodeStatus, false);
        }
        
        float ed = 0;
        foreach (Alliance alliance in Alliances)
        {
            if (alliance.teams.Contains(Tribes[curT].name))
            {
                ed++;
            }
        }
        float d = 0;
        foreach (Alliance alliance in Alliances)
        {
            if (alliance.teams.Contains(Tribes[curT].name))
            {
                float ee = 0;
                foreach (Contestant num in Tribes[curT].members)
                {
                    if (alliance.members.Contains(num))
                    {
                        ee++;
                    }
                }
                if (ee == 0)
                {
                    alliance.teams.Remove(Tribes[curT].name);
                    
                } 
            }
            
            if (alliance.teams.Contains(Tribes[curT].name))
            {
                MakeGroup(false, null, "name", alliance.name, "", alliance.members, EpisodeStatus.transform.GetChild(0), 0);
                if(ed < 2)
                {
                    EpisodeStatus.transform.GetChild(0).GetComponent<VerticalLayoutGroup>().spacing = -90;
                }
                
                d++;
            }
            //Debug.Log(alliance.name + ":" + string.Join(",  ", alliance.members.ConvertAll(i => i.nickname)));
        }
        if (d == 0)
        {
            /*
            GameObject allianceGO = Instantiate(GroupPrefab);
            allianceGO.GetComponent<UIGroup>().allianceText.text = "There are no alliances";
            Destroy(allianceGO.GetComponent<UIGroup>().tribeName.gameObject);
            allianceGO.GetComponent<SetupLayout>().Start();
            allianceGO.transform.parent = EpisodeStatus.transform.GetChild(0);
            float teamWidth = ConListWidth(allianceGO.transform.GetChild(2).childCount);
            allianceGO.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(teamWidth, allianceGO.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta.y);
            allianceGO.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 1f); */
            //EpisodeStatus.transform.parent.GetComponent<VerticalLayoutGroup>().spacing = -145; 
            MakeGroup(false, null, "", "There are no alliances", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0), 0);
        }
        lastThing = EpisodeStatus;
        if(curT == (Tribes.Count + ere) - 1)
        {
            curT = 0;
            tri = 0;
        } else
        {
            curT++;
        }
    }
    void TribeReward()
    {
        GameObject EpisodeRe = Instantiate(Prefabs[2]);
        EpisodeRe.transform.parent = Canvas.transform;
        EpisodeRe.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeRe.GetComponent<RectTransform>().offsetMax.y);
        EpisodeRe.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeRe.GetComponent<RectTransform>().offsetMin.x, 0);
        AddGM(EpisodeRe, true);
        List<Team> LosingTeams = new List<Team>();
        List<Team> TribesV = new List<Team>(Tribes);
        int ran = Random.Range(0, TribesV.Count);
        LosingTeams.Add(TribesV[ran]);
        //lastThing.SetActive(false);

        foreach (Team tribe in Tribes)
        {
            if (!LosingTeams.Contains(tribe))
            {
                MakeGroup(false, null, "", "", tribe.name + " Wins Reward!", new List<Contestant>(), EpisodeRe.transform.GetChild(0), 0);
            }
        }
    }
    void TribeImmunity()
    {
        GameObject EpisodeImm = Instantiate(Prefabs[2]);
        EpisodeImm.transform.parent = Canvas.transform;
        EpisodeImm.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeImm.GetComponent<RectTransform>().offsetMax.y);
        EpisodeImm.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeImm.GetComponent<RectTransform>().offsetMin.x, 0);
        AddGM(EpisodeImm, true);
        if (kidnapped != null)
        {
            for (int i = 0; i < Tribes.Count; i++)
            {
                if (Tribes[i].members.Contains(kidnapped))
                {
                    Tribes[i].members.Remove(kidnapped);
                }
                if (Tribes[i].name == kidnapped.team)
                {
                    Tribes[i].members.Add(kidnapped);
                }
            }
            List<Contestant> g = new List<Contestant>() { kidnapped };
            MakeGroup(false, null, "", "", kidnapped.nickname + " returns to their tribe.", g, EpisodeImm.transform.GetChild(0), 20);
            kidnapped = null;
        }
        LosingTribes = new List<Team>();
        List<Team> TribesV = new List<Team>(Tribes);
        if(curEp +1 == sea.MOPExpire)
        {
            MOPExpired = true;
            MakeGroup(false, null, "", "", "The Medallion of Power is no longer usable.", new List<Contestant>(), EpisodeImm.transform.GetChild(0), 0);
        }
        if (sea.MedallionOfPower && !MOPExpired)
        {
            string etext;
            int ran2 = Random.Range(0, 2);
            if(ran2 == 1)
            {
                
                int ran3 = Random.Range(0, 4);
                Team user = new Team();
                foreach (Team tribe in TribesV)
                {
                    if (tribe.name == MOP)
                    {
                        user = tribe;
                    }
                }
                if (ran3 == 3)
                {
                    TribesV.Remove(user);
                }
                etext = MOP + " uses the Medallion of Power.";
                List<Team> TribesVV = new List<Team>(Tribes);
                TribesVV.Remove(user);
                int ran4 = Random.Range(0, TribesVV.Count);
                MOP = TribesVV[ran4].name;
                etext += "\n \n" + MOP + " gets the Medallion of Power.";
            } else
            {
                etext = MOP + " doesn't use the Medallion of Power.";
            }
            MakeGroup(false, null, "", "", etext, new List<Contestant>(), EpisodeImm.transform.GetChild(0), 0);
        }
        int ran = Random.Range(0, TribesV.Count);
        LosingTribes.Add(TribesV[ran]);
        
        foreach (Team tribe in Tribes)
        {
            if (!LosingTribes.Contains(tribe))
            {
                MakeGroup(false, tribe, "", "", tribe.name + " Wins Immunity!", tribe.members, EpisodeImm.transform.GetChild(0), 20);
            }
        }
        lastThing = EpisodeImm;
        if(curExile.on && curExile.challenge == "Immunity")
        {
            if (!curExile.on && !curSwap.on)
            {

            }
            else if (curExile.on && !curSwap.on)
            {
                if (curExile.reason == "Winner" || curExile.reason == "Loser")
                {
                    string reason;
                    string reason2;
                    if(curExile.ownTribe)
                    {
                        List<Team> teams = new List<Team>(Tribes);
                        if(curExile.reason == "Winner")
                        {
                            foreach (Team t in LosingTribes)
                            {
                                teams.Remove(t);
                            }
                            reason2 = "The winning team can send someone from their tribe to exile island.";
                            reason = " is sent to exile by the winning team.";
                        } else
                        {
                            foreach (Team t in LosingTribes)
                            {
                                if(t != LosingTribes[0])
                                {
                                    teams.Remove(t);
                                }
                            }
                            reason2 = "The losing tribe can send someone from their tribe to exile";
                            reason = " is sent to exile by the losing team.";
                        }
                        int rann = Random.Range(0, teams[0].members.Count);
                        teams[0].members[rann].team = teams[0].name;
                        Exiled.Add(teams[0].members[rann]);
                        List<Contestant> g = new List<Contestant>() { teams[0].members[rann] };
                        MakeGroup(false, null, "", reason2, teams[0].members[rann].nickname + reason, g, EpisodeImm.transform.GetChild(0), 20);
                        teams[0].members.Remove(teams[0].members[rann]);
                        
                        teams = new List<Team>(Tribes);
                        if (curExile.two)
                        {
                            if (curExile.reason == "Winner")
                            {
                                foreach (Team t in Tribes)
                                {
                                    if (t != LosingTribes[0])
                                    {
                                        teams.Remove(t);
                                    }
                                }
                                reason2 = Exiled[0].nickname + " can send someone from the losing tribe to exile";
                                reason = " is sent to exile by " + Exiled[0].nickname;
                            }
                            else
                            {
                                foreach (Team t in LosingTribes)
                                {
                                    teams.Remove(t);
                                }
                                reason2 = Exiled[0].nickname + " can send someone from the winning tribe to exile";
                                reason = " is sent to exile by " + Exiled[0].nickname;
                            }
                            int rannn = Random.Range(0, teams[0].members.Count);
                            teams[0].members[rannn].team = teams[0].name;
                            Exiled.Add(teams[0].members[rannn]);
                            g = new List<Contestant>() { teams[0].members[rannn] };
                            MakeGroup(false, null, "", reason2, teams[0].members[rannn].nickname + reason, g, EpisodeImm.transform.GetChild(0), 20);
                            teams[0].members.Remove(teams[0].members[rannn]);
                        }
                    } else
                    {
                        List<Team> teams = new List<Team>(Tribes);
                        if (curExile.reason == "Winner")
                        {
                            foreach (Team t in Tribes)
                            {
                                if (t != LosingTribes[0])
                                {
                                    teams.Remove(t);
                                }
                            }
                            reason2 = "The winning team can send someone from the losing tribe to exile";
                            reason = " is sent to exile by the winning team.";
                        }
                        else
                        {
                            foreach (Team t in LosingTribes)
                            {
                                teams.Remove(t);
                            }
                            reason2 = "The losing team can send someone from the winning tribe to exile";
                            reason = " is sent to exile by the losing team.";
                        }
                        int rann = Random.Range(0, teams[0].members.Count);
                        teams[0].members[rann].team = teams[0].name;
                        Exiled.Add(teams[0].members[rann]);
                        List<Contestant> g = new List<Contestant>() { teams[0].members[rann] };
                        MakeGroup(false, null, "", reason2, teams[0].members[rann].nickname + reason, g, EpisodeImm.transform.GetChild(0), 20);
                        teams[0].members.Remove(teams[0].members[rann]);
                        teams = new List<Team>(Tribes);
                        if (curExile.two)
                        {
                            if (curExile.reason == "Winner")
                            {
                                foreach (Team t in LosingTribes)
                                {
                                    teams.Remove(t);
                                }
                                reason2 = Exiled[0].nickname + " can send someone from the winning tribe to exile";
                                reason = " is sent to exile by " + Exiled[0].nickname;
                            }
                            else
                            {
                                foreach (Team t in LosingTribes)
                                {
                                    if (t != LosingTribes[0])
                                    {
                                        teams.Remove(t);
                                    }
                                }
                                reason2 = Exiled[0].nickname + " can send someone from the losing tribe to exile";
                                reason = " is sent to exile by " + Exiled[0].nickname;
                            }
                            int rannn = Random.Range(0, LosingTribes[LosingTribes.Count - 1].members.Count);
                            LosingTribes[LosingTribes.Count - 1].members[rannn].team = LosingTribes[LosingTribes.Count - 1].name;
                            Exiled.Add(LosingTribes[LosingTribes.Count - 1].members[rannn]);
                            g = new List<Contestant>() { teams[0].members[rannn] };
                            MakeGroup(false, null, "", reason2, teams[0].members[rannn].nickname + reason, g, EpisodeImm.transform.GetChild(0), 20);
                            LosingTribes[LosingTribes.Count - 1].members.Remove(LosingTribes[LosingTribes.Count - 1].members[rann]);
                        }
                    }
                } else
                {
                    
                }
            }
            else if (curExile.on && curSwap.on)
            {
                Exiled[0].team = LosingTribes[LosingTribes.Count - 1].name;
            } 
        }
        if(curEvent.type == "KidnappingImmunity")
        {
            Team t = LosingTribes[Random.Range(0, LosingTribes.Count)];
            kidnapped = t.members[Random.Range(0, t.members.Count)];
            foreach (Team tt in Tribes)
            {
                if(tt.members.Contains(kidnapped))
                {
                    kidnapped.team = tt.name;
                }
            }
            t.members.Remove(kidnapped);
            List<Team> teams = new List<Team>(Tribes);
            foreach (Team tt in LosingTribes)
            {
                teams.Remove(tt);
            }
            teams[0].members.Add(kidnapped);
            Debug.Log("gg");
            List<Contestant> g = new List<Contestant>() { kidnapped };
            MakeGroup(false, null, "", teams[0].name + " can kidnap someone from the losing tribe.", kidnapped.nickname + " is kidnapped.", g, EpisodeImm.transform.GetChild(0), 20);
        } else
        {
            MakeGroup(false, null, "", "", LosingTribes[LosingTribes.Count -1].name + "  receive a message in a bottle to open after tribal council.", new List<Contestant>(), EpisodeImm.transform.GetChild(0), 0);
        }
        nextEvent += 1;
        NextEvent();
    }
    void TribalCouncil()
    {
        
        if (MergedTribe.members.Count == 3)
        {
            Final3Tribal(MergedTribe);
        }else if(MergedTribe.members.Count > 3)
        {
            if(curEvent.type == "MergeSplit")
            {
                Tribal(LosingTribes[curTTT]);
                curTTT++;
            } else
            {
                Tribal(MergedTribe);
            }
        } else
        {
            if (curTTT > LosingTribes.Count || curTTT < 0)
            {
                Debug.Log(curTTT);
            }
            Tribal(LosingTribes[curTTT]);
            curTTT++;
        }
        if(curEvent.type == "DoubleElim")
        {
            curTTT = 0;
        }
        NextEvent();
    }
    void Tribal(Team team)
    {
        bool what = false;
        string conPlacement = "";
        string vote = "";
        string juror = "";
        GameObject EpisodeStart = Instantiate(Prefabs[0]);
        EpisodeStart.transform.parent = Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStart.name = "Tribal Council";
        lastThing = EpisodeStart;
        AddGM(EpisodeStart, true);
        if(MergedTribe.members.Count < 1)
        {
            if (team != LosingTribes[0] && curEvent.context == "TribalKidnap")
            {
                kidnapped = team.members[Random.Range(0, team.members.Count)];
                foreach (Team tt in Tribes)
                {
                    if (tt.members.Contains(kidnapped))
                    {
                        kidnapped.team = tt.name;
                    }
                }
                team.members.Remove(kidnapped);
                List<Team> teams = new List<Team>(Tribes);
                foreach (Team tt in LosingTribes)
                {
                    teams.Remove(tt);
                }
                Tribes[Tribes.IndexOf(LosingTribes[0])].members.Add(kidnapped);
                List<Contestant> g = new List<Contestant>() { kidnapped };
                MakeGroup(false, team, "", "Before the vote, " + LosingTribes[0].name + " must kidnap one member of " + team.name + ". \n \n This person will skip tribal.", LosingTribes[0].name + " kidnaps " + kidnapped.nickname + ".", g, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
            } else if (team != LosingTribes[0] && curEvent.context == "TribalImmBV")
            {
                immune.Add(team.members[Random.Range(0, team.members.Count)]);
                List<Contestant> g = new List<Contestant>() { immune[immune.Count - 1], immune[0] };
                List<Contestant> w = new List<Contestant>() { immune[0] };

                MakeGroup(false, team, "", w[0].nickname + " has to grant one player on the tribe immunity.", "", w, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                MakeGroup(false, team, "", "", immune[0].nickname + " grants immunity to " + g[0].nickname, g, EpisodeStart.transform.GetChild(0).GetChild(0), 15);

            }
        }
        
        LosingTribe = team;
        lastVoteOff = EpisodeStart.transform.GetChild(0).GetChild(0).gameObject;
        string etext = "";
        if(team.members.Count > 2)
        {
            etext = "It's time to vote. \n \n I'll read the votes.";
            MakeGroup(true, team, "name", "", etext, team.members, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
            bool voted = Voting();
            if (voted)
            {
                CountVotes();

                AddVote(votes, votesRead);
                tie = new List<Contestant>();
                float maxValue = dic.Values.Max();
                foreach (KeyValuePair<Contestant, int> num in dic)
                {
                    if (num.Value == maxValue)
                    {
                        tie.Add(num.Key);
                        num.Key.inTie = true;
                    }
                    else
                    {

                    }
                }
            }
            else if (!voted)
            {

                Tribal(team);
            }
            if (tie.Count < 2)
            {
                //Debug.Log(votedOff.nickname + " has been eliminated. " + "Votes: " + dic[votedOff]);
                Eliminate();
            }
            else
            {
                tri++;
                if (cineTribal == true)
                {
                    GameObject EpisodeStartt = Instantiate(Prefabs[0]);
                    EpisodeStartt.transform.parent = Canvas.transform;
                    EpisodeStartt.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStartt.GetComponent<RectTransform>().offsetMax.y);
                    EpisodeStartt.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStartt.GetComponent<RectTransform>().offsetMin.x, 0);
                    EpisodeStartt.name = "Tribal Council";
                    lastThing = EpisodeStart;
                    what = true;
                    AddGM(EpisodeStartt, false);
                }
                bool rev = Revote(tie);
                if (rev)
                {
                    CountVotes();
                    //curEpp--;
                    AddVote(votes, votesRead);

                    float maxxValue = dic.Values.Max();
                    if (tie.Count < 2)
                    {
                        //Debug.Log(votedOff.nickname + " has been eliminated. " + "Votes: " + dic[votedOff]);
                        Eliminate();
                    }
                    else
                    {
                        if (MergedTribe.members.Count == 4)
                        {
                            Tiebreaker(tie, "FireChallenge");
                        }
                        else
                        {
                            Tiebreaker(tie, "Rocks");
                        }
                    }
                    tri = 0;
                }
            }
        }
        else if(team.members.Count == 2)
        {
            votes = new List<Contestant>();
            votesRead = new List<Contestant>();
            //Debug.Log("gg");
            etext = "A fire-making challenge will occur since there are only two castaways left.";
            MakeGroup(true, team, "name", "", etext, team.members, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
            Tiebreaker(team.members, "FireChallenge");
            votes.Add(votedOff);
            votesRead.Add(votedOff);
            AddVote(votes, votesRead);
        }
        else if(team.members.Count == 1)
        {
            //Debug.Log("kk");
            votes = new List<Contestant>();
            votesRead = new List<Contestant>();
            votedOff = team.members[0];
            votes.Add(votedOff);
            votesRead.Add(votedOff);
            AddVote(votes, votesRead);
            etext = "Sorry, " + votedOff.nickname + ", you're the only castaway left so you're automatically eliminated.";
            MakeGroup(true, team, "name", "", etext, team.members, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
            Eliminate();
        }
        
        //Voting
        //Determine who was voted out
        bool Voting()
        {
            float endVote = 0;
            targets = new List<Contestant>();
            foreach (Alliance all in Alliances)
            {
                if(all.teams.Contains(team.name))
                {
                    if (team.members.Count > all.members.Count)
                    {
                        Contestant target;
                        List<Contestant> teamV = new List<Contestant>(team.members);
                        foreach (Contestant num in all.members)
                        {
                            teamV.Remove(num);
                        }
                        if (immune != null)
                        {
                            foreach (Contestant num in immune)
                            {
                                teamV.Remove(num);
                            }
                        }
                        if (teamV.Count > 0)
                        {
                            int ran = Random.Range(0, teamV.Count);
                            target = teamV[ran];
                            all.target = target;
                        }

                    }
                    else
                    {

                    }
                }
                
            }
            for (int i = 0; i < team.members.Count; i++)
            {
                team.members[i].voteReason = "They voted based on personal preference.";
                List<Contestant> teamV = new List<Contestant>(team.members);
                teamV.Remove(team.members[i]);
                if (immune != null)
                {
                    foreach (Contestant num in immune)
                    {
                        if(team.members.Contains(num))
                        teamV.Remove(num);
                    }
                }
                int ran = Random.Range(0, teamV.Count);
                team.members[i].vote = teamV[ran];
                for (int j = 0; j < Alliances.Count; j++)
                {
                    if(Alliances[j].teams.Contains(team.name))
                    {
                        if (team.members.Count > Alliances[j].members.Count)
                        {
                            if (Alliances[j].members.Contains(team.members[i]) && team.members.Contains(Alliances[j].target))
                            {
                                team.members[i].vote = Alliances[j].target;
                            }
                        }
                    }
                }
                if (team.members[i].vote != team.members[i])
                {
                    endVote++;
                }
                else if (team.members[i].vote == team.members[i])
                {

                }
            } 

            if (endVote == team.members.Count)
            {
                return true;
            }
            else
            {
                Debug.Log(endVote + "Episode:" + curEp);
                return false;
            } 
        }
        bool Revote(List<Contestant> tie)
        {
            /*Debug.Log("Tie:");
            foreach (Contestant num in tie)
            {
                Debug.Log(num.nickname + " Votes: " + dic[num]);
            } */
            float endVote = 0;
            
            foreach(Alliance alliance in Alliances)
            {
                if(alliance.teams.Contains(team.name))
                {
                    if (team.members.Count > alliance.members.Count)
                    {
                        Contestant target;
                        List<Contestant> tieV = new List<Contestant>(tie);
                        alliance.altTargets = new List<Contestant>();
                        foreach (Contestant num in alliance.members)
                        {
                            if (tieV.Contains(num))
                            {
                                tieV.Remove(num);
                            }
                        }
                        if (tieV.Count > 0)
                        {
                            int ran = Random.Range(0, tieV.Count);
                            target = tieV[ran];
                            alliance.altTargets.Add(target);
                        }
                    }
                    else
                    {

                    }
                }
            }

            for (int i = 0; i < team.members.Count; i++)
            {
                if (tie.Count < team.members.Count)
                {
                    if (!tie.Contains(team.members[i]))
                    {
                        int ran = Random.Range(0, tie.Count);
                        Contestant rvote = tie[ran];
                        for (int j = 0; j < Alliances.Count; j++)
                        {
                            if(Alliances[j].teams.Contains(team.name))
                            {
                                if (team.members.Count > Alliances[j].members.Count)
                                {
                                    if (Alliances[j].members.Contains(team.members[i]))
                                    {
                                        if (tie.Contains(Alliances[j].target))
                                        {
                                            rvote = Alliances[j].target;
                                        }
                                        else
                                        {
                                            if (Alliances[j].altTargets.Count > 0)
                                            {
                                                rvote = Alliances[j].altTargets[0];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        team.members[i].altVotes.Add(rvote);
                        
                        if (rvote != team.members[i])
                        {
                            endVote++;
                        }
                        else if (rvote == team.members[i])
                        {

                        }
                    }
                } else
                {
                    List<Contestant> tieV = new List<Contestant>(tie);
                    tieV.Remove(team.members[i]);
                    int ran = Random.Range(0, tieV.Count);
                    Contestant rvote = tieV[ran];
                    for (int j = 0; j < Alliances.Count; j++)
                    {
                        //Contestant target = tie[Random.Range(0, tie.Count)];
                        if (team.members.Count > Alliances[j].members.Count)
                        {
                            if(Alliances[j].teams.Contains(team.name))
                            {
                                if (Alliances[j].members.Contains(team.members[i]))
                                {
                                    if (tie.Contains(Alliances[j].target))
                                    {
                                        rvote = Alliances[j].target;
                                    }
                                    else
                                    {
                                        if (Alliances[j].altTargets.Count > 0)
                                        {
                                            rvote = Alliances[j].altTargets[0];
                                        }
                                    }
                                }
                            }
                            
                        }
                    }
                    team.members[i].altVotes.Add(rvote);
                    if (rvote != team.members[i])
                    {
                        endVote++;
                    }
                    else if (rvote == team.members[i])
                    {

                    }
                }
            }
            if (endVote == team.members.Count - tie.Count)
            {
                return true;
            }
            else
            {
                if(team.members.Count - tie.Count == 0)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
        }
        void CountVotes()
        {
            votes = new List<Contestant>();
            votesRead = new List<Contestant>();
            e = false;
            foreach (Contestant num in team.members)
            {
                if(num.vote != null)
                {
                    if (tie.Count > 1)
                    {
                        if (team.members.Count > tie.Count)
                        {
                            if (!tie.Contains(num))
                            {
                                //Debug.Log("tri:" + ((int)tri - 1));
                                votes.Add(num.altVotes[(int)tri - 1]);
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            votes.Add(num.altVotes[(int)tri - 1]);
                        }
                        e = true;
                    }
                    else
                    {
                        votes.Add(num.vote);
                    }
                }
            }
            dic = new Dictionary<Contestant, int>();
            votedOff = votes[0];
            dic.Add(votes[0], 1);
            for (int i = 1; i < votes.Count; i++)
            {
                if (dic.ContainsKey(votes[i]))
                {
                    dic[votes[i]] += 1;
                    if (dic[votes[i]] > dic[votedOff])
                    {
                        votedOff = votes[i];
                    }
                }
                else if (!dic.ContainsKey(votes[i]))
                {
                    dic.Add(votes[i], 1);
                }
            }
            List<int> vot = new List<int>(dic.Values).OrderBy(x => x).ToList();
            vot.Reverse();
            if (tie.Count > 1)
            {
                vote = vote + "\n" + string.Join("-", vot) + " Revote";
            }
            else
            {
                vote += string.Join("-", vot) + " Vote";
            }
            tie = new List<Contestant>();
            float maxValuee = dic.Values.Max();
            List<float> votesSpread = new List<float>();
            foreach (KeyValuePair<Contestant, int> num in dic)
            {
                if (num.Value == maxValuee)
                {
                    tie.Add(num.Key);
                }
                else
                {
                    votesSpread.Add(num.Value);
                }
            }
            float enoughVotes = 0;
            if(votesSpread.Count > 0)
            {
                for (int i = votes.Count - 1; i > 0; i--)
                {
                    int enough = 0;
                    foreach (int num in votesSpread)
                    {
                        if (maxValuee - 1 > num + i && enoughVotes == 0)
                        {
                            enough++;
                        }
                    }
                    if (enough == votesSpread.Count)
                    {
                        if (i > 1)
                        {
                            enoughVotes = i - 1;
                        }
                        else
                        {
                            enoughVotes = i;
                        }

                    }
                }
            }
            else
            {
                if(tie.Count < 2 )
                {
                    if (votes.Count % 2 == 0)
                    {
                        enoughVotes = (votes.Count / 2) + 1;
                    }
                    else
                    {
                        enoughVotes = Mathf.Ceil(votes.Count / 2);
                    }
                } else
                {

                }
            }
            
            //Sort votes then generate each vote for UI 
            votesRead = votes.OrderBy(go => dic[go]).ToList();
            if (tie.Count < 2 && enoughVotes < votesRead.Count)
            {
                for (int i = 0; i < enoughVotes; i++)
                {
                    votesRead.Remove(votesRead[votesRead.Count - 1]);
                }
            }
            ShuffleVotes(votesRead);
            dicVR = new Dictionary<Contestant, int>();
            dicVR.Add(votesRead[0], 1);
            string votess;
            votess = " vote ";
            string votesLeft;
            if (showVL == true)
            {
                float vl = votes.Count - 1;
                votesLeft = ". " + vl + " Votes Left";
            }
            else
            {
                votesLeft = "";
            }
            List<string> votesSoFar = new List<string>();
            foreach (KeyValuePair<Contestant, int> num in dic)
            {
                if (num.Value > 1)
                {
                    votess = " votes ";
                }
                else
                {
                    votess = " vote ";
                }
                string v = dic[num.Key] + votess + num.Key.nickname;
                votesSoFar.Add(v);
            }
            votesSoFar = votesSoFar.OrderByDescending(go => go[0]).ToList();
            finalVotes = "Final vote count was " + string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + ".";
            
            if (cineTribal == true)
            {

            } else
            {
                List<Contestant> r = new List<Contestant>() {votesRead[0] };
                MakeGroup(false, null, dicVR[votesRead[0]] + votess + votesRead[0].nickname + votesLeft, "", "", r, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                for (int i = 1; i < votesRead.Count; i++)
                {
                    string evtext = "";
                    string atext = "";
                    
                    if (dicVR.ContainsKey(votesRead[i]))
                    {
                        dicVR[votesRead[i]] += 1;
                    }
                    else if (!dicVR.ContainsKey(votesRead[i]))
                    {
                        dicVR.Add(votesRead[i], 1);
                    }
                    votess = "";
                    votesLeft = "";
                    if (showVL == true)
                    {
                        float vl = votes.Count - i - 1;
                        if (vl > 0)
                        {
                            if (vl > 1)
                            {
                                votesLeft = ". " + vl + " Votes Left";
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
                    votesSoFar = new List<string>();
                    foreach (KeyValuePair<Contestant, int> num in dicVR)
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
                    string ctext = dicVR[votesRead[0]] + votess + votesRead[0].nickname + votesLeft;
                    ctext = string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + votesLeft;
                    List<Contestant> g = new List<Contestant>() { votesRead[i] };

                    if (i == votesRead.Count - 1 && tie.Count < 2)
                    {
                        string juryPM = "";
                        if (currentContestants - finaleAt <= juryAt && !sea.RedemptionIsland && !sea.EdgeOfExtinction)
                        {
                            float juryy = jury.Count + 1;
                            juryPM = " and " + Oridinal(juryy) + " member of the jury";
                        }
                        float placement = elimed;
                        string placementt = "";
                        placementt = Oridinal(placement);
                        elimed++;
                        atext = "The " + placementt + " eliminated from " + seasonTemp.nameSeason + juryPM + " is... ";
                        
                        votesSoFar = votesSoFar.OrderByDescending(go => go[0]).ToList();
                        evtext = finalVotes;
                        ctext = votesRead[i].nickname;
                        
                        MakeGroup(false, null, "name", atext, finalVotes, g, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                    }
                    else if (i == votesRead.Count - 1 && tie.Count > 1)
                    {
                        evtext = votedOff.nickname;
                        evtext = votesRead[i].nickname;
                        MakeGroup(false, null, "nname", atext, "", g, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                        if (e == false)
                        {
                            string firstline = "There is a tie and a revote. Those in in the tie will not revote, unless no one received votes on the original vote.";
                            string secondline = "Final vote count was " + string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + ".";
                            evtext = firstline + "\n" + "\n" + "\n" + secondline;
                        } else
                        {
                            string secondline = "Final vote count was " + string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + ".";
                            evtext = secondline;
                            elimed++;
                        }
                        MakeGroup(false, null, "name", "", evtext, tie, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                    } else
                    {
                        MakeGroup(false, null, ctext, atext, evtext, g, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                    }
                }
            }
            if(MergedTribe.members.Count < 1)
            {
                if (team != LosingTribes[0] && curEvent.context == "TribalImmAV" && !e)
                {
                    Contestant immunity = team.members[Random.Range(0, team.members.Count)];
                    List<Contestant> g = new List<Contestant>() { immunity, immune[0] };
                    List<Contestant> w = new List<Contestant>() { immune[0] };

                    MakeGroup(false, team, "", "", w[0].nickname + " granted one player on the tribe immunity.", w, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                    MakeGroup(false, team, "", "", immune[0].nickname + " granted immunity to " + g[0].nickname, g, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                    Dictionary<Contestant, int> dic2 = new Dictionary<Contestant, int>(dic);
                    immune.Add(immunity);
                    if (dic2.ContainsKey(immunity))
                    {
                        dic2.Remove(immunity);
                        int maxValue = dic2.Values.Max();
                        foreach (KeyValuePair<Contestant, int> num in dic2)
                        {
                            if (num.Value == maxValue)
                            {
                                votedOff = num.Key;
                            }
                        }
                        
                        vote = vote.Replace(dic[immunity].ToString(), "<color=red>" + dic[immunity] + "*</color>");
                        dic = dic2;
                    }
                }
            }
            
        }
        void Eliminate()
        {
            foreach (Alliance alliance in Alliances)
            {
                if (alliance.members.Contains(votedOff))
                {
                    alliance.members.Remove(votedOff);
                }
            }
            //Debug.Log(currentContestants - finaleAt <= juryAt);
            if (currentContestants - finaleAt <= juryAt && !sea.RedemptionIsland && !sea.EdgeOfExtinction)
            {
                juror = "Pre-Juror";
                votedOff.placement = juror + "\n" + vote;
                if(OCExpired)
                {
                    juror = "Juror";
                    votedOff.placement = juror + "\n" + vote;
                    jury.Add(votedOff);
                }
                //Debug.Log("juror: " + jury.Count);
            } else
            {
                juror = "Pre-Juror";
                votedOff.placement = juror + "\n" + vote;
                if (sea.Outcasts && !RIExpired)
                {
                    Outcasts.members.Add(votedOff);
                }
            }
            if(sea.RedemptionIsland)
            {
                juror = "Pre-Juror";
                votedOff.placement = juror + "\n" + vote;
                if(currentContestants <= 5 && RIExpired)
                {
                    juror = "Juror";
                    votedOff.placement = juror + "\n" + vote;
                    jury.Add(votedOff);
                } else
                {
                    conPlacement = "Voted Off Ep. " + (curEp + 1);
                    votedOff.placement = conPlacement + "\n" + juror + "\n" + vote;
                    RIsland.Add(votedOff);
                }
            }
            if(sea.EdgeOfExtinction)
            {
                juror = "Pre-Juror";
                votedOff.placement = juror + vote;
                if (currentContestants <= 6 && RIExpired)
                {
                    juror = "Juror";
                    votedOff.placement = juror + "\n" + vote;
                    jury.Add(votedOff);
                }
                else
                {
                    conPlacement = "Voted Off Ep. " + (curEp + 1);
                    votedOff.placement = conPlacement + "\n" + juror + "\n" + vote;
                    votedOff.teams.Add(new Color());
                    EOE.Add(votedOff);
                }
            }
            Eliminated.Add(votedOff);
            if(curEvent.type == "JointTribal")
            {
                foreach(Team t in Tribes)
                {
                    if(t.members.Contains(votedOff))
                    {
                        t.members.Remove(votedOff);
                    }
                }
            } else if(curEvent.type == "MergeSplit")
            {
                MergedTribe.members.Remove(votedOff);
            } else
            {
                team.members.Remove(votedOff);
            }
            
            if (team.members.Count == 0)
            {
                Tribes.Remove(team);
            }
            List<Contestant> r = new List<Contestant>() { votedOff };
            string bottle = "";
            if(curEvent.type == "DoubleElim" && MergedTribe.members.Count < 1)
            {
                bottle = "\n \n The message in the bottle instructs them to vote out another tribe member.";
            }
            if (cineTribal == true)
            {
                MakeGroup(false, null, "", "", votedOff.nickname + ", the tribe has spoken." + bottle, r, null, 5);
                if(what == true)
                {
                    //curEpp++;
                }
            } else
            {
                MakeGroup(false, null, "", "", votedOff.nickname + ", the tribe has spoken." + bottle, r, EpisodeStart.transform.GetChild(0).GetChild(0), 5);
            }
            currentContestants--;
            tie = new List<Contestant>();
            if(currentContestants == 4)
            {
                RIExpired = true;
            }
        }
        void Tiebreaker(List<Contestant> tie, string type)
        {
            switch (type)
            {
                case "Rocks":
                    Rocks();
                    break;
                case "FireChallenge":
                    FireChallenge();
                    break;
                case "Challenge":
                    Challenge();
                    break;
            }
            void Rocks()
            {
                List<Contestant> teamV = new List<Contestant>(team.members);
                if(team.members.Count > tie.Count)
                {
                    foreach (Contestant num in tie)
                    {
                        teamV.Remove(num);
                    }
                }
                if(immune != null)
                {
                    foreach (Contestant num in immune)
                    {
                        if (team.members.Contains(num))
                            teamV.Remove(num);
                    }
                }
                if (cineTribal == true)
                {
                    MakeGroup(false, null, "name", "", "Because the vote is a deadlock, rocks will be drawn.", teamV, null, 20);
                    //AddFinalVote(group);
                }
                else
                {
                    MakeGroup(false, null, "name", "", "Because the vote is a deadlock, rocks will be drawn.", teamV, EpisodeStart.transform.GetChild(0).GetChild(0), 20);
                    //group.transform.parent = EpisodeStart.transform.GetChild(0).GetChild(0);
                }
                votedOff = teamV[Random.Range(0, teamV.Count)];
                vote = vote + "\n Rocks Drawn";
                
                if (team.members.Count > tie.Count)
                {
                    if (!tie.Contains(votedOff))
                    {
                        //Debug.Log(votedOff.nickname + " has been eliminated. " + "Drew the purple rock");
                        Eliminate();
                    }
                } else
                {
                    Eliminate();
                }
            }
            void FireChallenge()
            {
                if (cineTribal == true)
                {
                    MakeGroup(false, null, "name", "", "Those tied will compete in a fire-making challenge. The loser will be eliminated from the game.", tie, null, 15);
                    //AddFinalVote(grouppp);
                }
                else
                {
                    MakeGroup(false, null, "name", "", "Those tied will compete in a fire-making challenge. The loser will be eliminated from the game.", tie, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
                    //grouppp.transform.parent = EpisodeStart.transform.GetChild(0).GetChild(0);
                }
                votedOff = tie[Random.Range(0, tie.Count)];
                vote = vote + "\n Tiebreaker";
                //Debug.Log(votedOff.nickname + " has been eliminated. " + "Lost firemaking");
                Eliminate();
            }
            void Challenge()
            {
                if (cineTribal == true)
                {
                    MakeGroup(false, null, "name", "", "Those tied will compete in a tiebreaker challenge. The loser will be eliminated from the game.", tie, null, 15);

                    //AddFinalVote(grouppp);
                }
                else
                {
                    MakeGroup(false, null, "name", "", "Those tied will compete in a fire-making challenge. The loser will be eliminated from the game.", tie, EpisodeStart.transform.GetChild(0).GetChild(0), 15);

                    //grouppp.transform.parent = EpisodeStart.transform.GetChild(0).GetChild(0);
                }
                votedOff = tie[Random.Range(0, tie.Count)];
                vote = vote + "\n Tiebreaker";
                //Debug.Log(votedOff.nickname + " has been eliminated. " + "Lost tiebreaker challenge");
                Eliminate();
            }
        }
    }
    void CineTribals()
    {
        votesRead = currentSeason.Episodes[curEv].votesReads[curTribal];
        votes = currentSeason.Episodes[curEv].votes[curTribal];
        foreach (GameObject torch in Torches)
        {
            torch.SetActive(true);
        }
        nextButton.gameObject.SetActive(false);
        VoteButton.SetActive(true);

        curVot = 0;
    }
    void MakeAlliances(Team team)
    {
        Alliance alliance1 = new Alliance();
        Alliance alliance2 = new Alliance();
        alliance1.name = team.name + " Alliance #1";
        alliance2.name = team.name + " Alliance #2";
        alliance1.teams.Add(team.name);
        alliance2.teams.Add(team.name);
        foreach(Contestant num in team.members)
        {
            int ran = Random.Range(0, 3);
            if (ran == 0 && alliance1.members.Count < team.members.Count - 1)
            {
                alliance1.members.Add(num);
            } else if (ran == 1 && alliance2.members.Count < team.members.Count - 1)
            {
                alliance2.members.Add(num);
            }
            else
            {

            }
        }
        if (alliance1.members.Count < 2 || alliance2.members.Count < 2)
        {
            if(team.members.Count == 3)
            {
                if(alliance1.members.Count > 1)
                {
                    Alliances.Add(alliance1);
                }
                if (alliance2.members.Count > 1)
                {
                    Alliances.Add(alliance2);
                }
            } else
            {
                if (alliance1.members.Count > 1)
                {
                    Alliances.Add(alliance1);
                }
                if (alliance2.members.Count > 1)
                {
                    Alliances.Add(alliance2);
                }
                //MakeAlliances(team);
            }
        } else
        {
            Alliances.Add(alliance1);
            Alliances.Add(alliance2);
        }
    }
    void TieGame()
    {
        //set ran in tribe immunity to 0 to tie at tribal.
        Alliances = new List<Alliance>();
        Alliance alliance1 = new Alliance();
        Alliance alliance2 = new Alliance();
        Alliance alliance3 = new Alliance();
        Alliance alliance4 = new Alliance();
        alliance1.members.Add(Tribes[1].members[0]);
        alliance1.members.Add(Tribes[1].members[1]);
        alliance1.members.Add(Tribes[1].members[2]);
        alliance1.members.Add(Tribes[1].members[3]);
        alliance2.members.Add(Tribes[1].members[4]);
        alliance2.members.Add(Tribes[1].members[5]);
        alliance2.members.Add(Tribes[1].members[6]);
        alliance2.members.Add(Tribes[1].members[7]);
        alliance1.teams.Add(Tribes[1].name);
        alliance2.teams.Add(Tribes[1].name);
        Alliances.Add(alliance1);
        Alliances.Add(alliance2);
        //Tribe2Alliances.Add(alliance1);
        //Tribe2Alliances.Add(alliance2);
    } 
    void MergeTribes()
    {
        bool r = false;
        Team lso = new Team();
        //lastThing.SetActive(false);
        foreach (Team tribe in Tribes)
        {
            foreach (Contestant num in tribe.members)
            {
                MergedTribe.members.Add(num);
            }
            if (tribe.members.Count == 1)
            {
                r = true;
                lso = tribe;
            }
        }
        if(r && Tribes.Count == 2)
        {
            foreach (Team tribe in Tribes)
            {
                if (tribe != lso)
                {
                    MergedTribe.name = tribe.name;
                    MergedTribe.tribeColor = tribe.tribeColor;
                }
            }
            
        } else
        {
            MergedTribe.name = seasonTemp.MergeTribeName;
            MergedTribe.tribeColor = seasonTemp.MergeTribeColor;
            r = false;
        }
        
        foreach (Alliance alliance in Alliances)
        {
            alliance.teams.Add(MergedTribe.name);
        }
        //Debug.Log("Merged!");
        //Debug.Log(MergedTribe.name + ":" + string.Join(",  ", MergedTribe.members.ConvertAll(i => i.nickname)));
        GameObject EpisodeStart = Instantiate(Prefabs[0]);
        EpisodeStart.transform.parent = Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        foreach (Contestant num in MergedTribe.members)
        {
            num.altVotes = new List<Contestant>();
            num.teams.Add(MergedTribe.tribeColor);
        }

        MakeGroup(true, MergedTribe, "name", "", "", MergedTribe.members, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
        lastThing = EpisodeStart;
        AddGM(EpisodeStart, true);
        List<Alliance> remove = new List<Alliance>();
        foreach (Alliance alliance in Alliances)
        {
            if (alliance.members.Count < 2)
            {
                remove.Add(alliance);
            }
        }
        foreach (Alliance alliance in remove)
        {
            Alliances.Remove(alliance);
        }
        LosingTribe = MergedTribe;
        nextEvent = 1;
        immune = new List<Contestant>();
        NextEvent();
    }
    void MergeStatus()
    {
        GameObject EpisodeStatus = Instantiate(Prefabs[1]);
        EpisodeStatus.transform.parent = Canvas.transform;
        EpisodeStatus.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStatus.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStatus.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStatus.GetComponent<RectTransform>().offsetMin.x, 0);
        AddGM(EpisodeStatus, true);
        foreach (Alliance alliance in Alliances)
        {
            MakeGroup(false, null, "name", alliance.name, "", alliance.members, EpisodeStatus.transform.GetChild(0), 0);
        }
        if (Alliances.Count < 2)
        {
            EpisodeStatus.transform.GetChild(0).GetComponent<VerticalLayoutGroup>().spacing = -90;
        }
        NextEvent();
    }
    void MergeReward()
    {
        Contestant winner;
        int ran = Random.Range(0, MergedTribe.members.Count);
        winner = MergedTribe.members[ran];

        GameObject EpisodeStart = Instantiate(Prefabs[2]);
        EpisodeStart.transform.parent = Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        lastThing = EpisodeStart;
        AddGM(EpisodeStart, true);
        List<Contestant> w = new List<Contestant>() { winner};
        MakeGroup(false, null, winner.nickname + " Wins Reward!", "", "", w, EpisodeStart.transform.GetChild(0), 20);
    }
    void MergeImmunity()
    {
        immune = new List<Contestant>();
        int ran = Random.Range(0, MergedTribe.members.Count);
        immune.Add(MergedTribe.members[ran]);
        GameObject EpisodeStart = Instantiate(Prefabs[2]);
        EpisodeStart.transform.parent = Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        lastThing = EpisodeStart;
        AddGM(EpisodeStart, true);

        List<Contestant> w = new List<Contestant>() { immune[0] };
        MakeGroup(false, null, immune[0].nickname + " Wins Immunity!", "", "", w, EpisodeStart.transform.GetChild(0), 20);
        if (curExile.on && curExile.challenge == "Immunity")
        {
            if (curExile.on && !curSwap.on)
            {
                if (curExile.reason == "Winner")
                {
                    List<Contestant> mergeT = new List<Contestant>(MergedTribe.members);
                    foreach(Contestant num in immune)
                    {
                        mergeT.Remove(num);
                    }
                    int rann = Random.Range(0, mergeT.Count);
                    mergeT[rann].team = MergedTribe.name;
                    Exiled.Add(mergeT[rann]);
                    w = new List<Contestant>() { mergeT[rann] };
                    MakeGroup(false, null, "", "", mergeT[rann].nickname + " is sent to exile by the immunity winner.", w, EpisodeStart.transform.GetChild(0), 20);
                    MergedTribe.members.Remove(mergeT[rann]);
                }
            }
            else if (curExile.on && curSwap.on)
            {
                Exiled[0].team = LosingTribes[LosingTribes.Count - 1].name;

            }
        }
        NextEvent();
    }
    void NextEpM()
    {
        //lastThing.SetActive(false);
        List<Alliance> remove = new List<Alliance>();
        GameObject EpisodeStart = Instantiate(Prefabs[0]);
        EpisodeStart.transform.parent = Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        lastThing = EpisodeStart;
        AddGM(EpisodeStart, true);
        foreach (Contestant num in MergedTribe.members)
        {
            num.altVotes = new List<Contestant>();
            num.inTie = false;
        }
        MakeGroup(true, MergedTribe, "name", "", "", MergedTribe.members, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
        lastThing = EpisodeStart;
        foreach (Alliance alliance in Alliances)
        {
            if (alliance.members.Count < 2)
            {
                remove.Add(alliance);
            }
        }
        foreach (Alliance alliance in remove)
        {
            Alliances.Remove(alliance);
        }
        if (jury.Count > 0)
        {
            //Debug.Log("Jury:" + string.Join(", ", jury.ConvertAll(i => i.nickname)));
        }
        nextEvent = 1;
        immune = new List<Contestant>();
        NextEvent();
    }
    void Reunion()
    {
        //Debug.Log("Finalists:" + string.Join(", ", MergedTribe.members.ConvertAll(i => i.nickname)));   
    }
    void WinnerReveal()
    {
        //Debug.Log(curEpp);
        GameObject EpisodeStart = Instantiate(Prefabs[0]);
        EpisodeStart.transform.parent = Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStart.name = "Tribal Council";
        AddGM(EpisodeStart, true);
        Contestant JurorRemoved = null;
        for (int i = 0; i < jury.Count; i++)
        {
            int ran = Random.Range(0, MergedTribe.members.Count);
            jury[i].vote = MergedTribe.members[ran];
        }
        CountJuryVotes();
        AddVote(votes, votesRead);
        tie = new List<Contestant>();
        float maxValue = dic.Values.Max();
        foreach (KeyValuePair<Contestant, int> num in dic)
        {
            if (num.Value == maxValue)
            {
                tie.Add(num.Key);
            }
            else
            {

            }
        }
        if (tie.Count < 2)
        {
            Winnerr();
        }
        else
        {
            JuryRevote();
        }
        NextEvent();
        void CountJuryVotes()
        {
            votes = new List<Contestant>();
            e = false;
            
            foreach (Contestant num in jury)
            {
                votes.Add(num.vote);
            }
            dic = new Dictionary<Contestant, int>();
            Winner = votes[0];
            dic.Add(votes[0], 1);
            for (int i = 1; i < votes.Count; i++)
            {
                if (dic.ContainsKey(votes[i]))
                {
                    dic[votes[i]] += 1;
                    if (dic[votes[i]] > dic[Winner])
                    {
                        Winner = votes[i];
                    }
                }
                else if (!dic.ContainsKey(votes[i]))
                {
                    dic.Add(votes[i], 1);
                }
            }
            tie = new List<Contestant>();
            float maxValuee = dic.Values.Max();
            List<float> votesSpread = new List<float>();
            dic = dic.OrderBy(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach(Contestant num in MergedTribe.members)
            {
                if(!dic.ContainsKey(num))
                {
                    num.placement = "Finalist \n" + "0 Votes To Win";
                    Eliminated.Add(num);
                }
            }
            foreach (KeyValuePair<Contestant, int> num in dic)
            {
                if (num.Value == maxValuee)
                {
                    tie.Add(num.Key);
                }
                else
                {
                    votesSpread.Add(num.Value);
                }
                num.Key.placement = "Finalist \n" + num.Value + " Votes To Win";
                if(num.Value == 1)
                {
                    num.Key.placement = num.Key.placement.Replace("Votes", "Vote");
                }
                Eliminated.Add(num.Key);
            }
            
            float enoughVotes = 0;
            if (votesSpread.Count > 0)
            {
                for (int i = votes.Count - 1; i > 0; i--)
                {
                    int enough = 0;
                    foreach (int num in votesSpread)
                    {
                        if (maxValuee - 1 > num + i && enoughVotes == 0)
                        {
                            enough++;
                        }
                    }
                    if (enough == votesSpread.Count)
                    {
                        if (i > 1)
                        {
                            enoughVotes = i - 1;
                        }
                        else
                        {
                            enoughVotes = i;
                        }

                    }
                }
            }
            else
            {
                if(tie.Count < 2)
                {
                    if (votes.Count % 2 == 0)
                    {
                        enoughVotes = (votes.Count / 2) + 1;
                    }
                    else
                    {
                        enoughVotes = Mathf.Ceil(votes.Count / 2);
                    }
                }
                
            }
            if(votes.Count < 1)
            {
                Debug.Log("gs");
            }
            //Sort votes then generate each vote for UI
            votesRead = votes.OrderBy(go => dic[go]).ToList();
            if (tie.Count < 2)
            {
                for (int i = 0; i < enoughVotes; i++)
                {
                    votesRead.Remove(votesRead[votesRead.Count - 1]);
                }
            }
            ShuffleVotes(votesRead);
            dicVR = new Dictionary<Contestant, int>();
            dicVR.Add(votesRead[0], 1);
            string votess;
            votess = " vote ";
            string votesLeft;
            if (showVL == true)
            {
                float vl = votes.Count - 1;
                votesLeft = ". " + vl + " Votes Left";
            }
            else
            {
                votesLeft = "";
            }
            List<string> votesSoFar = new List<string>();
            foreach (KeyValuePair<Contestant, int> num in dic)
            {
                if (num.Value > 1)
                {
                    votess = " votes ";
                }
                else
                {
                    votess = " vote ";
                }
                string v = dic[num.Key] + votess + num.Key.nickname;
                votesSoFar.Add(v);
            }
            votesSoFar = votesSoFar.OrderByDescending(go => go[0]).ToList();
            finalVotes = "Final vote count was " + string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + ".";
            if (cineTribal == true)
            {
                /*
                foreach (GameObject torch in Torches)
                {
                    torch.SetActive(true);
                }
                nextButton.gameObject.SetActive(false);
                VoteButton.SetActive(true);

                curVot = 0; */
                
            }
            else
            {
                List<Contestant> r = new List<Contestant>() { votesRead[0]};
                MakeGroup(false, null, dicVR[votesRead[0]] + votess + votesRead[0].nickname + votesLeft, "", "", r, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
                for (int i = 1; i < votesRead.Count; i++)
                {
                    if (dicVR.ContainsKey(votesRead[i]))
                    {
                        dicVR[votesRead[i]] += 1;
                    }
                    else if (!dicVR.ContainsKey(votesRead[i]))
                    {
                        dicVR.Add(votesRead[i], 1);
                    }
                    votess = "";
                    votesLeft = "";
                    if (showVL == true)
                    {
                        float vl = votes.Count - i - 1;
                        if (vl > 0)
                        {
                            if (vl > 1)
                            {
                                votesLeft = ". " + vl + " Votes Left";
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
                    votesSoFar = new List<string>();
                    foreach (KeyValuePair<Contestant, int> num in dicVR)
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
                    r = new List<Contestant>() { votesRead[i]};
                    if (i == votesRead.Count - 1 && tie.Count < 2)
                    {
                        string juryPM = "";
                        if (currentContestants - finaleAt <= juryAt)
                        {
                            float juryy = jury.Count + 1;
                            juryPM = " and " + Oridinal(juryy) + " member of the jury";
                        }
                        float placement = currentContestantsOG - currentContestants + 1;
                        string placementt = "";
                        placementt = Oridinal(placement);
                        MakeGroup(false, null, votesRead[i].nickname, "The winner of " + seasonTemp.nameSeason + " is... ", finalVotes, r, EpisodeStart.transform.GetChild(0).GetChild(0), 20);
                    }
                    else if (i == votesRead.Count - 1 && tie.Count > 1)
                    {
                        
                    } else
                    {
                        MakeGroup(false, null, string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + votesLeft, "", "", r, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                    }
                }
            }
        }
        void JuryRevote()
        {
            VotedOffCine.transform.GetChild(0).GetComponent<FlowLayoutGroup>().padding.top = 30;
            if (tie.Count != finaleAt)
            {
                Contestant con = new Contestant();
                foreach (Contestant num in MergedTribe.members)
                {
                    if (!tie.Contains(num))
                    {
                        con = num;
                    }
                }
                con.vote = tie[Random.Range(0, tie.Count)];
                string atext = "";
                if(JurorRemoved == null)
                {
                    atext = "Since there is a tie, the third place finalist will cast the deciding vote.";
                }

                List<Contestant> a = new List<Contestant>() {con.vote, con};
                if (cineTribal == true)
                {
                    MakeGroup(false, null, "", atext, con.nickname + "'s vote is " + con.vote.nickname + ".", a, null, 5);
                    //AddFinalVote(groupp);
                }
                else
                {
                    MakeGroup(false, null, "", atext, con.nickname + "'s vote is " + con.vote.nickname + ".", a, EpisodeStart.transform.GetChild(0).GetChild(0), 5);
                    //groupp.transform.parent = EpisodeStart.transform.GetChild(0).GetChild(0);
                }
                Winner = con.vote;
                Winnerr();
            }
            else if (tie.Count == finaleAt)
            {
                JurorRemoved = jury[0];
                
                RevealRemovedVote();
            }

        }
        void RevealRemovedVote()
        {
            List<Contestant> a = new List<Contestant>() { JurorRemoved.vote, JurorRemoved };
            List<Contestant> votesRe = new List<Contestant>(votes);
            votesRe.Remove(votesRe[0]);
            dic = new Dictionary<Contestant, int>();
            Winner = votesRe[0];
            dic.Add(votesRe[0], 1);
            for (int i = 1; i < votesRe.Count; i++)
            {
                if (dic.ContainsKey(votesRe[i]))
                {
                    dic[votesRe[i]] += 1;
                    if (dic[votesRe[i]] > dic[Winner])
                    {
                        Winner = votesRe[i];
                    }
                }
                else if (!dic.ContainsKey(votesRe[i]))
                {
                    dic.Add(votesRe[i], 1);
                }
            }
            tie = new List<Contestant>();
            float maxValuee = dic.Values.Max();
            foreach (KeyValuePair<Contestant, int> num in dic)
            {
                if (num.Value == maxValuee)
                {
                    tie.Add(num.Key);
                }
                else
                {

                }
            }
            if (tie.Count < 2)
            {
                if (cineTribal == true)
                {
                    MakeGroup(false, null, "", "Since there is a tie, the lowest placing juror will be removed.", JurorRemoved.nickname + "'s vote was " + JurorRemoved.vote.nickname + ".", a, null, 5);
                    //AddFinalVote(groupp);
                }
                else
                {
                    MakeGroup(false, null, "", "Since there is a tie, the lowest placing juror will be removed.", JurorRemoved.nickname + "'s vote was " + JurorRemoved.vote.nickname + ".", a, EpisodeStart.transform.GetChild(0).GetChild(0), 5);
                    //groupp.transform.parent = EpisodeStart.transform.GetChild(0).GetChild(0);
                }
                Winnerr();
            }
            else if (tie.Count > 1)
            {
                if(tie.Count != finaleAt)
                {
                    string etext = JurorRemoved.nickname + "'s vote was " + JurorRemoved.vote.nickname + "." +  "\n" + "\n" + "Since there is still a tie, the third place finalist will cast the deciding vote";
                    if (cineTribal == true)
                    {
                        MakeGroup(false, null, "", "Since there is a tie, the lowest placing juror will be removed.", JurorRemoved.nickname + "'s vote was " + JurorRemoved.vote.nickname + ".", a, null, 5);
                        //AddFinalVote(groupp);
                    }
                    else
                    {
                        MakeGroup(false, null, "", "Since there is a tie, the lowest placing juror will be removed.", etext, a, EpisodeStart.transform.GetChild(0).GetChild(0), 5);
                        //groupp.transform.parent = EpisodeStart.transform.GetChild(0).GetChild(0);
                    }
                    JuryRevote(); 
                } else
                {
                    JurorRemoved = jury[0];
                    votes.Remove(votes[0]);
                    jury.Remove(jury[0]);
                    RevealRemovedVote();
                }
            }
        }
        void Winnerr()
        {
            List<Contestant> a = new List<Contestant>() { Winner };
            if (cineTribal == true)
            {
                MakeGroup(false, null, "name", "", "Congratulations, " + Winner.fullname, a, null, 5);
            }
            else
            {
                MakeGroup(false, null, "name", "", "Congratulations, " + Winner.fullname, a, EpisodeStart.transform.GetChild(0).GetChild(0), 5);
            }
        }
    }
    void Final3Tribal(Team team)
    {
        List<Contestant> TeamV = new List<Contestant>(team.members);
        TeamV.Remove(immune[0]);
        immune[0].vote = TeamV[Random.Range(0, TeamV.Count)];
        votedOff = immune[0].vote;
        votes = new List<Contestant>();
        votes.Add(immune[0].vote);
        AddVote(votes, votes);
        GameObject EpisodeStart = Instantiate(Prefabs[0]);
        EpisodeStart.transform.parent = Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStart.name = "Tribal Council";
        AddGM(EpisodeStart, true);

        if (cineTribal == true)
        { 
            MakeGroup(false, null, "name", "", votes[0].nickname + ", the tribe has spoken", votes, EpisodeStart.transform.GetChild(0).GetChild(0), 0);

        } else
        {
            string juryPM = "";
            if (currentContestants - finaleAt <= juryAt && !sea.RedemptionIsland && !sea.EdgeOfExtinction)
            {
                float juryy = jury.Count + 1;
                juryPM = " and " + Oridinal(juryy) + " member of the jury";
            }
            float placement = elimed;
            string placementt = "";
            placementt = Oridinal(placement);
            string etext =  votes[0].nickname + ", the tribe has spoken" + "\n" + "Final vote count was 1 vote " + votes[0].nickname;
            MakeGroup(false, null, "name", "The " + placementt + " eliminated from " + seasonTemp.nameSeason + juryPM + " is... ", etext, votes, EpisodeStart.transform.GetChild(0).GetChild(0), 20);
        }
        foreach (Alliance alliance in Alliances)
        {
            if (alliance.members.Contains(votedOff))
            {
                alliance.members.Remove(votedOff);
            }
        }
        team.members.Remove(votedOff);
        votedOff.placement = "Juror" +"\n" +"1 Vote";
        Eliminated.Add(votedOff);
        if (currentContestants <= juryAt)
        {
            jury.Add(votedOff);
        }
        currentContestants--;
        NextEvent();
    }
    void FanFavorite()
    {
        Contestant fav = cast.cast[Random.Range(0, cast.cast.Count)];
        //Debug.Log("Fan Favorite:" + fav.nickname);
        GameObject EpisodeStart = Instantiate(Prefabs[0]);
        EpisodeStart.transform.parent = Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        //curEpp--;

        AddGM(EpisodeStart, true);
        //curEpp++;
        List<Contestant> a = new List<Contestant>() { fav };
        MakeGroup(false, null, "name", "The fan favorite is...", "", a, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
        NextEvent();
    }
    void Placements()
    {
        GameObject EpisodeStart = Instantiate(Prefabs[0]);
        EpisodeStart.transform.parent = Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStart.name = "Placements";
        AddGM(EpisodeStart, true);
        GameObject group = Instantiate(GroupPrefab);
        group.GetComponent<UIGroup>().tribeName.enabled = false;
        //group.GetComponent<UIGroup>().List.GetComponent<FlowLayoutGroup>().SpacingY = 30;
        
        group.transform.parent = EpisodeStart.transform.GetChild(0).GetChild(0);
        int highestLength = 0;
        int er = Eliminated.Count;
        if (er % 6 == 0)
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
        foreach (Contestant num in Eliminated)
        {
            string place = Oridinal(Eliminated.Count - Eliminated.IndexOf(num)) + " Place";
            if(Eliminated.Count - Eliminated.IndexOf(num) == 1)
            {
                place = "Winner";
            }
            num.placement = place + "\n" + num.placement;
            GameObject mem = Instantiate(ContestantPrefab);
            mem.GetComponentInChildren<Image>().sprite = num.image;
            mem.GetComponentInChildren<Text>().text = num.fullname + "\n "+ num.placement;
            mem.transform.GetChild(1).gameObject.SetActive(true);
            for (int j = 0; j < num.teams.Count; j++)
            {
                GameObject image = Instantiate(imagePrefab);
                image.GetComponent<Image>().color = num.teams[j];
                image.transform.parent = mem.transform.GetChild(1);
            }
            int ee = mem.GetComponentInChildren<Text>().text.Split('\n').Length - 4;
            if (ee > highestLength && Eliminated.IndexOf(num) > er)
            {
                highestLength = ee;
            }
            if(ee > 0)
            {
                mem.GetComponentInChildren<VerticalLayoutGroup>().padding.bottom -= 16*ee;
            }
            mem.transform.parent = group.transform.GetChild(2);
        }
        group.GetComponent<UIGroup>().List.GetComponent<FlowLayoutGroup>().SpacingY = (16*highestLength) + 2;
        group.GetComponent<UIGroup>().List.GetComponent<FlowLayoutGroup>().SpacingX = 38;
        float teamWidth = ConListWidth(group.transform.GetChild(2).childCount);
        group.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(teamWidth, group.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.y);
        group.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 1f);
        group.GetComponent<SetupLayout>().Start();
        h = false;
    }
    void Statistics()
    {

    }
    void ShowVotes()
    {
        GameObject EpisodeStart = Instantiate(Prefabs[0]);
        EpisodeStart.transform.parent = Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        //curEpp--;
        AddGM(EpisodeStart, true);
        //curEpp++;
        int d = 0;
        if(LosingTribe == Outcasts)
        {
            List<Contestant> team = new List<Contestant>(Outcasts.members);
            foreach (Contestant num in team)
            {
                if (num.vote != null)
                {
                    num.voteReason = "They think they will do well.";
                    GameObject group = Instantiate(GroupPrefab);
                    group.GetComponent<UIGroup>().tribeName.enabled = false;
                    string extraVotes = "";
                    if (num.altVotes.Count > 0)
                    {
                        foreach (Contestant vot in num.altVotes)
                        {
                            if (vot != num.vote)
                            {
                                GameObject memmm = Instantiate(ContestantPrefab);
                                memmm.GetComponentInChildren<Image>().sprite = vot.image;
                                memmm.GetComponentInChildren<Text>().enabled = false;
                                extraVotes += "\n" + "\n" + num.nickname + " voted for " + vot.nickname;
                                memmm.transform.parent = group.transform.GetChild(2);
                            }
                            else
                            {
                                extraVotes += "\n" + "\n" + num.nickname + " voted for " + vot.nickname;
                            }
                        }
                    }
                    else
                    {

                    }
                    GameObject memm = Instantiate(ContestantPrefab);
                    memm.GetComponentInChildren<Image>().sprite = num.vote.image;
                    memm.transform.parent = group.transform.GetChild(2);
                    memm.GetComponentInChildren<Text>().enabled = false;
                    GameObject mem = Instantiate(ContestantPrefab);
                    mem.GetComponentInChildren<Image>().sprite = num.image;
                    mem.transform.parent = group.transform.GetChild(2);
                    mem.GetComponentInChildren<Text>().enabled = false;
                    group.GetComponent<UIGroup>().eventText.text = num.nickname + " voted for " + num.vote.nickname + extraVotes + "\n" + "\n" + num.voteReason;
                    group.transform.parent = EpisodeStart.transform.GetChild(0).GetChild(0);
                    d++;
                }
            }
        }
        else
        {
            if (MergedTribe.members.Count < 1)
            {
                foreach (Team tribe in Tribes)
                {
                    float tieo = 0;

                    List<Contestant> team = new List<Contestant>(tribe.members);
                    team.Add(votedOff);
                    foreach (Contestant num in team)
                    {
                        if (num.inTie)
                        {
                            tieo++;
                        }
                    }

                    if (tribe == LosingTribe)
                    {

                        foreach (Contestant num in team)
                        {
                            if (num.vote != null)
                            {
                                GameObject group = Instantiate(GroupPrefab);
                                group.GetComponent<UIGroup>().tribeName.enabled = false;
                                string extraVotes = "";
                                if (num.altVotes.Count > 0)
                                {
                                    foreach (Contestant vot in num.altVotes)
                                    {
                                        if (vot != num.vote)
                                        {
                                            GameObject memmm = Instantiate(ContestantPrefab);
                                            memmm.GetComponentInChildren<Image>().sprite = vot.image;
                                            memmm.GetComponentInChildren<Text>().enabled = false;
                                            extraVotes += "\n" + "\n" + num.nickname + " voted for " + vot.nickname + " on revote #" + (num.altVotes.IndexOf(vot) + 1);
                                            memmm.transform.parent = group.transform.GetChild(2);
                                        }
                                        else
                                        {
                                            extraVotes += "\n" + "\n" + num.nickname + " voted for " + vot.nickname + " on revote #" + (num.altVotes.IndexOf(vot) + 1);
                                        }
                                    }
                                }
                                else
                                {

                                }
                                if (tieo > 1 && tieo != team.Count)
                                {
                                    if (num.inTie)
                                    {
                                        extraVotes = "\n" + "\n" + "Couldn't vote in the revote.";
                                    }
                                }
                                GameObject memm = Instantiate(ContestantPrefab);
                                memm.GetComponentInChildren<Image>().sprite = num.vote.image;
                                memm.transform.parent = group.transform.GetChild(2);
                                memm.GetComponentInChildren<Text>().enabled = false;
                                GameObject mem = Instantiate(ContestantPrefab);
                                mem.GetComponentInChildren<Image>().sprite = num.image;
                                mem.transform.parent = group.transform.GetChild(2);
                                mem.GetComponentInChildren<Text>().enabled = false;
                                group.GetComponent<UIGroup>().eventText.text = num.nickname + " voted for " + num.vote.nickname + extraVotes + "\n" + "\n" + num.voteReason;
                                group.transform.parent = EpisodeStart.transform.GetChild(0).GetChild(0);
                                d++;
                            }
                        }

                    }
                }
                if (d == 0)
                {
                    GameObject group = Instantiate(GroupPrefab);
                    group.GetComponent<UIGroup>().tribeName.enabled = false;
                    group.GetComponent<UIGroup>().eventText.text = "No votes were cast.";
                    group.transform.parent = EpisodeStart.transform.GetChild(0).GetChild(0);
                }
            }
            else
            {
                if (MergedTribe.members.Count + 1 == 3 && immune.Count > 0)
                {
                    GameObject group = Instantiate(GroupPrefab);
                    group.GetComponent<UIGroup>().tribeName.enabled = false;
                    immune[0].voteReason = "gaming";

                    GameObject memm = Instantiate(ContestantPrefab);
                    memm.GetComponentInChildren<Image>().sprite = immune[0].vote.image;
                    memm.transform.parent = group.transform.GetChild(2);
                    memm.GetComponentInChildren<Text>().enabled = false;
                    GameObject mem = Instantiate(ContestantPrefab);
                    mem.GetComponentInChildren<Image>().sprite = immune[0].image;
                    mem.transform.parent = group.transform.GetChild(2);
                    mem.GetComponentInChildren<Text>().enabled = false;
                    group.GetComponent<UIGroup>().eventText.text = immune[0].nickname + " voted for " + immune[0].vote.nickname + "\n" + "\n" + immune[0].voteReason;
                    group.transform.parent = EpisodeStart.transform.GetChild(0).GetChild(0);
                }
                else if (MergedTribe.members.Count == finaleAt)
                {
                    List<Contestant> juryy = new List<Contestant>(jury);
                    foreach (Contestant num in juryy)
                    {
                        if (num.vote != null)
                        {
                            num.voteReason = "votes";
                            GameObject group = Instantiate(GroupPrefab);
                            group.GetComponent<UIGroup>().tribeName.enabled = false;
                            GameObject memm = Instantiate(ContestantPrefab);
                            memm.GetComponentInChildren<Image>().sprite = num.vote.image;
                            memm.transform.parent = group.transform.GetChild(2);
                            memm.GetComponentInChildren<Text>().enabled = false;
                            GameObject mem = Instantiate(ContestantPrefab);
                            mem.GetComponentInChildren<Image>().sprite = num.image;
                            mem.transform.parent = group.transform.GetChild(2);
                            mem.GetComponentInChildren<Text>().enabled = false;
                            group.GetComponent<UIGroup>().eventText.text = num.nickname + " voted for " + num.vote.nickname + "\n" + "\n" + num.voteReason;
                            group.transform.parent = EpisodeStart.transform.GetChild(0).GetChild(0);

                        }
                    }
                }
                else
                {
                    float tieo = 0;
                    List<Contestant> team = new List<Contestant>(MergedTribe.members);
                    team.Add(votedOff);
                    foreach (Contestant num in team)
                    {
                        if (num.inTie)
                        {
                            tieo++;
                        }
                    }
                    foreach (Contestant num in team)
                    {
                        if (num.vote != null)
                        {
                            GameObject group = Instantiate(GroupPrefab);
                            group.GetComponent<UIGroup>().tribeName.enabled = false;
                            string extraVotes = "";
                            if (num.altVotes.Count > 0)
                            {
                                foreach (Contestant vot in num.altVotes)
                                {
                                    if (vot != num.vote)
                                    {
                                        GameObject memmm = Instantiate(ContestantPrefab);
                                        memmm.GetComponentInChildren<Image>().sprite = vot.image;
                                        memmm.GetComponentInChildren<Text>().enabled = false;
                                        extraVotes += "\n" + "\n" + num.nickname + " voted for " + vot.nickname + " on revote #" + (num.altVotes.IndexOf(vot) + 1);
                                        memmm.transform.parent = group.transform.GetChild(2);
                                    }
                                    else
                                    {
                                        extraVotes += "\n" + "\n" + num.nickname + " voted for " + vot.nickname + " on revote #" + (num.altVotes.IndexOf(vot) + 1);
                                    }
                                }
                            }
                            else
                            {

                            }
                            if (tieo > 1 && tieo != team.Count)
                            {
                                if (num.inTie)
                                {
                                    extraVotes = "\n" + "\n" + "Couldn't vote in the revote.";
                                }
                            }
                            GameObject memm = Instantiate(ContestantPrefab);
                            memm.GetComponentInChildren<Image>().sprite = num.vote.image;
                            memm.transform.parent = group.transform.GetChild(2);
                            memm.GetComponentInChildren<Text>().enabled = false;
                            GameObject mem = Instantiate(ContestantPrefab);
                            mem.GetComponentInChildren<Image>().sprite = num.image;
                            mem.transform.parent = group.transform.GetChild(2);
                            mem.GetComponentInChildren<Text>().enabled = false;
                            group.GetComponent<UIGroup>().eventText.text = num.nickname + " voted for " + num.vote.nickname + extraVotes + "\n" + "\n" + num.voteReason;
                            group.transform.parent = EpisodeStart.transform.GetChild(0).GetChild(0);
                        }
                    }
                }
            }
        }
        
        NextEvent();
    }
    public void ShuffleVotes(List<Contestant> votess)
    {
        for(int i = votess.Count - 3; i > 0; i--)
        {
            int swapIndex = Random.Range(0, i + 1);
            Contestant currentCon = votess[i];
            Contestant conToSwap = votess[swapIndex];
            votess[i] = conToSwap;
            votess[swapIndex] = currentCon;
        }
    }
    public void VoteReveal()
    {
        
        lastVoteOff.SetActive(false);
        if(votes.Count > 1)
        {
            if (Vote.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("voteIdle"))
            {
                string votesLeft = "";
                string votess = "";
                if (curVot == 0 && currentContestants + 1 != 3)
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
                    Vote.transform.GetChild(1).GetComponent<Text>().text = dicVR[votesRead[0]] + votess + votesRead[0].nickname + votesLeft;
                    curVot++;
                }
                else if (curVot == 0 && currentContestants + 1 == 3)
                {
                    string juryPM = "";
                    if (currentContestants + 1 - finaleAt <= juryAt)
                    {
                        float juryy = jury.Count - jurt;
                        juryPM = " and " + Oridinal(juryy) + " member of the jury";
                        jurt--;
                    }
                    float placement = elimed;

                    string placementt = "";
                    placementt = Oridinal(placement);
                    elimed++;
                    Vote.transform.GetChild(2).GetComponent<Text>().text = "The " + placementt + " eliminated from " + seasonTemp.nameSeason + juryPM + " is... ";
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
                        dicVR.Add(votesRead[curVot], 1);
                    }
                    votess = "";
                    votesLeft = "";
                    if (showVL == true)
                    {
                        float vl = votes.Count - curVot - 1 + jurVotesRemoved;

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
                    foreach (KeyValuePair<Contestant, int> num in dicVR)
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
                    if (curVot == votesRead.Count - 1 && tie.Count < 2)
                    {
                        nextButton.gameObject.SetActive(true);
                        VoteButton.SetActive(false);

                        string juryPM = "";
                        if (currentContestants + 1 - finaleAt <= juryAt)
                        {
                            float juryy = jury.Count - jurt;
                            juryPM = " and " + Oridinal(juryy) + " member of the jury";
                            jurt--;
                        }
                        float placement = elimed;
                        string placementt = "";
                        placementt = Oridinal(placement);
                        elimed++;
                        if (currentContestants + 1 == finaleAt)
                        {
                            Vote.transform.GetChild(2).GetComponent<Text>().text = "The winner of " + seasonTemp.nameSeason + " is... ";
                        }
                        else
                        {
                            Vote.transform.GetChild(2).GetComponent<Text>().text = "The " + placementt + " eliminated from " + seasonTemp.nameSeason + juryPM + " is... ";
                        }
                        foreach(UIGroup group in VotedOffCine.GetComponentsInChildren<UIGroup>())
                        {
                            if(group.eventText.text.Contains("returns to the game"))
                            {
                                Vote.transform.GetChild(2).GetComponent<Text>().text = "";
                                elimed--;
                            }
                        }
                        votesSoFar = new List<string>();
                        foreach (KeyValuePair<Contestant, int> num in dic)
                        {
                            if (num.Value > 1)
                            {
                                votess = " votes ";
                            }
                            else
                            {
                                votess = " vote ";
                            }
                            string v = dic[num.Key] + votess + num.Key.nickname;
                            votesSoFar.Add(v);
                        }
                        votesSoFar = votesSoFar.OrderByDescending(go => go[0]).ToList();
                        finalVotes = "Final vote count was " + string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + ".";
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
                        if (e == false && currentContestants != finaleAt)
                        {
                            string firstline = "There is a tie and a revote. Those in in the tie will not revote, unless no one received votes on the original vote.";
                            string secondline = "Final vote count was " + string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + ".";
                            MakeGroup(false, null, "", "", firstline + "\n" + "\n" + "\n" + secondline, tie, VotedOffCine.transform.GetChild(0), 20);
                        }
                        if (currentContestants == finaleAt && tie.Count != finaleAt)
                        {

                        }
                        else if (currentContestants == finaleAt && tie.Count == finaleAt)
                        {

                        }
                        curTribal++;
                        VotedOffCine.transform.parent.gameObject.SetActive(true);
                    }
                    curVot++;
                }
            }
            
        } else
        {
            nextButton.gameObject.SetActive(true);
            VoteButton.SetActive(false);

            string juryPM = "";
            if (currentContestants + 1 - finaleAt <= juryAt)
            {
                float juryy = jury.Count + 1 - (currentContestants + 1 - finaleAt);
                juryPM = " and " + Oridinal(juryy) + " member of the jury";
            }
            float placement = elimed;
            string placementt = "";
            placementt = Oridinal(placement);
            elimed++;
            Vote.transform.GetChild(0).GetComponent<Text>().text = votes[0].nickname;
            Vote.transform.GetChild(1).GetComponent<Text>().text = "";
            Vote.transform.GetChild(2).GetComponent<Text>().text = "";
            VotedOffCine.transform.parent.gameObject.SetActive(true);
            VotedOffCine.SetActive(true);
            curTribal++;
        }
        if (Vote.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("voteIdle") || Vote.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("voteRevealed"))
        {
            Vote.GetComponent<Animator>().SetTrigger("Reveal");
        }
    }
    public float ConListWidth(float contestants)
    {
        if(contestants % 6 == 0)
        {
            return 800f;
        } else if (contestants % 5 == 0)
        {
            return 700f;
        } else if (contestants % 4 == 0)
        {
            return 550f;
        } else
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
                return 800f;
            }
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
    void Swap()
    {
        if(curSwap.pickingRules == "altGender")
        {
            if(!genderEqual)
            {
                curSwap.pickingRules = "Any";
            }
        }
        swapper.DoSwap(curSwap.type);
        if(sea.MedallionOfPower)
        {
            int ran = Random.Range(0, Tribes.Count);
            MOP = Tribes[ran].name;
        }
        NextEvent();
    }
    void ExileI()
    {
        GameObject ExileEvent = Instantiate(Prefabs[0]);
        ExileEvent.transform.parent = Canvas.transform;
        ExileEvent.GetComponent<RectTransform>().offsetMax = new Vector2(0, ExileEvent.GetComponent<RectTransform>().offsetMax.y);
        ExileEvent.GetComponent<RectTransform>().offsetMax = new Vector2(ExileEvent.GetComponent<RectTransform>().offsetMin.x, 0);
        AddGM(ExileEvent, true);
        
        if (Exiled.Count == 1)
        {
            MakeGroup(false, null, Exiled[0].nickname + " is at Exile Island.", "", "", Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);
        }
        else
        {
            MakeGroup(false, null, "", "", Exiled[0].nickname + " and " + Exiled[1].nickname + " are at exile island.", Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);
        }
        switch(curExile.exileEvent)
        {
            case "Nothing":

                break;
        }
        if(!curExile.skipTribal)
        {
            foreach(Contestant num in Exiled)
            {
                if (Episodes[curEp].merged)
                {
                    if (MergedTribe.name == num.team)
                    {
                        num.team = "";
                        MergedTribe.members.Add(num);
                    }
                } else
                {
                    foreach (Team tribe in Tribes)
                    {
                        if (tribe.name == num.team)
                        {
                            num.team = "";
                            tribe.members.Add(num);
                        }
                    }
                }
                
            }
            
            Exiled = new List<Contestant>();
        }
        NextEvent();
    }
    void RedemptionIsland()
    {
        reTwists.RedemptionIsland();
    }
    void EOEStatus()
    {
        reTwists.EOEStatus();
    }
    void EOEReturnChallenge()
    {
        reTwists.EOEReturnChallenge();
    }
    void OutcastsImmunity()
    {
        reTwists.OutcastsImmunity();
    }
    void OutcastsTribal()
    {
        reTwists.OutcastsTribal();
    }
    void STribeImmunity()
    {
        oneTimeEvents.STribeImmunity();
    }
    void ImmVote()
    {
        oneTimeEvents.ImmVote(LosingTribes[0], LosingTribes[curTTT]);
    }
    void BeginningTwist()
    {
        oneTimeEvents.BeginningTwist();
    }

    public void MakeGroup(bool nameEnabled, Team teem, string conText, string aText, string eText, List<Contestant> cons, Transform ep, float spacing)
    {
        GameObject team = Instantiate(GroupPrefab);
        team.GetComponent<UIGroup>().tribeName.enabled = nameEnabled;
        if(nameEnabled)
        {
            team.GetComponent<UIGroup>().tribeName.text = teem.name;
            team.GetComponent<UIGroup>().tribeName.color = teem.tribeColor;
        }
        if(cons.Count > 0)
        {
            foreach(Contestant num in cons)
            {
                GameObject mem = Instantiate(ContestantPrefab);
                mem.GetComponentInChildren<Image>().sprite = num.image;
                if (conText == "name")
                {
                    mem.GetComponentInChildren<Text>().text = num.fullname;
                }
                else if (conText == "nname")
                {
                    mem.GetComponentInChildren<Text>().text = num.nickname;
                }
                else
                {
                    mem.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
                    mem.GetComponentInChildren<Text>().text = conText;
                }
                mem.transform.parent = team.transform.GetChild(2);
                if(nameEnabled && num.teams.Count > 1)
                {
                    mem.transform.GetChild(1).gameObject.SetActive(true);
                    for (int j = 0; j < num.teams.Count - 1; j++)
                    {
                        GameObject image = Instantiate(imagePrefab);
                        image.GetComponent<Image>().color = num.teams[j];
                        image.transform.parent = mem.transform.GetChild(1);
                    }
                }
            }
        }
        if (ep != null)
        {
            team.transform.parent = ep;
        } else
        {
            AddFinalVote(team);
        }
        float teamWidth = ConListWidth(team.transform.GetChild(2).childCount);
        team.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(teamWidth, team.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.y);
        team.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 1f);
        team.GetComponent<UIGroup>().allianceText.text = aText;
        team.GetComponent<UIGroup>().eventText.text = eText;
        if(spacing != 0)
        {
            team.GetComponent<VerticalLayoutGroup>().spacing = spacing;
        }
    }
}