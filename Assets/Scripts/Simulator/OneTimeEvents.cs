using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeasonParts;
using System.Linq;

public class OneTimeEvents : MonoBehaviour
{
    public void STribeImmunity()
    {
        if(GameManager.instance.curEvent.type == "")
        {
            GameManager.instance.curEvent.type = "JurorRemoval";
        }
        /*if(GameManager.instance.curEvent.type != "JointTribal" && GameManager.instance.curEvent.type != "MergeSplit" && GameManager.instance.curEvent.type != "JurorRemoval" && GameManager.instance.curEvent.type != "MergeSplitFiji" && !GameManager.instance.curExile.on)
        {
            GameManager.instance.curEp--;
            GameManager.instance.curEv = GameManager.instance.Episodes[GameManager.instance.curEp].events.IndexOf("STribeImmunity") + 1;
        }*/
        
        GameManager.instance.immune = new List<Contestant>();
        GameObject EpisodeStart = Instantiate(GameManager.instance.Prefabs[2]);
        EpisodeStart.transform.parent = GameManager.instance.Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStart.name = "Immunity Challenge";
        GameManager.instance.AddGM(EpisodeStart, true);
        List<Team> Trib = new List<Team>(GameManager.instance.Tribes);
        GameManager.instance.LosingTribes = new List<Team>();
        switch(GameManager.instance.curEvent.type)
        {
            case "MultiTribalEveryTeamImm":
                string te = "";
                if(GameManager.instance.curEvent.context == "ICGiven")
                {
                    for (int i = Trib.Count - 1; i > 0; i--)
                    {
                        int swapIndex = Random.Range(0, i + 1);
                        Team currentCon = Trib[i];
                        Team conToSwap = Trib[swapIndex];
                        Trib[i] = conToSwap;
                        Trib[swapIndex] = currentCon;
                    }
                    if(Trib.Count > 2)
                    {
                        te = "\n \n They can give someone on the other tribes immunity.";
                    } else
                    {
                        te = "\n \n They can give someone on the other tribe immunity.";
                    }
                }
                GameManager.instance.LosingTribes = new List<Team>(GameManager.instance.Tribes);
                for (int i = 0; i < Trib.Count; i++)
                {
                    GameManager.instance.immune.Add(Trib[i].members[Random.Range(0, Trib[i].members.Count)]);
                    List<Contestant> w = new List<Contestant>() { GameManager.instance.immune[i] };
                    if(GameManager.instance.curEvent.context == "ICGiven" && i > 0)
                    {
                        GameManager.instance.MakeGroup(false, null, w[0].nickname + " is given immunity.", "", "", w, EpisodeStart.transform.GetChild(0), 0);
                    }
                    else
                    {
                        w[0].advantages.Add(GameManager.instance.ImmunityNecklace);
                        GameManager.instance.MakeGroup(false, null, w[0].nickname + " Wins Immunity!" + te, "", "", w, EpisodeStart.transform.GetChild(0), 0);
                    }
                    GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("TribalCouncil");
                    GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("ShowVotes");
                }
                
                break;
            case "MultiTribalMultiTeam":
                for(int i = 0; i < GameManager.instance.curEvent.elim; i++)
                {
                    Team win = Trib[Random.Range(0, Trib.Count)];
                    GameManager.instance.MakeGroup(false, null, "name", "", win.name + " Wins Immunity!", win.members, EpisodeStart.transform.GetChild(0), 0);
                    Trib.Remove(win);
                }
                for (int i = 0; i < Trib.Count; i++)
                {
                    GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("TribalCouncil");
                    GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("ShowVotes");
                }
                GameManager.instance.LosingTribes = new List<Team>(Trib);
                break;
            case "MultiTribalOneImm":
                EpisodeStart.name = "Reward Challenge";
                Team reWinner = GameManager.instance.Tribes[Random.Range(0, GameManager.instance.Tribes.Count)];
                GameManager.instance.Tribes.Remove(reWinner); GameManager.instance.Tribes.Add(reWinner); GameManager.instance.Tribes.Reverse();
                GameManager.instance.LosingTribes = new List<Team>(GameManager.instance.Tribes);
                foreach (Team tw in GameManager.instance.Tribes)
                {
                    GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("TribalCouncil");
                    GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("ShowVotes");
                }
                GameManager.instance.MakeGroup(false, null, "name", "", reWinner.name + " wins reward & the chance for immunity!", reWinner.members, EpisodeStart.transform.GetChild(0), 20);

                GameObject EpisodeImm = Instantiate(GameManager.instance.Prefabs[2]);
                EpisodeImm.transform.parent = GameManager.instance.Canvas.transform;
                EpisodeImm.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeImm.GetComponent<RectTransform>().offsetMax.y);
                EpisodeImm.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeImm.GetComponent<RectTransform>().offsetMin.x, 0);
                EpisodeImm.name = "Immunity Challenge";
                GameManager.instance.AddGM(EpisodeImm, false);

                GameManager.instance.immune.Add(reWinner.members[Random.Range(0, reWinner.members.Count)]);

                List<Contestant> m = new List<Contestant>() { GameManager.instance.immune[0] };
                GameManager.instance.MakeGroup(false, null, m[0].nickname + " Wins Immunity!", "", "", m, EpisodeImm.transform.GetChild(0), 0);
                m[0].advantages.Add(GameManager.instance.ImmunityNecklace);

                break;
            case "MultiTribalReward":
                EpisodeStart.name = "Reward Challenge";
                Team rWinner = GameManager.instance.Tribes[Random.Range(0, GameManager.instance.Tribes.Count)];
                GameManager.instance.Tribes.Remove(rWinner); GameManager.instance.Tribes.Add(rWinner); GameManager.instance.Tribes.Reverse();
                GameManager.instance.LosingTribes = new List<Team>(GameManager.instance.Tribes);
                
                foreach(Team tw in GameManager.instance.Tribes)
                {
                    if(tw != rWinner && GameManager.instance.curEvent.context == "ImmunityVote")
                    {
                        GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("ImmVote");
                    }
                    GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("TribalCouncil");
                    GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("ShowVotes");
                }
                GameManager.instance.MakeGroup(false, null, "name", "", rWinner.name + " wins reward!", rWinner.members, EpisodeStart.transform.GetChild(0), 20);
                break;
            case "JointTribal":
                for (int i = 0; i < GameManager.instance.curEvent.elim; i++)
                {
                    Team win = Trib[Random.Range(0, Trib.Count)];
                    GameManager.instance.MakeGroup(false, null, "name", "", win.name + " Wins Immunity!", win.members, EpisodeStart.transform.GetChild(0), 15);
                    Trib.Remove(win);
                }

