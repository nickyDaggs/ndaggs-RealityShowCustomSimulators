using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EventType { Relationship, Stamina, Alliance}
public enum AllianceEventType { Create, Leave, Dissolve }
public enum SwapType { RegularSwap, RegularShuffle, ChallengeDissolve, DissolveLeastMembers, SchoolyardPick, SplitTribes, TribeChiefs, Mutiny, CISchoolyardPick }
public enum StatChoice { Physical, Endurance, Mental, Stamina, SocialSkills, Temperament, Forgivingness, Boldness, Strategic, Loyalty,  Influence, Intuition }
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
        public string logTeam()
        {
            string temp = "";
            foreach(Contestant num in members)
            {
                temp += num + " ";
            }
            return temp;
        }
        //public List<Alliance> alliances = new List<Alliance>();

        public Team()
        {

        }

        public Team(SavedWithinSim.SavedTeam savedTeam)
        {
            name = savedTeam.teamName;
            Color newCol;
            if (ColorUtility.TryParseHtmlString("#" + savedTeam.teamColor, out newCol))
            {
                tribeColor = newCol;
            }
            //List<string> L = new List<string> ( new string[10] );
            members = new List<Contestant>(new Contestant[savedTeam.teamSize]);
        }
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

        public TribeSwap()
        {

        }

        public TribeSwap(SavedWithinSim.SavedSwap swap)
        {
            on = swap.on;
            swapAt = swap.swapAt;
            type = (SwapType)swap.type;
            newTribes = swap.newTribes.ConvertAll(x => new Team(x));
            text = swap.text;
            ResizeTribes = swap.ResizeTribes;
            numberSwap = swap.numberSwap;
            leaderReason = swap.leaderReason;
            pickingRules = swap.pickingRules;
            exileIsland = new Exile(swap.exileIsland);
            exile = swap.exile; redIs = swap.redIs; orderBySize = swap.orderBySize;
        }
    }
    [System.Serializable]
    public class Episode
    {
        public string name;
        public List<Page> events = new List<Page>();
        
        //public List<GameObject> mems = new List<GameObject>();
    }
    [System.Serializable]
    public class OneTimeEvent
    {
        public string type = "";
        public string context = "";
        public int round;
        public int elim;

        public OneTimeEvent()
        {

        }

        public OneTimeEvent(SavedWithinSim.SavedOneTimeEvent ote)
        {
            type = ote.type;
            context = ote.context;
            round = ote.round;
            elim = ote.elim;
        }
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

        public Exile()
        {

        }

        public Exile(SavedWithinSim.SavedEPExile exile)
        {
            on = exile.on;
            reason = exile.reason;
            ownTribe = exile.ownTribe;
            exileEvent = exile.exileEvent;
            challenge = exile.challenge;
            skipTribal = exile.skipTribal;
            two = exile.two;
            both = exile.both;
        }
    }
    [System.Serializable]
    public class Twist
    {
        public int expireAt;
        public string expires;
        public List<int> epsSkipE = new List<int>();
        public List<int> epsSpecialE = new List<int>();
        public List<int> epsSkipRI = new List<int>();
        public List<int> epsSpecialRI = new List<int>();
        public List<Exile> SpecialEx = new List<Exile>();
        public Exile preMergeEIsland;
        public Exile MergeEIsland;
        public Team IOI;
        public List<HiddenAdvantage> EOEAdvantages = new List<HiddenAdvantage>();

        public Twist()
        {

        }

        public Twist(SavedWithinSim.SavedTwist twists)
        {
            expireAt = twists.expireAt;
            expires = twists.expires;
            epsSkipE = twists.epsSkipE;
            epsSpecialE = twists.epsSpecialE;
            epsSkipRI = twists.epsSkipRI;
            epsSpecialRI = twists.epsSpecialRI;

            SpecialEx = twists.SpecialEx.ConvertAll(x => new Exile(x));
            preMergeEIsland = new Exile(twists.preMergeEIsland);
            MergeEIsland = new Exile(twists.MergeEIsland);

            EOEAdvantages = twists.EOEAdvantages.ConvertAll(x => new HiddenAdvantage(x));
            IOI = new Team(twists.IOI);

        }

    }
    [System.Serializable]
    public class Page
    {
        public GameObject obj;
        public List<Contestant> Vote = new List<Contestant>();
        public List<Contestant> VotesRead = new List<Contestant>();
        public List<Contestant> Idols = new List<Contestant>();
        public List<GameObject> VoteObjs = new List<GameObject>();
        public string elim;
        public string voteCount;
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
        public List<HiddenAdvantage> options = new List<HiddenAdvantage>();
        public bool IOISweetened;
        public int hiddenChance = 25;
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public HiddenAdvantage()
        {

        }

        public HiddenAdvantage(SavedWithinSim.SavedHiddenAdvantage adv)
        {
            name = adv.name;

            //set advantage
            Advantage copyAdvantage = new Advantage();
            copyAdvantage.nickname = adv.advantageType.nickname;
            copyAdvantage.type = adv.advantageType.type;
            copyAdvantage.expiresAt = adv.advantageType.expiresAt;
            copyAdvantage.length = adv.advantageType.length;
            copyAdvantage.onlyUsable = adv.advantageType.onlyUsable;
            copyAdvantage.temp = adv.advantageType.temp;
            copyAdvantage.playOnOthers = adv.advantageType.playOnOthers;
            copyAdvantage.usedWhen = adv.advantageType.usedWhen;
            copyAdvantage.description = adv.advantageType.description;
            copyAdvantage.activate = adv.advantageType.activate;
            copyAdvantage.activated = adv.advantageType.activated;
            advantage = copyAdvantage;
            //set advantage

            hideAt = adv.hideAt;
            reHidden = adv.reHidden;
            hidden = adv.hidden;
            linkedToExile = adv.linkedToExile;
            length = adv.length;
            temp = adv.temp;
            giveAway = adv.giveAway;

            IOILesson = adv.IOILesson;
            options = adv.options.ConvertAll(x => new HiddenAdvantage(x));
            IOISweetened = adv.IOISweetened;
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
    
}
