using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text;
using SeasonParts;
using UnityEngine.UI.Extensions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Main script that simulates the season
    public Advantage ImmunityNecklace;
    public Advantage HiddenIdol;
    public SeasonTemplate seasonTemp;
    public List<ContestantEvent> events;
    public bool load;
    public List<Team> Tribes = new List<Team>();
    public List<Alliance> Alliances = new List<Alliance>();
    float nextEvent = 1;
    Contestant votedOff;
    List<Contestant> voters = new List<Contestant>();
    public List<Contestant> Exiled = new List<Contestant>();
    public float mergeAt, finaleAt, juryAt;
    public Button nextButton;
    public Button lastButton;
    public List<Contestant> immune =  new List<Contestant>();
    List<Contestant> targets;
    public List<Contestant> Eliminated;
    public float currentContestants;
    float currentContestantsOG;
    public bool merged, cineTribal, showVL, genderEqual, absorb;
    public GameObject Loading;
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
    public ExileIsland exileIsland;
    public ChallengeScript challenge;
    public int re;
    public int idols;
    public int curReward = 0, curImm = 0;
    int elimed = 1;
    int jurt;
    bool what = false;
    bool all = false;
    bool placement = false;
    [HideInInspector] public bool RIExpired = false, OCExpired = false, e = false, advant = false;
    [HideInInspector] public Contestant lastEOE = null, kidnapped = null;
    [HideInInspector] public Team Outcasts = new Team();
    [HideInInspector] public List<Contestant> jury = new List<Contestant>(), RIsland = new List<Contestant>(), EOE = new List<Contestant>(), tie = new List<Contestant>(), votesRead = new List<Contestant>(), votes = new List<Contestant>(), Idols = new List<Contestant>(), extraVote = new List<Contestant>();
    [HideInInspector] public List<Vote> Votes = new List<Vote>();
    [HideInInspector] public int curEv = 0, curEp = 0;
    [HideInInspector] public Dictionary<Contestant, int> dic = new Dictionary<Contestant, int>(), dicVR = new Dictionary<Contestant, int>();
    [HideInInspector] public Team MergedTribe = new Team();
    [HideInInspector] public Team LosingTribe = new Team();
    [HideInInspector] public List<Team> LosingTribes = new List<Team>();
    [HideInInspector] public string finalVotes;
    [HideInInspector] public float mergeRound = 0;
    [HideInInspector] public HiddenAdvantage EOEGiveAway;
    // Start is called before the first frame update
    public void Start()
    {
        if(load)
        {
            StartCoroutine(SetUp());
        } else
        {
            PlaySeason();
        }
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
                if(em.obj.name == "Placements")
                {
                    em.obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 1f);
                }
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
        List<Contestant> newCast = new List<Contestant>();
        for(int i = 0; i < Tribes.Count; i++)
        {
            for(int j = 0; j < Tribes[i].members.Count; j++)
            {
                Tribes[i].members[j] = Instantiate(cast.cast[con]);
                Tribes[i].members[j].votes = 1;
                challenge.RandomizeStats(Tribes[i].members[j]);
                Tribes[i].members[j].stats.Stamina = Tribes[i].members[j].stats.Stamina * 20;
                newCast.Add(Tribes[i].members[j]);
                con++;
            }
        }
        cast.cast = newCast;
    }
    //Function that creates the events for each episode.
    void CreateEpisodeSettings()
    {
        float curCon = currentContestants;
        float episodeCount = currentContestants - finaleAt;
        foreach (OneTimeEvent timeEvent in sea.oneTimeEvents)
        {
            if (timeEvent.type.Contains("MultiTribal") || timeEvent.type == "DoubleElim" || timeEvent.type == "MergeSplit")
            {
                episodeCount--;
            }
            if(timeEvent.type == "JurorRemoval")
            {
                episodeCount++;
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
        int episodeName = 1;
        int part = 1;
        foreach (OneTimeEvent timeEvent in sea.oneTimeEvents)
        {
            if (timeEvent.type == "FirstImpressions" && timeEvent.context == "RI")
            {
                episodeCount-= 2;
            }
        }
        if (sea.forcedFireMaking)
        {
            OneTimeEvent timedEvent = new OneTimeEvent();
            timedEvent.type = "ForcedFireMaking";
            if (sea.final == 2)
            {
                timedEvent.round = (int)episodeCount - 1;
            }
            else
            {
                timedEvent.round = (int)episodeCount;
            }
            sea.oneTimeEvents.Add(timedEvent);
        }    
        float curTeams = sea.Tribes.Count;
        int curSE = 0;
        bool riE = false;
        bool oc = false;
        for(int i = 0; i < episodeCount; i++)
        {
            EpisodeSetting ep = new EpisodeSetting();
            ep.name = "Episode " + episodeName;
            ep.nickname = "Ep. " + episodeName;
            if(episodeName == episodeCount)
            {
                ep.name = "Finale";
                ep.nickname = "Finale";
            }
            episodeName++;
            foreach (int num in sea.twoParts)
            {
                if (num == episodeName - 1)
                {
                    episodeName--;
                    ep.name += ", Part " + part;
                    ep.nickname = "Ep. " + episodeName + ", Pt. " + part;
                    
                    if (episodeName + 1 == episodeCount - 1)
                    {
                        ep.name = "Finale, Part " + part;
                        ep.nickname = "Finale, Pt. " + part;
                    }
                    part++;
                    if (part > 2)
                    {
                        part = 1;
                        episodeName++;
                    }
                }
                
            }
            foreach (int num in sea.threeParts)
            {
                if (num == episodeName - 1)
                {
                    episodeName--;
                    ep.name += ", Part " + part;
                    ep.nickname = "Ep. " + episodeName + ", Pt. " + part;
                    if(episodeName + 2 == episodeCount - 1)
                    {
                        ep.name = "Finale, Part " + part;
                        ep.nickname = "Finale, Pt. " + part;
                    }
                    part++;
                    if (part > 3)
                    {
                        part = 1;
                        episodeName++;
                    }
                }
            }
            ep.swap.on = false;
            ep.exileIsland.on = false;
            if (curCon == currentContestants)
            {
                if (sea.ExileIslandd)
                {
                    bool skip = false;
                    foreach (int num in sea.Twists.epsSkipE)
                    {
                        if (num == i + 1)
                        {
                            skip = true;
                        }
                    }
                    if (i + 1 < sea.Twists.expireAt && skip == false)
                    {
                        ep.exileIsland = sea.Twists.preMergeEIsland;
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
                if(ep.Event.type != "")
                {
                    if(ep.Event.type == "PalauStart")
                    {
                        EpisodeSetting episode = new EpisodeSetting();
                        episode.Event = ep.Event;
                        episode.events.Add("PalauStart");
                        Episodes.Add(episode);
                    } else if (ep.Event.type == "FijiStart")
                    {
                        ep.events.Add("FijiStart");
                    } else if(ep.Event.type != "DoubleElim")
                    {
                        if(ep.Event.type != "SchoolyardPick")
                        {
                            ep.events.Add("NextEp");
                        }
                        ep.events.Add("BeginningTwist");
                        if(ep.Event.type == "FirstImpressions" && ep.Event.context == "RI")
                        {
                            curCon -= 2;
                        }
                    }
                }
                ep.events.Add("NextEp");
                ep.events.Add("TribeStatus");
                if(sea.MedallionOfPower)
                {
                    ep.events.Add("MOPChallenge");
                }
                ep.events.Add("TribeImmunity");
                if (ep.exileIsland.on)
                {
                    ep.events.Add("ExileI");
                }
                ep.events.Add("TribeEvents");
                ep.events.Add("TribalCouncil");
                ep.events.Add("ShowVotes");
                if (ep.Event.type.Contains("DoubleElim"))
                {
                    ep.events.Add("TribalCouncil");
                    ep.events.Add("ShowVotes");
                    curCon--;
                }
            }
            else if (curCon > mergeAt)
            {
                if(sea.ExileIslandd)
                {
                    bool skip = false;
                    foreach(int num in sea.Twists.epsSkipE)
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
                        if(ep.swap.type != SwapType.Mutiny && ep.swap.type != SwapType.RegularSwap && ep.swap.type != SwapType.SplitTribes && ep.swap.type != SwapType.RegularSwap || (!ep.swap.ResizeTribes &&  ep.swap.type == SwapType.RegularShuffle))
                        {
                            curTeams = ep.swap.newTribes.Count;
                        }
                        
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
                if((sea.RedemptionIsland && i + 1 > 2) || (Episodes[Episodes.Count - 1].Event.type == "FirstImpressions" && Episodes[Episodes.Count - 1].Event.context == "RI"))
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
                }
                else
                {
                    ep.events.Add("TribeImmunity");
                }
                
                if (ep.exileIsland.on)
                {
                    ep.events.Add("ExileI");
                }
                ep.events.Add("TribeEvents");
                if (ep.Event.type.Contains("MultiTribal"))
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
                    foreach (int num in sea.Twists.epsSkipE)
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
                if(ep.Event.type == "MergeSplit" || ep.Event.type == "MergeSplitFiji")
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
                ep.events.Add("MergeEvents");
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
                    foreach (int num in sea.Twists.epsSkipE)
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
                if (ep.Event.type == "MergeSplit" || ep.Event.type == "JurorRemoval" || ep.Event.type == "MergeSplitFiji")
                {
                    ep.events.Add("STribeImmunity");
                    if(ep.Event.type == "JurorRemoval")
                    {
                        curCon++;
                    }
                }
                else
                {
                    ep.events.Add("MergeImmunity");
                }
                if (ep.exileIsland.on)
                {
                    ep.events.Add("ExileI");
                }
                ep.events.Add("MergeEvents");
                if (ep.Event.type != "JurorRemoval")
                {
                    ep.events.Add("TribalCouncil");
                    ep.events.Add("ShowVotes");
                }
                
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
                if (ep.Event.type == "ForcedFireMaking")
                {
                    ep = new EpisodeSetting();
                    ep.events.Add("NextEpM");
                    ep.events.Add("MergeStatus");
                    ep.events.Add("MergeImmunity");
                    ep.events.Add("ForcedFireMaking");
                    ep.events.Add("ShowVotes");
                    oc = true;
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
            extraVote = new List<Contestant>();
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
        if(curEp > Episodes.Count - 1)
        {
            Debug.Log("CurEp:" + curEp);
        }
        if(curEv > Episodes[curEp].events.Count - 1)
        {
            Debug.Log("CurEp:" + curEp + "curEv:" + curEv);
        }
        Invoke(Episodes[curEp].events[curEv], 0);
        curEv++;
        if (curEv >= Episodes[curEp].events.Count)
        {
            curEp++;
            curEv = 0;
            if(curEp > Episodes.Count - 1)
            {
                Loading.SetActive(false);
            }
        }
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
    void TribeEvents()
    {
        for (int i = 0; i < Tribes.Count; i++)
        {
            TribeEventss();
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
                VotedOffCine.transform.GetChild(0).GetChild(i).transform.SetParent(null);
            }
            foreach (GameObject torch in Torches)
            {
                torch.SetActive(false);
            }
            VotedOffCine.SetActive(false);
            VoteButton.SetActive(false);
            VotedOffCine.transform.parent.gameObject.SetActive(false);
            what = false;
        }
        if(what)
        { 
            if(Vote.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("voteRevealed"))
            {
                Vote.GetComponent<Animator>().SetTrigger("Reveal");
                VoteButton.SetActive(false);
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
            VotedOffCine.transform.parent.gameObject.SetActive(false);
            if(currentSeason.Episodes[curEp].events[curEv - 1].obj.name.Contains("Tribal Council"))
            {
                elimed++;
                if (currentContestants + 1 - finaleAt <= juryAt)
                {
                    jurt--;
                }
            }
            
            VoteButton.SetActive(false);
            what = false;
        }
        if(curEv - 1 != -1)
        {
            currentSeason.Episodes[curEp].events[curEv-1].obj.SetActive(false);
        }
        else
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
            what = true;
            int numb = currentSeason.Episodes[curEp].events.Count - 1;
            if (VotedOffCine.transform.GetChild(0).childCount > 0)
            {
                for (int i = 0; i < VotedOffCine.transform.GetChild(0).childCount; i++)
                {
                    VotedOffCine.transform.GetChild(0).GetChild(i).transform.SetParent(null);
                }
            }
            votes = currentSeason.Episodes[curEp].events[curEv].Vote;
            votesRead = currentSeason.Episodes[curEp].events[curEv].VotesRead;
            Idols = currentSeason.Episodes[curEp].events[curEv].Idols;

            foreach (GameObject torch in Torches)
            {
                torch.SetActive(true);
            }

            VoteButton.SetActive(true);
            lastVoteOff = currentSeason.Episodes[curEp].events[curEv].obj.transform.GetChild(0).GetChild(0).gameObject;
            
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
                if(!Idols.Contains(votesRead[0]))
                {
                    dicVR.Add(votesRead[0], 1);
                }

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
            if (curEp > Episodes.Count - 1)
            {
                nextButton.gameObject.SetActive(false);
            }
        }
    }
    public void LastGM()
    {
        if(curEp != 0 || curEv - 1 > 0)
        {
            curEv--;
            if (Vote.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("voteRevealed") || tri > 0)
            {
                Vote.GetComponent<Animator>().SetTrigger("Reveal");
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
                what = false;
            }
            if (what)
            {
                if (Vote.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("voteRevealed"))
                {
                    Vote.GetComponent<Animator>().SetTrigger("Reveal");
                    VoteButton.SetActive(false);
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
                VotedOffCine.transform.parent.gameObject.SetActive(false);

                VoteButton.SetActive(false);
                what = false;
            }
            if (curEv == -1)
            {
                curEp--;
                curEv = currentSeason.Episodes[curEp].events.Count - 1;
            }
            if(curEv == 0)
            {

            }
            currentSeason.Episodes[curEp].events[curEv].obj.SetActive(false);

            int number = curEv - 1;
            int ep = curEp;
            if(curEv != 0)
            {
                currentSeason.Episodes[curEp].events[curEv - 1].obj.SetActive(true);
            } else
            {
                currentSeason.Episodes[curEp - 1].events[currentSeason.Episodes[curEp - 1].events.Count - 1].obj.SetActive(true);
                number = currentSeason.Episodes[curEp - 1].events.Count - 1;
                ep = curEp - 1;
            }
            
            if (currentSeason.Episodes[ep].events[number].obj.name.Contains("Tribal Council") && cineTribal)
            {
                what = true;
                int numb = currentSeason.Episodes[ep].events.Count - 1;
                if (VotedOffCine.transform.GetChild(0).childCount > 0)
                {
                    for (int i = 0; i < VotedOffCine.transform.GetChild(0).childCount; i++)
                    {
                        VotedOffCine.transform.GetChild(0).GetChild(i).transform.SetParent(null);
                    }
                }
                votes = currentSeason.Episodes[ep].events[number].Vote;
                votesRead = currentSeason.Episodes[ep].events[number].VotesRead;
                Idols = currentSeason.Episodes[ep].events[number].Idols;

                foreach (GameObject torch in Torches)
                {
                    torch.SetActive(true);
                }
                //nextButton.gameObject.SetActive(false);
                VoteButton.SetActive(true);
                lastVoteOff = currentSeason.Episodes[ep].events[number].obj.transform.GetChild(0).GetChild(0).gameObject;
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
                if (currentSeason.Episodes[ep].events[number].VoteObjs.Count > 0)
                {
                    foreach (GameObject vot in currentSeason.Episodes[ep].events[number].VoteObjs)
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
                    if (!Idols.Contains(votesRead[0]))
                    {
                        dicVR.Add(votesRead[0], 1);
                    }

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
                    if (curTribal < currentSeason.Episodes[ep].votes.Count - 1)
                    {
                        foreach (Contestant num in currentSeason.Episodes[ep].votes[curTribal + 1])
                        {
                            if (tie.Contains(num))
                            {
                                re++;
                            }
                        }
                        if (re == currentSeason.Episodes[ep].votes[curTribal + 1].Count)
                        {
                            revoteNext = true;
                        }
                    }
                }
            }
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
    public void AddIdols(List<Contestant> gm)
    {
        int num = currentSeason.Episodes[curEpp].events.Count - 1;
        currentSeason.Episodes[curEpp].events[num].Idols = gm;
    }
    public void AddFinalVote(GameObject og)
    {
        int num = currentSeason.Episodes[curEpp].events.Count - 1;
        currentSeason.Episodes[curEpp].events[num].VoteObjs.Add(og);
    }
    void ShowCast()
    {
        GameObject EpisodeStart = Instantiate(Prefabs[0]);
        EpisodeStart.transform.parent = Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStart.name = "The Cast";
        AddGM(EpisodeStart, true);
        MakeGroup(false, null, "name", "", "", cast.cast, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
        NextEvent();
    }
    void NextEp()
    {
        List<Alliance> remove = new List<Alliance>();
        GameObject EpisodeStart = Instantiate(Prefabs[0]);
        EpisodeStart.transform.parent = Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        AddGM(EpisodeStart, true);
        foreach (Team team in Tribes)
        {
            foreach(Contestant num in team.members)
            {
                if (num.Relationships.Count == 0)
                {
                    List<Contestant> members = new List<Contestant>(cast.cast);
                    members.Remove(num);
                    foreach (Contestant con in members)
                    {
                        num.Relationships.Add(new Relationship() { person = con, Type=RelationshipType.Neutral, changeChance=10 });
                    }
                }
            }
        }
        
        for (int i = 0; i < Tribes.Count; i++)
        {
            //Debug.Log(tribe.name + ":" + string.Join(", ", tribe.members.ConvertAll(i => i.nickname)));
            MakeGroup(true, Tribes[i], "name", "", "", Tribes[i].members, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            foreach (Contestant num in Tribes[i].members)
            {
                List<Advantage> Remove = new List<Advantage>();
                if(num.advantages.Count > 0)
                {
                    foreach(Advantage advantage in num.advantages)
                    {
                        if(advantage.expiresAt > currentContestants || (advantage.length < 1 && advantage.temp) || (advantage.type == "ImmunityNecklace")) 
                        {
                            Remove.Add(advantage);
                        }
                    }
                }
                foreach(Advantage advantage in Remove)
                {
                    num.advantages.Remove(advantage);
                }
                num.altVotes = new List<Contestant>();
                num.inTie = false;
            }
        }
        remove = new List<Alliance>();
        foreach (Alliance alliance in Alliances)
        {
            if (alliance.members.Count < 2)
            {
                remove.Add(alliance);
            }
            foreach(Alliance all in Alliances)
            {
                if(all.members.Intersect(alliance.members).ToList().Count == alliance.members.Count && all != alliance)
                {
                    if(!remove.Contains(alliance))
                    {
                        alliance.name += "-" + all.name;
                        remove.Add(all);
                    }
                }
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
        GameObject EpisodeStatus = Instantiate(Prefabs[0]);
        EpisodeStatus.transform.parent = Canvas.transform;
        EpisodeStatus.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStatus.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStatus.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStatus.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStatus.name = Tribes[curT].name + " Status";
        if (curT == 0)
        {
            AddGM(EpisodeStatus, true);
        } else
        {
            AddGM(EpisodeStatus, false);
        }
        bool adv = false;
        float ed = 0;
        
        foreach (HiddenAdvantage hid in Tribes[curT].hiddenAdvantages)
        {
            if (hid.hideAt <= curEp + 1 && currentContestants >= hid.advantage.expiresAt)
            {
                if (!adv)
                {
                    adv = true;
                    MakeGroup(false, null, "", "<b>Advantages</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                }
                string nam = hid.name;
                if(!hid.name.Contains("Immunity Idol"))
                {
                    nam = "secret advantage";
                }
                string atext = "The " + nam + " is currently hidden.";
                if (!hid.hidden)
                {
                    if(hid.reHidden)
                    {
                        adv = true;
                        atext = "The " + nam + " is not currently hidden.";
                    } else
                    {
                        
                        atext = "";
                    }
                }
                if(atext != "")
                {
                    MakeGroup(false, null, "", atext, "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                }
                foreach (Contestant num in Tribes[curT].members)
                {
                    if (hid.hidden)
                    {
                        int ran = Random.Range(0, 2);
                        if (ran == 1)
                        {
                            Advantage av = Instantiate(hid.advantage);
                            av.nickname = hid.name;
                            if (hid.temp)
                            {
                                av.temp = true;
                                av.length = hid.length;
                            }
                            hid.hidden = false;
                            if (hid.advantage.type == "HalfIdol")
                            {
                                num.halfIdols.Add(num);
                            } else
                            {
                                num.advantages.Add(av);
                            }
                            List<Contestant> n = new List<Contestant>() { num };
                            MakeGroup(false, null, "", "", num.nickname + " finds the " + hid.name + "\n\n" + av.description, n, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                        }
                    }
                }
            }
        }
        
        List<Contestant> u = new List<Contestant>();
        foreach (Contestant num in Tribes[curT].members)
        {
            ContestantEvents.Instance.UpdateRelationships(num, Tribes[curT].members);
            List<Contestant> w = new List<Contestant>() { num };
            foreach (Advantage advantage in num.advantages)
            {
                if (!adv)
                {
                    adv = true;
                    MakeGroup(false, null, "", "<b>Advantages</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                }
                string extra = "";
                if (advantage.temp)
                {
                    if (advantage.length > 1)
                    {
                        extra = "\n \nThis can be used at the next " + advantage.length + " tribal councils.";
                    }
                    else
                    {
                        extra = "\n \nThis can be used at the next tribal council.";
                    }
                }
                if (currentContestants == advantage.expiresAt)
                {
                    extra = "\n \nThis is the last round to use it.";
                }
                int a = 0;
                if (advantage.onlyUsable.Count > 0)
                {
                    extra = "\n \nIt can't be used this round.";
                    foreach (int numb in advantage.onlyUsable)
                    {
                        if (currentContestants == numb)
                        {
                            a = numb;
                        }
                    }
                }
                else
                {
                    a = 0;
                }
                if(a != 0)
                {
                    if (a != advantage.onlyUsable[advantage.onlyUsable.Count - 1])
                    {
                        extra = "\n \nIt can be used this round.";
                    }
                    else
                    {
                        extra = "\n \nIt can be used this round.\n \nThis is the last round to use it.";
                    }
                }
                MakeGroup(false, null, "", "", num.nickname + " has the " + advantage.nickname + extra, w, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
            }
            int comb = 0;
            foreach(Contestant half in num.halfIdols)
            {
                u = new List<Contestant>() { half };
                if (num.halfIdols.Count > 1)
                {
                    if(Tribes[curT].members.Contains(half))
                    {
                        comb++;
                    }
                } else
                {
                    MakeGroup(false, null, "", "", num.nickname + " has the Half Idol.", u, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                }
                adv = true;
            }
            if(num.halfIdols.Count > 1)
            {
                if (comb == 2)
                {
                    foreach (Contestant half in num.halfIdols)
                    {
                        u = new List<Contestant>() { half };
                        MakeGroup(false, null, "", "", num.nickname + " has the Half Idol that is ready to be combined into a full idol.", u, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        num.halfIdols.Reverse();
                    }
                    
                    MakeGroup(false, null, "", "", num.halfIdols[1].nickname + " lets " + num.halfIdols[0].nickname + " have the Combined Hidden Immunity Idol.", num.halfIdols, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                    Advantage av = Instantiate(HiddenIdol);
                    av.nickname = "Combined Hidden Immunity Idol";
                    num.halfIdols[0].advantages.Add(av);
                    if(Tribes[curT].members.IndexOf(num.halfIdols[0]) < Tribes[curT].members.IndexOf(num) || Tribes[curT].members.IndexOf(num.halfIdols[0]) == Tribes[curT].members.IndexOf(num))
                    {
                        List<Contestant> ww = new List<Contestant>() { num.halfIdols[0] };
                        MakeGroup(false, null, "", "", num.halfIdols[0].nickname + " has the " + av.nickname, ww, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                    }
                    num.halfIdols = new List<Contestant>();
                } else
                {
                    num.halfIdols = new List<Contestant>();
                }
            }
            
            if (num.halfIdols.Count == 1)
            {
                List<Contestant> TribeV = new List<Contestant>(Tribes[curT].members);
                TribeV.Remove(num);
                num.halfIdols.Add(TribeV[Random.Range(0, TribeV.Count)]);
                List<Contestant> ex = new List<Contestant>();
                num.halfIdols.Reverse();
                MakeGroup(false, null, "", "", num.nickname + " transfers the half idol to " + num.halfIdols[0].nickname, num.halfIdols, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                num.halfIdols.Reverse();
            }
        }
        if (!adv && advant)
        {
            MakeGroup(false, null, "", "There are no secret advantages.", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
        }
        foreach (Alliance alliance in Alliances)
        {
            if (alliance.teams.Contains(Tribes[curT].name))
            {
                ed++;
            }
        }
        float d = 0;
        MakeGroup(false, null, "", "<b>Alliances</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
        all = true;
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
                MakeGroup(false, null, "name", alliance.name, "", alliance.members, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                if(ed < 2 && !adv && Tribes[curT].hiddenAdvantages.Count < 1)
                {
                    EpisodeStatus.transform.GetChild(0).GetComponent<VerticalLayoutGroup>().spacing = -90;
                } else
                {

                }
                
                d++;
            }
        }
        all = false;
        if (d == 0)
        {
            MakeGroup(false, null, "", "There are no alliances", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), 0);
        }
        lastThing = EpisodeStatus;
        if(curT >= Tribes.Count - 1)
        {
            curT = 0;
            tri = 0;
        }
        else
        {
            curT++;
        }
    }
    void TribeEventss()
    {
        GameObject EpisodeStatus = Instantiate(Prefabs[0]);
        EpisodeStatus.transform.parent = Canvas.transform;
        EpisodeStatus.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStatus.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStatus.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStatus.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStatus.name = Tribes[curT].name + " Status";
        if (curT == 0)
        {
            AddGM(EpisodeStatus, true);
        }
        else
        {
            AddGM(EpisodeStatus, false);
        }
        bool adv = false;

        foreach (HiddenAdvantage hid in Tribes[curT].hiddenAdvantages)
        {
            if (hid.hideAt <= curEp + 1 && currentContestants >= hid.advantage.expiresAt)
            {
                if (!adv)
                {
                    adv = true;
                    MakeGroup(false, null, "", "<b>Advantages</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                }
                string nam = hid.name;
                if (!hid.name.Contains("Immunity Idol"))
                {
                    nam = "secret advantage";
                }
                string atext = "The " + nam + " is currently hidden.";
                if (!hid.hidden)
                {
                    if (hid.reHidden)
                    {
                        adv = true;
                        atext = "The " + nam + " is not currently hidden.";
                    }
                    else
                    {

                        atext = "";
                    }
                }
                if (atext != "")
                {
                    MakeGroup(false, null, "", atext, "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                }
                foreach (Contestant num in Tribes[curT].members)
                {
                    if (hid.hidden)
                    {
                        int ran = Random.Range(0, 2);
                        if (ran == 1)
                        {
                            Advantage av = Instantiate(hid.advantage);
                            av.nickname = hid.name;
                            if (hid.temp)
                            {
                                av.temp = true;
                                av.length = hid.length;
                            }
                            hid.hidden = false;
                            if (hid.advantage.type == "HalfIdol")
                            {
                                num.halfIdols.Add(num);
                            }
                            else
                            {
                                num.advantages.Add(av);
                            }
                            List<Contestant> n = new List<Contestant>() { num };
                            MakeGroup(false, null, "", "", num.nickname + " finds the " + hid.name + "\n\n" + av.description, n, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                        }
                    }
                }
            }
        }

        List<Contestant> u = new List<Contestant>();
        foreach (Contestant num in Tribes[curT].members)
        {
            num.stats.Stamina -= 2;

            List<Contestant> w = new List<Contestant>() { num };
            foreach (Advantage advantage in num.advantages)
            {
                if (!adv)
                {
                    adv = true;
                    MakeGroup(false, null, "", "<b>Advantages</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                }
                string extra = "";
                if (advantage.temp)
                {
                    if (advantage.length > 1)
                    {
                        extra = "\n \nThis can be used at the next " + advantage.length + " tribal councils.";
                    }
                    else
                    {
                        extra = "\n \nThis can be used at the next tribal council.";
                    }
                }
                if (currentContestants == advantage.expiresAt)
                {
                    extra = "\n \nThis is the last round to use it.";
                }
                int a = 0;
                if (advantage.onlyUsable.Count > 0)
                {
                    extra = "\n \nIt can't be used this round.";
                    foreach (int numb in advantage.onlyUsable)
                    {
                        if (currentContestants == numb)
                        {
                            a = numb;
                        }
                    }
                }
                else
                {
                    a = 0;
                }
                if (a != 0)
                {
                    if (a != advantage.onlyUsable[advantage.onlyUsable.Count - 1])
                    {
                        extra = "\n \nIt can be used this round.";
                    }
                    else
                    {
                        extra = "\n \nIt can be used this round.\n \nThis is the last round to use it.";
                    }
                }
                MakeGroup(false, null, "", "", num.nickname + " has the " + advantage.nickname + extra, w, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
            }
            int comb = 0;
            foreach (Contestant half in num.halfIdols)
            {
                if (!adv)
                {
                    adv = true;
                    MakeGroup(false, null, "", "<b>Advantages</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                }
                u = new List<Contestant>() { half };
                if (num.halfIdols.Count > 1)
                {
                    if (Tribes[curT].members.Contains(half))
                    {
                        comb++;
                    }
                }
                else
                {
                    MakeGroup(false, null, "", "", num.nickname + " has the Half Idol.", u, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                }
                adv = true;
            }
            if (num.halfIdols.Count > 1)
            {
                if (comb == 2)
                {
                    foreach (Contestant half in num.halfIdols)
                    {
                        u = new List<Contestant>() { half };
                        MakeGroup(false, null, "", "", num.nickname + " has the Half Idol that is ready to be combined into a full idol.", u, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        num.halfIdols.Reverse();
                    }

                    MakeGroup(false, null, "", "", num.halfIdols[1].nickname + " lets " + num.halfIdols[0].nickname + " have the Combined Hidden Immunity Idol.", num.halfIdols, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                    Advantage av = Instantiate(HiddenIdol);
                    av.nickname = "Combined Hidden Immunity Idol";
                    num.halfIdols[0].advantages.Add(av);
                    if (Tribes[curT].members.IndexOf(num.halfIdols[0]) < Tribes[curT].members.IndexOf(num) || Tribes[curT].members.IndexOf(num.halfIdols[0]) == Tribes[curT].members.IndexOf(num))
                    {
                        List<Contestant> ww = new List<Contestant>() { num.halfIdols[0] };
                        MakeGroup(false, null, "", "", num.halfIdols[0].nickname + " has the " + av.nickname, ww, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                    }
                    num.halfIdols = new List<Contestant>();
                }
                else
                {
                    num.halfIdols = new List<Contestant>();
                }
            }

            if (num.halfIdols.Count == 1)
            {
                List<Contestant> TribeV = new List<Contestant>(Tribes[curT].members);
                TribeV.Remove(num);
                num.halfIdols.Add(TribeV[Random.Range(0, TribeV.Count)]);
                List<Contestant> ex = new List<Contestant>();
                num.halfIdols.Reverse();
                MakeGroup(false, null, "", "", num.nickname + " transfers the half idol to " + num.halfIdols[0].nickname, num.halfIdols, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                num.halfIdols.Reverse();
            }
        }
        if (!adv && advant)
        {
            MakeGroup(false, null, "", "There are no secret advantages.", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
        }

        bool eventt = false;

        int eventCap = 0;
        
        foreach (ContestantEvent Event in events)
        {
            int eventTimes = 100;
            List<Contestant> tribe = new List<Contestant>(Tribes[curT].members);
            bool waht = false;
            
            switch (Event.type)
            {
                case EventType.Stamina:
                    eventTimes = 3;
                    if (Event.staminaAffect > 0)
                    {
                        tribe = tribe.OrderBy(x => x.stats.Stamina).ToList();
                    } else
                    {
                        tribe = tribe.OrderByDescending(x => x.stats.Stamina).ToList();
                    }
                    foreach (Contestant num in tribe)
                    {
                        //&& !Event.type.Contains("Alliance")
                        //bool what = ContestantEvents.Instance.EventChance(Event, null, num);
                        if (ContestantEvents.Instance.EventChance(Event, new List<Contestant>(), num) == true && Random.Range(0, eventTimes) == 0 && eventCap < 11)
                        {
                            if (!eventt)
                            {
                                MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                                eventt = true;
                            }
                            ContestantEvents.Instance.DoEvent(Event, null, null, num, EpisodeStatus.transform.GetChild(0).GetChild(0));
                            eventTimes += eventTimes;
                            eventCap++;
                        }
                    }
                    break;
                case EventType.Relationship:
                    if (Event.relationshipAffect > 0)
                    {
                        tribe = tribe.OrderBy(x => ChallengeScript.Instance.GetPoints(x, Event.stats)).ToList();
                    }
                    else
                    {
                        tribe = tribe.OrderByDescending(x => ChallengeScript.Instance.GetPoints(x, Event.stats)).ToList();
                    }
                    //eventTimes = 50;
                    if(Event.overall)
                    {
                        foreach (Contestant num in tribe)
                        {
                            if (eventCap < 11 && Random.Range(0, 5 + eventCap) == 0)
                            {
                                waht = true;
                            }
                            //&& !Event.type.Contains("Alliance")
                            //bool what = ContestantEvents.Instance.EventChance(Event, null, num);

                            if (ContestantEvents.Instance.EventChance(Event, new List<Contestant>(), num) == true && Random.Range(0, eventTimes) == 0 && waht)
                            {
                                if (!eventt)
                                {
                                    MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                                    eventt = true;
                                }
                                ContestantEvents.Instance.DoEvent(Event, tribe, null, num, EpisodeStatus.transform.GetChild(0).GetChild(0));
                                eventTimes += eventTimes;
                                eventCap++;
                            }
                        }
                    } else
                    {
                        foreach (Contestant num in tribe)
                        {
                            if (eventCap < 11 && Random.Range(0, 5 + eventCap) == 0)
                            {
                                waht = true;
                            }
                            if (Event.relationshipAffect < 0)
                            {
                                num.Relationships = num.Relationships.OrderBy(x => x.Type).ThenByDescending(x => (int)x.Status * 10 + x.Extra).Where(x => tribe.Contains(x.person)).ToList();
                            }
                            else
                            {
                                num.Relationships = num.Relationships.OrderByDescending(x => x.Type).ThenByDescending(x => (int)x.Status * 10 + x.Extra).Where(x => tribe.Contains(x.person)).ToList();
                            }
                            foreach (Relationship re in num.Relationships)
                            {
                                if (ContestantEvents.Instance.EventChance(Event, new List<Contestant>(), num) == true && Random.Range(0, eventTimes) == 0 && waht && tribe.Contains(re.person))
                                {
                                    if (!eventt)
                                    {
                                        MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                                        eventt = true;
                                    }
                                    ContestantEvents.Instance.DoEvent(Event, new List<Contestant>() { re.person }, null, num, EpisodeStatus.transform.GetChild(0).GetChild(0));
                                    eventTimes += eventTimes;
                                    eventCap++;
                                }
                            }
                        }
                    }
                    
                    break;
                case EventType.Alliance:
                    eventTimes = 3;
                    switch (Event.allianceEvent)
                    {
                        case AllianceEventType.Create:
                            eventTimes = 2;
                            List<Alliance> AddAlliance = new List<Alliance>();
                            tribe = tribe.OrderByDescending(x => ChallengeScript.Instance.GetPoints(x, Event.stats)).ToList();
                            foreach (Contestant num in tribe)
                            {
                                ContestantEvents.Instance.join = false;
                                Alliance newAlliance = new Alliance();
                                newAlliance.members.Add(num);
                                num.Relationships = num.Relationships.OrderByDescending(x => x.Type).ThenByDescending(x => (int)x.Status * 10 + x.Extra).Where(x => tribe.Contains(x.person)).ToList();
                                foreach (Relationship re in num.Relationships)
                                {
                                    if (Tribes[curT].members.Contains(re.person) && !newAlliance.members.Contains(re.person) && re.person != num)
                                    {
                                        int stat = ContestantEvents.Instance.GetLoyalty(num, new List<Contestant>() { re.person });
                                        if (Random.Range(1, 11) <= stat)
                                        {
                                            newAlliance.members.Add(re.person);
                                        }
                                    }
                                }
                                if (eventCap < 11 && Random.Range(0, 5 + eventCap) == 0 && newAlliance.members.Count > 1)
                                {
                                    
                                    waht = true;
                                }
                                Contestant main = new Contestant();
                                foreach (Alliance alliance in Alliances)
                                {
                                    if (alliance.members.Where(x => newAlliance.members.Contains(x)).ToList().Count == alliance.members.Count)
                                    {
                                        waht = false;
                                    }
                                    else if (alliance.members.Where(x => newAlliance.members.Contains(x)).ToList().Count == alliance.members.Count - 1)
                                    {
                                        ContestantEvents.Instance.join = true;
                                        newAlliance.members = newAlliance.members.OrderBy(x => alliance.members.Contains(x)).ToList();
                                        main = newAlliance.members[newAlliance.members.Count - 1];
                                        alliance.members = newAlliance.members;
                                        newAlliance.name = alliance.name;
                                    }
                                }
                                if (ContestantEvents.Instance.EventChance(Event, newAlliance.members, num) == true && waht && Random.Range(0, eventTimes) == 0)
                                {
                                    if (!ContestantEvents.Instance.join)
                                    {
                                        AddAlliance.Add(newAlliance);
                                        Tribes[curT].allianceCount++;
                                        newAlliance.name = Tribes[curT].name + " Alliance #" + Tribes[curT].allianceCount;
                                    }
                                    
                                    if (!eventt)
                                    {
                                        MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                                        eventt = true;
                                    }
                                    ContestantEvents.Instance.DoEvent(Event, newAlliance.members, newAlliance, main, EpisodeStatus.transform.GetChild(0).GetChild(0));
                                    eventTimes += eventTimes; eventCap++;
                                } 
                            }
                            foreach (Alliance alliance in AddAlliance)
                            {
                                alliance.teams.Add(Tribes[curT].name);
                                Alliances.Add(alliance);
                            }
                            break;
                        case AllianceEventType.Dissolve:
                            List<Alliance> RemoveAlliance = new List<Alliance>();
                            foreach(Alliance alliance in Alliances)
                            {
                                if(alliance.teams.Contains(Tribes[curT].name))
                                {
                                    if (eventCap < 11 && Random.Range(0, 5 + eventCap) == 0)
                                    {
                                        waht = true;
                                    }
                                    if (ContestantEvents.Instance.EventChance(Event, alliance.members, null) == true && Random.Range(0, eventTimes) == 0 && waht)
                                    {
                                        if (!eventt)
                                        {
                                            MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                                            eventt = true;
                                        }
                                        ContestantEvents.Instance.DoEvent(Event, alliance.members, alliance, new Contestant(), EpisodeStatus.transform.GetChild(0).GetChild(0));
                                        eventTimes += eventTimes; eventCap++;
                                        RemoveAlliance.Add(alliance);
                                    }
                                }
                            }
                            foreach(Alliance ads in RemoveAlliance)
                            {
                                Alliances.Remove(ads);
                            }
                            break;
                        case AllianceEventType.Leave:
                            foreach (Alliance alliance in Alliances)
                            {
                                if (alliance.teams.Contains(Tribes[curT].name))
                                {
                                    List<Contestant> remove = new List<Contestant>();
                                    bool removed = false;
                                    foreach (Contestant num in alliance.members)
                                    {
                                        if (eventCap < 11 && Random.Range(0, 5 + eventCap) == 0)
                                        {
                                            waht = true;
                                        }
                                        if (ContestantEvents.Instance.EventChance(Event, alliance.members, num) == true && Random.Range(0, eventTimes) == 0 && waht & !removed)
                                        {
                                            if (!eventt)
                                            {
                                                MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                                                eventt = true;
                                            }
                                            ContestantEvents.Instance.DoEvent(Event, alliance.members, alliance, num, EpisodeStatus.transform.GetChild(0).GetChild(0));
                                            eventTimes += eventTimes; eventCap++;
                                            remove.Add(num);
                                            removed = true;
                                        }
                                    }
                                    foreach(Contestant num in remove)
                                    {
                                        alliance.members.Remove(num);
                                    }
                                }
                            }
                            break;
                    }
                    break;
            }
        }
        if(!eventt)
        {
            MakeGroup(false, null, "", "No events occured.", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
        }
        List<Alliance> removee = new List<Alliance>();
        foreach (Alliance alliance in Alliances)
        {
            if (alliance.members.Count < 2)
            {
                removee.Add(alliance);
            }
        }
        foreach (Alliance alliance in removee)
        {
            Alliances.Remove(alliance);
        }
        if (curT >= Tribes.Count - 1)
        {
            curT = 0;
            tri = 0;
        }
        else
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
        LosingTribes = new List<Team>();
        List<Team> TribesV = new List<Team>(Tribes);
        int ran = Random.Range(0, TribesV.Count);
        LosingTribes.Add(TribesV[ran]);
        //lastThing.SetActive(false);
        challenge.TribeChallenge(Tribes, sea.RewardChallenges[curReward].stats, Tribes.Count - 1);

        foreach (Team tribe in Tribes)
        {
            if (!LosingTribes.Contains(tribe))
            {
                MakeGroup(false, null, "", "", tribe.name + " Wins Reward!", new List<Contestant>(), EpisodeRe.transform.GetChild(0), 0);
                foreach(Contestant num in tribe.members)
                {
                    num.stats.Stamina += sea.RewardChallenges[curReward].rewardStamina;
                }
            }
        }
        curReward++;
        LosingTribes = new List<Team>();
    }
    void TribeImmunity()
    {
        string island = "";
        if (sea.IslandType == "Ghost")
        {
            island = "Ghost Island";
        }
        else if (sea.IslandType == "IOI")
        {
            island = "Island of the Idols";
        }
        else
        {
            island = "Exile Island";
        }
        if(immune.Count > 0)
        {
            foreach(Contestant num in immune)
            {
                num.advantages.Remove(ImmunityNecklace);
            }
            immune = new List<Contestant>();
        }
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
        if (curExile.on && curExile.challenge == "Immunity" && curExile.reason == "SitOut")
        {
            string safe = "";
            if(curExile.exileEvent == "Safety")
            {
                safe = "\n\nIf their tribe loses, they will return for Tribal Council but be immune from the vote.";
            }
            MakeGroup(false, null, "", "", "Each tribe must send one player from the opposite to Exile Island. They will sit out the challenge." + safe, new List<Contestant>(), EpisodeImm.transform.GetChild(0), 0);
            foreach (Team tribe in Tribes)
            {
                Contestant bye = tribe.members[Random.Range(0, tribe.members.Count)];
                bye.team = tribe.name;
                Exiled.Add(bye);
                tribe.members.Remove(bye);
            }
            MakeGroup(false, null, "nname", "", "The people sitting out are chosen.", Exiled, EpisodeImm.transform.GetChild(0), 0);
        }
        foreach (Team tribe in Tribes)
        {
            foreach (Contestant num in tribe.members)
            {
                if (num.IOIEvent == "CallerVB")
                {
                    if (Random.Range(0, 2) == 1)
                    {
                        MakeGroup(false, null, "", "", num.nickname + " is selected as caller for their tribe.\n\n" + "They win the " + num.savedAdv.name + "!", new List<Contestant>() { num }, EpisodeImm.transform.GetChild(0), 20);
                        Advantage av = Instantiate(num.savedAdv.advantage);
                        av.nickname = num.savedAdv.name;
                        if (num.savedAdv.temp)
                        {
                            av.temp = true;
                            av.length = num.savedAdv.length;
                        }
                        num.advantages.Add(av);
                    }
                    else
                    {
                        MakeGroup(false, null, "", "", num.nickname + " is not selected as caller for their tribe.\n\n" + num.nickname + " loses their vote at the next tribal council.", new List<Contestant>() { num }, EpisodeImm.transform.GetChild(0), 20);
                        num.votes--;
                    }
                    num.IOIEvent = "";
                }
            }
        }
        LosingTribes = new List<Team>();
        List<Team> TribesV = new List<Team>(Tribes);
        if(curEp +1 == sea.MOPExpire && sea.MedallionOfPower)
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
                etext += "\n\n" + MOP + " gets the Medallion of Power.";
            } else
            {
                etext = MOP + " doesn't use the Medallion of Power.";
            }
            MakeGroup(false, null, "", "", etext, new List<Contestant>(), EpisodeImm.transform.GetChild(0), 0);
        }
        //int ran = Random.Range(0, TribesV.Count);
        //LosingTribes.Add(TribesV[ran]);
        if(curImm <= sea.ImmunityChallenges.Count - 1)
        {
            challenge.TribeChallenge(Tribes, sea.ImmunityChallenges[curImm].stats, Tribes.Count - 1);
        } else
        {
            challenge.TribeChallenge(Tribes, new List<StatChoice>() { StatChoice.Physical, StatChoice.Mental, StatChoice.Endurance }, Tribes.Count - 1);
        }
        
        
        foreach (Team tribe in Tribes)
        {
            if (!LosingTribes.Contains(tribe))
            {
                MakeGroup(false, tribe, "", "", tribe.name + " Wins Immunity!", tribe.members, EpisodeImm.transform.GetChild(0), 20);
            } else
            {
                if(extraVote.Count > 0)
                {
                    foreach(Contestant num in tribe.members)
                    {
                        if(extraVote.Contains(num))
                        {
                            Advantage av = Instantiate(EOEGiveAway.advantage);
                            av.nickname = EOEGiveAway.name;
                            if (EOEGiveAway.temp)
                            {
                                av.temp = true;
                                av.length = EOEGiveAway.length;
                            }
                            EOEGiveAway.hidden = false;
                            if (EOEGiveAway.advantage.type == "HalfIdol")
                            {
                                num.halfIdols.Add(num);
                            }
                            else
                            {
                                num.advantages.Add(av);
                            }
                        }
                    }
                }
            }
        }
        
        lastThing = EpisodeImm;
        if(curExile.on && curExile.challenge == "Immunity" && curExile.reason != "")
        {
            if (curExile.reason == "Winner" || curExile.reason == "Loser")
            {
                string reason;
                string reason2;
                string extra = "";
                if (curExile.ownTribe)
                {
                    List<Team> teams = new List<Team>(Tribes);
                    if (curExile.reason == "Winner")
                    {
                        foreach (Team t in LosingTribes)
                        {
                            teams.Remove(t);
                        }
                        reason2 = "The winning team can send someone from their tribe to " + island + ".";
                        reason = " is sent to " + island + " by the winning team.";
                        if (curExile.both)
                        {
                            reason2 = "The winning team can send someone from each tribe to " + island + ".";
                        }
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
                        reason2 = "The losing tribe can send someone from their tribe to " + island + ".";
                        reason = " is sent to " + island + " by the losing team.";
                        if (curExile.both)
                        {
                            reason2 = "The losing team can send someone from each tribe to " + island + ".";
                        }
                        if (curExile.skipTribal)
                        {
                            reason2 += "\n\nThis player will not attend tribal council.";
                            if (curExile.both)
                            {
                                extra = "\n\nThis player will not attend tribal council.";
                            }
                        } 
                    }
                    
                    
                    int rann = Random.Range(0, teams[0].members.Count);
                    teams[0].members[rann].team = teams[0].name;
                    if (curExile.both)
                    {
                        reason =  " from " + teams[0].members[rann].team + "." + extra;
                        extra = "They choose ";
                    }
                    Exiled.Add(teams[0].members[rann]);
                    List<Contestant> g = new List<Contestant>() { teams[0].members[rann] };
                    MakeGroup(false, null, "", reason2, extra + teams[0].members[rann].nickname + reason, g, EpisodeImm.transform.GetChild(0), 20);
                    teams[0].members.Remove(teams[0].members[rann]);

                    teams = new List<Team>(Tribes);
                    if (curExile.two)
                    {
                        extra = "";
                        if (curExile.reason == "Winner")
                        {
                            foreach (Team t in Tribes)
                            {
                                if (t != LosingTribes[0])
                                {
                                    teams.Remove(t);
                                }
                            }
                            
                            reason2 = Exiled[0].nickname + " can send someone from the losing tribe to " + island + ".";
                            reason = " is sent to " + island + " by " + Exiled[0].nickname;
                            
                            if (curExile.skipTribal)
                            {
                                reason2 += "\n\nThis player will not attend tribal council.";
                                if(curExile.both)
                                {
                                    extra = "\n\nThis player will not attend tribal council.";
                                }
                            }
                        }
                        else
                        {
                            foreach (Team t in LosingTribes)
                            {
                                teams.Remove(t);
                            }
                            reason2 = Exiled[0].nickname + " can send someone from the winning tribe to " + island + ".";
                            reason = " is sent to " + island + " by " + Exiled[0].nickname;
                        }
                        int rannn = Random.Range(0, teams[0].members.Count);
                        teams[0].members[rannn].team = teams[0].name;
                        if (curExile.both)
                        {
                            reason2 = "";
                            reason = " from " + teams[0].members[rannn].team + "." + extra;
                            extra = "They choose ";
                        }
                        Exiled.Add(teams[0].members[rannn]);
                        g = new List<Contestant>() { teams[0].members[rannn] };
                        MakeGroup(false, null, "", reason2, extra + teams[0].members[rannn].nickname + reason, g, EpisodeImm.transform.GetChild(0), 20);
                        teams[0].members.Remove(teams[0].members[rannn]);
                    }
                }
                else
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
                        reason2 = "The winning team can send someone from the losing tribe to " + island + ".";
                        reason = " is sent to " + island + " by the winning team.";

                        if (curExile.skipTribal)
                        {
                            reason2 += "\n\nThis player will not attend tribal council.";
                        }
                    }
                    else
                    {
                        foreach (Team t in LosingTribes)
                        {
                            teams.Remove(t);
                        }
                        reason2 = "The losing team can send someone from the winning tribe to " + island + ".";
                        reason = " is sent to " + island + " by the losing team.";

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
                            reason = " is sent to " + island + " by " + Exiled[0].nickname;
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
                            reason = " is sent to " + island + " by " + Exiled[0].nickname;
                            if (curExile.skipTribal)
                            {
                                reason2 += "\n\nThis player will not attend tribal council.";
                            }
                        }

                        int rannn = Random.Range(0, LosingTribes[LosingTribes.Count - 1].members.Count);
                        
                        LosingTribes[LosingTribes.Count - 1].members[rannn].team = LosingTribes[LosingTribes.Count - 1].name;

                        Exiled.Add(LosingTribes[LosingTribes.Count - 1].members[rannn]);
                        g = new List<Contestant>() { LosingTribes[LosingTribes.Count - 1].members[rannn] };
                        MakeGroup(false, null, "", reason2, LosingTribes[LosingTribes.Count - 1].members[rannn].nickname + reason, g, EpisodeImm.transform.GetChild(0), 20);
                        LosingTribes[LosingTribes.Count - 1].members.Remove(LosingTribes[LosingTribes.Count - 1].members[rannn]);
                    }
                }
            }
        }
        else
        {
            if ((curSwap.on || curEvent.type == "FijiStart") && curExile.on && curExile.reason == "")
            {
                Exiled[0].team = LosingTribes[LosingTribes.Count - 1].name;
                if(curEvent.type == "FijiStart")
                {
                    Exiled[0].teams.Add(LosingTribes[LosingTribes.Count - 1].tribeColor);
                }
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
            List<Contestant> g = new List<Contestant>() { kidnapped };
            MakeGroup(false, null, "", teams[0].name + " can kidnap someone from the losing tribe.\n\nThis person will skip Tribal Council.", kidnapped.nickname + " is kidnapped.", g, EpisodeImm.transform.GetChild(0), 20);
        }
        else if (curEvent.type == "DoubleElim")
        {
            MakeGroup(false, null, "", "", LosingTribes[LosingTribes.Count -1].name + "  receive a message in a bottle to open after tribal council.", new List<Contestant>(), EpisodeImm.transform.GetChild(0), 0);
        }
        foreach(Team tribe in Tribes)
        {
            foreach (Contestant num in tribe.members)
            {
                if (num.IOIEvent == "HiddenVB")
                {
                    
                    if (Random.Range(0, 2) == 1)
                    {
                        MakeGroup(false, null, num.nickname + " finds the " + num.savedAdv.name + ".", "", "", new List<Contestant>() { num }, EpisodeImm.transform.GetChild(0), 20);
                        Advantage av = Instantiate(num.savedAdv.advantage);
                        av.nickname = num.savedAdv.name;
                        if (num.savedAdv.temp)
                        {
                            av.temp = true;
                            av.length = num.savedAdv.length;
                        }
                        num.advantages.Add(av);
                    }
                    else
                    {
                        MakeGroup(false, null, num.nickname + " doesn't find the " + num.savedAdv.name + ".", "", "", new List<Contestant>() { num }, EpisodeImm.transform.GetChild(0), 20);
                        num.votes--;
                    }
                    num.IOIEvent = "";
                }
            }
        }
        curImm++;
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
            if(curEvent.type == "MergeSplit" || curEvent.type == "MergeSplitFiji")
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
        bool aa = false;
        e = false;
        int tieNum = 2;
        Idols = new List<Contestant>();
        List<Contestant> nullIdols = new List<Contestant>();
        Votes = new List<Vote>();
        votes = new List<Contestant>();
        votesRead = new List<Contestant>();
        tie = new List<Contestant>();
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
                MakeGroup(false, team, "", "Before the vote, " + LosingTribes[0].name + " must kidnap one member of " + team.name + ". \n \nThis person will skip tribal.", LosingTribes[0].name + " kidnaps " + kidnapped.nickname + ".", g, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
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
            etext = "It's time to vote.";
            MakeGroup(true, team, "name", "", "", team.members, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
            List<Contestant> RRemove = new List<Contestant>();
            
            foreach (Contestant num in team.members)
            {
                if(num.safety > 0)
                {
                    List<Contestant> w = new List<Contestant>() { num };
                    MakeGroup(false, null, "", "", num.nickname + " has safety from the vote.", w, EpisodeStart.transform.GetChild(0).GetChild(0), 0);

                    immune.Add(num);
                    num.safety--;
                }
                if (num.advantages.Count > 0)
                {
                    List<Advantage> remove = new List<Advantage>();
                    List<Advantage> Add = new List<Advantage>();
                    foreach (Advantage advantage in num.advantages)
                    {
                        string extra = "";
                        if (currentContestants == advantage.expiresAt || (advantage.length == 1 && advantage.temp))
                        {
                            extra = "\n \nThis is the last round to use it.";
                        }
                        List<Contestant> w = new List<Contestant>() { num };
                        bool playable;
                        MakeGroup(false, null, "", "", num.nickname + " has the " + advantage.nickname + extra, w, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                        Contestant usedOn = null;
                        if (advantage.type == "ImmunityNecklace" || advantage.type == "VoteSteal" || advantage.type == "VoteBlocker")
                        {
                            List<Contestant> teamV = new List<Contestant>(team.members);
                            teamV.Remove(num);
                            if(advantage.type == "ImmunityNecklace")
                            {
                                foreach (Contestant numm in immune)
                                {
                                    teamV.Remove(numm);
                                }
                            }
                            
                            usedOn = teamV[Random.Range(0, teamV.Count)];
                        }
                        if (advantage.usedWhen == "BeforeVote" && (Random.Range(0, 10) == 1 || currentContestants == advantage.expiresAt || advantage.length == 1 || advantage.type == "VoteSacrifice"))
                        {
                            AdvantagePlay(EpisodeStart.transform.GetChild(0).GetChild(0), advantage, num, usedOn);
                            remove.Add(advantage);
                            if(advantage.type == "SafetyWithoutPower")
                            {
                                RRemove.Add(num);
                            } else if(advantage.type == "VoteSacrifice")
                            {
                                Advantage av = new Advantage();
                                av.expiresAt = 6;
                                av.nickname = "Extra Vote";
                                av.type = "ExtraVote";
                                Add.Add(av);
                            }
                        }
                    }
                    foreach (Advantage advantage in remove)
                    {
                        num.advantages.Remove(advantage);
                    }
                    foreach (Advantage advantage in Add)
                    {
                        num.advantages.Add(advantage);
                    }
                }
                
            }
            foreach (Contestant num in RRemove)
            {
                team.members.Remove(num);
            }

            MakeGroup(false, null, "", "", etext, new List<Contestant>(), EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            bool voted = Voting();
            if (voted)
            {
                CountVotes();

                AddVote(votes, votesRead);
                tie = new List<Contestant>();
                if(dic.Count > 0)
                {
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
                
            }
            else if (!voted)
            {
                Tribal(team);
            }
            if (tie.Count < tieNum && tie.Count > 0)
            {
                //Debug.Log(votedOff.nickname + " has been eliminated. " + "Votes: " + dic[votedOff]);
                if(tie.Count < 2)
                {
                    Eliminate();
                }
            }
            else
            {
                aa = true;
                bool rea = false;
                while(!rea)
                {
                    tri++;
                    if(tri == 2)
                    {
                        //Debug.Log("dfsa");
                    }
                    rea = RE();
                }
            }
        }
        else if(team.members.Count == 2)
        {
            MakeGroup(true, team, "name", "", "", team.members, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
            
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
            vote = "Eliminated by Default";
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
                        List<Contestant> teamV = new List<Contestant>();
                        foreach (Contestant num in all.members)
                        {
                            if(team.members.Except(all.members).Except(immune).ToList().Count > 0)
                            {
                                teamV.Add(num.PersonalTarget(team.members.Except(all.members).Except(immune).ToList()));
                            }

                        }
                        if (immune != null)
                        {
                            foreach (Contestant num in immune)
                            {
                                //teamV.Remove(num);
                            }
                        }
                        if (teamV.Count > 0)
                        {
                            int ran = Random.Range(0, teamV.Count);
                            target = teamV[ran];
                            all.target = target;
                        } else
                        {
                            int ran = Random.Range(0, teamV.Count);
                            teamV = new List<Contestant>(team.members.Except(all.members));
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
                for (int a = 0; a < team.members[i].votes; a++)
                {
                    Vote v = new Vote();
                    v.voter = team.members[i];
                    Votes.Add(v);
                    v.voter.voteReason = "They voted based on personal preference.";
                    List<Contestant> teamV = new List<Contestant>(team.members);
                    teamV.Remove(v.voter);
                    if (immune != null)
                    {
                        foreach (Contestant num in immune)
                        {
                            if (team.members.Contains(num))
                                teamV.Remove(num);
                        }
                    }
                    int ran = Random.Range(0, teamV.Count);
                    v.vote = v.voter.PersonalTarget(teamV.Except(immune).ToList());
                    for (int j = 0; j < Alliances.Count; j++)
                    {
                        if (Alliances[j].teams.Contains(team.name))
                        {
                            if (team.members.Count > Alliances[j].members.Count)
                            {
                                if (Alliances[j].members.Contains(v.voter) && team.members.Contains(Alliances[j].target) && Random.Range(1, 11) <= ContestantEvents.Instance.GetLoyalty(Votes[i].voter, Alliances[j].members))
                                {
                                    v.vote = Alliances[j].target;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }
        bool Revote(List<Contestant> tie)
        {
            float endVote = 0;

            foreach (Alliance alliance in Alliances)
            {
                if (alliance.teams.Contains(team.name))
                {
                    if (team.members.Count > alliance.members.Count)
                    {
                        Contestant target;
                        List<Contestant> tieV = new List<Contestant>(tie.Except(alliance.members));
                        alliance.altTargets = new List<Contestant>();
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

            for (int i = 0; i < Votes.Count; i++)
            {
                if (tie.Count < team.members.Count)
                {
                    if (!tie.Contains(Votes[i].voter))
                    {
                        int ran = Random.Range(0, tie.Count);
                        Contestant rvote = Votes[i].voter.PersonalTarget(tie);
                        for (int j = 0; j < Alliances.Count; j++)
                        {
                            if (Alliances[j].teams.Contains(team.name))
                            {
                                if (team.members.Count > Alliances[j].members.Count)
                                {
                                    if (Alliances[j].members.Contains(Votes[i].voter))
                                    {
                                        if (tie.Contains(Alliances[j].target) && Random.Range(1, 11) <= ContestantEvents.Instance.GetLoyalty(Votes[i].voter, Alliances[j].members))
                                        {
                                            rvote = Alliances[j].target;
                                        }
                                        else
                                        {
                                            if (Alliances[j].altTargets.Count > 0 && Random.Range(1, 11) <= ContestantEvents.Instance.GetLoyalty(Votes[i].voter, Alliances[j].members))
                                            {
                                                rvote = Alliances[j].altTargets[Alliances[j].altTargets.Count - 1];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        Votes[i].revotes.Add(rvote);
                    }
                }
                else
                {

                    List<Contestant> tieV = new List<Contestant>(tie);
                    tieV.Remove(Votes[i].voter);
                    int ran = Random.Range(0, tieV.Count);
                    Contestant rvote = tieV[ran];
                    for (int j = 0; j < Alliances.Count; j++)
                    {
                        //Contestant target = tie[Random.Range(0, tie.Count)];
                        if (team.members.Count > Alliances[j].members.Count)
                        {
                            if (Alliances[j].teams.Contains(team.name))
                            {
                                if (Alliances[j].members.Contains(Votes[i].voter))
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
                    Votes[i].revotes.Add(rvote);
                    if (rvote != team.members[i])
                    {
                        endVote++;
                    }
                    else if (rvote == team.members[i])
                    {

                    }
                }
            }
            return true;
        }
        bool VoteRestart()
        {
            float endVote = 0;
            targets = new List<Contestant>();
            foreach (Alliance all in Alliances)
            {
                if (all.teams.Contains(team.name))
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

            for (int i = 0; i < Votes.Count; i++)
            {
                
                List<Contestant> teamV = new List<Contestant>(team.members);
                teamV.Remove(Votes[i].voter);
                if (immune != null)
                {
                    foreach (Contestant num in immune)
                    {
                        if (team.members.Contains(num))
                            teamV.Remove(num);
                    }
                }
                int ran = Random.Range(0, teamV.Count);
                Contestant rvote = Votes[i].voter.PersonalTarget(teamV.Except(immune).ToList()); ;
                for (int j = 0; j < Alliances.Count; j++)
                {
                    if (Alliances[j].teams.Contains(team.name))
                    {
                        if (team.members.Count > Alliances[j].members.Count)
                        {
                            if (Alliances[j].members.Contains(Votes[i].voter) && team.members.Contains(Alliances[j].target) && Random.Range(1, 11) <= ContestantEvents.Instance.GetLoyalty(Votes[i].voter, Alliances[j].members))
                            {
                                rvote = Alliances[j].target;
                            }
                        }
                    }
                }
                Votes[i].revotes.Add(rvote);

                if (rvote != team.members[i])
                {
                    endVote++;
                }
                else if (rvote == team.members[i])
                {

                }
                
            }
            return true;
        }
        void CountVotes()
        {
            string eventext = "";
            
            foreach (Vote num in Votes)
            {
                
                if (tie.Count > 1)
                {
                    if (team.members.Count > tie.Count)
                    {
                        if (!tie.Contains(num.voter))
                        {
                            votes.Add(num.revotes[num.revotes.Count - 1]);
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        votes.Add(num.revotes[num.revotes.Count - 1]);
                    }
                    
                }
                else
                {
                    votes.Add(num.vote);
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
            
            
            float maxValuee = dic.Values.Max();
            tie = new List<Contestant>();
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
            List<Contestant> superIdols = new List<Contestant>();
            foreach (Contestant num in team.members)
            {
                foreach(Advantage advantage in num.advantages)
                {
                    List<Contestant> w = new List<Contestant>() { num };

                    Contestant usedOn = null;
                    List<Contestant> hasIdol = new List<Contestant>();
                    foreach (Contestant con in team.members)
                    {
                        foreach(Advantage ad in num.advantages)
                        {
                            if(ad.type == "HiddenImmunityIdol" && con != num)
                            {
                                hasIdol.Add(con);
                            }
                        }
                    }
                    if(hasIdol.Count > 0)
                    {
                        usedOn = hasIdol[Random.Range(0, hasIdol.Count)];
                    }
                    if (advantage.usedWhen == "AfterVotes" && advantage.type == "IdolNullifier" && Random.Range(0, 10) == 0 && hasIdol.Count > 0 && !aa)
                    {
                        AdvantagePlay(EpisodeStart.transform.GetChild(0).GetChild(0), advantage, num, usedOn);
                    }
                }
                if (num.advantages.Count > 0 && !aa)
                {
                    List<Advantage> remove = new List<Advantage>();

                    foreach (Advantage advantage in num.advantages)
                    {
                        List<Contestant> w = new List<Contestant>() { num };

                        Contestant usedOn = null;
                        bool playable = false;
                        if (advantage.usedWhen == "AfterVotes")
                        {
                            playable = true;
                        }
                        if(advantage.type == "SuperIdol")
                        {
                            superIdols.Add(num);
                        }
                        if (advantage.name == "Ally Idol" || immune.Contains(num) || Idols.Contains(num))
                        {
                            List<Contestant> teamV = new List<Contestant>(team.members);
                            teamV.Remove(num);
                            foreach(Contestant con in immune)
                            {
                                if (teamV.Contains(con))
                                    teamV.Remove(con);
                            }
                            foreach (Contestant con in Idols)
                            {
                                if (teamV.Contains(con))
                                    teamV.Remove(con);
                            }
                            usedOn = teamV[Random.Range(0, teamV.Count)];
                            
                        } 
                        int ran = Random.Range(0, 10);
                        if(tie.Contains(num) && usedOn == null)
                        {
                            ran = Random.Range(0, 5);
                        }
                        if(usedOn != null)
                        {
                            if (tie.Contains(usedOn))
                            ran = Random.Range(0, 5);
                        }
                        if((immune.Contains(num) || Idols.Contains(num)) && usedOn == null)
                        {
                            playable = false;
                        }
                        /*&& ran == 1* && tie.Contains(num)*/
                        
                        if (playable && (ran == 1 || currentContestants == advantage.expiresAt || advantage.length == 1))
                        {
                            bool a = false;
                            if (advantage.onlyUsable.Count > 0)
                            {
                                foreach(int numb in advantage.onlyUsable)
                                {
                                    if(currentContestants == numb)
                                    {
                                        a = true;
                                    }
                                }
                            } else
                            {
                                a = true;
                            }
                            if(a)
                            {
                                AdvantagePlay(EpisodeStart.transform.GetChild(0).GetChild(0), advantage, num, usedOn);
                                remove.Add(advantage);
                            }
                        }
                    }
                    foreach (Advantage advantage in remove)
                    {
                        
                        num.advantages.Remove(advantage);
                    }
                }
            }
            
            if (!aa)
            {
                MakeGroup(false, null, "", "", "I'll read the votes.", new List<Contestant>(), EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            }

            tie = new List<Contestant>();
            float enoughVotes = 0;
            if (dic.Count > 0)
            {
                maxValuee = dic.Values.Max();
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
                    if (tie.Count < 2)
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
                    else
                    {

                    }
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
            if (tie.Count == tieNum && aa)
            {
                e = true;
            }
            else
            {
                e = false;
            }
            
            ShuffleVotes(votesRead);

            Dictionary<Contestant, int> dicIdol = new Dictionary<Contestant, int>();
            Contestant immunity = team.members[Random.Range(0, team.members.Count)];
            
            if (!aa && Idols.Count > 0)
            {
                
                dicIdol.Add(Idols[0], 1);
                for (int i = 1; i < Idols.Count; i++)
                {
                    if (dicIdol.ContainsKey(Idols[i]))
                    {
                        dicIdol[Idols[i]] += 1;
                    }
                    else if (!dic.ContainsKey(Idols[i]))
                    {
                        dicIdol.Add(Idols[i], 1);
                    }
                }
                Idols = Idols.OrderBy(go => dicIdol[go]).ToList();
                foreach (Contestant num in Idols)
                {
                    votesRead.Insert(0, num);
                }
            }
            Dictionary<Contestant, int> vot = new Dictionary<Contestant, int>(dic.Concat(dicIdol).OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value));
            
            if (aa)
            {
                vote = vote + "\n" + string.Join("-", vot.Values) + " Revote";
            }
            else
            {
                foreach(KeyValuePair<Contestant, int> num in vot)
                {
                    if(vot.Keys.ToList().IndexOf(num.Key) < vot.Count - 1)
                    {
                        if (dicIdol.Contains(num))
                        {
                            vote += "<color=red>" + dicIdol[num.Key] + "*</color>-"; 
                        } else
                        {
                            if (MergedTribe.members.Count < 1)
                            {
                                if (team != LosingTribes[0] && curEvent.context == "TribalImmAV" && !aa && immunity == num.Key || (num.Value == dic.Values.Max() && superIdols.Contains(num.Key)))
                                {
                                    vote += "<color=red>" + dic[num.Key] + "*</color>-";
                                } else
                                {
                                    vote += dic[num.Key] + "-";
                                }
                            } else
                            {
                                if(num.Value == dic.Values.Max() && superIdols.Contains(num.Key))
                                {
                                    vote += "<color=red>" + dic[num.Key] + "*</color>-";
                                }
                                else
                                {
                                    vote += dic[num.Key] + "-";
                                }
                            }
                        }
                    } else
                    {
                        if (dicIdol.Contains(num) || (num.Value == dic.Values.Max() && superIdols.Contains(num.Key)))
                        {
                            if (num.Value == dic.Values.Max() && superIdols.Contains(num.Key))
                            {
                                vote += "<color=red>" + dic[num.Key] + "*</color>";
                            }
                            else
                            {
                                vote += "<color=red>" + dicIdol[num.Key] + "*</color>";
                            }
                        }
                        else
                        {
                            vote += dic[num.Key] + " Vote";
                        }
                    }
                }
                //vote += string.Join("-", vot) + " Vote";
                
            }
            

            dicVR = new Dictionary<Contestant, int>();
            
            if (!Idols.Contains(votesRead[0]))
            {
                dicVR.Add(votesRead[0], 1);
            }

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
                else if (num.Value == 1)
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
                if (Idols.Contains(votesRead[0]))
                {
                    dicVR.Remove(votesRead[0]);
                    MakeGroup(false, null, "nname", "", "<color=red>DOES NOT COUNT</color>", r, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                }
                else
                {
                    MakeGroup(false, null, dicVR[votesRead[0]] + votess + votesRead[0].nickname + votesLeft, "", "", r, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                }
                
                for (int i = 1; i < votesRead.Count; i++)
                {
                    string evtext = "";
                    string atext = "";
                    
                    if (dicVR.ContainsKey(votesRead[i]))
                    {
                        dicVR[votesRead[i]] += 1;
                    }
                    else if (!dicVR.ContainsKey(votesRead[i]) )
                    {
                        if(!Idols.Contains(votesRead[i]))
                        {
                            dicVR.Add(votesRead[i], 1);
                        }
                    }
                    votess = "";
                    votesLeft = "";
                    if (showVL == true)
                    {
                        
                        float vl = votes.Count - i - 1 + Idols.Count;
                        if (aa)
                        {
                            vl = votes.Count - i - 1;
                        }
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
                    string ctext = string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + votesLeft;
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
                        if (Idols.Contains(votesRead[i]))
                        {
                            atext = "";
                            finalVotes = "<color=red>DOES NOT COUNT</color>";
                        }
                        MakeGroup(false, null, "name", atext, finalVotes, g, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                    }
                    else if (i == votesRead.Count - 1 && tie.Count > 1)
                    {
                        evtext = votedOff.nickname;
                        evtext = votesRead[i].nickname;
                        MakeGroup(false, null, "nname", atext, "", g, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                        if (e == false)
                        {
                            string firstline = "There is a tie and a revote. Those in in the tie will not revote, unless no one received votes on the original vote.\n\n\n";
                            string secondline = "Final vote count was " + string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + ".";
                            eventext = firstline + secondline;
                        }
                        else 
                        {
                            string secondline = "Final vote count was " + string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + ".";
                            evtext = secondline;
                            elimed++;
                            MakeGroup(false, null, "name", "", evtext, tie, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                        }
                        
                    } else
                    {
                        if(Idols.Contains(votesRead[i]))
                        {
                            MakeGroup(false, null, "nname", atext, "<color=red>DOES NOT COUNT</color>", g, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                        } else
                        {
                            MakeGroup(false, null, ctext, atext, evtext, g, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                        }
                    }
                }
            }
            
            if(MergedTribe.members.Count < 1)
            {
                if (team != LosingTribes[0] && curEvent.context == "TribalImmAV" && !aa)
                {
                    
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
                    }
                }
            }
            foreach (Contestant num in team.members)
            {
                if (num.advantages.Count > 0)
                {
                    List<Advantage> remove = new List<Advantage>();

                    foreach (Advantage advantage in num.advantages)
                    {
                        List<Contestant> w = new List<Contestant>() { num };

                        Contestant usedOn = null;
                        if (advantage.type == "ImmunityNecklace" || advantage.type == "VoteSteal" || advantage.type == "VoteBlocker")
                        {
                            List<Contestant> teamV = new List<Contestant>(team.members);
                            teamV.Remove(num);

                            usedOn = teamV[Random.Range(0, teamV.Count)];
                        }
                        if (advantage.usedWhen == "AfterVotesRead")
                        {
                            if(advantage.type == "SuperIdol" && tie.Contains(num))
                            {
                                AdvantagePlay(EpisodeStart.transform.GetChild(0).GetChild(0), advantage, num, usedOn);
                                remove.Add(advantage);
                                if(tie.Count > 1)
                                {
                                    eventext = "There is a tie and a revote. Those in in the tie will not revote, unless no one received votes on the original vote.\n\n\nFinal vote count was " + string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + ".";
                                }
                            }
                            
                        }
                    }
                    foreach (Advantage advantage in remove)
                    {
                        num.advantages.Remove(advantage);
                    }
                }
            }
            if(eventext != "")
            {
                if(tie.Count < 2)
                {
                    MakeGroup(false, null, "name", "", "Final vote count was " + string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + ".", new List<Contestant>(), EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                } else
                {
                    MakeGroup(false, null, "name", "", eventext , tie, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                }
            }
            AddIdols(Idols);
        }
        void Eliminate()
        {
            foreach (Advantage advantage in votedOff.advantages)
            {
                foreach (HiddenAdvantage hid in sea.islandHiddenAdvantages)
                {
                    if (advantage.nickname == hid.name && hid.reHidden)
                    {
                        hid.hidden = true;
                    }
                }
                foreach (Team tribe in Tribes)
                {
                    foreach (HiddenAdvantage hid in tribe.hiddenAdvantages)
                    {
                        if (advantage.nickname == hid.name && hid.reHidden)
                        {
                            hid.hidden = true;
                        }
                    }
                }
                if (MergedTribe.members.Count > 0)
                {
                    foreach (HiddenAdvantage hid in MergedTribe.hiddenAdvantages)
                    {
                        if (advantage.nickname == hid.name && hid.reHidden)
                        {
                            hid.hidden = true;
                        }
                        foreach (HiddenAdvantage hidden in sea.islandHiddenAdvantages)
                        {
                            if (advantage.nickname == hid.name && hidden.hidden && hid.name == hidden.name && hidden.linkedToExile && hidden.hideAt <= curEp + 1)
                            {
                                if (hid.length == 0)
                                {
                                    hid.hidden = true;
                                    hid.length++;
                                    hidden.hidden = false;
                                }
                                else
                                {
                                    hid.hidden = false;
                                    hidden.hidden = true;
                                }
                            }
                        }
                    }
                }
            }
            foreach (Alliance alliance in Alliances)
            {
                if (alliance.members.Contains(votedOff))
                {
                    alliance.members.Remove(votedOff);
                }
            }

            string text = votedOff.nickname + "'s relationships; " + string.Join(", ", votedOff.Relationships.ConvertAll(i => i.person.nickname + ":" + i.ToString()));
            //Debug.Log(text);
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
                    conPlacement = "Voted Off " + Episodes[curEp].nickname;

                    votedOff.placement = conPlacement + "\n" + juror + "\n" + vote;
                    RIsland.Add(votedOff);
                }
            }
            if(sea.EdgeOfExtinction)
            {
                votedOff.advantages = new List<Advantage>();
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
                    conPlacement = "Voted Off " + Episodes[curEp].nickname;
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
            } else if(curEvent.type == "MergeSplit" || curEvent.type == "MergeSplitFiji")
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
                bottle = "\n \nThe message in the bottle instructs them to vote out another tribe member.";
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
            foreach(Contestant num in team.members)
            {
                foreach(Advantage advantage in num.advantages)
                {
                    if(advantage.temp)
                    {
                        advantage.length--;
                    }
                }
                if(num.votes < 1)
                {
                    num.votes++;
                } else if(num.votes > 1)
                {
                    num.votes = 1;
                }
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
                if(team.members.Count > 2)
                {
                    vote = vote + "\nTiebreaker";
                } else
                {
                    vote = "Tiebreaker";
                }
                
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
        void AdvantagePlay(Transform obj, Advantage advantage, Contestant user, Contestant usedOn)
        {
            Dictionary<Contestant, int> dic2 = new Dictionary<Contestant, int>(dic);
            List<Contestant> n = new List<Contestant>() { user };
            string evetext = user.nickname + " uses the " + advantage.nickname;
            
            if (usedOn != null)
            {
                evetext += " on " + usedOn.nickname;
                if (advantage.type == "IdolNullifier")
                {
                    evetext = user.nickname + " secretly uses the " + advantage.nickname + " on " + usedOn.nickname + "\n\nThe first idol played on " + usedOn.nickname + " will be negated.";
                } 
                n.Add(usedOn); n.Reverse();
                if(advantage.type != "ImmunityNecklace" && advantage.type != "VoteSteal" && advantage.type != "VoteBlocker")
                {
                    user = usedOn;
                }
            }
            if (advantage.type == "HiddenImmunityIdol")
            {
                evetext += "\n\nAny votes cast against " + user.nickname + " will not count.";
                if (nullIdols.Contains(user))
                {
                    evetext += "\n\nBecause of the Idol Nullifier, this idol is negated.";
                }
            }    
            foreach (HiddenAdvantage hid in sea.islandHiddenAdvantages)
            {
                if (advantage.nickname == hid.name && hid.reHidden && sea.IslandType == "Exile")
                {
                    hid.hidden = true;
                }
            }
            foreach (Team tribe in Tribes)
            {
                foreach(HiddenAdvantage hid in tribe.hiddenAdvantages)
                {
                    if(advantage.nickname == hid.name && hid.reHidden)
                    {
                        hid.hidden = true;
                    }
                }
            }
            
            if (MergedTribe.members.Count > 0)
            {
                foreach (HiddenAdvantage hid in MergedTribe.hiddenAdvantages)
                {
                    if (advantage.nickname == hid.name && hid.reHidden)
                    {
                        hid.hidden = true;
                    }
                    foreach (HiddenAdvantage hidden in sea.islandHiddenAdvantages)
                    {
                        if (advantage.name == hid.name && hidden.name == hid.name && hid.linkedToExile && hid.hideAt <= curEp + 2)
                        {
                            if (hid.length == 0)
                            {
                                hid.hidden = true;
                                hid.length++;
                                hidden.hidden = false;
                            }
                            else
                            {
                                hid.hidden = false;
                                hidden.hidden = true;
                            }
                        }
                    }
                }
            }
            
            switch (advantage.type)
            {
                case "PreventiveIdol":
                    immune.Add(user);
                    evetext += "\n \n No one can vote for " + user.nickname;
                    break;
                case "ImmunityNecklace":
                    immune.Add(usedOn);
                    immune.Remove(user);
                    evetext = user.nickname + " gives individual immunity to " + usedOn.nickname + "\n \n " + user.nickname + " is now vulnerble";
                    break;
                case "SafetyWithoutPower":
                    immune.Add(user);
                    Exiled.Add(user);
                    user.team = team.name;
                    evetext += "\n \n" + user.nickname + " leaves tribal council";
                    break;
                case "ExtraVote":
                    user.votes++;
                    evetext += "\n \n" + user.nickname + " will vote one more time";
                    break;
                case "VoteSteal":
                    user.votes++;
                    usedOn.votes--;
                    evetext += "\n \n" + usedOn.nickname + " loses a vote." + "\n \n" + user.nickname + " will vote one more time";
                    break;
                case "VoteBlocker":
                    usedOn.votes--;
                    evetext += "\n \n" + usedOn.nickname + " loses a vote";
                    break;
                case "SuperIdol":
                    immune.Add(user);
                    if (dic2.ContainsKey(user))
                    {
                        dic2.Remove(user);
                        tie = new List<Contestant>();
                        if (dic2.Count > 0)
                        {
                            int maxValue = dic2.Values.Max();
                            foreach (KeyValuePair<Contestant, int> num in dic2)
                            {
                                if (num.Value == maxValue)
                                {
                                    votedOff = num.Key;
                                    tie.Add(num.Key);
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("ff");
                        }
                        /*int a = vote.IndexOf(dic[user].ToString());
                        StringBuilder sb = new StringBuilder(vote);
                        sb.Remove(a, dic[user].ToString().Length);
                        Debug.Log(a.ToString().Length);
                        sb.Insert(a, "<color=red>" + dic[user] + "*</color>");
                        vote = sb.ToString();*/
                        dic = dic2;
                    }
                    break;
                case "VoteSacrifice":
                    user.votes--;
                    break;
                case "IdolNullifier":
                    nullIdols.Add(user);
                    break;
                case "HiddenImmunityIdol":
                    if(!nullIdols.Contains(user))
                    {
                        immune.Add(user);
                        if (dic2.ContainsKey(user))
                        {
                            dic2.Remove(user);

                            foreach (Contestant num in votes)
                            {
                                if (num == user)
                                {
                                    Idols.Add(num);
                                }
                            }
                            foreach (Contestant num in Idols)
                            {
                                if (votes.Contains(num))
                                {
                                    votes.Remove(num);
                                }
                            }
                            tie = new List<Contestant>();
                            if (dic2.Count > 0)
                            {
                                int maxValue = dic2.Values.Max();
                                foreach (KeyValuePair<Contestant, int> num in dic2)
                                {
                                    if (num.Value == maxValue)
                                    {
                                        votedOff = num.Key;
                                        tie.Add(num.Key);
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log("ff");
                            }
                            dic = dic2;
                        }
                    }
                    break;
            }
            MakeGroup(false, null, "", "", evetext + ".", n, obj, 0);
        }
        bool RE()
        {
            tieNum = tie.Count;
            votes = new List<Contestant>();
            votesRead = new List<Contestant>();


            List<Contestant> teamV = new List<Contestant>(team.members);
            List<Contestant> o = new List<Contestant>();
            foreach (Contestant num in immune)
            {
                if (team.members.Contains(num))
                    teamV.Remove(num);
                o.Add(num);
            }
            if (teamV.Count == 1)
            {
                votedOff = teamV[0];
                vote = vote + "\nEliminated by Default";
                Eliminate();
                tri = 0;
                return true;
            }
            else if(teamV.Count == 0)
            {
                Tiebreaker(o, "FireChallenge");
                tri = 0;
                return true;
            }
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
            bool rev = true;
            if(tie.Count < 1)
            {
                rev = VoteRestart();
            } else
            {
                rev = Revote(tie);
            }
            if (rev)
            {
                
                CountVotes();
                //curEpp--;
                AddVote(votes, votesRead);
                
                float maxxValue = dic.Values.Max();
                if (tie.Count < tieNum)
                {
                    //Debug.Log(votedOff.nickname + " has been eliminated. " + "Votes: " + dic[votedOff]);
                    if (tie.Count < 2)
                    {
                        Eliminate();
                        tri = 0;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if(tie.Count < 1)
                    {
                        return false;
                    } else
                    {
                        if (MergedTribe.members.Count == 4)
                        {
                            Tiebreaker(tie, "FireChallenge");
                        }
                        else
                        {
                            Tiebreaker(tie, "Rocks");
                        }
                        tri = 0;
                    }
                    
                    return true;
                }
            }
            return false;
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
    public void MakeAlliances(Team team)
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
                    team.allianceCount++;
                    Alliances.Add(alliance1);
                }
                if (alliance2.members.Count > 1)
                {
                    team.allianceCount++;
                    Alliances.Add(alliance2);
                }
            } else
            {
                if (alliance1.members.Count > 1)
                {
                    team.allianceCount++;
                    Alliances.Add(alliance1);
                }
                if (alliance2.members.Count > 1)
                {
                    team.allianceCount++;
                    Alliances.Add(alliance2);
                }
                //MakeAlliances(team);
            }
        } else
        {
            team.allianceCount += 2;
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
        if(r && Tribes.Count == 2 && absorb)
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
            MergedTribe.hiddenAdvantages = sea.mergeHiddenAdvantages;
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
        idols = 0;
        foreach (Contestant num in MergedTribe.members)
        {
            if (num.advantages.Count > 0)
            {
                foreach (Advantage advantage in num.advantages)
                {
                    if (advantage.type == "HiddenImmunityIdol" && !advantage.temp)
                    {
                        idols++;
                    }
                }

            }
        }
        NextEvent();
    }
    void MergeStatus()
    {
        GameObject EpisodeStatus = Instantiate(Prefabs[0]);
        EpisodeStatus.transform.parent = Canvas.transform;
        EpisodeStatus.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStatus.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStatus.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStatus.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStatus.name = MergedTribe.name + " Status";
        AddGM(EpisodeStatus, true);
        bool adv = false;

        foreach (HiddenAdvantage hid in MergedTribe.hiddenAdvantages)
        {
            bool asdsa = false; 
            if(hid.advantage.type == "HiddenImmunityIdol" && idols >= sea.idolLimit)
            {
                asdsa = true;
            }
            if (hid.linkedToExile)
            {
                asdsa = true;
                if(hid.hidden)
                {
                    asdsa = false;
                }
            }
            
            if (hid.hideAt <= curEp + 1 && currentContestants >= hid.advantage.expiresAt && !asdsa)
            {
                if (!adv)
                {
                    adv = true;
                    MakeGroup(false, null, "", "<b>Advantages</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                }
                string nam = hid.name;
                if (!hid.name.Contains("Immunity Idol"))
                {
                    nam = "secret advantage";
                }
                string atext = "The " + nam + " is currently hidden.";
                if (!hid.hidden)
                {
                    if (hid.reHidden)
                    {
                        atext = "The " + nam + " is not currently hidden.";
                    }
                    else
                    {
                        atext = "";
                    }
                }
                if (atext != "")
                {
                    MakeGroup(false, null, "", atext, "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                }
                foreach (Contestant num in MergedTribe.members)
                {
                    if (hid.hidden)
                    {
                        int ran = Random.Range(0, 2);
                        if (ran == 1)
                        {
                            Advantage av = Instantiate(hid.advantage);
                            av.nickname = hid.name;
                            if (hid.temp)
                            {
                                av.temp = true;
                                av.length = hid.length;
                            }
                            hid.hidden = false;
                            if (hid.advantage.type == "HalfIdol")
                            {
                                num.halfIdols.Add(num);
                            }
                            else
                            {
                                num.advantages.Add(av);
                            }
                            List<Contestant> n = new List<Contestant>() { num };
                            MakeGroup(false, null, "", "", num.nickname + " finds the " + hid.name + "\n\n" + av.description, n, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                        }
                    }
                }
            }
        }
        
        
        foreach (Contestant num in MergedTribe.members)
        {
            ContestantEvents.Instance.UpdateRelationships(num, MergedTribe.members);
            List<Contestant> w = new List<Contestant>() { num };
            foreach (Advantage advantage in num.advantages)
            {
                if (!adv)
                {
                    adv = true;
                    MakeGroup(false, null, "", "<b>Advantages</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                }
                MakeGroup(false, null, "", "", num.nickname + " has the " + advantage.nickname, w, EpisodeStatus.transform.GetChild(0).GetChild(0), 0);
            }
            
            int comb = 0;
            foreach (Contestant half in num.halfIdols)
            {
                if (!adv)
                {
                    adv = true;
                    MakeGroup(false, null, "", "<b>Advantages</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                }
                List<Contestant> u = new List<Contestant>() { half };
                if (num.halfIdols.Count > 1)
                {
                    if (MergedTribe.members.Contains(half))
                    {
                        comb++;
                    }
                }
                else
                {
                    MakeGroup(false, null, "", "", num.nickname + " has the Half Idol.", u, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                }
            }
            if (num.halfIdols.Count > 1)
            {
                if (comb == 2)
                {
                    foreach (Contestant half in num.halfIdols)
                    {
                        List<Contestant> u = new List<Contestant>() { half };
                        MakeGroup(false, null, "", "", num.nickname + " has the Half Idol that is ready to be combined into a full idol.", u, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        num.halfIdols.Reverse();
                    }

                    MakeGroup(false, null, "", "", num.halfIdols[1].nickname + " lets " + num.halfIdols[0].nickname + " have the Combined Hidden Immunity Idol.", num.halfIdols, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                    Advantage av = Instantiate(HiddenIdol);
                    av.nickname = "Combined Hidden Immunity Idol";
                    num.halfIdols[0].advantages.Add(av);
                    if (Tribes[curT].members.IndexOf(num.halfIdols[0]) < Tribes[curT].members.IndexOf(num) || Tribes[curT].members.IndexOf(num.halfIdols[0]) == Tribes[curT].members.IndexOf(num))
                    {
                        List<Contestant> ww = new List<Contestant>() { num.halfIdols[0] };
                        MakeGroup(false, null, "", "", num.halfIdols[0].nickname + " has the " + av.nickname, ww, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                    }
                    num.halfIdols = new List<Contestant>();
                }
                else
                {
                    num.halfIdols = new List<Contestant>();
                }
            }

            if (num.halfIdols.Count == 1)
            {
                List<Contestant> TribeV = new List<Contestant>(Tribes[curT].members);
                TribeV.Remove(num);
                num.halfIdols.Add(TribeV[Random.Range(0, TribeV.Count)]);
                List<Contestant> ex = new List<Contestant>();
                num.halfIdols.Reverse();
                MakeGroup(false, null, "", "", num.nickname + " transfers the half idol to " + num.halfIdols[0].nickname, num.halfIdols, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                num.halfIdols.Reverse();
            }
        }
        if (!adv && advant)
        {
            MakeGroup(false, null, "", "There are no secret advantages.", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
        }
        MakeGroup(false, null, "", "<b>Alliances</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
        foreach (Alliance alliance in Alliances)
        {
            MakeGroup(false, null, "name", alliance.name, "", alliance.members, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
        }
        if (Alliances.Count < 2)
        {
            EpisodeStatus.transform.GetChild(0).GetComponent<VerticalLayoutGroup>().spacing = 0;
        } else if(Alliances.Count < 1)
        {
            MakeGroup(false, null, "", "", "There are no alliances", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
        }
        NextEvent();
    }
    void MergeEvents()
    {
        GameObject EpisodeStatus = Instantiate(Prefabs[0]);
        EpisodeStatus.transform.parent = Canvas.transform;
        EpisodeStatus.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStatus.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStatus.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStatus.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStatus.name = MergedTribe.name + " Status";
        AddGM(EpisodeStatus, true);
        bool adv = false;

        foreach (HiddenAdvantage hid in MergedTribe.hiddenAdvantages)
        {
            if (hid.hideAt <= curEp + 1 && currentContestants >= hid.advantage.expiresAt)
            {
                if (!adv)
                {
                    adv = true;
                    MakeGroup(false, null, "", "<b>Advantages</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                }
                string nam = hid.name;
                if (!hid.name.Contains("Immunity Idol"))
                {
                    nam = "secret advantage";
                }
                string atext = "The " + nam + " is currently hidden.";
                if (!hid.hidden)
                {
                    if (hid.reHidden)
                    {
                        adv = true;
                        atext = "The " + nam + " is not currently hidden.";
                    }
                    else
                    {

                        atext = "";
                    }
                }
                if (atext != "")
                {
                    MakeGroup(false, null, "", atext, "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                }
                foreach (Contestant num in Tribes[curT].members)
                {
                    if (hid.hidden)
                    {
                        int ran = Random.Range(0, 2);
                        if (ran == 1)
                        {
                            Advantage av = Instantiate(hid.advantage);
                            av.nickname = hid.name;
                            if (hid.temp)
                            {
                                av.temp = true;
                                av.length = hid.length;
                            }
                            hid.hidden = false;
                            if (hid.advantage.type == "HalfIdol")
                            {
                                num.halfIdols.Add(num);
                            }
                            else
                            {
                                num.advantages.Add(av);
                            }
                            List<Contestant> n = new List<Contestant>() { num };
                            MakeGroup(false, null, "", "", num.nickname + " finds the " + hid.name + "\n\n" + av.description, n, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                        }
                    }
                }
            }
        }

        List<Contestant> u = new List<Contestant>();
        foreach (Contestant num in MergedTribe.members)
        {
            num.stats.Stamina -= 2;

            List<Contestant> w = new List<Contestant>() { num };
            foreach (Advantage advantage in num.advantages)
            {
                if (!adv)
                {
                    adv = true;
                    MakeGroup(false, null, "", "<b>Advantages</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                }
                string extra = "";
                if (advantage.temp)
                {
                    if (advantage.length > 1)
                    {
                        extra = "\n \nThis can be used at the next " + advantage.length + " tribal councils.";
                    }
                    else
                    {
                        extra = "\n \nThis can be used at the next tribal council.";
                    }
                }
                if (currentContestants == advantage.expiresAt)
                {
                    extra = "\n \nThis is the last round to use it.";
                }
                int a = 0;
                if (advantage.onlyUsable.Count > 0)
                {
                    extra = "\n \nIt can't be used this round.";
                    foreach (int numb in advantage.onlyUsable)
                    {
                        if (currentContestants == numb)
                        {
                            a = numb;
                        }
                    }
                }
                else
                {
                    a = 0;
                }
                if (a != 0)
                {
                    if (a != advantage.onlyUsable[advantage.onlyUsable.Count - 1])
                    {
                        extra = "\n \nIt can be used this round.";
                    }
                    else
                    {
                        extra = "\n \nIt can be used this round.\n \nThis is the last round to use it.";
                    }
                }
                MakeGroup(false, null, "", "", num.nickname + " has the " + advantage.nickname + extra, w, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
            }
            int comb = 0;
            foreach (Contestant half in num.halfIdols)
            {
                if (!adv)
                {
                    adv = true;
                    MakeGroup(false, null, "", "<b>Advantages</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                }
                u = new List<Contestant>() { half };
                if (num.halfIdols.Count > 1)
                {
                    if (Tribes[curT].members.Contains(half))
                    {
                        comb++;
                    }
                }
                else
                {
                    MakeGroup(false, null, "", "", num.nickname + " has the Half Idol.", u, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                }
                adv = true;
            }
            if (num.halfIdols.Count > 1)
            {
                if (comb == 2)
                {
                    foreach (Contestant half in num.halfIdols)
                    {
                        u = new List<Contestant>() { half };
                        MakeGroup(false, null, "", "", num.nickname + " has the Half Idol that is ready to be combined into a full idol.", u, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        num.halfIdols.Reverse();
                    }

                    MakeGroup(false, null, "", "", num.halfIdols[1].nickname + " lets " + num.halfIdols[0].nickname + " have the Combined Hidden Immunity Idol.", num.halfIdols, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                    Advantage av = Instantiate(HiddenIdol);
                    av.nickname = "Combined Hidden Immunity Idol";
                    num.halfIdols[0].advantages.Add(av);
                    if (Tribes[curT].members.IndexOf(num.halfIdols[0]) < Tribes[curT].members.IndexOf(num) || Tribes[curT].members.IndexOf(num.halfIdols[0]) == Tribes[curT].members.IndexOf(num))
                    {
                        List<Contestant> ww = new List<Contestant>() { num.halfIdols[0] };
                        MakeGroup(false, null, "", "", num.halfIdols[0].nickname + " has the " + av.nickname, ww, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                    }
                    num.halfIdols = new List<Contestant>();
                }
                else
                {
                    num.halfIdols = new List<Contestant>();
                }
            }

            if (num.halfIdols.Count == 1)
            {
                List<Contestant> TribeV = new List<Contestant>(Tribes[curT].members);
                TribeV.Remove(num);
                num.halfIdols.Add(TribeV[Random.Range(0, TribeV.Count)]);
                List<Contestant> ex = new List<Contestant>();
                num.halfIdols.Reverse();
                MakeGroup(false, null, "", "", num.nickname + " transfers the half idol to " + num.halfIdols[0].nickname, num.halfIdols, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                num.halfIdols.Reverse();
            }
        }
        if (!adv && advant)
        {
            MakeGroup(false, null, "", "There are no secret advantages.", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
        }

        bool eventt = false;

        int eventCap = 0;
        
        foreach (ContestantEvent Event in events)
        {
            int eventTimes = 100;
            List<Contestant> tribe = new List<Contestant>(MergedTribe.members);
            bool waht = false;
            
            switch (Event.type)
            {
                case EventType.Stamina:
                    eventTimes = 3;
                    if (Event.staminaAffect > 0)
                    {
                        tribe = tribe.OrderBy(x => x.stats.Stamina).ToList();
                    }
                    else
                    {
                        tribe = tribe.OrderByDescending(x => x.stats.Stamina).ToList();
                    }
                    foreach (Contestant num in tribe)
                    {
                        //&& !Event.type.Contains("Alliance")
                        //bool what = ContestantEvents.Instance.EventChance(Event, null, num);
                        if (ContestantEvents.Instance.EventChance(Event, new List<Contestant>(), num) == true && Random.Range(0, eventTimes) == 0 && eventCap < 11)
                        {
                            
                            if (!eventt)
                            {
                                MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                                eventt = true;
                            }
                            ContestantEvents.Instance.DoEvent(Event, null, null, num, EpisodeStatus.transform.GetChild(0).GetChild(0));
                            eventTimes += eventTimes; eventCap++;
                        }
                    }
                    break;
                case EventType.Relationship:
                    if (Event.relationshipAffect > 0)
                    {
                        tribe = tribe.OrderBy(x => ChallengeScript.Instance.GetPoints(x, Event.stats)).ToList();
                    }
                    else
                    {
                        tribe = tribe.OrderByDescending(x => ChallengeScript.Instance.GetPoints(x, Event.stats)).ToList();
                    }
                    //eventTimes = 50;
                    foreach (Contestant num in tribe)
                    {
                        //&& !Event.type.Contains("Alliance")
                        //bool what = ContestantEvents.Instance.EventChance(Event, null, num);
                        if (Event.contestants > 0)
                        {
                            if (Event.relationshipAffect < 0)
                            {
                                num.Relationships = num.Relationships.OrderBy(x => x.Type).ThenByDescending(x => (int)x.Status * 10 + x.Extra).Where(x => tribe.Contains(x.person)).ToList();
                            }
                            else
                            {
                                num.Relationships = num.Relationships.OrderByDescending(x => x.Type).ThenByDescending(x => (int)x.Status * 10 + x.Extra).Where(x => tribe.Contains(x.person)).ToList();
                            }
                        }
                        
                        if (Event.overall)
                        {
                            if (eventCap < 11 && Random.Range(0, 5 + eventCap) == 0)
                            {
                                waht = true;
                            }
                            if (ContestantEvents.Instance.EventChance(Event, new List<Contestant>(), num) == true && Random.Range(0, eventTimes) == 0 && Event.overall && waht)
                            {
                                if (!eventt)
                                {
                                    MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                                    eventt = true;
                                }
                                ContestantEvents.Instance.DoEvent(Event, tribe, null, num, EpisodeStatus.transform.GetChild(0).GetChild(0));
                                eventTimes += eventTimes; eventCap++;
                            }
                        }
                        else if(!Event.overall)
                        {
                            foreach (Relationship re in num.Relationships)
                            {
                                if (eventCap < 11 && Random.Range(0, 5 + eventCap) == 0)
                                {
                                    waht = true;
                                }
                                if (ContestantEvents.Instance.EventChance(Event, new List<Contestant>(), num) == true && Random.Range(0, eventTimes) == 0 && waht && Random.Range(0, eventTimes) == 0)
                                {
                                    if (!eventt)
                                    {
                                        MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                                        eventt = true;
                                    }
                                    ContestantEvents.Instance.DoEvent(Event, new List<Contestant>() { re.person }, null, num, EpisodeStatus.transform.GetChild(0).GetChild(0));
                                    eventTimes += eventTimes; eventCap++;
                                }
                            }
                        }
                    }
                    break;
                case EventType.Alliance:
                    eventTimes = 3;
                    switch (Event.allianceEvent)
                    {
                        case AllianceEventType.Create:
                            List<Alliance> AddAlliance = new List<Alliance>();
                            tribe = tribe.OrderByDescending(x => ChallengeScript.Instance.GetPoints(x, Event.stats)).ToList();
                            foreach(Contestant num in tribe)
                            {
                                Alliance newAlliance = new Alliance();
                                newAlliance.members.Add(num);
                                num.Relationships = num.Relationships.OrderByDescending(x => x.Type).ThenByDescending(x => (int)x.Status * 10 + x.Extra).Where(x => tribe.Contains(x.person)).ToList();
                                foreach(Relationship re in num.Relationships)
                                {
                                    if(MergedTribe.members.Contains(re.person) && !newAlliance.members.Contains(re.person) && re.person != num)
                                    {
                                        int stat = ContestantEvents.Instance.GetLoyalty(num, new List<Contestant>() { re.person });
                                        if(Random.Range(1, 11) < stat)
                                        {
                                            newAlliance.members.Add(re.person);
                                        }
                                    }
                                }
                                if (eventCap < 11 && Random.Range(0, 5 + eventCap) == 0 && newAlliance.members.Count > 1)
                                {
                                    waht = true;
                                }
                                Contestant main = new Contestant();
                                foreach (Alliance alliance in Alliances)
                                {
                                    if (alliance.members.Intersect(newAlliance.members).ToList().Count == alliance.members.Count)
                                    {
                                        waht = false;
                                    }
                                    else if (alliance.members.Where(x => newAlliance.members.Contains(x)).ToList().Count == alliance.members.Count - 1)
                                    {
                                        ContestantEvents.Instance.join = true;
                                        newAlliance.members = newAlliance.members.OrderBy(x => alliance.members.Contains(x)).ToList();
                                        main = newAlliance.members[newAlliance.members.Count - 1];
                                        alliance.members = newAlliance.members;
                                        newAlliance.name = alliance.name;
                                    }
                                }
                                if (ContestantEvents.Instance.EventChance(Event, newAlliance.members, num) == true && waht)
                                {
                                    if(!ContestantEvents.Instance.join)
                                    {
                                        AddAlliance.Add(newAlliance);
                                        MergedTribe.allianceCount++;
                                        newAlliance.name = MergedTribe.name + " Alliance #" + MergedTribe.allianceCount;
                                    }
                                    
                                    if (!eventt)
                                    {
                                        MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                                        eventt = true;
                                    }
                                    ContestantEvents.Instance.DoEvent(Event, newAlliance.members, newAlliance, main, EpisodeStatus.transform.GetChild(0).GetChild(0));
                                    eventTimes += eventTimes; eventCap++;
                                }
                                ContestantEvents.Instance.join = false;
                            }
                            foreach (Alliance alliance in AddAlliance)
                            {
                                alliance.teams.Add(MergedTribe.name);
                                Alliances.Add(alliance);
                            }
                            break;
                        case AllianceEventType.Dissolve:
                            List<Alliance> RemoveAlliance = new List<Alliance>();
                            foreach (Alliance alliance in Alliances)
                            {
                                if (eventCap < 11 && Random.Range(0, 5 + eventCap) == 0)
                                {
                                    waht = true;
                                }
                                if (ContestantEvents.Instance.EventChance(Event, alliance.members, null) == true && Random.Range(0, eventTimes) == 0 && waht)
                                {
                                    if (!eventt)
                                    {
                                        MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                                        eventt = true;
                                    }
                                    ContestantEvents.Instance.DoEvent(Event, alliance.members, alliance, new Contestant(), EpisodeStatus.transform.GetChild(0).GetChild(0));
                                    eventTimes += eventTimes; eventCap++;
                                    RemoveAlliance.Add(alliance);
                                }
                            }
                            foreach (Alliance ads in RemoveAlliance)
                            {
                                Alliances.Remove(ads);
                            }
                            break;
                        case AllianceEventType.Leave:
                            foreach (Alliance alliance in Alliances)
                            {
                                List<Contestant> remove = new List<Contestant>();
                                bool removed = false;
                                foreach (Contestant num in alliance.members)
                                {
                                    if (eventCap < 11 && Random.Range(0, 5 + eventCap) == 0)
                                    {
                                        waht = true;
                                    }
                                    if (ContestantEvents.Instance.EventChance(Event, alliance.members, num) == true && Random.Range(0, eventTimes) == 0 && waht & !removed)
                                    {
                                        if (!eventt)
                                        {
                                            MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                                            eventt = true;
                                        }
                                        ContestantEvents.Instance.DoEvent(Event, alliance.members, alliance, num, EpisodeStatus.transform.GetChild(0).GetChild(0));
                                        eventTimes += eventTimes; eventCap++;
                                        remove.Add(num);
                                        removed = true;
                                    }
                                }
                                foreach (Contestant num in remove)
                                {
                                    alliance.members.Remove(num);
                                }
                            }
                            break;
                    }
                    break;
            }
        }
        if (!eventt)
        {
            MakeGroup(false, null, "", "No events occured.", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
        }
        List<Alliance> removee = new List<Alliance>();
        foreach (Alliance alliance in Alliances)
        {
            if (alliance.members.Count < 2)
            {
                removee.Add(alliance);
            }
        }
        foreach (Alliance alliance in removee)
        {
            Alliances.Remove(alliance);
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
        foreach(Contestant num in MergedTribe.members)
        {
            if(num.challengeAdvantage)
            {
                int Ran = Random.Range(0, 1);
                if(Ran == 1)
                {
                    ran = MergedTribe.members.IndexOf(num);
                }
                num.challengeAdvantage = false;
            }
        }
        if (curImm <= sea.ImmunityChallenges.Count - 1)
        {
            challenge.IndividualChallenge(MergedTribe, sea.ImmunityChallenges[curImm].stats, 1);
        }
        else
        {
            challenge.IndividualChallenge(MergedTribe, new List<StatChoice>() { StatChoice.Physical, StatChoice.Mental, StatChoice.Endurance }, 1);
        }
        //immune.Add(MergedTribe.members[ran]);
        //MergedTribe.members[ran].advantages.Add(ImmunityNecklace);
        GameObject EpisodeStart = Instantiate(Prefabs[2]);
        EpisodeStart.transform.parent = Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        lastThing = EpisodeStart;
        AddGM(EpisodeStart, true);

        List<Contestant> w = new List<Contestant>() { immune[0] };
        MakeGroup(false, null, immune[0].nickname + " Wins Immunity!", "", "", w, EpisodeStart.transform.GetChild(0), 20);
        foreach(Contestant num in MergedTribe.members)
        {
            if(num.IOIEvent == "PredictImmunity")
            {
                if (immune[0] == num.vote)
                {
                    MakeGroup(false, null, num.nickname + " wins the " + num.savedAdv.name + ".", "", "", new List<Contestant>() { num }, EpisodeStart.transform.GetChild(0), 20);
                    Advantage av = Instantiate(num.savedAdv.advantage);
                    av.nickname = num.savedAdv.name;
                    if (num.savedAdv.temp)
                    {
                        av.temp = true;
                        av.length = num.savedAdv.length;
                    }
                    num.advantages.Add(av);
                }
                else
                {
                    MakeGroup(false, null, num.nickname + " loses their vote at the next tribal council.", "", "", new List<Contestant>() { num }, EpisodeStart.transform.GetChild(0), 20);
                    num.votes--;
                }
                num.IOIEvent = "";
            }
        }
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
        curImm++;
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
        idols = 0;
        foreach (Contestant num in MergedTribe.members)
        {
            List<Advantage> Remove = new List<Advantage>();
            if (num.advantages.Count > 0)
            {
                foreach (Advantage advantage in num.advantages)
                {
                    if(advantage.type == "HiddenImmunityIdol" && !advantage.temp)
                    {
                        idols++;
                    }
                    if (advantage.expiresAt > currentContestants || (advantage.length < 1 && advantage.temp))
                    {
                        Remove.Add(advantage);
                    }
                }
                
            }
            foreach (Advantage advantage in Remove)
            {
                num.advantages.Remove(advantage);
            }
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
        foreach(Contestant num in immune)
        {
            num.advantages.Remove(ImmunityNecklace);
        }
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
        Idols = new List<Contestant>();
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
                if(tie.Count < 2 && votes.Count > 1)
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
                        if (!Idols.Contains(votesRead[curVot]))
                        {
                            dicVR.Add(votesRead[i], 1);
                        }
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
        GameObject EpisodeStart = Instantiate(Prefabs[0]);
        EpisodeStart.transform.parent = Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStart.name = "Tribal Council";
        AddGM(EpisodeStart, true);
        List<Contestant> TeamV = new List<Contestant>(team.members);
        TeamV.Remove(immune[0]);
        immune[0].vote = TeamV[Random.Range(0, TeamV.Count)];
        votedOff = immune[0].vote;
        votes = new List<Contestant>() { immune[0].vote };
        //votes.Add(immune[0].vote);
        if(votes.Count < 1)
        {
            Debug.Log("ff");
        }
        AddVote(votes, votes);
        
        MakeGroup(true, team, "name", "", "", team.members, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
        if (cineTribal == true)
        { 
            MakeGroup(false, null, "name", "", votes[0].nickname + ", the tribe has spoken", votes, null, 0);
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
    void ForcedFireMaking()
    {
        GameObject EpisodeStart = Instantiate(Prefabs[0]);
        EpisodeStart.transform.parent = Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStart.name = "Tribal Council";
        AddGM(EpisodeStart, true);
        string giveUp = "";
        Contestant immunity = immune[0];
        List<Contestant> TeamV = new List<Contestant>(MergedTribe.members);
        TeamV.Remove(immunity);
        Contestant safe = TeamV[Random.Range(0, TeamV.Count)];
        TeamV.Remove(safe);
        Contestant otherSafe = TeamV[Random.Range(0, TeamV.Count)];
        if (Random.Range(0, 10) == 0)
        {
            TeamV.Remove(otherSafe);
            TeamV.Add(immunity);
            giveUp = "\n\nHowever, they decide to give up individual immunity and compete in fire.";
        }

        Votes = new List<Vote>();
        votedOff = TeamV[Random.Range(0, TeamV.Count)];
        votes = new List<Contestant>() { votedOff };
        AddVote(votes, votes);

        MakeGroup(false, null, "", "", immunity.nickname + " must grant safety to one other castaway." + giveUp, new List<Contestant>() { immunity}, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
        if(giveUp == "")
        {
            MakeGroup(false, null, "", "", "They grant safety to " + safe.nickname + ".", new List<Contestant>() { safe}, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
        }
        else
        {
            MakeGroup(false, null, "", "", "They give immunity to " + otherSafe.nickname + " and grants safety to " + safe.nickname + ".", new List<Contestant>() { safe, otherSafe}, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
        }

        MakeGroup(false, null, "name", "", "The remaining castaways compete in a fire-making challenge for a place in the final three.", TeamV, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
        string juryPM = "";
        if (currentContestants - finaleAt <= juryAt && !sea.RedemptionIsland && !sea.EdgeOfExtinction)
        {
            float juryy = jury.Count + 1;
            juryPM = " and " + Oridinal(juryy) + " member of the jury";
        }
        float placement = elimed;
        string placementt = "";
        placementt = Oridinal(placement);
        string etext = votes[0].nickname + ", the tribe has spoken.";
        MakeGroup(false, null, "name", "", etext, votes, EpisodeStart.transform.GetChild(0).GetChild(0), 20);
        foreach (Alliance alliance in Alliances)
        {
            if (alliance.members.Contains(votedOff))
            {
                alliance.members.Remove(votedOff);
            }
        }
        MergedTribe.members.Remove(votedOff);
        votedOff.placement = "Juror" + "\n" + "Lost Fire-Making";
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
        EpisodeStart.name = "Fan Favorite";


        AddGM(EpisodeStart, true);

        List<Contestant> a = new List<Contestant>() { fav };
        MakeGroup(false, null, "name", "The fan favorite is...", "", a, EpisodeStart.transform.GetChild(0).GetChild(0), 10);
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
        placement = true;
        //Eliminated.Reverse();
        MakeGroup(false, null, "placement", "", "", Eliminated, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
        placement = false;

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
        EpisodeStart.name = "The Votes";
        AddGM(EpisodeStart, true);
        //curEpp++;
        int d = 0;
        if (LosingTribe == Outcasts)
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
            if (MergedTribe.members.Count + 1 == 3 && immune.Count > 0)
            {
                immune[0].voteReason = "gaming";
                List<Contestant> w = new List<Contestant>() { immune[0].vote, immune[0] };
                string etext = immune[0].nickname + " voted for " + immune[0].vote.nickname + "\n" + "\n" + immune[0].voteReason;
                MakeGroup(false, null, "", "", etext, w, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            }
            else if (MergedTribe.members.Count == finaleAt && Episodes[curEp].events.Contains("Placements") && curEv > 0)
            {
                List<Contestant> juryy = new List<Contestant>(jury);
                foreach (Contestant num in juryy)
                {
                    if (num.vote != null)
                    {
                        num.voteReason = "votes";

                        string etext = num.nickname + " voted for " + num.vote.nickname + "\n" + "\n" + num.voteReason;
                        List<Contestant> w = new List<Contestant>() { num.vote, num };
                        MakeGroup(false, null, "", "", etext, w, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                    }
                }
            }
            else
            {
                List<Vote> a = new List<Vote>(Votes);
                foreach (Vote num in Votes)
                {
                    if(num.voter.nickname == votedOff.nickname)
                    {
                        a.Remove(num);
                    }
                    float tieo = 0;
                    foreach (Contestant numm in LosingTribe.members)
                    {
                        if (numm.inTie)
                        {
                            tieo++;
                        }
                    }
                    num.voter.voteReason = "They voted based on personal preference.";
                    List<Contestant> w = new List<Contestant>() { num.vote, num.voter };
                    if (num.vote != null)
                    {
                        string extraVotes = "";
                        if (num.revotes.Count > 0)
                        {
                            foreach (Contestant vot in num.revotes)
                            {
                                if (vot != num.vote)
                                {
                                    w.Insert(0, vot);
                                    extraVotes += "\n" + "\n" + num.voter.nickname + " voted for " + vot.nickname + " on revote #" + (num.revotes.IndexOf(vot) + 1);
                                }
                                else
                                {
                                    extraVotes += "\n" + "\n" + num.voter.nickname + " voted for " + vot.nickname + " on revote #" + (num.revotes.IndexOf(vot) + 1);
                                }
                            }
                        }
                        else
                        {

                        }
                        if (tieo > 1 && tieo != LosingTribe.members.Count)
                        {
                            if (num.voter.inTie)
                            {
                                extraVotes = "\n" + "\n" + "Couldn't vote in the revote.";
                            }
                        }

                        string etext = num.voter.nickname + " voted for " + num.vote.nickname + extraVotes + "\n" + "\n" + num.voter.voteReason;

                        MakeGroup(false, null, "", "", etext, w, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                        d++;
                    }
                }
            }
            if (Votes.Count == 0)
            {
                MakeGroup(false, null, "", "", "No votes were cast.", new List<Contestant>(), EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            }
            
        }
        foreach (Advantage av in votedOff.advantages)
        {
            if(av.name.Contains("Legacy Advantage"))
            {
                Contestant b = LosingTribe.members[Random.Range(0, LosingTribe.members.Count)];
                b.advantages.Add(av);
                List<Contestant> w = new List<Contestant>() { b, votedOff };
                MakeGroup(false, null, "", "", votedOff.nickname + " gifts the Legacy Advantage to " + b.nickname + ".", w, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
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
        what = true;
        lastVoteOff.SetActive(false);
        if(votesRead.Count > 1)
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
                    
                    if(Idols.Contains(votesRead[0]))
                    {
                        Vote.transform.GetChild(1).GetComponent<Text>().text = "<color=red>DOES NOT COUNT</color>";
                    } else
                    {
                        Vote.transform.GetChild(1).GetComponent<Text>().text = dicVR[votesRead[0]] + votess + votesRead[0].nickname + votesLeft;
                    }
                    curVot++;
                }
                else if (curVot == 0 && currentContestants == 3)
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
                        if (!Idols.Contains(votesRead[curVot]))
                        {
                            dicVR.Add(votesRead[curVot], 1);
                        }
                    }
                    votess = "";
                    votesLeft = "";
                    if (showVL == true)
                    {
                        float vl = votes.Count - curVot - 1 + jurVotesRemoved + Idols.Count;

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
                    if(Idols.Contains(votesRead[curVot]))
                    {
                        Vote.transform.GetChild(1).GetComponent<Text>().text = "<color=red>DOES NOT COUNT</color>";
                    }
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
                        if (currentContestants == finaleAt)
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
            Vote.transform.GetChild(1).GetComponent<Text>().text = "Final vote count was 1 vote " + votes[0].nickname;
            Vote.transform.GetChild(2).GetComponent<Text>().text = "The " + placementt + " eliminated from " + seasonTemp.nameSeason + juryPM + " is... ";
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
                if(contestants > 10)
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
        exileIsland.DoExile();
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
    void PalauStart()
    {
        oneTimeEvents.PalauStart();
    }
    void FijiStart()
    {
        oneTimeEvents.FijiStart();
    }
    IEnumerator SetUp()
    {
        yield return new WaitForSeconds(.01f);
        cineTribal = SeasonMenuManager.instance.cineTribal;
        absorb = SeasonMenuManager.instance.absorb;
        seasonTemp = SeasonMenuManager.instance.curSeason;
        cast = SeasonMenuManager.instance.curCast;
        PlaySeason();
    }
    void PlaySeason()
    {
        curSwap.on = false;
        curExile.on = false;
        
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
        foreach (GameObject torch in Torches)
        {
            torch.SetActive(false);
        }
        cast = Instantiate(cast);
        sea = Instantiate(seasonTemp);
        Tribes = new List<Team>(sea.Tribes);
        mergeAt = sea.mergeAt; juryAt = sea.jury; finaleAt = sea.final;
        bool oneEv = false;
        foreach (OneTimeEvent one in sea.oneTimeEvents)
        {
            if((one.type == "SchoolyardPick" || one.type == "PalauStart" || one.type == "FijiStart") && one.context != "FirstLast" )
            {
                oneEv = true;
                if(one.type == "FijiStart")
                {
                    currentContestants++;
                }
            }
        }
        if(!oneEv)
        {
            SetSeason();
            int male = 0;
            int female = 0;
            foreach (Team tribe in Tribes)
            {
                //MakeAlliances(tribe);
                currentContestants += tribe.members.Count;
                foreach (Contestant num in tribe.members)
                {
                    num.teams.Add(tribe.tribeColor);
                    if (num.gender == "M")
                    {
                        male++;
                    }
                    else if (num.gender == "F")
                    {
                        female++;
                    }
                }
                if (tribe.hiddenAdvantages.Count > 0)
                {
                    advant = true;
                }
            }
            if (male == female)
            {
                genderEqual = true;
            }
        } else
        {
            foreach (Team tribe in sea.Tribes)
            {
                currentContestants += tribe.members.Count;
            }

        }
        nextButton.onClick.AddListener(NextGM);
        lastButton.onClick.AddListener(LastGM);
        
        if (sea.mergeHiddenAdvantages.Count > 0)
        {
            advant = true;
        }
        if (sea.islandHiddenAdvantages.Count > 0)
        {
            advant = true;
        }
        if (!sea.Outcasts)
        {
            OCExpired = true;
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
    public void Quit()
    {
        Destroy(SeasonMenuManager.instance.gameObject);
        SceneManager.LoadScene(0);
    }

    public void MakeGroup(bool nameEnabled, Team teem, string conText, string aText, string eText, List<Contestant> cons, Transform ep, float spacing)
    {
        GameObject team = Instantiate(GroupPrefab);
        team.GetComponent<UIGroup>().tribeName.enabled = nameEnabled;
        if(nameEnabled)
        {
            team.GetComponent<UIGroup>().tribeName.text = teem.name;
            team.GetComponent<UIGroup>().tribeName.color = teem.tribeColor;
        } else
        {
            team.GetComponent<UIGroup>().tribeName.gameObject.SetActive(false);
        }
        int highestLength = 0;
        int er = Eliminated.Count;
        if (placement)
        {
            if (Eliminated.Count % 6 == 0)
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
                else if (conText == "placement")
                {
                    string place = Oridinal(Eliminated.Count - Eliminated.IndexOf(num)) + " Place";
                    if (Eliminated.Count - Eliminated.IndexOf(num) == 1)
                    {
                        place = "Winner";
                    }
                    num.placement = place + "\n" + num.placement;
                    mem.GetComponentInChildren<Text>().text = num.fullname + "\n " + num.placement;
                }
                else 
                {
                    mem.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
                    mem.GetComponentInChildren<Text>().text = conText;
                }
                if(all && !Tribes[curT].members.Contains(num))
                {
                    mem.GetComponentInChildren<Image>().material = grayScale;
                }

                mem.transform.parent = team.transform.GetChild(2);
                if((nameEnabled || all) && num.teams.Count > 1)
                {
                    mem.transform.GetChild(1).gameObject.SetActive(true);
                    for (int j = 0; j < num.teams.Count - 1; j++)
                    {
                        GameObject image = Instantiate(imagePrefab);
                        image.GetComponent<Image>().color = num.teams[j];
                        image.transform.parent = mem.transform.GetChild(1);
                    }
                }
                if (placement)
                {
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
        } else
        {
            AddFinalVote(team);
        }
        float teamWidth = ConListWidth(team.transform.GetChild(2).childCount);
        if (placement)
        {
            team.GetComponent<UIGroup>().List.GetComponent<FlowLayoutGroup>().SpacingY = (16 * highestLength) + 2;
            team.GetComponent<UIGroup>().List.GetComponent<FlowLayoutGroup>().SpacingX = 40;
        }
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