                Team Joint = new Team();
                Joint.tribeColor = Color.white;
                for (int i = 0; i < Trib.Count; i++)
                {
                    if(i > 0)
                    {
                        Joint.name += "\n" + "<color=#" + ColorUtility.ToHtmlStringRGBA(Trib[i].tribeColor) + ">" + Trib[i].name + "</color>";
                    } else
                    {
                        Joint.name +="<color=#" + ColorUtility.ToHtmlStringRGBA(Trib[i].tribeColor) + ">" + Trib[i].name + "</color>";
                    }
                    foreach(Contestant num in Trib[i].members)
                    {
                        Joint.members.Add(num);
                    }
                    
                }
                foreach(Team t in Trib)
                {
                    foreach (Alliance all in GameManager.instance.Alliances)
                    {
                        if(all.teams.Contains(t.name))
                        {
                            all.teams.Add(Joint.name);
                        }
                    }
                }
                GameManager.instance.LosingTribes.Add(Joint);
                break;
            case "MergeSplit":
                int split = (int)Mathf.Round(GameManager.instance.MergedTribe.members.Count / 2);
                Team MT = new Team();
                Team MT2 = new Team();
                MT.members = new List<Contestant>(GameManager.instance.MergedTribe.members); MT.name = "Group 1"; MT.tribeColor = GameManager.instance.MergedTribe.tribeColor;
                MT2.name = "Group 2"; MT2.tribeColor = GameManager.instance.MergedTribe.tribeColor;
                for(int i = 0; i < split; i++)
                {
                    Contestant num = MT.members[Random.Range(0, MT.members.Count)];
                    MT2.members.Add(num);
                    MT.members.Remove(num);
                }
                GameManager.instance.LosingTribes = new List<Team>() { MT, MT2};
                foreach(Team group in GameManager.instance.LosingTribes)
                {
                    GameManager.instance.MakeGroup(false, null, "name", "", "", group.members, EpisodeStart.transform.GetChild(0), 15);
                    Contestant num = group.members[Random.Range(0, group.members.Count)];
                    GameManager.instance.immune.Add(num);
                    List<Contestant> r = new List<Contestant>() { num };
                    GameManager.instance.MakeGroup(false, null, num.nickname + " wins immunity!", "", "", r, EpisodeStart.transform.GetChild(0), 0);
                    num.advantages.Add(GameManager.instance.ImmunityNecklace);
                }
                GameManager.instance.Tribes = GameManager.instance.LosingTribes;

                int insert = GameManager.instance.Episodes[GameManager.instance.curEp].events.IndexOf("MergeEvents");
                GameManager.instance.Episodes[GameManager.instance.curEp].events.Insert(insert, "TribeEvents");
                GameManager.instance.Episodes[GameManager.instance.curEp].events.Remove("MergeEvents");

                
                GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("TribalCouncil");
                GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("ShowVotes");
                break;
            case "MergeSplitFiji":
                int splitt = (int)Mathf.Round(GameManager.instance.MergedTribe.members.Count / 2);
                Team MTT = new Team();
                Team MTT2 = new Team();
                MTT.members = new List<Contestant>(GameManager.instance.MergedTribe.members); MTT.name = "Team #1"; MTT.tribeColor = GameManager.instance.MergedTribe.tribeColor;
                MTT2.name = "Team #2"; MTT2.tribeColor = GameManager.instance.MergedTribe.tribeColor;
                for (int i = 0; i < splitt; i++)
                {
                    Contestant num = MTT.members[Random.Range(0, MTT.members.Count)];
                    MTT2.members.Add(num);
                    MTT.members.Remove(num);
                }
                List<Team> ard = new List<Team>() { MTT, MTT2 };
                GameManager.instance.LosingTribes = new List<Team>() { MTT, MTT2 };
                foreach (Team group in GameManager.instance.LosingTribes)
                {
                    GameManager.instance.MakeGroup(false, null, "name", group.name, "", group.members, EpisodeStart.transform.GetChild(0), 15);
                }
                GameManager.instance.LosingTribes.Remove(GameManager.instance.LosingTribes[Random.Range(0, 2)]);
                foreach(Team team in ard)
                {
                    if(!GameManager.instance.LosingTribes.Contains(team))
                    {
                        GameManager.instance.MakeGroup(false, team, "", "", team.name + " Wins Immunity!", team.members, EpisodeStart.transform.GetChild(0), 0);

                    } else
                    {
                        team.name = GameManager.instance.MergedTribe.name;
                    }
                }
                GameManager.instance.Tribes = ard.Except(GameManager.instance.LosingTribes).ToList();
                GameManager.instance.TribeEventss();

                GameManager.instance.Episodes[GameManager.instance.curEp].events.Remove("MergeEvents");
                break;
            case "MergeSplit41":
                int splittt = (int)Mathf.Round(GameManager.instance.MergedTribe.members.Count / 2) - 1;
                Team MTTT = new Team();
                Team MTTT2 = new Team();
                List<Contestant> Exile = new List<Contestant>();
                MTTT.members = new List<Contestant>(GameManager.instance.MergedTribe.members); MTTT.name = "Team #1"; MTTT.tribeColor = GameManager.instance.MergedTribe.tribeColor;
                MTTT2.name = "Team #2"; MTTT2.tribeColor = GameManager.instance.MergedTribe.tribeColor;
                for (int i = 0; i < splittt; i++)
                {
                    Contestant num = MTTT.members[Random.Range(0, MTTT.members.Count)];
                    MTTT2.members.Add(num);
                    MTTT.members.Remove(num);
                }

                for (int i = 0; i < 2; i++)
                {
                    Contestant num = MTTT.members[Random.Range(0, MTTT.members.Count)];
                    Exile.Add(num);
                    MTTT.members.Remove(num);
                }

                List<Team> ardd = new List<Team>() { MTTT, MTTT2 };
                GameManager.instance.LosingTribes = new List<Team>() { MTTT, MTTT2 };
                foreach (Team group in GameManager.instance.LosingTribes)
                {
                    GameManager.instance.MakeGroup(false, null, "name", group.name, "", group.members, EpisodeStart.transform.GetChild(0), 15);
                }

                GameManager.instance.MakeGroup(false, null, "name", "", "Two players will sit out of the challenge.", Exile, EpisodeStart.transform.GetChild(0), 15);

