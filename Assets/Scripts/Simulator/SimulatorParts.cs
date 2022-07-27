using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EventType { Relationship, Stamina, Alliance}
public enum AllianceEventType { Create, Leave, Dissolve }
public enum SwapType { RegularSwap, RegularShuffle, ChallengeDissolve, DissolveLeastMembers, SchoolyardPick, SplitTribes, TribeChiefs, Mutiny, CISchoolyardPick }
public enum StatChoice { Physical, Endurance, Mental, Stamina, SocialSkills, Temperament, Strategic, Loyalty, Forgivingness, Boldness, Influence, Intuition }
public enum RelationshipType { Dislike, Neutral, Like }
public enum RelationshipStatus { Slight, Small, Medium, Strong, Extreme }
public enum Environment { Peaceful=1, Nice, Normal, Harsh, Chaotic }
namespace SeasonParts
{
    //namespace that includes several classes that are used for the simulator
    
    [System.Serializable]
    public class Alliance
    {
        //Alliance are groups of contestants that vote together
        public string name;
        public List<string> teams = new List<string>();
        public List<Contestant> members = new List<Contestant>();
        public List<Contestant> mainTargets = new List<Contestant>();
        public List<Contestant> altTargets = new List<Contestant>();
        public List<Contestant> splitVoters = new List<Contestant>();
    }
    [System.Serializable]
    public class Team
    {
        public string name;
        public Color tribeColor;
        public List<Contestant> members = new List<Contestant>();
        public List<HiddenAdvantage> hiddenAdvantages = new List<HiddenAdvantage>();
        public bool remove;
        public int allianceCount;
        public Environment environment = Environment.Normal;
        //public List<Alliance> alliances = new List<Alliance>();
    }
    [System.Serializable]
    public class EpisodeSetting
    {
        public string name;
        public string nickname;
        public float con;
        public bool merged;
        public TribeSwap swap = new TribeSwap();
        public Exile exileIsland = new Exile();
        public List<string> events = new List<string>();
        public bool elimAllButTwo = false;
        public OneTimeEvent Event = new OneTimeEvent();
    }
    [System.Serializable]
    public class TribeSwap
    {
        public bool on = false;
        public float swapAt;
        public SwapType type;
        public List<Team> newTribes;
        public string text;
        public bool ResizeTribes;
        public float numberSwap;
        public string leaderReason;
        public string pickingRules;
        public Exile exileIsland;
        public bool exile;
        public bool redIs;
        public bool orderBySize;
    }
    [System.Serializable]
    public class Episode
    {
        public string name;
        public List<Page> events = new List<Page>();
        public List<List<Contestant>> votes = new List<List<Contestant>>();
        public List<List<Contestant>> votesReads = new List<List<Contestant>>();
        public List<GameObject> finalVote = new List<GameObject>();
        //public List<GameObject> mems = new List<GameObject>();
    }
    [System.Serializable]
    public class OneTimeEvent
    {
        public string type = "";
        public string context = "";
        public int round;
        public int elim;
    }
    [System.Serializable]
    public class Exile
    {
        public bool on = false;
        public string reason;
        public bool ownTribe = false;
        public string exileEvent;
        public string challenge;
        public bool skipTribal = false;
        public bool two = false;
        public bool both = false;
    }
    [System.Serializable]
    public class Twist
    {
        public int expireAt;
        public List<int> epsSkipE = new List<int>();
        public List<int> epsSpecialE = new List<int>();
        public List<int> epsSkipRI = new List<int>();
        public List<int> epsSpecialRI = new List<int>();
        public List<Exile> SpecialEx = new List<Exile>();
        public Exile preMergeEIsland;
        public Exile MergeEIsland;
        public Team IOI;
        public List<HiddenAdvantage> EOEAdvantages;
    }
    [System.Serializable]
    public class Page
    {
        public GameObject obj;
        public List<Contestant> Vote;
        public List<Contestant> VotesRead;
        public List<Contestant> Idols = new List<Contestant>();
        public List<GameObject> VoteObjs = new List<GameObject>();
    }
    [System.Serializable]
    public class HiddenAdvantage
    {
        public string name;
        public Advantage advantage;
        public int hideAt;
        public bool reHidden;
        public bool hidden;
        public bool linkedToExile;
        public int length;
        public bool temp;
        public bool giveAway;
        public string IOILesson;
        public List<HiddenAdvantage> options;
        public bool IOISweetened;
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
    [System.Serializable]
    public class Vote
    {
        public Contestant voter;
        public Contestant vote;
        public List<Contestant> revotes = new List<Contestant>();
        public bool extra = false;
    }
    [System.Serializable]
    public class Stats
    {
        public int Physical = 3;
        public int Endurance = 3;
        public int Mental = 3;
        public int Stamina = 3;
        public int SocialSkills = 3;
        public int Temperament = 3;
        public int Strategic = 3;
        public int Loyalty = 3;
        public int Forgivingness = 3;
        public int Boldness = 3;
        public int Influence = 3;
        public int Intuition = 3;
    }
    [System.Serializable]
    public class Challenge
    {
        public string name;
        public string description;
        public List<string> rewards = new List<string>();
        public int rewardStamina;
        public List<Contestant> Groups = new List<Contestant>();
        public bool sitout;
        public List<StatChoice> stats = new List<StatChoice>() { StatChoice.Stamina};
    }
    [System.Serializable]
    public class ContestantEvent
    {
        public EventType type;
        public string eventText;
        public AllianceEventType allianceEvent;
        public int contestants;
        public int relationshipAffect;
        public int staminaAffect;
        public int limit;
        public bool overall;
        public List<StatChoice> stats;
    }
    [System.Serializable]
    public class Relationship
    {
        public Contestant person;
        public RelationshipType Type;
        public RelationshipStatus Status;
        public int Extra;
        public int changeChance;
        public bool perm;
        public override string ToString()
        {
            string type = "";
            string small = "small";
            if(Type == RelationshipType.Like)
            {
                type = " bond";
                if (Status == RelationshipStatus.Extreme)
                {
                    return "unbreakable bond";
                }
            } else if (Type == RelationshipType.Dislike)
            {
                type = " dislike";
                small = "mild";
                if(Status == RelationshipStatus.Extreme)
                {
                    return "extreme hatred";
                }
            } else
            {
                return "";
            }

            switch(Status)
            {
                case RelationshipStatus.Slight:
                    return "slight" + type;
                case RelationshipStatus.Small:
                    return small + type;
                case RelationshipStatus.Medium:
                    return "medium" + type;
                case RelationshipStatus.Strong:
                    return "strong" + type;
            }
            return "";
        }
    }
}

public class SimulatorParts : MonoBehaviour
{
    /*
            if(sea.advantages)
            {
                etext = "If there's an advantage you want to play, this is the time to do so.\n\n";
            }
            List<Contestant> RRemove = new List<Contestant>();
            foreach (Contestant num in team.members)
            {
                if (num.safety > 0)
                {
                    List<Contestant> w = new List<Contestant>() { num };
                    MakeGroup(false, null, "", "", num.nickname + " has safety from the vote.", w, EpisodeStart.transform.GetChild(0).GetChild(0), 0);

                    immune.Add(num);
                    num.safety--;
                }
                if (num.advantages.Count > 0)
                {
                    List<Advantage> remove = new List<Advantage>();

                    foreach (Advantage advantage in num.advantages)
                    {
                        string extra = "";
                        if (currentContestants == advantage.expiresAt || (advantage.length == 1 && advantage.temp))
                        {
                            extra = "\n \nThis is the last round to use it.";
                        }
                        List<Contestant> w = new List<Contestant>() { num };

                        MakeGroup(false, null, "", "", num.nickname + " has the " + advantage.nickname + extra, w, EpisodeStart.transform.GetChild(0).GetChild(0), 0);
                        Contestant usedOn = null;
                        if (advantage.type == "VoteSteal" || advantage.type == "VoteBlocker")
                        {
                            List<Contestant> teamV = new List<Contestant>(team.members);
                            teamV.Remove(num);

                            usedOn = teamV[Random.Range(0, teamV.Count)];
                        }
                        bool playable = false;
                        foreach(int numb in advantage.onlyUsable)
                        {

                        }
                        if(advantage.onlyUsable.Count > 0)
                        {
                            playable = true;
                        }
                        if ((advantage.type == "VoteSteal" || advantage.type == "VoteBlocker" || advantage.type == "SafetyWithoutPower" || advantage.type == "ExtraVote" || advantage.type == "HiddenImmunityIdol" || advantage.type == "PreventiveIdol" || advantage.type == "SuperIdol") && playable && Random.Range(0, 3) == 1)
                        {
                            AdvantagePlay(EpisodeStart.transform.GetChild(0).GetChild(0), advantage, num, usedOn);
                            remove.Add(advantage);
                            if (advantage.type == "SafetyWithoutPower")
                            {
                                RRemove.Add(num);
                            }
                        }
                    }
                    foreach (Advantage advantage in remove)
                    {
                        num.advantages.Remove(advantage);
                    }
                }
            }
            foreach (Contestant num in RRemove)
            {
                team.members.Remove(num);
            }
            team.members = team.members.OrderBy(x => x.votes).ToList();
            if(team.members.Count == 1 || team.members[0].votes < team.members[1].votes)
            {
                votes = new List<Contestant>();
                votesRead = new List<Contestant>();
                votedOff = team.members[0];
                votes.Add(votedOff);
                votesRead.Add(votedOff);
                AddVote(votes, votesRead);
                vote = "Auto-Elimination";
                etext = "Sorry, " + votedOff.nickname + ", you're the only castaway who is vulnerable so you're automatically eliminated.";
                if(team.members[0].votes < team.members[1].votes)
                {
                    etext = "Sorry, " + votedOff.nickname + ", you don't have the majority of votes so you're automatically eliminated.";
                }
                Eliminate();
            } else
            {
                List<Contestant> teamV = new List<Contestant>(team.members);
                foreach(Contestant num in immune)
                {
                    teamV.Remove(num);
                }
                if(teamV.Count == 1)
                {
                    votes = new List<Contestant>();
                    votesRead = new List<Contestant>();
                    votedOff = team.members[0];
                    votes.Add(votedOff);
                    votesRead.Add(votedOff);
                    AddVote(votes, votesRead);
                    vote = "Auto-Elimination";
                    etext = "Sorry, " + votedOff.nickname + ", you're the only castaway vulnerable so you're automatically eliminated.";
                    Eliminate();
                } 
                else
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
            }
            */
}
