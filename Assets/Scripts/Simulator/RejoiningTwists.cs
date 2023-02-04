using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeasonParts;
using System.Linq;

public class RejoiningTwists : MonoBehaviour
{
    Contestant penalizer = null;
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
            if(GameManager.instance.currentContestants == 4)
            {
                winner.teams.Add(GameManager.instance.MergedTribe.tribeColor);
            }
            GameManager.instance.currentContestants++;
            RIEvent.name = "Returning Duel";
        }
        else
        {
            if (GameManager.instance.Episodes[GameManager.instance.curEp].elimAllButTwo)
            {
                List<Contestant> RIT = new List<Contestant>(GameManager.instance.RIsland);
                int ranW1 = Random.Range(0, GameManager.instance.RIsland.Count);
                Contestant loser = GameManager.instance.RIsland[ranW1];
                GameManager.instance.RIsland.Remove(GameManager.instance.RIsland[ranW1]);
                GameManager.instance.RIsland.Insert(0, loser);
                for (int i = 0; i < GameManager.instance.RIsland.Count; i++)
                {
                    List<Contestant> n = new List<Contestant>() { GameManager.instance.RIsland[i] };

                    if (GameManager.instance.RIsland[i] == loser)
                    {
                        GameManager.instance.MakeGroup(false, null, GameManager.instance.RIsland[i].nickname + " loses and leaves the game.", "", "", n, RIEvent.transform.GetChild(0).GetChild(0), 20);
                        remove.Add(GameManager.instance.RIsland[i]);
                    } else
                    {
                        GameManager.instance.MakeGroup(false, null, GameManager.instance.RIsland[i].nickname + " wins and remains on redemption island.", "", "", n, RIEvent.transform.GetChild(0).GetChild(0), 20);
                    }
                }

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
        foreach(HiddenAdvantage advantage in GameManager.instance.sea.Twists.EOEAdvantages)
        {
            if(GameManager.instance.curEp + 1 == advantage.hideAt)
            {
                
                Contestant finder = GameManager.instance.EOE[Random.Range(0, GameManager.instance.EOE.Count)];
                string a = "a";
                if("aeiouAEIOU".IndexOf(advantage.name) == 0 )
                {
                    a = "an";
                }
                if (advantage.giveAway && advantage.name != "ImmunityAdvantage")
                {
                    if (GameManager.instance.MergedTribe.members.Count < 1)
                    {
                        GameManager.instance.MakeGroup(false, null, "", "", finder.nickname + " finds an advantage and can assign " + a + " " + advantage.name + " to a player attending the next tribal council.", new List<Contestant>() { finder }, EOEStatus.transform.GetChild(0).GetChild(0), 20);
                        GameManager.instance.EOEGiveAway = advantage;
                        foreach (Team tribe in GameManager.instance.Tribes)
                        {
                            GameManager.instance.extraVote.Add(tribe.members[Random.Range(0, tribe.members.Count)]);
                        }
                        GameManager.instance.MakeGroup(false, null, "name", "", "They plan on who they will give the advantage to." , GameManager.instance.extraVote, EOEStatus.transform.GetChild(0).GetChild(0), 20);
                    }
                    else
                    {
                        Contestant given = GameManager.instance.MergedTribe.members[Random.Range(0, GameManager.instance.MergedTribe.members.Count)];
                        string etext = finder.nickname + " finds an advantage and can assign " + a + " " + advantage.name + " to a remaining player in the game and gives it to " + given.nickname + ".";
                        if (advantage.IOILesson == "Practice")
                        {
                            GameManager.instance.MakeGroup(false, null, "", "", finder.nickname + " finds two advantages. They can practice for the next re-entry challenge.", new List<Contestant>() { finder }, EOEStatus.transform.GetChild(0).GetChild(0), 20);
                            etext = finder.nickname + " can also assign " + a + " " + advantage.name + " to a remaining player in the game and gives it to " + given.nickname + ".";
                            finder.challengeAdvantage = true;
                        }
                        Advantage av = Instantiate(advantage.advantage);
                        av.nickname = advantage.name;
                        if (advantage.temp)
                        {
                            av.temp = true;
                            av.length = advantage.length;
                        }
                        advantage.hidden = false;
                        if (advantage.advantage.type == "HalfIdol")
                        {
                            given.halfIdols.Add(given);
                        }
                        else
                        {
                            given.advantages.Add(av);
                        }
                        
                        GameManager.instance.MakeGroup(false, null, "", "", etext, new List<Contestant>() { given, finder }, EOEStatus.transform.GetChild(0).GetChild(0), 20);
                    }
                } else
                {
                    if(advantage.name == "Practice")
                    {
                        GameManager.instance.MakeGroup(false, null, "", "", finder.nickname + " finds an advantage. They can practice for the next re-entry challenge.", new List<Contestant>() { finder }, EOEStatus.transform.GetChild(0).GetChild(0), 20);
                        finder.challengeAdvantage = true;
                    } else if (advantage.name == "Penalize")
                    {
                        GameManager.instance.MakeGroup(false, null, "", "", finder.nickname + " finds an advantage and can penalize another player for re-entry challenge.", new List<Contestant>() { finder }, EOEStatus.transform.GetChild(0).GetChild(0), 20);
                        penalizer = finder;
                    } else if(advantage.name == "ImmunityAdvantage")
                    {
                        Contestant given = GameManager.instance.MergedTribe.members[Random.Range(0, GameManager.instance.MergedTribe.members.Count)];
                        GameManager.instance.MakeGroup(false, null, "", "", finder.nickname + " finds an advantage for the next immunity challenge and gives it to " + given.nickname + ".", new List<Contestant>() { given, finder }, EOEStatus.transform.GetChild(0).GetChild(0), 20);
                        given.challengeAdvantage = true;
                    }
                }

            }
        }
        if (remove.Count > 0)
        {
            foreach (Contestant num in remove)
            {
                GameManager.instance.EOE.Remove(num);
                num.placement = num.placement + "\n Raised Flag " + GameManager.instance.Episodes[GameManager.instance.curEp].nickname;
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
        if(penalizer != null)
        {
            List<Contestant> Edge = new List<Contestant>(GameManager.instance.EOE);
            Edge.Remove(penalizer);
            Contestant target = Edge[Random.Range(0, Edge.Count)];
            
            GameManager.instance.MakeGroup(false, null, "", "", penalizer.nickname + " penalizes " + target.nickname + ".", new List<Contestant>() { target, penalizer}, EOEStatus.transform.GetChild(0).GetChild(0), 20);
            Edge.Add(penalizer);
            if (Random.Range(0, 2) == 0)
            {
                Edge.Remove(target);
                winner = Edge[Random.Range(0, Edge.Count)];
            }
            penalizer = null;
        }
        foreach(Contestant num in GameManager.instance.EOE)
        {
            if(num.challengeAdvantage)
            {
                num.challengeAdvantage = false;
                if (Random.Range(0, 2) == 0)
                {
                    winner = num;
                }
            }
        }
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
            lastWinner = GameManager.instance.Outcasts;//Trib[ran];

            if (Trib.Count > 1)
            {
                GameManager.instance.MakeGroup(false, null, "name", "", Trib[ran].name + " Wins Immunity!", Trib[ran].members, EpisodeStart.transform.GetChild(0), 20);
            }

            Trib.Remove(GameManager.instance.Outcasts);//Trib[ran]);
        }

        bool oWin = false;
        foreach (Team tribe in GameManager.instance.Tribes)
        {
            foreach (Team members in Trib)
            {
                if (members == tribe)
                {
                    
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
        OutCasts.name = "The Outcasts"; OutCasts.tribeColor.a = 1;
        GameManager.instance.MakeGroup(true, OutCasts, "name", "", "", OutCasts.members, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
        foreach (Contestant num in GameManager.instance.Outcasts.members)
        {
            List<Contestant> OC = new List<Contestant>(GameManager.instance.Outcasts.members);
            OC.Remove(num);
            num.target = OC.OrderBy(x => num.goodValue(x)).First();
            num.altVotes = new List<Contestant>();
        }
        if (GameManager.instance.LosingTribes.Count > 1 && GameManager.instance.Outcasts.members.Count > 2)
        {
            for (int i = 0; i < GameManager.instance.LosingTribes.Count - 1; i++)
            {
                foreach (Contestant num in GameManager.instance.Outcasts.members)
                {
                    List<Contestant> OC = new List<Contestant>(GameManager.instance.Outcasts.members);
                    OC.Remove(num);
                    OC.Remove(num.target);
                    OC = OC.Except(num.altVotes).ToList();
                    num.altVotes.Add(OC.OrderBy(x => num.goodValue(x)).First());
                }
            }
        }
        GameManager.instance.votes = new List<Contestant>();
        GameManager.instance.votesRead = new List<Contestant>();
        GameManager.instance.e = false;
        foreach (Contestant num in GameManager.instance.Outcasts.members)
        {
            if (num.target != null)
            {
                GameManager.instance.votes.Add(num.target);
                if (num.altVotes.Count > 0)
                {
                    GameManager.instance.votes.Add(num.altVotes[0]);
                }
            }
        }
        GameManager.instance.dicVotes = new Dictionary<Contestant, int>();
        //votedOff = votes[0];
        GameManager.instance.dicVotes.Add(GameManager.instance.votes[0], 1);
        for (int i = 1; i < GameManager.instance.votes.Count; i++)
        {
            if (GameManager.instance.dicVotes.ContainsKey(GameManager.instance.votes[i]))
            {
                GameManager.instance.dicVotes[GameManager.instance.votes[i]] += 1;
            }
            else if (!GameManager.instance.dicVotes.ContainsKey(GameManager.instance.votes[i]))
            {
                GameManager.instance.dicVotes.Add(GameManager.instance.votes[i], 1);
            }
        }
        GameManager.instance.tie = new List<Contestant>();
        float maxValuee = GameManager.instance.dicVotes.Values.Max();
        List<float> votesSpread = new List<float>();
        int sec = 0;

        foreach (KeyValuePair<Contestant, int> num in GameManager.instance.dicVotes)
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
        ties.Add(GameManager.instance.tie, GameManager.instance.dicVotes.Values.Max());
        foreach (KeyValuePair<Contestant, int> num in GameManager.instance.dicVotes)
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
        GameManager.instance.votesRead = GameManager.instance.votes.OrderBy(go => GameManager.instance.dicVotes[go]).ToList();
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
        foreach (KeyValuePair<Contestant, int> num in GameManager.instance.dicVotes)
        {
            if (num.Value > 1)
            {
                votess = " votes ";
            }
            else
            {
                votess = " vote ";
            }
            string v = GameManager.instance.dicVotes[num.Key] + votess + num.Key.nickname;
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
                        GameManager.instance.currentContestants++;
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