                GameManager.instance.LosingTribes.Remove(GameManager.instance.LosingTribes[Random.Range(0, 2)]);
                foreach (Team team in ardd)
                {
                    if (!GameManager.instance.LosingTribes.Contains(team))
                    {
                        GameManager.instance.MakeGroup(false, team, "", "", team.name + " Wins Immunity!\n\nThey can choose to save one of the people who sat out and send the other to exile.", team.members, EpisodeStart.transform.GetChild(0), 0);
                        GameManager.instance.immune = team.members;
                        Contestant saved = Exile[Random.Range(0, Exile.Count)];
                        GameManager.instance.immune.Add(saved);
                        Exile.Remove(saved);
                        GameManager.instance.MakeGroup(false, team, "", "", saved.nickname + " is saved.\n\n" + Exile[0].nickname + " is sent to Exile for two days.", new List<Contestant>() { saved }, EpisodeStart.transform.GetChild(0), 0);


                        team.name = GameManager.instance.MergedTribe.name;
                    }
                    else
                    {
                        team.name = GameManager.instance.MergedTribe.name;
                    }
                }
                int insertt = GameManager.instance.Episodes[GameManager.instance.curEp].events.IndexOf("MergeEvents");
                GameManager.instance.Episodes[GameManager.instance.curEp].events.Insert(insertt, "ExileI");

                GameManager.instance.MergedTribe.members.Remove(Exile[0]);
                GameManager.instance.Exiled = Exile;
                GameManager.instance.curExile = new Exile() {exileEvent="Hourglass", skipTribal=false };

                break;
            case "DoOrDie":
                List<Contestant> sitOut = new List<Contestant>();
                List<Contestant> inChallenge = new List<Contestant>(GameManager.instance.MergedTribe.members);
                foreach(Contestant con in GameManager.instance.MergedTribe.members)
                {
                    if(Random.Range(0, 5) == 0 && sitOut.Count < GameManager.instance.MergedTribe.members.Count - 2)
                    {
                        sitOut.Add(con);
                        inChallenge.Remove(con);
                    }
                }

                string s = sitOut.Count + " contestant decides to sit out of the challenge.";

                if(sitOut.Count > 1)
                {
                    s = sitOut.Count + " contestants decide to sit out of the challenge.";
                } else if(sitOut.Count == 0)
                {
                    s = "None of the contestants decide to sit out of the challenge.";
                }
                GameManager.instance.MakeGroup(false, null, "", "The contestants are given the choice to sit out of the challenge and avoid the twist.", s, sitOut, EpisodeStart.transform.GetChild(0), 20);



                Contestant Loser = inChallenge[Random.Range(0, inChallenge.Count)];
                inChallenge.Remove(Loser);
                List<Contestant> U = new List<Contestant>() { Loser };
                GameManager.instance.MakeGroup(false, null, Loser.nickname + " falls first!\n\nThey must now partake in a game of chance at tribal council to decide whether they will earn safety or be eliminated.", "", "", U, EpisodeStart.transform.GetChild(0), 20);
                GameManager.instance.DoOrDie = Loser;
                GameManager.instance.immune.Add(Loser);

