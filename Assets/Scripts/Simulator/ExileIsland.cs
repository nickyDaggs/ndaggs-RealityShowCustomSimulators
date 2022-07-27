using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeasonParts;
using System.Linq;

public class ExileIsland : MonoBehaviour
{
    public void DoExile()
    {
        Debug.Log("worked");
        GameObject ExileEvent = Instantiate(GameManager.instance.Prefabs[0]);
        ExileEvent.transform.parent = GameManager.instance.Canvas.transform;
        ExileEvent.GetComponent<RectTransform>().offsetMax = new Vector2(0, ExileEvent.GetComponent<RectTransform>().offsetMax.y);
        ExileEvent.GetComponent<RectTransform>().offsetMax = new Vector2(ExileEvent.GetComponent<RectTransform>().offsetMin.x, 0);
        GameManager.instance.AddGM(ExileEvent, true);
        string island = "";
        if(GameManager.instance.sea.IslandType == "Ghost")
        {
            island = "Ghost Island.";
        } else if(GameManager.instance.sea.IslandType == "IOI")
        {
            island = "Island of the Idols.";
            GameManager.instance.MakeGroup(true, GameManager.instance.sea.Twists.IOI, "name", "", "", GameManager.instance.sea.Twists.IOI.members, ExileEvent.transform.GetChild(0).GetChild(0), 0);
        } else
        {
            island = "Exile Island.";
        }

        if (GameManager.instance.Exiled.Count == 1)
        {
            GameManager.instance.MakeGroup(false, null, GameManager.instance.Exiled[0].nickname + " is at " + island, "", "", GameManager.instance.Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);
        }
        else
        {
            GameManager.instance.MakeGroup(false, null, "", "", GameManager.instance.Exiled[0].nickname + " and " + GameManager.instance.Exiled[1].nickname + " are at exile island.", GameManager.instance.Exiled, ExileEvent.transform.GetChild(0).GetChild(0), 20);
        }
        if(GameManager.instance.sea.IslandType == "Ghost")
        {
            string atext = "There is no secret advantage available.";
            bool adv = false;
            foreach (HiddenAdvantage hid in GameManager.instance.sea.islandHiddenAdvantages)
            {
                if (hid.hideAt == GameManager.instance.curEp + 1)
                {
                    int ran = 1;//Random.Range(0, 2);
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
                            
                            if (ran == 1)
                            {
                                bool b = false;
                                string loseVote = "";
                                while(!b)
                                {
                                    int ran2 = Random.Range(0, 2);
                                    if(ran2 == 1)
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
                            if (ran == 1)
                            {
                                int ran2 = Random.Range(0, 2);
                                if (ran2 == 1)
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
                            int ran = Random.Range(0, 2);
                            if (ran == 1)
                            {
                                Advantage av = Instantiate(hid.advantage);
                                av.name = hid.name;
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
                if (Random.Range(0, 1) == 0 && team.members.Count > (GameManager.instance.currentContestants - GameManager.instance.mergeAt))
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
                    if (team.members.Count > (GameManager.instance.currentContestants - GameManager.instance.mergeAt))
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
                GameManager.instance.AddGM(ImmChall, false);
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
}
