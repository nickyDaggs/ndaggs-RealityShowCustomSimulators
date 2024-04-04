using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeasonParts;
using System.Linq;

public class ExileIsland : MonoBehaviour
{
    public Contestant nextIOI;
    public Advantage SwapIdol;
    public List<Advantage> NewEraBagsAdvantages;

    public void Start()
    {
        for (int i = NewEraBagsAdvantages.Count - 1; i > 0; i--)
        {
            int swapIndex = Random.Range(0, i + 1);
            Advantage currentCon = NewEraBagsAdvantages[i];
            Advantage conToSwap = NewEraBagsAdvantages[swapIndex];
            NewEraBagsAdvantages[i] = conToSwap;
            NewEraBagsAdvantages[swapIndex] = currentCon;
        }
    }

    public void DoExile()
    {
        GameObject ExileEvent = Instantiate(GameManager.instance.Prefabs[0]);
        ExileEvent.transform.parent = GameManager.instance.Canvas.transform;
        ExileEvent.GetComponent<RectTransform>().offsetMax = new Vector2(0, ExileEvent.GetComponent<RectTransform>().offsetMax.y);
        ExileEvent.GetComponent<RectTransform>().offsetMax = new Vector2(ExileEvent.GetComponent<RectTransform>().offsetMin.x, 0);
        GameManager.instance.AddGM(ExileEvent, true, 0);

        if(GameManager.instance.curExile.reason == "Random" && GameManager.instance.sea.IslandType == "IOI")
        {
            GameManager.instance.Exiled = new List<Contestant>();
            GameManager.instance.Exiled.Add(nextIOI);
        } 

        string island = "";
        if (GameManager.instance.sea.IslandType == "Ghost")
        {
            island = "Ghost Island.";
        } else if (GameManager.instance.sea.IslandType == "IOI")
        {
            island = "Island of the Idols.";
            GameManager.instance.MakeGroup(true, GameManager.instance.sea.Twists.IOI, "name", "", "", GameManager.instance.sea.Twists.IOI.members, ExileEvent.transform.GetChild(0).GetChild(0), 0);
        } 
        else
        {
            island = "Exile Island.";
        }
        if(GameManager.instance.sea.IslandType != "Journeys" && GameManager.instance.curExile.reason != "Note" && GameManager.instance.curExile.reason != "IOITribeSend" && GameManager.instance.curExile.reason != "RandomSend")
        {
            if (GameManager.instance.Exiled.Count == 1)
            {
                GameManager.instance.MakeGroup(false, null, GameManager.instance.Exiled[0].nickname + " is at " + island, "", "", GameManager.instance.Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);
            }
            else if (GameManager.instance.Exiled.Count > 1 && GameManager.instance.curExile.two)
            {
                GameManager.instance.MakeGroup(false, null, "", "", GameManager.instance.Exiled[0].nickname + " and " + GameManager.instance.Exiled[1].nickname + " are at exile island.", GameManager.instance.Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);
            }
        } else
        {
            //Debug.Log("dsfa");
            if (GameManager.instance.sea.IslandType == "IOI")
            {
                GameManager.instance.Exiled = new List<Contestant>();
                GameManager.instance.Exiled.Add(GameManager.Instance.MergedTribe.members[Random.Range(0, GameManager.Instance.MergedTribe.members.Count)]);
            }
            
            if (GameManager.instance.curExile.reason == "Note")
            {
                GameManager.instance.MakeGroup(false, null, "", "", "There is a hanging note enticing one player to visit the Island of the Idols.\n\n" + GameManager.instance.Exiled[0].nickname + " arrives.", GameManager.instance.Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);
            } else if(GameManager.instance.curExile.reason == "IOITribeSend")
            {
                GameManager.instance.MakeGroup(false, null, "", "", "The tribe sends one person to visit the Island of the Idols.\n\n" + GameManager.instance.Exiled[0].nickname + " arrives.", GameManager.instance.Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);
            }
            else if (GameManager.instance.curExile.reason == "RandomSend")
            {
                GameManager.instance.MakeGroup(false, null, "", "", "One person is randomly sent to the Island of the Idols.\n\n" + GameManager.instance.Exiled[0].nickname + " arrives.", GameManager.instance.Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);
            }
        }
        
        if(GameManager.instance.sea.IslandType == "Ghost")
        {
            string atext = "There is no secret advantage available.";
            bool adv = false;
            foreach (HiddenAdvantage hid in GameManager.instance.sea.islandHiddenAdvantages)
            {
                if (hid.hideAt == GameManager.instance.currentContestants)
                {
                    int ran = Random.Range(0, 7);
                    string nam = hid.name;
                    atext = "There is a secret advantage available.\n\n" + GameManager.instance.Exiled[0].nickname + " can wager their next vote for a chance to win.";
                    if (!adv)
                    {
                        adv = true;
                    }
                    foreach (Contestant num in GameManager.instance.Exiled)
                    {
                        if(hid.advantage.type.Contains("ImmunityIdol") && hid.advantage.temp)
                        {
                            Advantage av = Instantiate(hid.advantage);
                            av.name = "Temporary Hidden Immunity Idol";
                            av.temp = true;
                            av.length = 1;
                            atext = GameManager.instance.Exiled[0].nickname + " finds a Temporary Hidden Immunity Idol that's good for one round.\n\nThey can wager their vote to extend it up to 5 extra rounds.";

                            if (ran < num.stats.Boldness)
                            {
                                bool b = false;
                                string loseVote = "";
                                while(!b)
                                {
                                    int ran2 = Random.Range(0, 7);
                                    if(ran2 < num.stats.Mental)
                                    {
                                        av.length++;
                                        int Ran = Random.Range(0, 2);
                                        if (Ran == 1 || av.length == 6)
                                        {
                                            b = true;
                                            loseVote = "\n\nThey do not lose their vote.";
                                        }
                                    } else
                                    {
                                        loseVote = "\n\nThey lose their vote.";
                                        GameManager.instance.Exiled[0].votes--;
                                        b = true;
                                    }
                                }
                                atext += "\n\nThey wager their vote";
                                if (av.length - 1 >= 1)
                                {
                                    atext += "\n\n" + GameManager.instance.Exiled[0].nickname + " extends the life of their idol by " + (av.length - 1) + " extra rounds." ;
                                }
                                atext += loseVote;
                            }
                            else
                            {
                                atext += "\n\nThey decide to not wager their vote";
                            }
                            num.advantages.Add(av);
                        } else
                        {
                            if (ran < num.stats.Boldness)
                            {
                                int ran2 = Random.Range(0, 7);
                                if (ran2 <= num.stats.Mental)
                                {
                                    Advantage av = Instantiate(hid.advantage);
                                    av.name = hid.name;
                                    if (hid.temp)
                                    {
                                        av.temp = true;
                                        av.length = hid.length;
                                    }
                                    
                                    atext += "\n\nThey wager their vote and win the secret advantage.\n\nIt's a " + hid.name;
                                    if(hid.giveAway)
                                    {
                                        atext += "\n\n" + hid.advantage.description;
                                        atext += "\n\nHowever, this must be immediately given to someone on another tribe.";
                                        List<Team> teamV = new List<Team>();
                                        foreach(Team t in GameManager.instance.Tribes)
                                        {
                                            if(t.name != num.team)
                                            {
                                                teamV.Add(t);
                                            }
                                        }
                                        Team a = teamV[Random.Range(0, teamV.Count)];
                                        Contestant b = a.members[Random.Range(0, a.members.Count)];
                                        b.advantages.Add(av);
                                    } else
                                    {
                                        if(hid.name == "Challenge Advantage")
                                        {
                                            num.challengeAdvantage = true;
                                        } else
                                        {
                                            num.advantages.Add(av);
                                            atext += "\n\n" + hid.advantage.description;
                                        }
                                    }
                                }
                                else
                                {
                                    atext += "\n\nThey wager their vote and lose.";
                                    GameManager.instance.Exiled[0].votes--;
                                }
                            }
                            else
                            {
                                atext += "\n\nThey decide to not wager their vote";
                            }
                        }
                        
                    }
                }
            }
            GameManager.instance.MakeGroup(false, null, "", atext, "", new List<Contestant>(), ExileEvent.transform.GetChild(0).GetChild(0), 0);
        }
        else if(GameManager.instance.sea.IslandType == "IOI")
        {
            bool adv = false;
            foreach(HiddenAdvantage hid in GameManager.instance.sea.islandHiddenAdvantages)
            {
                if (hid.hideAt == GameManager.instance.curEp + 1)
                {
                    int ran = 1;//Random.Range(0, 2);
                    string nam = hid.name;
                    if (!adv)
                    {
                        adv = true;
                    }
                    string atext = "The lesson is <b>" + hid.IOILesson + "</b>";
                    Contestant idol = GameManager.instance.sea.Twists.IOI.members[Random.Range(0, GameManager.instance.sea.Twists.IOI.members.Count)];
                    
                    string etext = "";
                    foreach(Contestant num in GameManager.instance.Exiled)
                    {
                        num.IOIEvent = "";
                        
                        if (hid.temp)
                        {
                            if (hid.length > 1)
                            {
                                atext += "\n\nIn order to win a " + hid.name + " good for the next " + hid.length + " Tribal Councils, ";
                            }
                            else
                            {
                                atext += "\n\nIn order to win a " + hid.name + " good for the next Tribal Council, ";
                            }
                        }
                        else
                        {
                            atext += "\n\nIn order to win a " + hid.name + ", ";
                        }
                        if (hid.options.Count > 1)
                        {
                            atext = "";
                            atext += "\n\nIn order to win a " + string.Join(", ", new List<HiddenAdvantage>(hid.options).ConvertAll(i => i.name));
                            atext = atext.Replace(", " + hid.options[hid.options.Count - 1].name, " or " + hid.options[hid.options.Count - 1].name);
                            foreach(HiddenAdvantage hidden in hid.options)
                            {
                                if (hidden.temp)
                                {
                                    if(hidden.length > 1)
                                    {
                                        atext += " good for the next " + hidden.length + " tribal councils, ";
                                    } else
                                    {
                                        atext += " good for the next tribal council, ";
                                    }
                                    
                                }
                                
                            }
                        }
                        switch (hid.IOILesson)
                        {
                            case "Fire-making":
                                atext += num.nickname + " must defeat " + idol.nickname + " in a fire-making challenge.";
                                break;
                            case "Paying attention":
                                atext += num.nickname + " must defeat answer four out of five questions correctly about the personal life of " + idol.nickname + ".";
                                break;
                            case "Staying calm under pressure":
                                atext += num.nickname + " must sneak into the other tribe's camp, light a torch, and sneak back without getting caught.";
                                break;
                            case "Persuasion":
                                atext += num.nickname + " must persuade their tribemates to let her be the caller in the next Immunity Challenge.";
                                
                                if(Random.Range(0, 2) == 1)
                                {
                                    etext += "They decide to play.";
                                    num.savedAdv = hid;
                                    num.IOIEvent = "CallerVB";
                                } else
                                {
                                    etext += "They decide not to play.";
                                    num.IOIEvent = "A";
                                }
                                break;
                            case "Courage":
                                atext += num.nickname + " must be able to obtain the Vote Blocker hidden at the next Immunity Challenge.";
                                if (Random.Range(0, 2) == 1)
                                {
                                    etext += "They decide to play.";
                                    num.savedAdv = hid;
                                    num.IOIEvent = "HiddenVB";
                                }
                                else
                                {
                                    etext += "They decide not to play.";
                                    num.IOIEvent = "A";
                                }
                                break;
                            case "Calculating risks":
                                atext += num.nickname + " must complete an unknown challenge.";
                                break;
                            case "Taking false opportunities":
                                atext = "There is no game. " + num.nickname + " loses their vote at the next tribal council just by arriving at the Island of the Idols. They are given the opportunity to sabotage another player.";
                                num.IOIEvent = "A";
                                num.votes--;
                                break;
                            case "Situational awareness":
                                atext += num.nickname + " must correctly predict the person who will win the next Immunity Challenge.";
                                
                                if (Random.Range(0, 2) == 1)
                                {
                                    etext += "They decide to play.";
                                    num.savedAdv = hid;
                                    num.IOIEvent = "PredictImmunity";
                                    num.target = GameManager.instance.MergedTribe.members[Random.Range(0, GameManager.instance.MergedTribe.members.Count)];
                                }
                                else
                                {
                                    etext += "They decide not to play.";
                                    num.IOIEvent = "A";
                                }
                                break;
                            case "Jury management":
                                atext += num.nickname + " must win a coin toss.";
                                break;
                        }
                        if (num.IOIEvent == "")
                        {
                            if (ran == 1)
                            {
                                int ran2 = Random.Range(0, 2);
                                if (ran2 == 1)
                                {
                                    HiddenAdvantage a = new HiddenAdvantage();
                                    string opt = "";
                                    if (hid.options.Count > 1)
                                    {
                                        a = hid.options[Random.Range(0, hid.options.Count)];
                                        opt = "They choose the " + a.name + ".";
                                    } else
                                    {
                                        a = hid;
                                    }
                                    Advantage av = Instantiate(a.advantage);
                                    av.nickname = a.name;
                                    if (a.temp && hid.options.Count < 2)
                                    {
                                        av.temp = true;
                                        av.length = a.length;
                                        if(a.length > 1)
                                        {
                                            etext += "They decide to play.\n\n" + num.nickname + " wins!\n\nThe prize is a " + a.name + " that's good for " + hid.length + " Tribal Councils.";
                                            if(opt != "")
                                            {
                                                etext = etext.Replace("The prize is a " + a.name + " that's good for " + hid.length + " Tribal Councils.", opt);
                                            }
                                        }
                                        else
                                        {
                                            etext += "They decide to play.\n\n" + num.nickname + " wins!\n\nThe prize is a " + a.name + " that's good for the next Tribal Council.";
                                            if (opt != "")
                                            {
                                                etext = etext.Replace("The prize is a " + a.name + " that's good for for the next Tribal Council.", opt);
                                            }
                                        }
                                    }
                                    else 
                                    {
                                        etext += "They decide to play.\n\n" + num.nickname + " wins!\n\nThe prize is a " + a.name;
                                        if (opt != "")
                                        {
                                            etext = etext.Replace("The prize is a " + a.name, opt);
                                        }
                                    }
                                    num.advantages.Add(av);
                                    //etext += "\n\n" + hid.advantage.description;
                                }
                                else
                                {
                                    etext += "They decide to play.\n\n" + num.nickname + " loses!\n\nThe penalty is a lost vote at the next tribal council.";
                                    GameManager.instance.Exiled[0].votes--;
                                }
                            }
                            else
                            {
                                etext += "They decide not to play.";

                                if (hid.IOISweetened)
                                {
                                    if (hid.temp)
                                    {
                                        hid.length++;
                                    }
                                    etext += "\n\nThe deal is sweetened.";
                                    if (hid.temp)
                                    {
                                        etext += "\n\nIn order to win a " + hid.name + " good for the next <b>" + hid.length + "<\b> Tribal Councils, ";
                                    }
                                    else
                                    {
                                        etext += "\n\nIn order to win a " + hid.name + ", ";
                                    }
                                    switch (hid.IOILesson)
                                    {
                                        case "Paying attention":
                                            etext += num.nickname + " must defeat answer <b>three<\b> out of five questions correctly about the personal life of " + idol.nickname + ".";
                                            break;
                                        default:
                                            etext += num.nickname + " must defeat " + idol.nickname + " in an easier challenge.";
                                            break;
                                    }
                                    int Ran = Random.Range(0, 2);
                                    if (Ran == 1)
                                    {

                                        int ran2 = Random.Range(0, 2);
                                        if (ran2 == 1)
                                        {
                                            Advantage av = Instantiate(hid.advantage);
                                            av.nickname = hid.name;
                                            if (hid.temp)
                                            {
                                                av.temp = true;
                                                av.length = hid.length;
                                            }


                                            if (hid.temp)
                                            {
                                                etext += "\n\nThey decide to play.\n\n" + num.nickname + " wins!\n\nThe prize is a "+ hid.name + " that's good for the next" + hid.length + " Tribal Councils.";
                                            }
                                            else
                                            {
                                                etext += "\n\nThey decide to play.\n\n" + num.nickname + " wins!\n\nThe prize is a " + hid.name;
                                            }
                                            num.advantages.Add(av);
                                        }
                                        else
                                        {
                                            etext += "\n\nThey decide to play.\n\n" + num.nickname + " loses!\n\nThe penalty is a lost vote at the next tribal council.";
                                            GameManager.instance.Exiled[0].votes--;
                                        }
                                    }
                                    else
                                    {
                                        etext += "\n\nThey still decide not to play.";
                                    }

                                }
                            }

                        }
                        GameManager.instance.MakeGroup(false, null, "", atext, etext, new List<Contestant>() { num }, ExileEvent.transform.GetChild(0).GetChild(0), 0);
                        if(num.IOIEvent == "PredictImmunity")
                        {
                            List<Contestant> n = new List<Contestant>() {num.target};
                            GameManager.instance.MakeGroup(false, null, "", "", num.nickname  + " predicts " + num.target.nickname + " will win immunity.", n, ExileEvent.transform.GetChild(0).GetChild(0), 0);
                        }
                    }
                    
                }
            }
            if(GameManager.instance.curExile.exileEvent == "ChooseRandom")
            {
                GameManager.instance.MakeGroup(false, null, "Before leaving, " + GameManager.instance.Exiled[0].nickname + " must randomly select one player on the other tribe to visit Island of the Idols next.", "", "", GameManager.instance.Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);

                Team nextTribe = GameManager.instance.Tribes.FindAll(x => x.name != GameManager.instance.Exiled[0].team).ToList()[0];
                nextIOI = nextTribe.members[Random.Range(0, nextTribe.members.Count)];
                //"Before leaving, " + GameManager.instance.Exiled[0].nickname + " must randomly select one player on the other tribe to visit Island of the Idols next."
            }
        }
        else if (GameManager.instance.sea.IslandType == "Journeys")
        {

            if (GameManager.instance.Exiled.Count < 1)
            {
                for(int i = 0; i < GameManager.Instance.Tribes.Count; i++)
                {
                    GameManager.instance.Exiled.Add(GameManager.Instance.Tribes[i].members[Random.Range(0, GameManager.Instance.Tribes[i].members.Count)]);
                    if(GameManager.instance.curExile.exileEvent == "SwapIdol")
                    {
                        GameManager.instance.Exiled[i].team = GameManager.Instance.Tribes[i].name;

                    }
                }
            }
            if(GameManager.instance.curExile.exileEvent != "PublicRisk")
            {
                GameManager.instance.MakeGroup(false, null, "nname", "", "The " + GameManager.instance.Exiled.Count + " chosen players from separate tribes go on a journey together.\n\nAt the end of their journey, they are told to separate to go make a private decision.", GameManager.instance.Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);
            }
            else
            {
                GameManager.instance.MakeGroup(false, null, "nname", "", "The " + GameManager.instance.Exiled.Count + " chosen players from separate tribes go on a journey together.", GameManager.instance.Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);
            }

            HiddenAdvantage curAdv = new HiddenAdvantage();

            string advJourney = "The advantage for risking a vote will be a/an ";

            int risked = 0;

            switch (GameManager.instance.curExile.exileEvent)
            {
                case "Shipwheel":
                    
                    foreach (HiddenAdvantage hid in GameManager.instance.sea.islandHiddenAdvantages)
                    {
                        if (hid.hideAt == GameManager.instance.currentContestants)
                        {
                            int ran = Random.Range(0, 7);
                            string nam = hid.name;
                            advJourney += nam + ".";
                            curAdv = hid;
                            GameManager.instance.JourneyAdvantage = hid;
                            //advJorne
                        }
                    }
                    GameManager.instance.MakeGroup(false, null, "", "", "Each of the players are faced with a dilemma: they can choose to 'protect' their vote or 'risk' it for an advantage.\n\nIf they unanimously choose 'protect' then they will all keep their votes and nothing will happen.\n\nIf they unanimously choose to 'risk' then they will all lose their votes for their next tribal council.\n\nIf there is a split in their decisions, then those who chose 'risk' will earn an advantage and keep their vote.\n\n" + advJourney, GameManager.instance.Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);

                    for(int i = 0; i < GameManager.Instance.Exiled.Count; i++)
                    {
                        string decision = "";
                        if(Random.Range(1, 7) <= GameManager.Instance.Exiled[i].stats.Boldness)
                        {
                            decision = " risks their vote.";
                            GameManager.Instance.Exiled[i].JourneyRisk.Add(curAdv.IOILesson);
                            GameManager.Instance.Exiled[i].journeyAdv.Add(curAdv);
                            risked++;
                        } else
                        {
                            decision = " protects their vote.";
                        }
                        GameManager.instance.MakeGroup(false, null, "", "", GameManager.Instance.Exiled[i].nickname + decision, new List<Contestant>() { GameManager.Instance.Exiled[i] }, ExileEvent.transform.GetChild(0).GetChild(0), 20);
                    }
                    if(GameManager.Instance.Exiled.Count == risked)
                    {
                        
                        for (int i = 0; i < GameManager.Instance.Exiled.Count; i++)
                        {
                            GameManager.Instance.Exiled[i].JourneyRisk = new List<string>();
                            GameManager.Instance.Exiled[i].votes--;
                            //Debug.Log(GameManager.Instance.Exiled[i].votes);
                        }
                    }
                    for (int i = 0; i < GameManager.Instance.Tribes.Count; i++)
                    {
                        for (int j = 0; j < GameManager.Instance.Tribes[i].members.Count; j++)
                        {
                            Contestant real = GameManager.Instance.Exiled.Find(x => x.simID == GameManager.Instance.Tribes[i].members[j].simID);
                            if(real != null)
                            {
                                
                                GameManager.Instance.Tribes[i].members[j] = real;
                                //Debug.Log(GameManager.Instance.Tribes[i].members[j].votes);
                            }
                        }
                    }
                    break;
                case "Tarp":
                    advJourney = "The advantage would be a/an ";

                    foreach (HiddenAdvantage hid in GameManager.instance.sea.islandHiddenAdvantages)
                    {
                        if (hid.hideAt == GameManager.instance.currentContestants)
                        {
                            int ran = Random.Range(0, 7);
                            string nam = hid.name;
                            advJourney += nam + ".";
                            curAdv = hid;
                            GameManager.instance.JourneyAdvantage = hid;
                            //advJorne
                        }
                    }
                    GameManager.instance.MakeGroup(false, null, "", "", "Each of the players are faced with a dilemma: they can choose to get a tarp for their tribe or risk their vote for an advantage.\n\nIf they unanimously choose to get a tarp then they all of their tribes will earn a tribe.\n\nIf they unanimously choose to risk their vote then they will all lose their votes for their next tribal council.\n\nIf there is a split in their decisions, then those who chose to risk a vote will earn an advantage, while those who chose a tarp get nothing.\n\n" + advJourney, GameManager.instance.Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);
                    

                    for (int i = 0; i < GameManager.Instance.Exiled.Count; i++)
                    {
                        string decision = "";
                        if (Random.Range(1, 7) <= GameManager.Instance.Exiled[i].stats.Boldness)
                        {
                            decision = " chooses an advantage.";
                            GameManager.Instance.Exiled[i].JourneyRisk.Add(curAdv.IOILesson);
                            GameManager.Instance.Exiled[i].journeyAdv.Add(curAdv);
                            risked++;
                        }
                        else
                        {
                            decision = " chooses a tarp for their tribe.";
                        }
                        GameManager.instance.MakeGroup(false, null, "", "", GameManager.Instance.Exiled[i].nickname + decision, new List<Contestant>() { GameManager.Instance.Exiled[i] }, ExileEvent.transform.GetChild(0).GetChild(0), 20);
                    }
                    if (GameManager.Instance.Exiled.Count == risked)
                    {
                        for (int i = 0; i < GameManager.Instance.Exiled.Count; i++)
                        {
                            GameManager.Instance.Exiled[i].JourneyRisk = new List<string>();
                            GameManager.Instance.Exiled[i].votes--;
                            //Debug.Log(GameManager.Instance.Exiled[i].votes);
                        }
                    }

                    for (int i = 0; i < GameManager.Instance.Tribes.Count; i++)
                    {
                        for (int j = 0; j < GameManager.Instance.Tribes[i].members.Count; j++)
                        {
                            Contestant real = GameManager.Instance.Exiled.Find(x => x.simID == GameManager.Instance.Tribes[i].members[j].simID);
                            if (real != null)
                            {
                                GameManager.Instance.Tribes[i].members[j] = real;
                                
                                //Debug.Log(GameManager.Instance.Tribes[i].members[j].votes);
                            }
                            if (risked == 0)
                            {
                                GameManager.Instance.Tribes[i].members[j].stats.Stamina += 10;
                            }
                        }
                    }
                    break;
                case "PublicRisk":
                    foreach (HiddenAdvantage hid in GameManager.instance.sea.islandHiddenAdvantages)
                    {
                        if (hid.hideAt == GameManager.instance.currentContestants)
                        {
                            int ran = Random.Range(0, 7);
                            string nam = hid.name;
                            advJourney += nam + ".";
                            curAdv = hid;
                            GameManager.instance.JourneyAdvantage = hid;
                            //advJorne
                        }
                    }
                    GameManager.instance.MakeGroup(false, null, "", "", "Each of the players are publicly faced with a dilemma: they can choose to 'protect' their vote or 'risk' it for an advantage.\n\nThere are three bags that correspond to amount of players risking their vote.\n\nIn each bag, there is one token which grants the advantage.\n\nIf a player receives an empty token, they get no advantage and lose their vote." + advJourney, GameManager.instance.Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);

                    List<Contestant> riskers = new List<Contestant>();

                    for (int i = 0; i < GameManager.Instance.Exiled.Count; i++)
                    {
                        string decision = "";
                        if (Random.Range(1, 7) <= GameManager.Instance.Exiled[i].stats.Boldness)
                        {
                            decision = " risks their vote.";
                            riskers.Add(GameManager.Instance.Exiled[i]);
                            risked++;
                        }
                        else
                        {
                            decision = " protects their vote.";
                        }
                        GameManager.instance.MakeGroup(false, null, "", "", GameManager.Instance.Exiled[i].nickname + decision, new List<Contestant>() { GameManager.Instance.Exiled[i] }, ExileEvent.transform.GetChild(0).GetChild(0), 20);
                    }
                    int ranWin = Random.Range(0, riskers.Count);
                    for(int i = 0; i < riskers.Count; i++)
                    {
                        if(i == ranWin)
                        {
                            riskers[i].JourneyRisk.Add(curAdv.IOILesson);
                            riskers[i].journeyAdv.Add(curAdv);
                            Debug.Log(riskers[i].nickname);
                        } else
                        {
                            riskers[i].JourneyRisk = new List<string>();
                            riskers[i].votes--;
                        }
                    }

                    for (int i = 0; i < GameManager.Instance.Tribes.Count; i++)
                    {
                        for (int j = 0; j < GameManager.Instance.Tribes[i].members.Count; j++)
                        {
                            Contestant real = GameManager.Instance.Exiled.Find(x => x.simID == GameManager.Instance.Tribes[i].members[j].simID);
                            if (real != null)
                            {

                                GameManager.Instance.Tribes[i].members[j] = real;
                                //Debug.Log(GameManager.Instance.Tribes[i].members[j].votes);
                            }
                        }
                    }
                    break;
                    //Do this
                case "ForcedBag":
                    GameManager.instance.MakeGroup(false, null, "", "", "Each of the players are faced with a bag that contains three packages. Two of the packages will cause them to lose their vote, one of the packages holds an advantage.\n\nThey must draw at least one package.\n\nIf they draw the advantage, they can leave.\n\nIf they lose their vote, they can draw from the bag again to either lose another vote or gain the advantage.\n\nEach player will get a different advantage if they draw one.", GameManager.instance.Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);
                    for (int i = 0; i < GameManager.Instance.Exiled.Count; i++)
                    {
                        string decision = "";
                        int ran = Random.Range(0, 3);
                        if(ran == 0)
                        {
                            decision = " successfully pulls the advantage!\n\nThey obtain the " + NewEraBagsAdvantages[i].name + ".";
                            GameManager.Instance.Exiled[i].advantages.Add(NewEraBagsAdvantages[i]);
                        } else
                        {
                            decision = " loses their vote on their first pull.\n\nThey could leave now or try to pull from the bag again.";
                            GameManager.Instance.Exiled[i].votes--;
                            if (Random.Range(1, 7) <= GameManager.Instance.Exiled[i].stats.Boldness)
                            {
                                ran = Random.Range(0, 2);
                                if (ran == 0)
                                {
                                    decision += "\n\nThey successfully pull the advantage!\n\nThey obtain the " + NewEraBagsAdvantages[i].name + ".";
                                    GameManager.Instance.Exiled[i].advantages.Add(NewEraBagsAdvantages[i]);
                                } else
                                {
                                    decision += "\n\nThey lose their vote again on their second pull.";
                                }
                            } else
                            {
                                decision += "\n\nThey choose to leave and return back to their camp.";
                            }
                        }
                        GameManager.instance.MakeGroup(false, null, "", "", GameManager.Instance.Exiled[i].nickname + decision, new List<Contestant>() { GameManager.Instance.Exiled[i] }, ExileEvent.transform.GetChild(0).GetChild(0), 20);
                    }

                    for (int i = 0; i < GameManager.Instance.Tribes.Count; i++)
                    {
                        for (int j = 0; j < GameManager.Instance.Tribes[i].members.Count; j++)
                        {
                            Contestant real = GameManager.Instance.Exiled.Find(x => x.simID == GameManager.Instance.Tribes[i].members[j].simID);
                            if (real != null)
                            {

                                GameManager.Instance.Tribes[i].members[j] = real;
                                //Debug.Log(GameManager.Instance.Tribes[i].members[j].votes);
                            }
                        }
                    }
                    break;
                case "IndividualChallenge":
                    foreach (HiddenAdvantage hid in GameManager.instance.sea.islandHiddenAdvantages)
                    {
                        if (hid.hideAt == GameManager.instance.currentContestants)
                        {
                            int ran = Random.Range(0, 7);
                            string nam = hid.name;
                            advJourney += nam + ".";
                            curAdv = hid;
                            GameManager.instance.JourneyAdvantage = hid;
                            //advJorne
                        }
                    }
                    GameManager.instance.MakeGroup(false, null, "", "", "Each of the players are faced with a dilemma: they can choose to protect their vote by going back to camp or risk their vote for an advantage by playing a challenge.\n\nIf they win the challenge, they receive an advantage.\n\nIf they lose, they lose their vote for the next tribal council.\n\n" + advJourney, GameManager.instance.Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);
                    for (int i = 0; i < GameManager.Instance.Exiled.Count; i++)
                    {
                        string decision = "";
                        if (Random.Range(1, 7) <= GameManager.Instance.Exiled[i].stats.Boldness)
                        {
                            decision = " risks their vote.";
                            if (Random.Range(1, 7) <= GameManager.Instance.Exiled[i].stats.Mental)
                            {
                                GameManager.Instance.Exiled[i].advantages.Add(curAdv.advantage);
                                decision += "\n\nThey successfully win the challenge and earn the " + curAdv.name;
                            }
                            else
                            {
                                GameManager.Instance.Exiled[i].votes--;
                                decision += "\n\nThey lose the challenge and have lost their vote.";
                            }
                        }
                        else
                        {
                            decision = " protects their vote and heads back to camp.";
                        }
                        GameManager.instance.MakeGroup(false, null, "", "", GameManager.Instance.Exiled[i].nickname + decision, new List<Contestant>() { GameManager.Instance.Exiled[i] }, ExileEvent.transform.GetChild(0).GetChild(0), 20);
                    }

                    for (int i = 0; i < GameManager.Instance.Tribes.Count; i++)
                    {
                        for (int j = 0; j < GameManager.Instance.Tribes[i].members.Count; j++)
                        {
                            Contestant real = GameManager.Instance.Exiled.Find(x => x.simID == GameManager.Instance.Tribes[i].members[j].simID);
                            if (real != null)
                            {

                                GameManager.Instance.Tribes[i].members[j] = real;
                                //Debug.Log(GameManager.Instance.Tribes[i].members[j].votes);
                            }
                        }
                    }
                    break;
                case "AmuletDilemma":
                    break;
                case "SwapIdol":
                    GameManager.instance.MakeGroup(false, null, "", "", "Each of the players will now be swapped into a new tribe.\n\nEach of them receive an idol that will only work before tribes are on the same beach.", GameManager.instance.Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);
                    for (int i = 0; i < GameManager.instance.Exiled.Count; i++)
                    {
                        Contestant swapped = GameManager.instance.Exiled[i];
                        swapped.advantages.Add(SwapIdol);
                        int gam = i + 1;
                        if (gam > GameManager.instance.Exiled.Count - 1)
                        {
                            gam = 0;
                        }
                        GameManager.instance.Tribes.Find(x => x.name == swapped.team).members.Remove(swapped);
                        swapped.team = "";
                        GameManager.instance.Tribes[gam].members.Add(swapped);
                        
                        foreach (Alliance alliance in GameManager.instance.Alliances)
                        {
                            if (alliance.members.Contains(swapped))
                            {
                                if (!alliance.teams.Contains(GameManager.instance.Tribes[gam].name))
                                {
                                    alliance.teams.Add(GameManager.instance.Tribes[gam].name);
                                }
                            }
                        }

                    }
                    foreach (Team tribe in GameManager.instance.Tribes)
                    {
                        foreach (Contestant num in tribe.members)
                        {
                            num.teams.Add(tribe.tribeColor);
                        }
                    }
                    break;
            }
        }
        else
        {
            if (GameManager.instance.sea.islandHiddenAdvantages.Count > 0)
            {
                GameManager.instance.MakeGroup(false, null, "", "<b>Advantages</b>", "", new List<Contestant>(), ExileEvent.transform.GetChild(0).GetChild(0), 0);
            }
            bool adv = false;
            bool t = false;
            foreach(HiddenAdvantage hid in GameManager.instance.sea.islandHiddenAdvantages)
            {
                bool asdsa = false;
                if (hid.advantage.type == "HiddenImmunityIdol" && GameManager.instance.idols >= GameManager.instance.sea.idolLimit && hid.hidden)
                {
                    asdsa = true;
                }
                if (hid.hideAt <= GameManager.instance.curEp + 1 && GameManager.instance.currentContestants >= hid.advantage.expiresAt && !asdsa)
                {
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
                            t = true;
                            atext = "The " + nam + " is not currently hidden.";
                        }
                        else
                        {
                            
                            atext = "";
                        }
                    }
                    if (atext != "")
                    {
                        GameManager.instance.MakeGroup(false, null, "", atext, "", new List<Contestant>(), ExileEvent.transform.GetChild(0).GetChild(0), 0);
                    }
                    foreach (Contestant num in GameManager.instance.Exiled)
                    {
                        if (hid.hidden)
                        {
                            t = true;
                            int ran = Random.Range(0, hid.hiddenChance);
                            if (hid.advantage.type == "IdolNullifier")
                            {
                                ran = Random.Range(0, 2);
                            }
                            if (ran < num.stats.Strategic)
                            {
                                Advantage av = Instantiate(hid.advantage);
                                //av.name = hid.name;
                                av.nickname = hid.name;
                                if (hid.temp)
                                {
                                    av.temp = true;
                                    av.length = hid.length;
                                }
                                num.advantages.Add(av);
                                hid.hidden = false;
                                List<Contestant> n = new List<Contestant>() { num };
                                GameManager.instance.MakeGroup(false, null, "", "", num.nickname + " finds the " + hid.name + "\n\n" + av.description, n, ExileEvent.transform.GetChild(0).GetChild(0), 10);
                            }
                        }
                    }
                }
            }
            
