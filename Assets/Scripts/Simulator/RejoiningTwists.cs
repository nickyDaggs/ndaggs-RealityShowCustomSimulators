using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeasonParts;
using System.Linq;

public class RejoiningTwists : MonoBehaviour
{
    public void RedemptionIsland()
    {
        GameObject RIEvent = Instantiate(GameManager.instance.Prefabs[0]);
        RIEvent.transform.parent = GameManager.instance.Canvas.transform;
        RIEvent.GetComponent<RectTransform>().offsetMax = new Vector2(0, RIEvent.GetComponent<RectTransform>().offsetMax.y);
        RIEvent.GetComponent<RectTransform>().offsetMax = new Vector2(RIEvent.GetComponent<RectTransform>().offsetMin.x, 0);
        GameManager.instance.AddGM(RIEvent, true);
        List<Contestant> remove = new List<Contestant>();
        if (GameManager.instance.currentContestants == GameManager.instance.mergeAt && !GameManager.instance.RIExpired || GameManager.instance.currentContestants == 4)
        {
            GameManager.instance.RIExpired = true;
            int ranW = Random.Range(0, GameManager.instance.RIsland.Count);
            Contestant winner = GameManager.instance.RIsland[ranW];
            for (int i = 0; i < GameManager.instance.RIsland.Count; i++)
            {
                if (GameManager.instance.RIsland[i] != winner)
                {
                    List<Contestant> n = new List<Contestant>() { GameManager.instance.RIsland[i] };
                    GameManager.instance.MakeGroup(false, null, GameManager.instance.RIsland[i].nickname + " loses and leaves the game.", "", "", n, RIEvent.transform.GetChild(0).GetChild(0), 0);
                    remove.Add(GameManager.instance.RIsland[i]);
                }
            }
            List<Contestant> r = new List<Contestant>() { winner };
            GameManager.instance.MakeGroup(false, null, winner.nickname + " wins and returns to the game.", "", "", r, RIEvent.transform.GetChild(0).GetChild(0), 0);
            winner.teams.Add(new Color());
            GameManager.instance.Eliminated.Remove(winner);
            GameManager.instance.MergedTribe.members.Add(winner);
            GameManager.instance.RIsland.Remove(winner);
            GameManager.instance.currentContestants++;
            RIEvent.name = "Returning Duel";
        }
        else
        {
            if (GameManager.instance.Episodes[GameManager.instance.curEp].elimAllButTwo)
            {
                List<Contestant> RIT = new List<Contestant>(GameManager.instance.RIsland);
                int ranW1 = Random.Range(0, GameManager.instance.RIsland.Count);
                Contestant winner1 = GameManager.instance.RIsland[ranW1];
                RIT.Remove(winner1);
                int ranW2 = Random.Range(0, RIT.Count);
                Contestant winner2 = RIT[ranW2];
                for (int i = 0; i < GameManager.instance.RIsland.Count; i++)
                {
                    if (GameManager.instance.RIsland[i] != winner1 && GameManager.instance.RIsland[i] != winner2)
                    {
                        List<Contestant> n = new List<Contestant>() { GameManager.instance.RIsland[i] };
                        GameManager.instance.MakeGroup(false, null, GameManager.instance.RIsland[i].nickname + " loses and leaves the game.", "", "", n, RIEvent.transform.GetChild(0).GetChild(0), 20);
                        remove.Add(GameManager.instance.RIsland[i]);
                    }
                }


                List<Contestant> r = new List<Contestant>() { winner1 };
                GameManager.instance.MakeGroup(false, null, winner1.nickname + " wins and remains on redemption island.", "", "", r, RIEvent.transform.GetChild(0).GetChild(0), 20);
                
                List<Contestant> a = new List<Contestant>() { winner2 };
                GameManager.instance.MakeGroup(false, null, winner2.nickname + " wins and remains on redemption island.", "", "", r, RIEvent.transform.GetChild(0).GetChild(0), 20);
            }
            else
            {
                int ranW = Random.Range(0, GameManager.instance.RIsland.Count);
                Contestant winner = GameManager.instance.RIsland[ranW];
                for (int i = 0; i < GameManager.instance.RIsland.Count; i++)
                {
                    if (GameManager.instance.RIsland[i] != winner)
                    {
                        List<Contestant> n = new List<Contestant>() { GameManager.instance.RIsland[i] };
                        GameManager.instance.MakeGroup(false, null, GameManager.instance.RIsland[i].nickname + " loses and leaves the game.", "", "", n, RIEvent.transform.GetChild(0).GetChild(0), 20);
                        remove.Add(GameManager.instance.RIsland[i]);
                    }
                }

                List<Contestant> r = new List<Contestant>() { winner };
                GameManager.instance.MakeGroup(false, null, winner.nickname + " wins and remains on redemption island.", "", "", r, RIEvent.transform.GetChild(0).GetChild(0), 20);
            }
            GameManager.instance.RIExpired = false;
        }
        foreach (Contestant num in remove)
        {
            if (GameManager.instance.currentContestants - GameManager.instance.finaleAt + 2 <= GameManager.instance.juryAt)
            {
                num.placement = num.placement.Replace("Pre-Juror", "Juror");
                GameManager.instance.jury.Add(num);
            }
            GameManager.instance.RIsland.Remove(num);
        }
        GameManager.instance.NextEvent();
    }
    public void EOEStatus()
    {
        GameObject EOEStatus = Instantiate(GameManager.instance.Prefabs[0]);
        EOEStatus.transform.parent = GameManager.instance.Canvas.transform;
        EOEStatus.GetComponent<RectTransform>().offsetMax = new Vector2(0, EOEStatus.GetComponent<RectTransform>().offsetMax.y);
        EOEStatus.GetComponent<RectTransform>().offsetMax = new Vector2(EOEStatus.GetComponent<RectTransform>().offsetMin.x, 0);
        GameManager.instance.AddGM(EOEStatus, true);
        List<Contestant> remove = new List<Contestant>();
        if (GameManager.instance.EOE[GameManager.instance.EOE.Count - 1] != GameManager.instance.lastEOE)
        {
            List<Contestant> r = new List<Contestant>() { GameManager.instance.EOE[GameManager.instance.EOE.Count - 1] };
            GameManager.instance.MakeGroup(false, null, "", "", r[0].nickname + " arrives on the Edge of Extinction", r, EOEStatus.transform.GetChild(0).GetChild(0), 20);
            GameManager.instance.lastEOE = GameManager.instance.EOE[GameManager.instance.EOE.Count - 1];
        }
        foreach (Contestant num in GameManager.instance.EOE)
        {
            int diff = GameManager.instance.EOE.Count - remove.Count;
            if (diff > 2)
            {
                int quit = Random.Range(0, 8);
                if (quit == 3)
                {
                    remove.Add(num);
                }
            }

        }

        string are = "are";
        string casta = "castaways";
        if (GameManager.instance.EOE.Count == 1)
        {
            are = "is";
            casta = "castaway";
        }
        GameManager.instance.MakeGroup(false, null, "", "", "There " + are + " " + GameManager.instance.EOE.Count + " " + casta + " on the Edge of Extinction.", GameManager.instance.EOE, EOEStatus.transform.GetChild(0).GetChild(0), 20);
        if (remove.Count > 0)
        {
            foreach (Contestant num in remove)
            {
                GameManager.instance.EOE.Remove(num);
                num.placement = num.placement + "\n Raised Flag Ep. " + (GameManager.instance.curEp + 1);
                if (GameManager.instance.Episodes[GameManager.instance.curEp].merged)
                {
                    num.placement = num.placement.Replace("Pre-Juror", "Juror");
                    GameManager.instance.jury.Add(num);
                }
            }
            string comma = "";
            string raise = "raise";
            if (remove.Count > 2)
            {
                comma = ",";
            }
            if (remove.Count == 1)
            {
                raise = "raises";
            }
            string etext = string.Join(comma, new List<Contestant>(remove).ConvertAll(i => i.nickname)) + " " + raise + " the flag to leave the game";
            if (remove.Count > 1)
            {
                etext = etext.Replace(comma + remove[remove.Count - 1].nickname, " and " + remove[remove.Count - 1].nickname);
            }
            GameManager.instance.MakeGroup(false, null, "", "", etext, remove, EOEStatus.transform.GetChild(0).GetChild(0), 20);
        }
        else
        {
            GameManager.instance.MakeGroup(false, null, "", "", "No one raises the flag to leave the game.", new List<Contestant>(), EOEStatus.transform.GetChild(0).GetChild(0), 20);
        }
        GameManager.instance.NextEvent();
    }
    public void EOEReturnChallenge()
    {
        GameObject EOEStatus = Instantiate(GameManager.instance.Prefabs[0]);
        EOEStatus.transform.parent = GameManager.instance.Canvas.transform;
        EOEStatus.GetComponent<RectTransform>().offsetMax = new Vector2(0, EOEStatus.GetComponent<RectTransform>().offsetMax.y);
        EOEStatus.GetComponent<RectTransform>().offsetMax = new Vector2(EOEStatus.GetComponent<RectTransform>().offsetMin.x, 0);
        GameManager.instance.AddGM(EOEStatus, true);
        EOEStatus.name = "Return Challenge";
        List<Contestant> remove = new List<Contestant>();
        if (GameManager.instance.EOE[GameManager.instance.EOE.Count - 1] != GameManager.instance.lastEOE)
        {
            List<Contestant> r = new List<Contestant>() { GameManager.instance.EOE[GameManager.instance.EOE.Count - 1] };
            GameManager.instance.MakeGroup(false, null, "", "", r[0].nickname + " arrives on the Edge of Extinction", r, EOEStatus.transform.GetChild(0).GetChild(0), 20);
            GameManager.instance.lastEOE = null;
        }

        GameManager.instance.MakeGroup(false, null, "", "", "There are " + GameManager.instance.EOE.Count + " castaways on the Edge of Extinction. They will now compete in a challenge to return to the game.", GameManager.instance.EOE, EOEStatus.transform.GetChild(0).GetChild(0), 20);
        Contestant winner = GameManager.instance.EOE[Random.Range(0, GameManager.instance.EOE.Count)];
        GameManager.instance.EOE.Remove(winner);
        if(!GameManager.instance.Episodes[GameManager.instance.curEp].events.Contains("MergeTribes"))
        {
            winner.teams.Add(GameManager.instance.seasonTemp.MergeTribeColor);
        }
        
        GameManager.instance.Eliminated.Remove(winner);
        winner.halfIdols.Add(winner);
        GameManager.instance.MergedTribe.members.Add(winner);
        GameManager.instance.currentContestants++;

        List<Contestant> n = new List<Contestant>() { winner };
        GameManager.instance.MakeGroup(false, null, "", "", winner.nickname + " returns to the game.", n, EOEStatus.transform.GetChild(0).GetChild(0), 20);
        foreach (Contestant num in GameManager.instance.EOE)
        {
            int diff = GameManager.instance.EOE.Count - remove.Count;
            if (diff > 2)
            {
                int quit = Random.Range(0, 8);
                if (quit == 3)
                {
                    remove.Add(num);
                }
            }
        }
        if (GameManager.instance.currentContestants - 1 != GameManager.instance.mergeAt)
        {
            foreach (Contestant num in GameManager.instance.EOE)
            {
                num.placement = num.placement.Replace("Pre-Juror", "Juror");
                GameManager.instance.jury.Add(num);
            }
            GameManager.instance.RIExpired = true;
        }
        else
        {
            if (remove.Count > 0)
            {
                string comma = "";
                string raise = "raise";
                if (remove.Count > 2)
                {
                    comma = ",";
                }
                if (remove.Count == 1)
                {
                    raise = "raises";
                }
                string etext = string.Join(comma, new List<Contestant>(remove).ConvertAll(i => i.nickname)) + " " + raise + " the flag to leave the game";
                if (remove.Count > 1)
                {
                    etext = etext.Replace(comma + remove[remove.Count - 1].nickname, " and " + remove[remove.Count - 1].nickname);
                }
                GameManager.instance.MakeGroup(false, null, "", "", etext, remove, EOEStatus.transform.GetChild(0).GetChild(0), 20);
            }
            else
            {
                GameManager.instance.MakeGroup(false, null, "", "", "No one raises the flag to leave the game.", new List<Contestant>(), EOEStatus.transform.GetChild(0).GetChild(0), 20);
            }
        }
        GameManager.instance.NextEvent();
    }
    public void OutcastsImmunity()
    {
        GameManager.instance.immune = new List<Contestant>();
        GameManager.instance.LosingTribes = new List<Team>();
        GameObject EpisodeStart = Instantiate(GameManager.instance.Prefabs[0]);
        EpisodeStart.transform.parent = GameManager.instance.Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStart.name = "OutcastsImmunity";
        GameManager.instance.RIExpired = true;
        Team lastWinner = new Team();
        List<Team> Trib = new List<Team>();
        Trib.Add(GameManager.instance.Outcasts);
        foreach (Team tribe in GameManager.instance.Tribes)
        {
            Trib.Add(tribe);
        }
        GameManager.instance.Outcasts.name = "Outcasts";
        while (lastWinner != GameManager.instance.Outcasts)
        {
            int ran = Random.Range(0, Trib.Count);
            lastWinner = Trib[ran];

            if (Trib.Count > 1)
            {
                GameManager.instance.MakeGroup(false, null, "name", "", Trib[ran].name + " Wins Immunity!", Trib[ran].members, EpisodeStart.transform.GetChild(0), 20);
            }
            
            Trib.Remove(Trib[ran]);
        }

        bool oWin = false;
        foreach (Team tribe in GameManager.instance.Tribes)
        {
            foreach (Team members in Trib)
            {
                if (members == tribe)
                {
                    if (!oWin)
                    {
                        GameManager.instance.curEp--;
                        GameManager.instance.curEv = GameManager.instance.Episodes[GameManager.instance.curEp].events.Count;
                    }
                    GameManager.instance.LosingTribes.Add(tribe);
                    GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("TribalCouncil");
                    GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("ShowVotes");
                    oWin = true;
                }
            }
        }
        if (oWin == true)
        {
            GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("OutcastsTribal");
            GameManager.instance.Episodes[GameManager.instance.curEp].events.Add("ShowVotes");
        }
        else
        {
            GameManager.instance.OCExpired = true;

        }
        GameManager.instance.re = GameManager.instance.LosingTribes.Count;
        GameManager.instance.AddGM(EpisodeStart, true);
        GameManager.instance.NextEvent();
    }
    public void OutcastsTribal()
    {
        List<Contestant> tie2 = new List<Contestant>();
        Dictionary<List<Contestant>, int> ties = new Dictionary<List<Contestant>, int>();
        int returnees = 0;
        GameObject EpisodeStart = Instantiate(GameManager.instance.Prefabs[0]);
        EpisodeStart.transform.parent = GameManager.instance.Canvas.transform;
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStart.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStart.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStart.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStart.name = "Outcasts Tribal Council";
        GameManager.instance.AddGM(EpisodeStart, true);
        Team OutCasts = new Team();
        OutCasts = GameManager.instance.Outcasts;
        GameManager.instance.LosingTribe = GameManager.instance.Outcasts;
        foreach (Contestant num in GameManager.instance.Outcasts.members)
        {
            List<Contestant> OC = new List<Contestant>(GameManager.instance.Outcasts.members);
            OC.Remove(num);
            num.vote = OC[Random.Range(0, OC.Count)];
            num.altVotes = new List<Contestant>();
        }
        if (GameManager.instance.LosingTribes.Count > 1)
        {
            for (int i = 0; i < GameManager.instance.LosingTribes.Count - 1; i++)
            {
                foreach (Contestant num in GameManager.instance.Outcasts.members)
                {
                    List<Contestant> OC = new List<Contestant>(GameManager.instance.Outcasts.members);
                    OC.Remove(num);
                    num.altVotes.Add(OC[Random.Range(0, OC.Count)]);
                }
            }
        }
        GameManager.instance.votes = new List<Contestant>();
        GameManager.instance.votesRead = new List<Contestant>();
        GameManager.instance.e = false;
        foreach (Contestant num in GameManager.instance.Outcasts.members)
        {
            if (num.vote != null)
            {
                GameManager.instance.votes.Add(num.vote);
                if (num.altVotes.Count > 0)
                {
                    GameManager.instance.votes.Add(num.altVotes[0]);
                }
            }
        }
        GameManager.instance.dic = new Dictionary<Contestant, int>();
        //votedOff = votes[0];
        GameManager.instance.dic.Add(GameManager.instance.votes[0], 1);
        for (int i = 1; i < GameManager.instance.votes.Count; i++)
        {
            if (GameManager.instance.dic.ContainsKey(GameManager.instance.votes[i]))
            {
                GameManager.instance.dic[GameManager.instance.votes[i]] += 1;
            }
            else if (!GameManager.instance.dic.ContainsKey(GameManager.instance.votes[i]))
            {
                GameManager.instance.dic.Add(GameManager.instance.votes[i], 1);
            }
        }
        GameManager.instance.tie = new List<Contestant>();
        float maxValuee = GameManager.instance.dic.Values.Max();
        List<float> votesSpread = new List<float>();
        int sec = 0;

        foreach (KeyValuePair<Contestant, int> num in GameManager.instance.dic)
        {
            if (num.Value == maxValuee)
            {
                GameManager.instance.tie.Add(num.Key);
            }
            else
            {
                if (num.Value > sec)
                {
                    sec = num.Value;
                }
                votesSpread.Add(num.Value);
            }
        }
        ties.Add(GameManager.instance.tie, GameManager.instance.dic.Values.Max());
        foreach (KeyValuePair<Contestant, int> num in GameManager.instance.dic)
        {
            bool tieAdded = false;
            foreach (KeyValuePair<List<Contestant>, int> key in ties)
            {
                if (num.Value == key.Value)
                {
                    if (!key.Key.Contains(num.Key))
                    {
                        key.Key.Add(num.Key);
                    }
                    tieAdded = true;
                }
            }
            if (!tieAdded)
            {
                List<Contestant> newTie = new List<Contestant>();
                newTie.Add(num.Key);
                ties.Add(newTie, num.Value);
            }
        }
        //Sort votes then generate each vote for UI 
        GameManager.instance.votesRead = GameManager.instance.votes.OrderBy(go => GameManager.instance.dic[go]).ToList();
        GameManager.instance.ShuffleVotes(GameManager.instance.votesRead);
        GameManager.instance.dicVR = new Dictionary<Contestant, int>();
        GameManager.instance.dicVR.Add(GameManager.instance.votesRead[0], 1);
        string votess;
        votess = " vote ";
        string votesLeft;
        if (GameManager.instance.showVL == true)
        {
            float vl = GameManager.instance.votes.Count - 1;
            votesLeft = ". " + vl + " Votes Left";
        }
        else
        {
            votesLeft = "";
        }
        List<string> votesSoFar = new List<string>();
        foreach (KeyValuePair<Contestant, int> num in GameManager.instance.dic)
        {
            if (num.Value > 1)
            {
                votess = " votes ";
            }
            else
            {
                votess = " vote ";
            }
            string v = GameManager.instance.dic[num.Key] + votess + num.Key.nickname;
            votesSoFar.Add(v);
        }
        votesSoFar = votesSoFar.OrderByDescending(go => go[0]).ToList();
        GameManager.instance.finalVotes = "Final vote count was " + string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + ".";
        GameManager.instance.AddVote(GameManager.instance.votes, GameManager.instance.votesRead);
        if (GameManager.instance.cineTribal == true)
        {

        }
        else
        {
            string ctext = GameManager.instance.dicVR[GameManager.instance.votesRead[0]] + votess + GameManager.instance.votesRead[0].nickname + votesLeft;
            List<Contestant> n = new List<Contestant>() { GameManager.instance.votesRead[0] };
            GameManager.instance.MakeGroup(false, null, ctext, "", "", n, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            for (int i = 1; i < GameManager.instance.votesRead.Count; i++)
            {
                if (GameManager.instance.dicVR.ContainsKey(GameManager.instance.votesRead[i]))
                {
                    GameManager.instance.dicVR[GameManager.instance.votesRead[i]] += 1;
                }
                else if (!GameManager.instance.dicVR.ContainsKey(GameManager.instance.votesRead[i]))
                {
                    GameManager.instance.dicVR.Add(GameManager.instance.votesRead[i], 1);
                }
                votess = "";
                votesLeft = "";
                if (GameManager.instance.showVL == true)
                {
                    float vl = GameManager.instance.votes.Count - i - 1;
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
                foreach (KeyValuePair<Contestant, int> num in GameManager.instance.dicVR)
                {
                    if (num.Value > 1)
                    {
                        votess = " votes ";
                    }
                    else
                    {
                        votess = " vote ";
                    }
                    string v = GameManager.instance.dicVR[num.Key] + votess + num.Key.nickname;

                    votesSoFar.Add(v);
                }
                votesSoFar = votesSoFar.OrderByDescending(go => go[0]).ToList();


                string cctext = string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + votesLeft;

                List<Contestant> r = new List<Contestant>() { GameManager.instance.votesRead[i] };

                if (i == GameManager.instance.votesRead.Count - 1)
                {
                    GameManager.instance.MakeGroup(false, null, "nname", "Final vote... ", "All votes have been read." + "\n" + GameManager.instance.finalVotes, r, EpisodeStart.transform.GetChild(0).GetChild(0), 20);
                }
                else if (i == GameManager.instance.votesRead.Count - 1 && GameManager.instance.tie.Count > 2)
                {
                    GameManager.instance.MakeGroup(false, null, "nname", "", "All votes have been read." + "\n" + GameManager.instance.finalVotes, r, EpisodeStart.transform.GetChild(0).GetChild(0), 20);

                    string etext = "";
                    if (GameManager.instance.e == false)
                    {
                        string firstline = "There is a tie. Those in in the tie will compete in a firemaking challenge.";
                        etext = firstline;
                    }
                    else
                    {
                        string secondline = "Final vote count was " + string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + ".";
                        etext = secondline;
                    }

                    GameManager.instance.MakeGroup(false, null, "name", "", etext, GameManager.instance.tie, EpisodeStart.transform.GetChild(0).GetChild(0), 20);
                } else
                {
                    GameManager.instance.MakeGroup(false, null, string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + votesLeft, "", "", r, EpisodeStart.transform.GetChild(0).GetChild(0), 20);
                }
            }
        }
        ties.ToDictionary(x => x.Value).Reverse();
        foreach (KeyValuePair<List<Contestant>, int> key in ties)
        {
            if (returnees < GameManager.instance.LosingTribes.Count)
            {
                if (key.Key.Count <= GameManager.instance.LosingTribes.Count - returnees)
                {
                    foreach (Contestant num in key.Key)
                    {
                        num.teams.Add(new Color());
                        num.safety++;
                        GameManager.instance.MergedTribe.members.Add(num);
                        GameManager.instance.Eliminated.Remove(num);
                        List<Contestant> n = new List<Contestant>() { num };
                        returnees++;
                        GameManager.instance.currentContestants++;
                        if (GameManager.instance.cineTribal)
                        {
                            GameManager.instance.MakeGroup(false, null, num.nickname + " will return to the game.", "", "", n, null, 0);
                        }
                        else
                        {
                            GameManager.instance.MakeGroup(false, null, num.nickname + " will return to the game.", "", "", n, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                        }
                    }
                }
                else
                {
                    string firstline = "There is a tie. Those in in the tie will compete in a firemaking challenge.";

                    if (GameManager.instance.cineTribal)
                    {
                        GameManager.instance.MakeGroup(false, null, "nname", "", firstline, key.Key, null, 20);
                    }
                    else
                    {
                        GameManager.instance.MakeGroup(false, null, "nname", "", firstline, key.Key, EpisodeStart.transform.GetChild(0).GetChild(0), 20);
                    }
                    for (int i = 0; i < GameManager.instance.LosingTribes.Count - returnees; i++)
                    {
                        int ran1 = Random.Range(0, key.Key.Count);
                        key.Key[ran1].teams.Add(new Color());
                        key.Key[ran1].safety++;
                        GameManager.instance.MergedTribe.members.Add(key.Key[ran1]);
                        GameManager.instance.Eliminated.Remove(key.Key[ran1]);

                        List<Contestant> n = new List<Contestant>() { key.Key[ran1] };
                        
                        key.Key.Remove(key.Key[ran1]);
                        
                        if (GameManager.instance.cineTribal)
                        {
                            GameManager.instance.MakeGroup(false, null, n[0].nickname + " will return to the game.", "", "", n, null, 0);
                        }
                        else
                        {
                            GameManager.instance.MakeGroup(false, null, n[0].nickname + " will return to the game.", "", "", n, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                        }
                        returnees++;
                    }
                }
            }
        }
        GameManager.instance.OCExpired = true;
        GameManager.instance.NextEvent();
    }
}