                Contestant Winner = inChallenge[Random.Range(0, inChallenge.Count)];
                U = new List<Contestant>() { Winner };
                GameManager.instance.MakeGroup(false, null, Winner.nickname + " Wins Immunity!", "", "", U, EpisodeStart.transform.GetChild(0), 20);
                GameManager.instance.immune.Add(Winner);
                Winner.advantages.Add(GameManager.instance.ImmunityNecklace);
                break;
            case "JurorRemoval":
                Contestant winner = GameManager.instance.MergedTribe.members[Random.Range(0, GameManager.instance.MergedTribe.members.Count)];
                List<Contestant> u = new List<Contestant>() { winner };
                GameManager.instance.MakeGroup(false, null, winner.nickname + " Wins Reward!\n\nThey win the power to remove a juror.", "", "", u, EpisodeStart.transform.GetChild(0), 20);
                GameObject EpisodeTribal = Instantiate(GameManager.instance.Prefabs[0]);
                EpisodeTribal.transform.parent = GameManager.instance.Canvas.transform;
                EpisodeTribal.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
                EpisodeTribal.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
                EpisodeTribal.name = "Tribal Council";
                GameManager.instance.AddGM(EpisodeTribal, false);
                GameManager.instance.MakeGroup(true, GameManager.instance.MergedTribe, "name", "", "No one will be voted out. Instead, one castaway chosen by " + winner.nickname + " will be removed from the jury.", GameManager.instance.MergedTribe.members, EpisodeTribal.transform.GetChild(0).GetChild(0), 15);
                GameManager.instance.MakeGroup(false, null, "name", "", "", GameManager.instance.jury, EpisodeTribal.transform.GetChild(0).GetChild(0), 15);
                Contestant elim = GameManager.instance.jury[Random.Range(0, GameManager.instance.jury.Count)];
                GameManager.instance.jury.Remove(elim);
                GameManager.instance.MakeGroup(false, null, "", "", elim.nickname + " is removed from the jury.", new List<Contestant>() { elim}, EpisodeTribal.transform.GetChild(0).GetChild(0), 15);
                elim.placement = elim.placement.Replace("Juror", "<color=red>Juror</color>");
                break;
            
        }

        GameManager.instance.NextEvent();
    }
    public void ImmVote(Team teamVoting, Team kid)
    {
        GameObject EpisodeStart = Instantiate(GameManager.instance.Prefabs[0]);
        EpisodeStart.transform.parent = GameManager.instance.Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStart.name = "Tribal Council";
        string etext = teamVoting.name + " will vote for one member of " + kid.name + " to have immunity. \n \n It's time to vote. \n \n I'll read the votes.";
        GameManager.instance.MakeGroup(true, kid, "name", "", etext, kid.members, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
        GameManager.instance.AddGM(EpisodeStart, true);
        Contestant Imm = new Contestant();
        foreach(Contestant num in teamVoting.members)
        {
            num.target = kid.members[Random.Range(0, kid.members.Count)];
        }
        List<Contestant> votes = new List<Contestant>();
        //e = false;
        foreach (Contestant num in teamVoting.members)
        {
            if (num.target != null)
            {
                votes.Add(num.target);
            }
        }
        Dictionary<Contestant, int> dic = new Dictionary<Contestant, int>();
        Imm = votes[0];
        dic.Add(votes[0], 1);
        for (int i = 1; i < votes.Count; i++)
        {
            if (dic.ContainsKey(votes[i]))
            {
                dic[votes[i]] += 1;
                if (dic[votes[i]] > dic[Imm])
                {
                    Imm = votes[i];
                }
            }
            else if (!dic.ContainsKey(votes[i]))
            {
                dic.Add(votes[i], 1);
            }
        }
        List<int> vot = new List<int>(dic.Values).OrderBy(x => x).ToList();
        vot.Reverse();

        List<Contestant> tie = new List<Contestant>();
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

        //Sort votes then generate each vote for UI 
        votes = votes.OrderBy(go => dic[go]).ToList();
        GameManager.instance.ShuffleVotes(votes);
        Dictionary<Contestant, int> dicVR = new Dictionary<Contestant, int>();
        dicVR.Add(votes[0], 1);
        string votess;
        votess = " vote ";
        string votesLeft;
        if (GameManager.instance.showVL == true)
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
        string finalVotes = "Final vote count was " + string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + ".";

        if (GameManager.instance.cineTribal == true)
        {
            GameManager.instance.AddVote(votes, votes, finalVotes);
        }
        else
        {
            List<Contestant> r = new List<Contestant>() { votes[0] };
            GameManager.instance.MakeGroup(false, null, dicVR[votes[0]] + votess + votes[0].nickname + votesLeft, "", "", r, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            for (int i = 1; i < votes.Count; i++)
            {
                string evtext = "";
                string atext = "";

                if (dicVR.ContainsKey(votes[i]))
                {
                    dicVR[votes[i]] += 1;
                }
                else if (!dicVR.ContainsKey(votes[i]))
                {
                    dicVR.Add(votes[i], 1);
                }
                votess = "";
                votesLeft = "";
                if (GameManager.instance.showVL == true)
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
                string ctext = dicVR[votes[0]] + votess + votes[0].nickname + votesLeft;
                ctext = string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + votesLeft;
                List<Contestant> g = new List<Contestant>() { votes[i] };

                if (i == votes.Count - 1 )
                {

                    votesSoFar = votesSoFar.OrderByDescending(go => go[0]).ToList();
                    evtext = finalVotes;
                    ctext = votes[i].nickname;

                    GameManager.instance.MakeGroup(false, null, "name", atext, finalVotes + "\n All the votes have been read.", g, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                }
                else
                {
                    GameManager.instance.MakeGroup(false, null, ctext, atext, evtext, g, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                }
                
            }

        }
        string rock = "";
        if (tie.Count > 1)
        {
            if(GameManager.instance.cineTribal)
            {
                GameManager.instance.MakeGroup(false, null, "", "There is a Tie", "Those in the tie will draw rocks for immunity.", tie, null, 0);
            }
            else
            {
                GameManager.instance.MakeGroup(false, null, "", "There is a Tie", "Those in the tie will draw rocks for immunity.", tie, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            }
            GameManager.instance.MakeGroup(false, null, "", "There is a Tie", "Those in the tie will draw rocks for immunity.", tie, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            rock = " draws the correct rock and ";
        }
        List<Contestant> m = new List<Contestant>() { Imm };
        if (GameManager.instance.cineTribal)
        {
            GameManager.instance.MakeGroup(false, null, Imm.nickname + rock + " will have immunity for the vote.", "", "", m, null, 0);
        }
        else
        {
            GameManager.instance.MakeGroup(false, null, Imm.nickname + rock + " will have immunity for the vote.", "", "", m, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
        }
        Imm.safety++;
        GameObject EpisodeVote = Instantiate(GameManager.instance.Prefabs[0]);
        EpisodeVote.transform.parent = GameManager.instance.Canvas.transform;
        EpisodeVote.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeVote.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        //curEpp--;
        GameManager.instance.AddGM(EpisodeVote, false);
        foreach(Contestant num in teamVoting.members)
        {
            if(num.target != null)
            {
                num.voteReason = "They like them.";
                List<Contestant> e = new List<Contestant>() { num.target, num };
                GameManager.instance.MakeGroup(false, null, "", "", num.nickname + " voted for " + num.target.nickname + "\n" + "\n" + num.voteReason, e, EpisodeVote.transform.GetChild(0).GetChild(0), 0);
            }
        }
        GameManager.instance.NextEvent();
    }
    public void BeginningTwist()
    {
        bool altGender = false;
        if (GameManager.instance.curEvent.elim > 0)
        {
            altGender = true;
        }
        if(GameManager.instance.curEvent.type == "SchoolyardPick")
        {
            GameObject EpisodeCast = Instantiate(GameManager.instance.Prefabs[0]);
            EpisodeCast.transform.parent = GameManager.instance.Canvas.transform;
            EpisodeCast.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeCast.GetComponent<RectTransform>().offsetMax.y);
            EpisodeCast.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeCast.GetComponent<RectTransform>().offsetMin.x, 0);
            EpisodeCast.name = "The Cast";
            GameManager.instance.AddGM(EpisodeCast, true);
            GameManager.instance.MakeGroup(false, null, "name", "", "", GameManager.instance.cast.cast, EpisodeCast.transform.GetChild(0).GetChild(0), 0);
        }
        
        GameObject EpisodeStart = Instantiate(GameManager.instance.Prefabs[0]);
        EpisodeStart.transform.parent = GameManager.instance.Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStart.name = "The Twist";
        GameManager.instance.AddGM(EpisodeStart, false);
        switch (GameManager.instance.curEvent.type)
        {
            case "SchoolyardPick":
                List<Contestant> Leaders = new List<Contestant>();
                if (GameManager.instance.curEvent.context == "FirstLast")
                {
                    foreach(Team tribe in GameManager.instance.Tribes)
                    {
                        Leaders.Add(tribe.members[0]);
                    }
                    GameManager.instance.MakeGroup(false, null, "name", "", "Players are chosen to select tribes.", Leaders, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                    int con = 1;
                    int num = 0;

                    for (int i = 0; i < GameManager.instance.cast.cast.Count - GameManager.instance.Tribes.Count; i++)
                    {
                        if(con < GameManager.instance.Tribes[num].members.Count)
                        {
                            //Contestant picker = Leaders[num];
                            //Contestant picked = GameManager.instance.Tribes[num].members[con];
                            List<Contestant> p = new List<Contestant>() { GameManager.instance.Tribes[num].members[con], Leaders[num] };
                            GameManager.instance.MakeGroup(false, null, "", "", p[1].nickname + " chooses " + p[0].nickname + ".", p, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                        }
                        
                        num++;
                        if(num > GameManager.instance.Tribes.Count - 1)
                        {
                            num = 0;
                            con++;
                        }
                    }
                }
                else
                {
                    GameManager.instance.Tribes = new List<Team>(GameManager.instance.sea.Tribes);
                    List<Contestant> cast = new List<Contestant>(GameManager.instance.cast.cast);
                    List<List<Contestant>> Genders = new List<List<Contestant>>() { new List<Contestant>(), new List<Contestant>() };
                    foreach (Contestant contest in cast)
                    {
                        if (contest.gender == "M")
                        {
                            Genders[0].Add(contest);
                        }
                        else
                        {
                            Genders[1].Add(contest);
                        }
                    }
                    string etext = "";
                    if(GameManager.instance.curEvent.context == "Oldest")
                    {
                        cast = cast.OrderByDescending(x => x.Age).ToList();
                        etext = "The oldest players are chosen to select tribes.";
                    } else if (GameManager.instance.curEvent.context == "Youngest")
                    {
                        cast = cast.OrderBy(x => x.Age).ToList();
                        etext = "The youngest players are chosen to select tribes.";
                    }
                    else if(GameManager.instance.curEvent.context == "Random")
                    {
                        for (int i = cast.Count - 1; i > 0; i--)
                        {
                            int swapIndex = Random.Range(0, i + 1);
                            Contestant currentCon = cast[i];
                            Contestant conToSwap = cast[swapIndex];
                            cast[i] = conToSwap;
                            cast[swapIndex] = currentCon;
                        }
                        etext = "Random players are chosen to select tribes.";
                    }
                    for (int i = 0; i < GameManager.instance.Tribes.Count; i++)
                    {
                        GameManager.instance.sea.Tribes[i].members[0] = Instantiate(cast[0]);
                        Leaders.Add(cast[0]);
                        cast.Remove(cast[0]);
                    }
                    GameManager.instance.MakeGroup(false, null, "name", "", etext, Leaders, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                    int con = 1;
                    int num = 0;
                    int rounds = cast.Count;
                    for (int i = 0; i < rounds; i++)
                    {
                        if (con < GameManager.instance.Tribes[num].members.Count)
                        {
                            //Contestant picker = Leaders[num];
                            //Contestant picked = GameManager.instance.Tribes[num].members[con];\
                            int ran = 0;
                            if (altGender)
                            {
                                int gender = 0;
                                foreach (List<Contestant> list in Genders)
                                {
                                    if (!list.Contains(GameManager.instance.Tribes[num].members[con - 1]))
                                    {
                                        gender = Genders.IndexOf(list);
                                    }
                                }
                                ran = Random.Range(0, Genders[gender].Count);
                                GameManager.instance.Tribes[num].members[con] = Instantiate(Genders[gender][ran]);
                                cast.Remove(Genders[gender][ran]);
                            }
                            else
                            {
                                ran = Random.Range(0, cast.Count);
                                GameManager.instance.Tribes[num].members[con] = Instantiate(cast[ran]);
                                cast.Remove(cast[ran]);
                            }
                            List<Contestant> p = new List<Contestant>() { GameManager.instance.Tribes[num].members[con], Leaders[num] };
                            GameManager.instance.MakeGroup(false, null, "", "", p[1].nickname + " chooses " + p[0].nickname + ".", p, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                        }

                        num++;
                        if (num > GameManager.instance.Tribes.Count - 1)
                        {
                            num = 0;
                            con++;
                        }
                    }
                    int male = 0;
                    int female = 0;
                    foreach (Team tribe in GameManager.instance.Tribes)
                    {
                        GameManager.instance.MakeAlliances(tribe);
                        foreach (Contestant contest in tribe.members)
                        {
                            contest.votes = 1;
                            contest.teams.Add(tribe.tribeColor);
                            if (contest.gender == "M")
                            {
                                male++;
                            }
                            else if (contest.gender == "F")
                            {
                                female++;
                            }
                            GameManager.instance.challenge.RandomizeStats(contest);
                        }
                        if (tribe.hiddenAdvantages.Count > 0)
                        {
                            GameManager.instance.advant = true;
                        }
                    }
                    if (male == female)
                    {
                        GameManager.instance.genderEqual = true;
                    }
                }
                break;
            case "FirstImpressions":
                if(GameManager.instance.curEvent.context == "RI")
                {
                    GameManager.instance.MakeGroup(false, null, "", "", "Each tribe will immediately vote out one member.", new List<Contestant>(), EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                    foreach(Team tribe in GameManager.instance.Tribes)
                    {
                        GameObject EpisodeVote = Instantiate(GameManager.instance.Prefabs[0]);
                        EpisodeVote.transform.parent = GameManager.instance.Canvas.transform;
                        EpisodeVote.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
                        EpisodeVote.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
                        EpisodeVote.name = "First Impressions ";
                        GameManager.instance.AddGM(EpisodeVote, false);
                        List<Contestant> votes = new List<Contestant>();
                        List<Contestant> tie = new List<Contestant>();

                        foreach (Contestant num in tribe.members)
                        {
                            List<Contestant> tribeV = new List<Contestant>(tribe.members);
                            tribeV.Remove(num);
                            num.target = tribe.members[Random.Range(0, tribe.members.Count)];
                            List<Contestant> e = new List<Contestant>() { num.target, num };
                            GameManager.instance.MakeGroup(false, null, "", "", num.nickname + " voted for " + num.target.nickname, e, EpisodeVote.transform.GetChild(0).GetChild(0), 0);
                            votes.Add(num.target);
                        }
                        Dictionary<Contestant, int> dic = new Dictionary<Contestant, int>();
                        Contestant votedOff = votes[0];
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
                        if (tie.Count > 1)
                        {
                            GameManager.instance.MakeGroup(false, null, "name", "", "There is a tie.\n\nRocks will determine who is eliminated.", tie, EpisodeVote.transform.GetChild(0).GetChild(0), 0);
                            votedOff = tie[Random.Range(0, tie.Count)];
                        }
                        GameManager.instance.MakeGroup(false, null, "", "", votedOff.nickname + ", the tribe has spoken. \n \n" + votedOff.nickname +" will now go to Redemption Island", new List<Contestant>() { votedOff}, EpisodeVote.transform.GetChild(0).GetChild(0), 0);
                        votedOff.placement = "Voted Out Ep. 1 \n Eliminated";
                        GameManager.instance.Eliminated.Add(votedOff);
                        tribe.members.Remove(votedOff);
                        GameManager.instance.RIsland.Add(votedOff);
                        GameManager.instance.currentContestants--;
                    }
                    
                }
                else if(GameManager.instance.curEvent.context == "Tocantins")
                {
                    GameManager.instance.MakeGroup(false, null, "", "", "Each tribe selects who they think will be the biggest liability based on first impressions alone.", new List<Contestant>(), EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                    GameObject EpisodeVote = Instantiate(GameManager.instance.Prefabs[0]);
                    EpisodeVote.transform.parent = GameManager.instance.Canvas.transform;
                    EpisodeVote.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
                    EpisodeVote.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
                    EpisodeVote.name = "First Impressions ";
                    GameManager.instance.AddGM(EpisodeVote, false);
                    List<Contestant> liabilities = new List<Contestant>();
                    foreach (Team tribe in GameManager.instance.Tribes)
                    {
                        GameManager.instance.MakeGroup(true, tribe, "name", "", "", tribe.members, EpisodeVote.transform.GetChild(0).GetChild(0), 0);

                        List<Contestant> votes = new List<Contestant>();
                        List<Contestant> tie = new List<Contestant>();
                        foreach (Contestant num in tribe.members)
                        {
                            List<Contestant> tribeV = new List<Contestant>(tribe.members);
                            tribeV.Remove(num);
                            num.target = tribe.members[Random.Range(0, tribe.members.Count)];
                            List<Contestant> e = new List<Contestant>() { num.target, num };
                            GameManager.instance.MakeGroup(false, null, "", "", num.nickname + " votes for " + num.target.nickname, e, EpisodeVote.transform.GetChild(0).GetChild(0), 0);
                            votes.Add(num.target);
                        }
                        Dictionary<Contestant, int> dic = new Dictionary<Contestant, int>();
                        Contestant votedOff = votes[0];
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
                        if(tie.Count > 1)
                        {
                            GameManager.instance.MakeGroup(false, null, "name", "", "There is a tie for the biggest liability.\n\nRocks will determine who is selected.", tie, EpisodeVote.transform.GetChild(0).GetChild(0), 0);
                            votedOff = tie[Random.Range(0, tie.Count)];
                        }
                        liabilities.Add(votedOff);
                        GameManager.instance.MakeGroup(false, null, "", "", votedOff.nickname + " is the biggest liability.", new List<Contestant>() { votedOff }, EpisodeVote.transform.GetChild(0).GetChild(0), 0);
                        
                    }
                    GameManager.instance.MakeGroup(false, null, "name", "", "The biggest liabilities will be airlifted to their tribe camps.\n\nAt camp, they have two options.\n\nThey can look for an idol that gives them immunity at their first vote.\n\nOr they can get a head start on their shelter.", liabilities, EpisodeVote.transform.GetChild(0).GetChild(0), 0);
                    foreach(Contestant num in liabilities)
                    {
                        if(Random.Range(0, 2) == 1)
                        {
                            if(Random.Range(0, 2) == 1)
                            {
                                GameManager.instance.MakeGroup(false, null, "", "", num.nickname + " chooses to look for the idol and finds it.", new List<Contestant>() { num}, EpisodeVote.transform.GetChild(0).GetChild(0), 0);
                                num.safety++;
                            }
                            else
                            {
                                GameManager.instance.MakeGroup(false, null, "", "", num.nickname + " chooses to look for the idol and doesn't find it.", new List<Contestant>() { num }, EpisodeVote.transform.GetChild(0).GetChild(0), 0);

                            }
                        }
                        else
                        {
                            GameManager.instance.MakeGroup(false, null, "", "", num.nickname + " chooses to get a head start on their shelter.", new List<Contestant>() { num }, EpisodeVote.transform.GetChild(0).GetChild(0), 0);
                        }
                    }
                }
                else if(GameManager.instance.curEvent.context == "Leaders")
                {
                    GameManager.instance.MakeGroup(false, null, "", "", "Each tribe selects who they think will be the best leader based on first impressions alone.", new List<Contestant>(), EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                    GameObject EpisodeVote = Instantiate(GameManager.instance.Prefabs[0]);
                    EpisodeVote.transform.parent = GameManager.instance.Canvas.transform;
                    EpisodeVote.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
                    EpisodeVote.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
                    EpisodeVote.name = "First Impressions ";
                    GameManager.instance.AddGM(EpisodeVote, true);
                    List<Contestant> liabilities = new List<Contestant>();
                    foreach (Team tribe in GameManager.instance.Tribes)
                    {
                        GameManager.instance.MakeGroup(true, tribe, "name", "", "", tribe.members, EpisodeVote.transform.GetChild(0).GetChild(0), 0);

                        List<Contestant> votes = new List<Contestant>();
                        List<Contestant> tie = new List<Contestant>();
                        foreach (Contestant num in tribe.members)
                        {
                            List<Contestant> tribeV = new List<Contestant>(tribe.members);
                            tribeV.Remove(num);
                            num.target = tribe.members[Random.Range(0, tribe.members.Count)];
                            List<Contestant> e = new List<Contestant>() { num.target, num };
                            GameManager.instance.MakeGroup(false, null, "", "", num.nickname + " votes for " + num.target.nickname, e, EpisodeVote.transform.GetChild(0).GetChild(0), 0);
                            votes.Add(num.target);
                        }
                        Dictionary<Contestant, int> dic = new Dictionary<Contestant, int>();
                        Contestant votedOff = votes[0];
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
                        if (tie.Count > 1)
                        {
                            GameManager.instance.MakeGroup(false, null, "name", "", "There is a tie for the leader.\n\nRocks will determine who is selected.", tie, EpisodeVote.transform.GetChild(0).GetChild(0), 0);
                            votedOff = tie[Random.Range(0, tie.Count)];
                        }
                        liabilities.Add(votedOff);
                        GameManager.instance.MakeGroup(false, null, "", "", votedOff.nickname + " is chosen as the leader.", new List<Contestant>() { votedOff }, EpisodeVote.transform.GetChild(0).GetChild(0), 0);

                        GameManager.instance.MakeGroup(false, null, "name", "", "The leaders will make decisions for their tribes.", liabilities, EpisodeVote.transform.GetChild(0).GetChild(0), 0);

                        GameManager.instance.TribeLeaders = liabilities;
                    }
                }
                break;
            case "MarooningAdvantage":
                foreach(HiddenAdvantage hid in GameManager.instance.sea.twistHiddenAdvantages)
                {
                    foreach(Team tribe in GameManager.instance.Tribes)
                    {
                        foreach(Contestant num in tribe.members)
                        {
                            if(hid.hidden)
                            {
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
                                    GameManager.instance.MakeGroup(false, null, "", "", num.nickname + " finds the " + hid.name + "\n\n" + av.description, n, EpisodeStart.transform.GetChild(0), 10);
                                }
                            }
                        }
                    }
                }
                break;
            
        }
        GameManager.instance.NextEvent();
    }
    public void PalauStart()
    {
        bool altGender = false;
        if(GameManager.instance.curEvent.elim > 0)
        {
            altGender = true;
        }
        GameObject EpisodeCast = Instantiate(GameManager.instance.Prefabs[0]);
        EpisodeCast.transform.parent = GameManager.instance.Canvas.transform;
        EpisodeCast.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeCast.GetComponent<RectTransform>().offsetMax.y);
        EpisodeCast.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeCast.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeCast.name = "The Cast";
        GameManager.instance.AddGM(EpisodeCast, false);
        GameManager.instance.MakeGroup(false, null, "name", "", "", GameManager.instance.cast.cast, EpisodeCast.transform.GetChild(0).GetChild(0), 0);

        GameObject EpisodeImm = Instantiate(GameManager.instance.Prefabs[2]);
        EpisodeImm.transform.parent = GameManager.instance.Canvas.transform;
        EpisodeImm.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeImm.GetComponent<RectTransform>().offsetMax.y);
        EpisodeImm.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeImm.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeImm.name = "Immunity Challenge";
        GameManager.instance.AddGM(EpisodeImm, false);

        GameManager.instance.Tribes = new List<Team>() { new Team { name = "The Cast", tribeColor = Color.white, members = GameManager.instance.cast.cast } };
        GameManager.instance.TribeEventss();

        GameObject EpisodeStart = Instantiate(GameManager.instance.Prefabs[0]);
        EpisodeStart.transform.parent = GameManager.instance.Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStart.name = "The Twist";
        GameManager.instance.AddGM(EpisodeStart, false);
        List<Contestant> Leaders = new List<Contestant>();
        List<Contestant> cast = new List<Contestant>(GameManager.instance.cast.cast);
        if (GameManager.instance.curEvent.context == "FirstLast")
        {
            foreach (Team tribe in GameManager.instance.Tribes)
            {
                Leaders.Add(tribe.members[0]);
                GameManager.instance.MakeGroup(false, null, tribe.members[0].nickname + " wins immunity.", "", "", new List<Contestant>() { tribe.members[0] }, EpisodeImm.transform.GetChild(0), 0);

                foreach (Contestant contest in tribe.members)
                {
                    cast.Remove(contest);
                }
            }
            GameManager.instance.MakeGroup(false, null, "name", "", "The winners of the challenge will decide the tribes.", Leaders, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            int con = 1;
            int num = 0;

            for (int i = 0; i < GameManager.instance.cast.cast.Count - GameManager.instance.Tribes.Count; i++)
            {
                if (con < GameManager.instance.Tribes[num].members.Count)
                {
                    //Contestant picker = Leaders[num];
                    //Contestant picked = GameManager.instance.Tribes[num].members[con];
                    List<Contestant> p = new List<Contestant>() { GameManager.instance.Tribes[num].members[con], Leaders[num] };
                    GameManager.instance.MakeGroup(false, null, "", "", p[1].nickname + " chooses " + p[0].nickname + ".", p, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                }

                num++;
                if (num > GameManager.instance.Tribes.Count - 1)
                {
                    num = 0;
                    con++;
                }
            }
            foreach (Contestant contest in cast)
            {
                contest.placement = "Pre-Juror\nEliminated";
                GameManager.instance.Eliminated.Add(contest);
            }
            if (cast.Count > 0)
            {
                GameManager.instance.MakeGroup(false, null, "name", "", "Those who weren't picked are eliminated.", cast, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            }
        }
        else
        {
            List<List<Contestant>> Genders = new List<List<Contestant>>() { new List<Contestant>(), new List<Contestant>()};
            foreach (Contestant contest in cast)
            {
                if (contest.gender == "M")
                {
                    Genders[0].Add(contest);
                }
                else
                {
                    Genders[1].Add(contest);
                }
            }
            int rounds = 0;
            GameManager.instance.Tribes = new List<Team>(GameManager.instance.sea.Tribes);
            for(int i  = 0; i < GameManager.instance.Tribes.Count; i++)
            {
                if(altGender)
                {
                    if(i < Genders.Count)
                    {
                        Leaders.Add(Genders[i][Random.Range(0, Genders[i].Count)]);
                        Genders[i].Remove(Leaders[i]);
                    } else
                    {
                        if(i % 2 == 0)
                        {
                            Leaders.Add(Genders[1][Random.Range(0, Genders[1].Count)]);
                            Genders[1].Remove(Leaders[i]);
                        } else
                        {
                            Leaders.Add(Genders[0][Random.Range(0, Genders[0].Count)]);
                            Genders[0].Remove(Leaders[i]);
                        }
                    }
                    

                } else
                {
                    Leaders.Add(cast[Random.Range(0, cast.Count)]);
                }
                
                GameManager.instance.Tribes[i].members[0] = Instantiate(Leaders[i]);
                GameManager.instance.MakeGroup(false, null, GameManager.instance.Tribes[i].members[0].nickname + " wins immunity.", "", "", new List<Contestant>() { GameManager.instance.Tribes[i].members[0] }, EpisodeImm.transform.GetChild(0), 0);

                cast.Remove(Leaders[i]);
                
                rounds += GameManager.instance.sea.Tribes[i].members.Count;
            }
            
            GameManager.instance.MakeGroup(false, null, "name", "", "The winners of the challenge will decide the tribes.", Leaders, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            int con = 1;
            int num = 0;
            
            for (int i = 0; i < rounds; i++)
            {

                if (con < GameManager.instance.Tribes[num].members.Count)
                {
                    //Contestant picker = Leaders[num];
                    //Contestant picked = GameManager.instance.Tribes[num].members[con];\
                    int ran = 0;
                    if (altGender)
                    {
                        int gender = 0;
                        foreach(List<Contestant> list in Genders)
                        {
                            if(GameManager.instance.Tribes[num].members[con - 1].gender == "M")
                            {
                                gender = 1;
                            } else
                            {
                                gender = 0;
                            }
                        }
                        if(Genders[gender].Count < 1)
                        {
                            foreach (List<Contestant> list in Genders)
                            {
                                if (list.Count > 0)
                                {
                                    gender = Genders.IndexOf(list);
                                }
                            }
                        }
                        ran = Random.Range(0, Genders[gender].Count);
                        GameManager.instance.Tribes[num].members[con] = Instantiate(Genders[gender][ran]);
                        
                        cast.Remove(Genders[gender][ran]);
                        Genders[gender].Remove(Genders[gender][ran]);
                    } else
                    {
                        ran = Random.Range(0, cast.Count);
                        GameManager.instance.Tribes[num].members[con] = Instantiate(cast[ran]);
                        cast.Remove(cast[ran]);
                    }
                    
                    List<Contestant> p = new List<Contestant>() { GameManager.instance.Tribes[num].members[con], GameManager.instance.Tribes[num].members[con - 1] };
                    GameManager.instance.MakeGroup(false, null, "", "", p[1].nickname + " chooses " + p[0].nickname + ".", p, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                }

                num++;
                if (num > GameManager.instance.Tribes.Count - 1)
                {
                    num = 0;
                    con++;
                }
            }
            foreach (Contestant contest in cast)
            {
                contest.placement = "Pre-Juror\nEliminated";
                GameManager.instance.Eliminated.Add(contest);
            }
            if (cast.Count > 0)
            {
                GameManager.instance.MakeGroup(false, null, "name", "", "Those who weren't picked are eliminated.", cast, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            }
            int male = 0;
            int female = 0;
            
            foreach (Team tribe in GameManager.instance.Tribes)
            {
                GameManager.instance.MakeAlliances(tribe);
                foreach (Contestant contest in tribe.members)
                {
                    contest.votes = 1;
                    contest.teams.Add(tribe.tribeColor);
                    if (contest.gender == "M")
                    {
                        male++;
                    }
                    else if (contest.gender == "F")
                    {
                        female++;
                    }
                    GameManager.instance.challenge.RandomizeStats(contest);
                }
                if (tribe.hiddenAdvantages.Count > 0)
                {
                    GameManager.instance.advant = true;
                }
            }
            if (male == female)
            {
                GameManager.instance.genderEqual = true;
            }
        }
        GameManager.instance.NextEvent();
    }
    public void FijiStart()
    {
        bool altGender = false;
        if (GameManager.instance.curEvent.elim > 0)
        {
            altGender = true;
        }
        GameObject EpisodeCast = Instantiate(GameManager.instance.Prefabs[0]);
        EpisodeCast.transform.parent = GameManager.instance.Canvas.transform;
        EpisodeCast.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeCast.GetComponent<RectTransform>().offsetMax.y);
        EpisodeCast.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeCast.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeCast.name = "The Cast";
        GameManager.instance.AddGM(EpisodeCast, true);
        GameManager.instance.MakeGroup(false, null, "name", "", "", GameManager.instance.cast.cast, EpisodeCast.transform.GetChild(0).GetChild(0), 0);

        GameManager.instance.Tribes = new List<Team>() { new Team { name = "The Cast", tribeColor = Color.white, members = GameManager.instance.cast.cast } };
        GameManager.instance.TribeEventss();

        GameObject EpisodeStart = Instantiate(GameManager.instance.Prefabs[0]);
        EpisodeStart.transform.parent = GameManager.instance.Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStart.name = "The Twist";
        GameManager.instance.AddGM(EpisodeStart, false);
        Contestant Leader = new Contestant();
        List<Contestant> cast = new List<Contestant>(GameManager.instance.cast.cast);
        if (GameManager.instance.curEvent.context == "FirstLast")
        {
            foreach (Team tribe in GameManager.instance.Tribes)
            {
                //Leaders.Add(tribe.members[0]);

                foreach (Contestant contest in tribe.members)
                {
                    cast.Remove(contest);
                }
            }
            Leader = Instantiate(cast[0]);
            GameManager.instance.MakeGroup(false, null, "", "The players select who best represents them as a leader.", Leader.nickname + " is chosen and will select the tribes.", new List<Contestant>() { Leader }, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            int con = 1;
            int num = 0;

            for (int i = 0; i < GameManager.instance.cast.cast.Count - 1; i++)
            {
                if (con < GameManager.instance.Tribes[num].members.Count)
                {
                    List<Contestant> p = new List<Contestant>() { GameManager.instance.Tribes[num].members[con], Leader };
                    GameManager.instance.MakeGroup(false, null, "", "", p[1].nickname + " chooses " + p[0].nickname + " for "+ GameManager.instance.Tribes[num].name + ".", p, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                }

                num++;
                if (num > GameManager.instance.Tribes.Count - 1)
                {
                    num = 0;
                    con++;
                }
            }
            if (cast.Count > 0)
            {
                GameManager.instance.MakeGroup(false, null, "name", "", "Those who weren't picked are eliminated.", cast, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            }
        }
        else
        {
            List<List<Contestant>> Genders = new List<List<Contestant>>() { new List<Contestant>(), new List<Contestant>() };
            
            int rounds = 0;
            int can = Random.Range(0, cast.Count);
            GameManager.instance.Tribes = new List<Team>(GameManager.instance.sea.Tribes);
            Leader = Instantiate(cast[can]);
            cast.Remove(cast[can]);
            GameManager.instance.MakeGroup(false, null, "", "The players select who best represents them as a leader.", Leader.nickname + " is chosen and will select the tribes.", new List<Contestant>() { Leader }, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            int con = 0;
            int num = 0;
            foreach (Contestant contest in cast)
            {
                if (contest.gender == "M")
                {
                    Genders[0].Add(contest);
                }
                else
                {
                    Genders[1].Add(contest);
                }
            }
            rounds = cast.Count;
            for (int i = 0; i < rounds; i++)
            {
                if (con < GameManager.instance.Tribes[num].members.Count)
                {
                    //Contestant picker = Leaders[num];
                    //Contestant picked = GameManager.instance.Tribes[num].members[con];\
                    int ran = 0;
                    if (altGender)
                    {
                        int gender = 0;
                        if(Genders[gender].Count < 1)
                        {
                            gender = 1;
                        }
                        ran = Random.Range(0, Genders[gender].Count);
                        GameManager.instance.Tribes[num].members[con] = Instantiate(Genders[gender][ran]);

                        cast.Remove(Genders[gender][ran]);
                        Genders[gender].Remove(Genders[gender][ran]);
                    }
                    else
                    {
                        ran = Random.Range(0, cast.Count);
                        GameManager.instance.Tribes[num].members[con] = Instantiate(cast[ran]);
                        cast.Remove(cast[ran]);
                    }

                    List<Contestant> p = new List<Contestant>() { GameManager.instance.Tribes[num].members[con], Leader };
                    GameManager.instance.MakeGroup(false, null, "", "", p[1].nickname + " chooses " + p[0].nickname + " for " + GameManager.instance.Tribes[num].name + ".", p, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                }

                num++;
                if (num > GameManager.instance.Tribes.Count - 1)
                {
                    num = 0;
                    con++;
                }
            }
            int male = 0;
            int female = 0;
            GameManager.instance.MakeGroup(false, null, "", "", Leader.nickname + "  will go to Exile Island and join the losing tribe after Tribal Council.", new List<Contestant>() { Leader }, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            GameManager.instance.Exiled.Add(Leader);
            foreach (Team tribe in GameManager.instance.Tribes)
            {
                //GameManager.instance.MakeAlliances(tribe);
                foreach (Contestant contest in tribe.members)
                {
                    contest.votes = 1;
                    contest.teams.Add(tribe.tribeColor);
                    if (contest.gender == "M")
                    {
                        male++;
                    }
                    else if (contest.gender == "F")
                    {
                        female++;
                    }
                    GameManager.instance.challenge.RandomizeStats(contest);
                }
                if (tribe.hiddenAdvantages.Count > 0)
                {
                    GameManager.instance.advant = true;
                }
            }
            if (male == female)
            {
                GameManager.instance.genderEqual = true;
            }
        }
        GameManager.instance.NextEvent();
    }
    public void FakeMerge()
    {
        GameManager.Instance.OW = true;
        GameObject EpisodeStart = Instantiate(GameManager.instance.Prefabs[0]);
        EpisodeStart.transform.parent = GameManager.instance.Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStart.name = "Fake Merge";
        GameManager.Instance.owStatus = true;
        Team tribe = new Team() { tribeColor = Color.white };

        for (int i = 0; i < GameManager.Instance.Tribes.Count; i++)
        {
            tribe.members.AddRange(GameManager.Instance.Tribes[i].members);
            //tribe.hiddenAdvantages.Concat(teams[i].hiddenAdvantages);
        }

        GameManager.Instance.MakeGroup(false, null, "name", "", "The castaways are told they will live on one camp for the rest of the game.\n\nThey assume they have merged.\n\nHowever, this is actually a fake merge.", tribe.members, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
        GameManager.Instance.owStatus = false;
        GameManager.instance.AddGM(EpisodeStart, true);

        GameManager.instance.NextEvent();
    }
}