            if (GameManager.instance.sea.islandHiddenAdvantages.Count > 0 && !t)
            {
                GameManager.instance.MakeGroup(false, null, "", "There are no secret advantages hidden.", "", new List<Contestant>(), ExileEvent.transform.GetChild(0).GetChild(0), 0);
            }
        }

        switch (GameManager.instance.curExile.exileEvent)
        {
            case "Nothing":

                break;
            case "Safety":
                foreach(Contestant num in GameManager.instance.Exiled)
                {
                    foreach(Team tribe in GameManager.instance.LosingTribes)
                    {
                        if(tribe.name == num.team)
                        {
                            num.safety++;
                        }
                    }
                }
                break;
            case "UrnMutiny":
                Contestant correct = GameManager.instance.Exiled[Random.Range(0, GameManager.instance.Exiled.Count)];
                Team team = new Team();
                foreach (Team tribe in GameManager.instance.Tribes)
                {
                    if (tribe.name == correct.team)
                    {
                        team = tribe;
                    }
                }
                //&& team.members.Count > (GameManager.instance.currentContestants - GameManager.instance.mergeAt)
                if (Random.Range(0, 7) <= correct.stats.Boldness && team.members.Count > 1)
                {
                    //Debug.Log("SUS");
                    List<Team> TribesV = new List<Team>(GameManager.instance.Tribes);
                    if (TribesV.Contains(team))
                    {
                        TribesV.Remove(team);
                    }

                    int ran2 = Random.Range(0, TribesV.Count);
                    correct.team = TribesV[ran2].name;
                    string extra = "They mutiny to " + TribesV[ran2].name + ".";
                    GameManager.instance.MakeGroup(false, null, "", "There are two urns.\n\nOne contains an idol clue and the option of mutinying to the other tribe.\n\nThe other contains nothing.", correct.nickname + " chooses the bottle with the clue and the options.\n\nThey mutiny to " + TribesV[ran2].name + ".", new List<Contestant>() { correct }, ExileEvent.transform.GetChild(0).GetChild(0), 0);
                    foreach (Team tribe in GameManager.instance.Tribes)
                    {
                        foreach (Contestant num in tribe.members)
                        {
                            num.teams.Add(tribe.tribeColor);
                        }
                        foreach (Contestant num in GameManager.instance.Exiled)
                        {
                            if (num.team == tribe.name)
                            {
                                num.teams.Add(tribe.tribeColor);
                            }
                        }
                    }
                }
                else
                {

                    if (team.members.Count > 1)
                    {
                        GameManager.instance.MakeGroup(false, null, "", "There are two urns.\n\nOne contains an idol clue and the option of mutinying to the other tribe.\n\nThe other contains nothing.", correct.nickname + " chooses the bottle with the clue and the options.\n\nThey decide not to mutiny.", new List<Contestant>() { correct }, ExileEvent.transform.GetChild(0).GetChild(0), 0);
                    }
                    else
                    {
                        GameManager.instance.MakeGroup(false, null, "", "There are two urns.\n\nOne contains an idol clue and the option of mutinying to the other tribe.\n\nThe other contains nothing.", correct.nickname + " chooses the bottle with the clue and the options.\n\nTheir tribe is too small to mutiny.", new List<Contestant>() { correct }, ExileEvent.transform.GetChild(0).GetChild(0), 0);
                    }
                }
                break;
            case "Hourglass":
                Contestant ex = GameManager.instance.Exiled[0];
                string choice = "It's revealed that " + ex.nickname + " did not smash the hourglass.";
                GameManager.instance.MergedTribe.members.Add(ex);

                if (Random.Range(0, 5) < ex.stats.Strategic)
                {
                    choice = "It's revealed that " + ex.nickname + " smashed the hourglass.";
                    GameManager.Instance.immune = GameManager.instance.MergedTribe.members.Except(GameManager.instance.immune).ToList();
                }
                GameManager.instance.MakeGroup(false, null, "", "", "At Exile, " + ex.nickname + " is given the chance to reverse time by breaking an hourglass." +
                    "\n\nIf they break the hourglass, the results of the challenge will be reversed, meaning they and the losing team will become safe while the winning team must compete for individual immunity.", new List<Contestant>(), ExileEvent.transform.GetChild(0).GetChild(0), 0);
                GameObject ImmChall = Instantiate(GameManager.instance.Prefabs[2]);
                ImmChall.transform.parent = GameManager.instance.Canvas.transform;
                ImmChall.GetComponent<RectTransform>().offsetMax = new Vector2(0, ExileEvent.GetComponent<RectTransform>().offsetMax.y);
                ImmChall.GetComponent<RectTransform>().offsetMax = new Vector2(ExileEvent.GetComponent<RectTransform>().offsetMin.x, 0);
                GameManager.instance.AddGM(ImmChall, false, 2);
                GameManager.instance.MakeGroup(false, null, ex.nickname + " returns to the tribe.\n\n" + choice, "", "", new List<Contestant>() { ex}, ImmChall.transform.GetChild(0), 0);


                Team competing = new Team() { members=GameManager.instance.MergedTribe.members.Except(GameManager.instance.immune).ToList() };
                if (GameManager.instance.curImm <= GameManager.instance.sea.ImmunityChallenges.Count - 1)
                {
                    GameManager.instance.challenge.IndividualChallenge(competing, GameManager.instance.sea.ImmunityChallenges[GameManager.instance.curImm].stats, 1);
                }
                else
                {
                    GameManager.instance.challenge.IndividualChallenge(competing, new List<StatChoice>() { StatChoice.Physical, StatChoice.Mental, StatChoice.Endurance }, 1);
                }
                //immune.Add(MergedTribe.members[ran]);
                //MergedTribe.members[ran].advantages.Add(ImmunityNecklace);


                //lastThing = EpisodeStart;

                List<Contestant> w = new List<Contestant>() { GameManager.instance.immune[GameManager.instance.immune.Count - 1] };
                GameManager.instance.MakeGroup(false, null, GameManager.instance.immune[GameManager.instance.immune.Count - 1].nickname + " Wins Immunity!", "", "", w, ImmChall.transform.GetChild(0), 20);
                break;
        }
        if (!GameManager.instance.curExile.skipTribal)
        {
            foreach (Contestant num in GameManager.instance.Exiled)
            {
                if (GameManager.instance.Episodes[GameManager.instance.curEp].merged)
                {
                    if (GameManager.instance.MergedTribe.name == num.team)
                    {
                        num.team = "";
                        GameManager.instance.MergedTribe.members.Add(num);
                    }
                }
                else
                {
                    foreach (Team tribe in GameManager.instance.Tribes)
                    {
                        if (tribe.name == num.team)
                        {
                            num.team = "";
                            tribe.members.Add(num);
                        }
                    }
                }
            }

            GameManager.instance.Exiled = new List<Contestant>();
        }
        GameManager.instance.NextEvent();
    }

    public void FuckTravis(Contestant con)
    {
        if(con.nickname.Contains("Travis"))
        {
            con.stats.Forgivingness = 1;
            con.stats.Boldness = 1;
            con.stats.Endurance = 1;
            con.stats.Physical = 1;
            con.stats.SocialSkills = 1;
            con.stats.Intuition = 1;
            con.stats.Loyalty = 1;
            con.stats.Mental = 1;
            con.stats.Temperament = 1;
            con.stats.Stamina = 1;
            con.stats.Strategic = 1;
            con.stats.Influence = 1;
        }
    }
}
