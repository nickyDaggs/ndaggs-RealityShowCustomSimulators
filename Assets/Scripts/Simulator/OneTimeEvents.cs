﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeasonParts;
using System.Linq;

public class OneTimeEvents : MonoBehaviour
{
    public void STribeImmunity()
    {
        if(GameManager.instance.curEvent.type != "JointTribal" && GameManager.instance.curEvent.type != "MergeSplit")
        {
            GameManager.instance.curEp--;
            GameManager.instance.curEv = GameManager.instance.Episodes[GameManager.instance.curEp].events.IndexOf("STribeImmunity") + 1;
        }

        
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
                        Joint.name += "\n" + Trib[i].name;
                    } else
                    {
                        Joint.name += Trib[i].name;
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
                MT.members = new List<Contestant>(GameManager.instance.MergedTribe.members); MT.name = GameManager.instance.MergedTribe.name; MT.tribeColor = GameManager.instance.MergedTribe.tribeColor;
                MT2.name = GameManager.instance.MergedTribe.name; MT2.tribeColor = GameManager.instance.MergedTribe.tribeColor;
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
                }
                GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("TribalCouncil");
                GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("ShowVotes");
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
            num.vote = kid.members[Random.Range(0, kid.members.Count)];
        }
        List<Contestant> votes = new List<Contestant>();
        //e = false;
        foreach (Contestant num in teamVoting.members)
        {
            if (num.vote != null)
            {
                votes.Add(num.vote);
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
            GameManager.instance.AddVote(votes, votes);
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
        GameManager.instance.immune.Add(Imm);
        GameObject EpisodeVote = Instantiate(GameManager.instance.Prefabs[0]);
        EpisodeVote.transform.parent = GameManager.instance.Canvas.transform;
        EpisodeVote.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeVote.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        //curEpp--;
        GameManager.instance.AddGM(EpisodeVote, false);
        foreach(Contestant num in teamVoting.members)
        {
            if(num.vote != null)
            {
                num.voteReason = "They like them.";
                List<Contestant> e = new List<Contestant>() { num.vote, num };
                GameManager.instance.MakeGroup(false, null, "", "", num.nickname + " voted for " + num.vote.nickname + "\n" + "\n" + num.voteReason, e, EpisodeVote.transform.GetChild(0).GetChild(0), 0);
            }
        }
        GameManager.instance.NextEvent();
    }
    public void BeginningTwist()
    {
        GameObject EpisodeStart = Instantiate(GameManager.instance.Prefabs[0]);
        EpisodeStart.transform.parent = GameManager.instance.Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStart.name = "The Twist";
        GameManager.instance.AddGM(EpisodeStart, true);
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
                    string etext = "";
                    if(GameManager.instance.curEvent.context == "Oldest")
                    {
                        cast = cast.OrderByDescending(x => x.Age).ToList();
                        etext = "The oldest players are chosen to select tribes.";
                    } else if (GameManager.instance.curEvent.context == "Youngest")
                    {
                        cast = cast.OrderBy(x => x.Age).ToList();
                        etext = "The youngest players are chosen to select tribes.";
                    } else if(GameManager.instance.curEvent.context == "Random")
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
                            int ran = Random.Range(0, cast.Count);
                            GameManager.instance.Tribes[num].members[con] = Instantiate(cast[ran]);
                            cast.Remove(cast[ran]);
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
                        foreach(Contestant num in tribe.members)
                        {
                            List<Contestant> tribeV = new List<Contestant>(tribe.members);
                            tribeV.Remove(num);
                            num.vote = tribe.members[Random.Range(0, tribe.members.Count)];
                            List<Contestant> e = new List<Contestant>() { num.vote, num };
                            GameManager.instance.MakeGroup(false, null, "", "", num.nickname + " voted for " + num.vote.nickname, e, EpisodeVote.transform.GetChild(0).GetChild(0), 0);
                            votes.Add(num.vote);
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
                        GameManager.instance.MakeGroup(false, null, "", "", votedOff.nickname + ", the tribe has spoken. \n \n" + votedOff.nickname +" will now go to Redemption Island", new List<Contestant>() { votedOff}, EpisodeVote.transform.GetChild(0).GetChild(0), 0);
                        tribe.members.Remove(votedOff);
                        GameManager.instance.RIsland.Add(votedOff);
                    }
                    
                }
                break;
        }
        GameManager.instance.NextEvent();
    }
    public void PalauStart()
    {

    }
}