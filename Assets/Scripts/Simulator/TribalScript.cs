using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeasonParts;
using System.Linq;

public class TribalScript : MonoBehaviour
{
    GameManager manager;

    List<Contestant> targets = new List<Contestant>();
    //Debug.Log("Targets:" + string.Join(",", targets.Select(x => x.nickname)) + " Count:" + targets.Count);
    bool aa = false;
    bool e = false;
    int tieNum = 2;
    List<Contestant> Idols = new List<Contestant>();
    List<Contestant> nullIdols = new List<Contestant>();
    List<Vote> Votes = new List<Vote>();
    List<Contestant> votes = new List<Contestant>();
    List<Contestant> votesRead = new List<Contestant>();
    List<Contestant> tie = new List<Contestant>();
    bool what = false;
    string conPlacement = "";
    string vote = "";
    string juror = "";
    Team LosingTribe;
    Contestant DoOrDie;
    public Team team;
    public List<Contestant> immune;
    public bool cineTribal;
    GameObject EpisodeStart;
    public Contestant votedOff;
    int tri = 0;
    string finalVotes;
    public Advantage ImmunityNecklace;

    Dictionary<Contestant, int> dic = new Dictionary<Contestant, int>(), dicVR = new Dictionary<Contestant, int>();
    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<GameManager>();
    }

    void resetValues()
    {
        targets = new List<Contestant>();
        //Debug.Log("Targets:" + string.Join(",", targets.Select(x => x.nickname)) + " Count:" + targets.Count);
        aa = false;
        e = false;
        tieNum = 2;
        Idols = new List<Contestant>();
        nullIdols = new List<Contestant>();
        Votes = new List<Vote>();
        votes = new List<Contestant>();
        votesRead = new List<Contestant>();
        tie = new List<Contestant>();
        what = false;
        conPlacement = "";
        vote = "";
        juror = "";
    }

    public void DoTribal()
    {
        
        resetValues();

        votedOff = manager.votedOff;
        //votedOff = new Contestant();
        EpisodeStart = manager.MakePage("Tribal Council", 0, true);
        if (manager.MergedTribe.members.Count < 1)
        {
            if (team != manager.LosingTribes[0] && manager.curEvent.context == "TribalKidnap")
            {
                manager.kidnapped = team.members[Random.Range(0, team.members.Count)];
                foreach (Team tt in manager.Tribes)
                {
                    if (tt.members.Contains(manager.kidnapped))
                    {
                        manager.kidnapped.team = tt.name;
                    }
                }
                team.members.Remove(manager.kidnapped);
                List<Team> teams = new List<Team>(manager.Tribes);
                foreach (Team tt in manager.LosingTribes)
                {
                    teams.Remove(tt);
                }
                manager.Tribes[manager.Tribes.IndexOf(manager.LosingTribes[0])].members.Add(manager.kidnapped);
                List<Contestant> g = new List<Contestant>() { manager.kidnapped };
                manager.MakeGroup(false, team, "", "Before the vote, " + manager.LosingTribes[0].name + " must kidnap one member of " + team.name + ". \n \nThis person will skip tribal.", manager.LosingTribes[0].name + " kidnaps " + manager.kidnapped.nickname + ".", g, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
            }
            else if (team != manager.LosingTribes[0] && manager.curEvent.context == "TribalImmBV")
            {
                immune.Add(team.members[Random.Range(0, team.members.Count)]);
                List<Contestant> g = new List<Contestant>() { immune[manager.immune.Count - 1], manager.immune[0] };
                List<Contestant> w = new List<Contestant>() { immune[0] };

                manager.MakeGroup(false, team, "", w[0].nickname + " has to grant one player on the tribe immunity.", "", w, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                manager.MakeGroup(false, team, "", "", immune[0].nickname + " grants immunity to " + g[0].nickname, g, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
            }
        }

        LosingTribe = team;
        manager.LosingTribe = team;
        string etext = "";

        if (team.members.Count > 2)
        {
            if (manager.teamTargets.Find(x => x.name == team.name) == null)
            {
                manager.TribeTargeting(team);
            }
            targets = manager.teamTargets.Find(x => x.name == team.name).members;
            for(int i = 0; i < targets.Count; i++)
            {
                if (!team.members.Contains(targets[i]))
                {
                    targets = targets.Except(immune).ToList();
                    targets.Remove(targets[i]);
                    Debug.Log("");
                    
                    while (targets.Count() < 2)
                    {
                        targets.Add(team.members.Except(immune).Except(targets).ToList()[Random.Range(0, team.members.Except(immune).Except(targets).ToList().Count())]);
                    }
                    foreach (Contestant mem in team.members)
                    {
                        mem.PersonalTarget(targets);
                    }
                }
            }
            

            etext = "It's time to vote.";
            manager.MakeGroup(true, team, "name", "", "", team.members, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
            if (DoOrDie != null)
            {
                List<bool> boxes = new List<bool>() { false, false, true };
                int num = Random.Range(1, 4);
                bool chosen = boxes[Random.Range(0, 3)];

                string swap = "They decide to keep their current box.";
                string safe = "... safe!";

                if (Random.Range(0, 2) == 0)
                {
                    swap = "They decide to swap their choice.";
                    chosen = !chosen;
                }

                if (!chosen)
                {
                    safe = "... not safe!";
                }

                List<Contestant> DOD = new List<Contestant>() { DoOrDie };
                manager.MakeGroup(false, null, "", "Before the players can vote, " + DoOrDie.nickname + " must complete a game of chance to determine their fate.",
                    DoOrDie.nickname + " is given a choice between three boxes.\n\nIf they choose the correct box, they will be safe." +
                    "\n\nHowever, if they choose one of two incorrect boxes, they will be automatically eliminated and tribal will be cancelled." +
                    "\n\n" + DoOrDie.nickname + " chooses the " + manager.Oridinal(num) + " box.\n\nOne of the other boxes is revealed to be incorrect and " + DoOrDie.nickname + " is given the choice to swap their box." +
                    "\n\n" + swap + "\n\nTheir box is revealed to be" + safe, DOD, EpisodeStart.transform.GetChild(0).GetChild(0), 15);

                if (chosen)
                {
                    DoOrDie.safety++;
                    DoOrDie = null;
                }
                else
                {
                    votedOff = DoOrDie;
                    DoOrDie = null;
                    vote = "Lost Challenge";
                    manager.Eliminate(vote, conPlacement, EpisodeStart, team);
                    return;
                }
            }
            List<Contestant> RRemove = new List<Contestant>();

            foreach (Contestant num in team.members)
            {
                if (num.safety > 0)
                {
                    List<Contestant> w = new List<Contestant>() { num };
                    manager.MakeGroup(false, null, "", "", num.nickname + " has safety from the vote.", w, EpisodeStart.transform.GetChild(0).GetChild(0), 0);

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
                        if (manager.currentContestants == advantage.expiresAt || (advantage.length == 1 && advantage.temp))
                        {
                            extra = "\n \nThis is the last round to use it.";
                        }
                        List<Contestant> w = new List<Contestant>() { num };
                        bool playable = true;
                        manager.MakeGroup(false, null, "", "", num.nickname + " has the " + advantage.nickname + extra, w, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                        Contestant usedOn = null;
                        int ran = Random.Range(0, 10);
                        if (advantage.type == "ImmunityNecklace" || advantage.type == "VoteSteal" || advantage.type == "VoteBlocker")
                        {
                            List<Contestant> teamV = new List<Contestant>(team.members);
                            teamV.Remove(num);
                            if (advantage.type == "ImmunityNecklace")
                            {
                                foreach (Contestant numm in immune)
                                {
                                    teamV.Remove(numm);
                                }
                                ran = Random.Range(0, num.stats.Strategic * 100);
                                if (targets.Contains(usedOn))
                                    playable = false;
                            }

                            usedOn = teamV[Random.Range(0, teamV.Count)];
                        }
                        if (advantage.type == "PreventiveIdol")
                        {
                            if (targets.Contains(num))
                                playable = true;
                        }
                        if (advantage.usedWhen == "BeforeVote" && playable && (ran == 1 || manager.currentContestants == advantage.expiresAt || advantage.length == 1 || advantage.type == "VoteSacrifice"))
                        {
                            if (advantage.type == "SafetyWithoutPower")
                            {
                                if (!num.advantages.Contains(ImmunityNecklace))
                                {
                                    AdvantagePlay(EpisodeStart.transform.GetChild(0).GetChild(0), advantage, num, usedOn);
                                    RRemove.Add(num);
                                    remove.Add(advantage);
                                }
                            }
                            else if (advantage.type == "VoteSacrifice")
                            {
                                Advantage av = new Advantage();
                                av.expiresAt = 6;
                                av.nickname = "Extra Vote";
                                av.type = "ExtraVote";
                                Add.Add(av);
                                remove.Add(advantage);
                            }
                            else
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

            if (team.members.Except(immune).Count() < 2)
            {
                votedOff = team.members.Except(immune).ToList()[0];
                vote = vote + "\nEliminated by Default";
                manager.Eliminate(vote, conPlacement, EpisodeStart, team);
                tri = 0;
                return;
            }

            if (targets.Count < 2)
            {
                targets.Add(team.members.Except(immune).Except(targets).ToList()[Random.Range(0, targets.Except(immune).Except(targets).Count())]);
            }

            manager.MakeGroup(false, null, "", "", etext, new List<Contestant>(), EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            bool voted = Voting();
            if (voted)
            {
                CountVotes();

                manager.AddVote(votes, votesRead, finalVotes);
                tie = new List<Contestant>();
                if (dic.Count > 0)
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
                DoTribal();
            }
            if (tie.Count < tieNum && tie.Count > 0)
            {
                //Debug.Log(votedOff.nickname + " has been eliminated. " + "Votes: " + dic[votedOff]);
                if (tie.Count < 2)
                {
                    manager.Eliminate(vote, conPlacement, EpisodeStart, team);
                }
            }
            else
            {
                aa = true;
                bool rea = false;
                foreach (Vote vot in Votes)
                {
                    Debug.Assert(team.members.Contains(vot.voter), vot.voter);
                }
                while (!rea)
                {
                    tri++;
                    if (tri == 2)
                    {
                        //Debug.Log("dfsa");
                    }
                    rea = RE();
                }
            }
        }
        else if (team.members.Count == 2)
        {
            manager.MakeGroup(true, team, "name", "", "", team.members, EpisodeStart.transform.GetChild(0).GetChild(0), 15);

            votes = new List<Contestant>();
            votesRead = new List<Contestant>();
            //Debug.Log("gg");
            etext = "A fire-making challenge will occur since there are only two castaways left.";
            manager.MakeGroup(true, team, "name", "", etext, team.members, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
            Tiebreaker(team.members, "FireChallenge");
            votes.Add(votedOff);
            votesRead.Add(votedOff);
            manager.AddVote(votes, votesRead, "");
        }
        else if (team.members.Count == 1)
        {
            //Debug.Log("kk");
            votes = new List<Contestant>();
            votesRead = new List<Contestant>();
            votedOff = team.members[0];
            votes.Add(votedOff);
            votesRead.Add(votedOff);
            manager.AddVote(votes, votesRead,"");
            vote = "Eliminated by Default";
            etext = "Sorry, " + votedOff.nickname + ", you're the only castaway left so you're automatically eliminated.";
            manager.MakeGroup(true, team, "name", "", etext, team.members, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
            manager.Eliminate(vote, conPlacement, EpisodeStart, team);
        }
        //manager.votedOff = votedOff;
        manager.Votes = Votes;
    }

    bool Voting()
    {
        for (int i = 0; i < team.members.Count; i++)
        {
            for (int a = 0; a < team.members[i].votes; a++)
            {
                bool contains = false;

                Vote v = new Vote();
                v.voter = team.members[i];
                if (targets.Contains(v.voter))
                {
                    contains = true;
                    targets.Remove(v.voter);
                }
                //v.voter.voteReason = "They voted based on personal preference.";

                v.vote = v.voter.MakeVote(targets, team.members);
                if (v.vote == null)
                {
                    Debug.Log("ADAD");
                }
                else
                {
                    Votes.Add(v);
                }

                if (contains)
                {
                    targets.Add(v.voter);
                }
            }
        }

        return true;
    }
    bool Revote(List<Contestant> tie)
    {
        //Debug.Log("Tie:" + string.Join(",", tie.Select(x => x.nickname)));
        //Debug.Log("Members:" + team.members.Count + "TMem:" + team.members.Except(tie).ToList().Count);
        foreach (Alliance alliance in manager.Alliances)
        {
            alliance.splitVoters = new List<Contestant>();
            alliance.altTargets = new List<Contestant>();
            int intersect = team.members.Intersect(alliance.members).ToList().Count;
            if (intersect > 1 && intersect != team.members.Count)
            {
                Contestant target = alliance.mainTargets.Find(x => team.members.Contains(x));
                if (!tie.Contains(target))
                {
                    alliance.mainTargets.Remove(alliance.mainTargets.Find(x => team.members.Contains(x)));
                    alliance.members = alliance.members.OrderByDescending(x => x.stats.Influence).ToList();
                    alliance.mainTargets.Add(alliance.members[0].PersonalTarget(tie));
                }
            }
        }


        for (int i = 0; i < Votes.Count; i++)
        {
            if (tie.Count < team.members.Count)
            {
                if (!tie.Contains(Votes[i].voter) && team.members.Except(tie).ToList().Contains(Votes[i].voter))
                {
                    //Debug.Log("a");

                    foreach (Contestant t in tie)
                    {
                        if (t.nickname == Votes[i].voter.nickname)
                        {
                            Contestant a = Votes[i].voter;

                            Debug.Log("New:" + a.nickname + " Target:" + a.target);
                        }
                    }
                    Votes[i].voter.target = tie.OrderByDescending(x => Votes[i].voter.value(x)).ToList().First();
                    Contestant rvote = Votes[i].voter.MakeVote(tie, team.members);
                    if (tie.Contains(Votes[i].voter))
                    {
                        rvote = null;
                        Debug.Log(Votes[i].voter);
                    }
                    if (rvote != null)
                    {
                        Votes[i].revotes.Add(rvote);
                    }
                }
                else
                {
                    //Debug.Log("what");
                }
            }
            else
            {
                //Debug.Log("gggg");
                //Votes[i].voter.PersonalTarget(tie);
                List<Contestant> tieV = new List<Contestant>(tie);
                tieV.Remove(Votes[i].voter);
                Votes[i].voter.target = tieV.OrderByDescending(x => Votes[i].voter.value(x)).ToList().First();

                Contestant rvote = Votes[i].voter.MakeVote(tieV, team.members);

                Votes[i].revotes.Add(rvote);
            }
        }

        return true;
    }
    bool VoteRestart()
    {
        if (targets.Except(immune).ToList().Count < 2)
        {
            targets = team.members.Except(immune).ToList();
        }
        else
        {
            targets = targets.Except(immune).ToList();
        }
        foreach (Alliance alliance in manager.Alliances)
        {
            alliance.splitVoters = new List<Contestant>();
            alliance.altTargets = new List<Contestant>();
            int intersect = team.members.Intersect(alliance.members).ToList().Count;
            if (intersect > 1 && intersect != team.members.Count)
            {
                if (!targets.Except(immune).ToList().Contains(alliance.mainTargets.Find(x => team.members.Contains(x))))
                {
                    alliance.mainTargets.Remove(alliance.mainTargets.Find(x => team.members.Contains(x)));
                    alliance.members = alliance.members.OrderByDescending(x => x.stats.Influence).ToList();
                    alliance.mainTargets.Add(alliance.members[0].PersonalTarget(targets.Except(alliance.members).ToList()));
                }
            }
        }

        for (int i = 0; i < Votes.Count; i++)
        {
            bool contains = false;
            if (targets.Contains(Votes[i].voter))
            {
                contains = true;
                targets.Remove(Votes[i].voter);
            }
            Votes[i].voter.target = targets.Except(immune).OrderByDescending(x => Votes[i].voter.value(x)).ToList().First();
            Contestant rvote = Votes[i].voter.MakeVote(targets.Except(immune).ToList(), team.members);

            if (rvote != null)
            {
                Votes[i].revotes.Add(rvote);
            }
            //Votes[i].revotes.Add(rvote);
            if (contains)
            {
                targets.Add(Votes[i].voter);
            }
        }
        return true;
    }
    void CountVotes()
    {
        string eventext = "";
        foreach (Vote num in Votes)
        {
            if (aa && num.revotes.Count > 0)
            {
                if (team.members.Count > tieNum)
                {
                    if (team.members.Except(tie).ToList().Contains(num.voter))
                    {
                        if (num.revotes.Count > 0)
                        {
                            votes.Add(num.revotes[num.revotes.Count - 1]);
                            //Debug.Log(num.voter);
                        }
                    }
                }
                else
                {
                    votes.Add(num.revotes[num.revotes.Count - 1]);
                }

            }
            else
            {
                if (!tie.Contains(num.voter) && team.members.Except(tie).ToList().Contains(num.voter))
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
            foreach (Advantage advantage in num.advantages)
            {
                List<Contestant> w = new List<Contestant>() { num };

                Contestant usedOn = null;
                List<Contestant> hasIdol = new List<Contestant>();
                foreach (Contestant con in team.members)
                {
                    foreach (Advantage ad in num.advantages)
                    {
                        if (ad.type == "HiddenImmunityIdol" && con != num)
                        {
                            hasIdol.Add(con);
                        }
                    }
                }
                if (hasIdol.Count > 0)
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
                    if (advantage.type == "SuperIdol")
                    {
                        superIdols.Add(num);
                    }
                    if (advantage.name == "Ally Idol" || immune.Contains(num) || Idols.Contains(num))
                    {
                        List<Contestant> teamV = new List<Contestant>(team.members);
                        teamV.Remove(num);
                        foreach (Contestant con in immune)
                        {
                            if (teamV.Contains(con))
                                teamV.Remove(con);
                        }
                        foreach (Contestant con in Idols)
                        {
                            if (teamV.Contains(con))
                                teamV.Remove(con);
                        }
                        usedOn = teamV.OrderBy(x => num.goodValue(x)).ToList().First();
                    }
                    int ran = Random.Range(0, 10);
                    if (tie.Contains(num) && usedOn == null && Random.Range(1, 6) == num.stats.Intuition)
                    {
                        ran = Random.Range(0, 6 - num.stats.Intuition);
                    }
                    if (usedOn != null)
                    {
                        if (tie.Contains(usedOn) && Random.Range(1, 6) == num.stats.Intuition)
                            ran = Random.Range(0, 6 - num.stats.Intuition);
                    }
                    if ((immune.Contains(num) || Idols.Contains(num)) && usedOn == null)
                    {
                        playable = false;
                    }
                    /*&& ran == 1* && tie.Contains(num)*/

                    if (playable && (ran == 0 || manager.currentContestants == advantage.expiresAt || advantage.length == 1))
                    {
                        bool a = false;
                        if (advantage.onlyUsable.Count > 0)
                        {
                            foreach (int numb in advantage.onlyUsable)
                            {
                                if (manager.currentContestants == numb)
                                {
                                    a = true;
                                }
                            }
                        }
                        else
                        {
                            a = true;
                        }
                        if (a)
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
            manager.MakeGroup(false, null, "", "", "I'll read the votes.", new List<Contestant>(), EpisodeStart.transform.GetChild(0).GetChild(0), 0);
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
                        enoughVotes = (votes.Count / 2) - 1;
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

        manager.ShuffleVotes(votesRead);

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
            foreach (KeyValuePair<Contestant, int> num in vot)
            {
                if (vot.Keys.ToList().IndexOf(num.Key) < vot.Count - 1)
                {
                    if (dicIdol.Contains(num))
                    {
                        vote += "<color=red>" + dicIdol[num.Key] + "*</color>-";
                    }
                    else
                    {
                        if (manager.MergedTribe.members.Count < 1)
                        {
                            if (team != manager.LosingTribes[0] && manager.curEvent.context == "TribalImmAV" && !aa && immunity == num.Key || (num.Value == dic.Values.Max() && superIdols.Contains(num.Key)))
                            {
                                vote += "<color=red>" + dic[num.Key] + "*</color>-";
                            }
                            else
                            {
                                vote += dic[num.Key] + "-";
                            }
                        }
                        else
                        {
                            if (num.Value == dic.Values.Max() && superIdols.Contains(num.Key))
                            {
                                vote += "<color=red>" + dic[num.Key] + "*</color>-";
                            }
                            else
                            {
                                vote += dic[num.Key] + "-";
                            }
                        }
                    }
                }
                else
                {
                    if (dicIdol.Contains(num))
                    {
                        vote += "<color=red>" + dicIdol[num.Key] + "*</color>";
                    }
                    else
                    {
                        if (num.Value == dic.Values.Max() && superIdols.Contains(num.Key))
                        {
                            vote += "<color=red>" + dic[num.Key] + "*</color>";
                        }
                        else
                        {
                            vote += dic[num.Key] + " Vote";
                        }
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
        if (manager.showVL == true)
        {
            float vl = votes.Count - 1;
            votesLeft = ". " + vl + " Votes Left";
            if (vl == 1)
            {
                votesLeft = ". " + vl + " Vote Left";
            }
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

        }
        else
        {
            List<Contestant> r = new List<Contestant>() { votesRead[0] };
            if (Idols.Contains(votesRead[0]))
            {
                dicVR.Remove(votesRead[0]);
                manager.MakeGroup(false, null, "nname", "", "<color=red>DOES NOT COUNT</color>", r, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            }
            else
            {
                manager.MakeGroup(false, null, dicVR[votesRead[0]] + votess + votesRead[0].nickname + votesLeft, "", "", r, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            }

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
                    if (!Idols.Contains(votesRead[i]))
                    {
                        dicVR.Add(votesRead[i], 1);
                    }
                }
                votess = "";
                votesLeft = "";
                if (manager.showVL == true)
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
                    if (manager.currentContestants - manager.finaleAt <= manager.juryAt && !manager.sea.RedemptionIsland && !manager.sea.EdgeOfExtinction)
                    {
                        float juryy = manager.jury.Count + 1;
                        juryPM = " and " + manager.Oridinal(juryy) + " member of the jury";
                    }
                    float placement = manager.elimed;
                    string placementt = "";
                    placementt = manager.Oridinal(placement);
                    manager.elimed++;
                    atext = "The " + placementt + " eliminated from " + manager.seasonTemp.nameSeason + juryPM + " is... ";

                    votesSoFar = votesSoFar.OrderByDescending(go => go[0]).ToList();
                    evtext = finalVotes;
                    ctext = votesRead[i].nickname;
                    if (Idols.Contains(votesRead[i]))
                    {
                        atext = "";
                        finalVotes = "<color=red>DOES NOT COUNT</color>";
                    }
                    manager.MakeGroup(false, null, "name", atext, finalVotes, g, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                }
                else if (i == votesRead.Count - 1 && tie.Count > 1)
                {
                    evtext = votedOff.nickname;
                    evtext = votesRead[i].nickname;
                    manager.MakeGroup(false, null, "nname", atext, "", g, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
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
                        manager.elimed++;
                        manager.MakeGroup(false, null, "name", "", evtext, tie, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                    }

                }
                else
                {
                    if (Idols.Contains(votesRead[i]))
                    {
                        manager.MakeGroup(false, null, "nname", atext, "<color=red>DOES NOT COUNT</color>", g, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                    }
                    else
                    {
                        manager.MakeGroup(false, null, ctext, atext, evtext, g, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                    }
                }
            }
        }

        if (manager.MergedTribe.members.Count < 1)
        {
            if (team != manager.LosingTribes[0] && manager.curEvent.context == "TribalImmAV" && !aa)
            {

                List<Contestant> g = new List<Contestant>() { immunity, immune[0] };
                List<Contestant> w = new List<Contestant>() { immune[0] };

                manager.MakeGroup(false, team, "", "", w[0].nickname + " granted one player on the tribe immunity.", w, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                manager.MakeGroup(false, team, "", "", immune[0].nickname + " granted immunity to " + g[0].nickname, g, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
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
                        if (advantage.type == "SuperIdol" && tie.Contains(num))
                        {
                            AdvantagePlay(EpisodeStart.transform.GetChild(0).GetChild(0), advantage, num, usedOn);
                            remove.Add(advantage);
                            if (tie.Count > 1)
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
        if (eventext != "")
        {
            if (tie.Count < 2)
            {
                manager.MakeGroup(false, null, "name", "", "Final vote count was " + string.Join(", ", new List<string>(votesSoFar).ConvertAll(go => go)) + ".", new List<Contestant>(), EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            }
            else
            {
                manager.MakeGroup(false, null, "name", "", eventext, tie, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
            }
        }
        manager.AddIdols(Idols);
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
            if (team.members.Count > tie.Count)
            {
                foreach (Contestant num in tie)
                {
                    teamV.Remove(num);
                }
            }
            if (immune != null)
            {
                foreach (Contestant num in immune)
                {
                    if (team.members.Contains(num))
                        teamV.Remove(num);
                }
            }
            if (cineTribal == true)
            {
                manager.MakeGroup(false, null, "name", "", "Because the vote is a deadlock, rocks will be drawn.", teamV, null, 20);
                //AddFinalVote(group);
            }
            else
            {
                manager.MakeGroup(false, null, "name", "", "Because the vote is a deadlock, rocks will be drawn.", teamV, EpisodeStart.transform.GetChild(0).GetChild(0), 20);
                //group.transform.parent = EpisodeStart.transform.GetChild(0).GetChild(0);
            }
            votedOff = teamV[Random.Range(0, teamV.Count)];
            vote = vote + "\nRocks Drawn";

            if (team.members.Count > tie.Count)
            {
                if (!tie.Contains(votedOff))
                {
                    //Debug.Log(votedOff.nickname + " has been eliminated. " + "Drew the purple rock");
                    
                    manager.Eliminate(vote, conPlacement, EpisodeStart, team);

                }
            }
            else
            {
                manager.Eliminate(vote, conPlacement, EpisodeStart, team);
            }
            
        }
        void FireChallenge()
        {
            if (cineTribal == true)
            {
                manager.MakeGroup(false, null, "name", "", "Those tied will compete in a fire-making challenge. The loser will be eliminated from the game.", tie, null, 15);
                //AddFinalVote(grouppp);
            }
            else
            {
                manager.MakeGroup(false, null, "name", "", "Those tied will compete in a fire-making challenge. The loser will be eliminated from the game.", tie, EpisodeStart.transform.GetChild(0).GetChild(0), 15);
                //grouppp.transform.parent = EpisodeStart.transform.GetChild(0).GetChild(0);
            }
            votedOff = tie[Random.Range(0, tie.Count)];
            if (team.members.Count > 2)
            {
                vote = vote + "\nTiebreaker";
            }
            else
            {
                vote = "Tiebreaker";
            }

            //Debug.Log(votedOff.nickname + " has been eliminated. " + "Lost firemaking");
            manager.Eliminate(vote, conPlacement, EpisodeStart, team);
        }
        void Challenge()
        {
            if (cineTribal == true)
            {
                manager.MakeGroup(false, null, "name", "", "Those tied will compete in a tiebreaker challenge. The loser will be eliminated from the game.", tie, null, 15);

                //AddFinalVote(grouppp);
            }
            else
            {
                manager.MakeGroup(false, null, "name", "", "Those tied will compete in a fire-making challenge. The loser will be eliminated from the game.", tie, EpisodeStart.transform.GetChild(0).GetChild(0), 15);

                //grouppp.transform.parent = EpisodeStart.transform.GetChild(0).GetChild(0);
            }
            votedOff = tie[Random.Range(0, tie.Count)];
            vote = vote + "\n Tiebreaker";
            //Debug.Log(votedOff.nickname + " has been eliminated. " + "Lost tiebreaker challenge");
            manager.Eliminate(vote, conPlacement, EpisodeStart, team);
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
            if (advantage.type != "ImmunityNecklace" && advantage.type != "VoteSteal" && advantage.type != "VoteBlocker")
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
        foreach (HiddenAdvantage hid in manager.sea.islandHiddenAdvantages)
        {
            if (advantage.nickname == hid.name && hid.reHidden && manager.sea.IslandType == "Exile")
            {
                hid.hidden = true;
                hid.hiddenChance = 25;
            }
        }
        foreach (Team tribe in manager.Tribes)
        {
            foreach (HiddenAdvantage hid in tribe.hiddenAdvantages)
            {
                if (advantage.nickname == hid.name && hid.reHidden)
                {
                    hid.hidden = true;
                    hid.hiddenChance = 20;
                }
            }
        }

        if (manager.MergedTribe.members.Count > 0)
        {
            foreach (HiddenAdvantage hid in manager.MergedTribe.hiddenAdvantages)
            {
                if (advantage.nickname == hid.name && hid.reHidden)
                {
                    hid.hidden = true;
                    hid.hiddenChance = 20;
                }
                foreach (HiddenAdvantage hidden in manager.sea.islandHiddenAdvantages)
                {
                    if (advantage.name == hid.name && hidden.name == hid.name && hid.linkedToExile && hid.hideAt <= manager.curEp + 2)
                    {
                        if (hid.length == 0)
                        {
                            hid.hidden = true;
                            hid.hiddenChance = 20;
                            hid.length++;
                            hidden.hidden = false;
                        }
                        else
                        {
                            hid.hidden = false;
                            hidden.hidden = true;
                            hidden.hiddenChance = 20;
                        }
                    }
                }
            }
        }

        switch (advantage.type)
        {
            case "PreventiveIdol":
                immune.Add(user);
                targets.Remove(user);
                evetext += "\n \n No one can vote for " + user.nickname;
                break;
            case "ImmunityNecklace":
                immune.Add(usedOn);
                immune.Remove(user);
                targets.Remove(user);
                if (targets.Contains(usedOn))
                {
                    targets.Remove(usedOn);
                    targets.Add(user);
                    foreach (Contestant num in team.members)
                    {
                        if (num.target == usedOn)
                        {
                            num.target = user;
                        }
                    }
                    foreach (Alliance alliance in manager.Alliances)
                    {
                        if (alliance.mainTargets.Contains(usedOn))
                        {
                            alliance.mainTargets.Remove(usedOn);
                            alliance.mainTargets.Add(user);
                        }
                    }
                }

                evetext = user.nickname + " gives individual immunity to " + usedOn.nickname + "\n \n " + user.nickname + " is now vulnerble";
                break;
            case "SafetyWithoutPower":
                immune.Add(user);
                manager.Exiled.Add(user);
                targets.Remove(user);
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
                if (!nullIdols.Contains(user))
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
        manager.MakeGroup(false, null, "", "", evetext + ".", n, obj, 0);
    }
    bool RE()
    {
        tieNum = tie.Count;
        if (tieNum == 0)
        {
            tieNum = 2;
        }
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
            manager.Eliminate(vote, conPlacement, EpisodeStart, team);
            tri = 0;
            return true;
        }
        else if (teamV.Count == 0)
        {
            Tiebreaker(o, "FireChallenge");
            tri = 0;
            return true;
        }
        if (cineTribal == true)
        {
            GameObject EpisodeStartt = Instantiate(manager.Prefabs[0]);
            EpisodeStartt.transform.parent = manager.Canvas.transform;
            EpisodeStartt.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStartt.GetComponent<RectTransform>().offsetMax.y);
            EpisodeStartt.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStartt.GetComponent<RectTransform>().offsetMin.x, 0);
            EpisodeStartt.name = "Tribal Council";
            what = true;
            manager.AddGM(EpisodeStartt, false);
        }
        bool rev = true;
        if (tie.Count < 1)
        {
            rev = VoteRestart();
        }
        else
        {
            rev = Revote(tie);
        }
        if (rev)
        {
            CountVotes();
            //curEpp--;
            manager.AddVote(votes, votesRead, finalVotes);

            float maxxValue = dic.Values.Max();
            if (tie.Count < tieNum)
            {
                //Debug.Log(votedOff.nickname + " has been eliminated. " + "Votes: " + dic[votedOff]);
                if (tie.Count < 2)
                {
                    manager.Eliminate(vote, conPlacement, EpisodeStart, team);
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
                if (tie.Count < 1)
                {
                    return false;
                }
                else
                {
                    if (manager.MergedTribe.members.Count == 4)
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
