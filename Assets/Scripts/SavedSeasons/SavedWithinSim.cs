using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeasonParts;
using System.Linq;

public class SavedWithinSim : MonoBehaviour
{

    // Update is called once per frame
    void SaveSeasonTemplate(SeasonTemplate temp)
    {
        
    }

    public SeasonTemplate LoadSeason(SavedTemplate temp)
    {
        SeasonTemplate load = new SeasonTemplate();

        load.nameSeason = temp.seasonName;

        load.Tribes = temp.tribes.ConvertAll(x => new Team(x));
        load.mergeAt = temp.mergeAt;
        load.jury = temp.juryMembers;
        load.final = temp.final;
        load.Tiebreaker = temp.tiebreaker;
        load.ReturningPlayers = temp.returningPlayers;

        load.MergeTribeName = temp.mergeTribe.teamName;
        Color newCol;
        if (ColorUtility.TryParseHtmlString("#" + temp.mergeTribe.teamColor, out newCol))
        {
            load.MergeTribeColor = newCol;
        }
        //load.mergeTribe.teamColor = ColorUtility.ToHtmlStringRGB(temp.MergeTribeColor);

        load.swaps = temp.swaps.ConvertAll(x => new TribeSwap(x));

        load.ImmunityChallenges = temp.immChallenges.ConvertAll(x => new Challenge() {challengeName=x.challName, description=x.challDescript, stats=x.challStats.ConvertAll(a => (StatChoice)a), rewards = x.rewards, rewardStamina = x.rewardStamina, sitout = x.sitout });

        load.RewardChallenges = temp.reChallenges.ConvertAll(x => new Challenge() { challengeName = x.challName, description = x.challDescript, stats = x.challStats.ConvertAll(a => (StatChoice)a), rewards = x.rewards, rewardStamina = x.rewardStamina, sitout=x.sitout  });
        load.rewardSkips = temp.reSkips;
        load.ExileIslandd = temp.exileIsland; load.RedemptionIsland = temp.redemptionIsland; load.Outcasts = temp.outcasts; load.MedallionOfPower = temp.medallionOfPower;
        load.forcedFireMaking = temp.forcedFireMaking; load.idolsInPlay = temp.idolsInPlay; load.NoRewards = temp.NoRewards;
        load.OneWorld = temp.OneWorld; load.HavesVsHaveNots = temp.HavesVsHaveNots;
        load.MOPExpire = temp.MOPExpire; load.OWExpire = temp.OWExpire; load.idolLimit = temp.idolLimit;
        load.twoParts = temp.twoParts; load.threeParts = temp.threeParts;
        load.IslandType = temp.islandType;

        load.mergeHiddenAdvantages = temp.mergeHiddenAdvantages.ConvertAll(x => new HiddenAdvantage(x));
        load.twistHiddenAdvantages = temp.twistHiddenAdvantages.ConvertAll(x => new HiddenAdvantage(x));
        load.islandHiddenAdvantages = temp.islandHiddenAdvantages.ConvertAll(x => new HiddenAdvantage(x));

        load.Twists = new Twist(temp.twist);

        //finish IOI later
        if (temp.islandType == "IOI")
        {
            for(int i = 0; i < temp.twist.IOI.contestants.Count; i++)
            {
                SimLoader.LoadContestant con = new SimLoader.LoadContestant(temp.twist.IOI.contestants[i]);

                Contestant mentor = new Contestant() {nickname=con.nickname, fullname=con.fullname, image=con.sprite };
                load.Twists.IOI.members.Add(mentor);
            }
        }

        load.oneTimeEvents = temp.oneTimeEvents.ConvertAll(x => new OneTimeEvent(x));

        return load;
    }



    [System.Serializable]
    public class SeasonCast
    {
        List<SimLoader.SavedContestant> cons;

        public SeasonCast(SimLoader.SavedSeason season)
        {
            cons = season.contestants;
        }
    }

    [System.Serializable]
    public class SavedTemplate
    {
        public string seasonName;
        public List<SavedTeam> tribes;
        public float mergeAt;
        public float juryMembers;
        public float final;
        public string tiebreaker;
        public string returningPlayers;
        public SavedTeam mergeTribe = new SavedTeam();
        public List<SavedSwap> swaps;

        public List<SavedChallenge> immChallenges;
        public List<SavedChallenge> reChallenges;
        public List<int> reSkips;


        public bool exileIsland, redemptionIsland, outcasts, medallionOfPower, forcedFireMaking, idolsInPlay, NoRewards, OneWorld, HavesVsHaveNots;
        public int MOPExpire, OWExpire, idolLimit;
        public List<int> twoParts;
        public List<int> threeParts;
        public string islandType;

        public List<SavedHiddenAdvantage> mergeHiddenAdvantages, twistHiddenAdvantages, islandHiddenAdvantages;

        public SavedTwist twist;

        public List<SavedOneTimeEvent> oneTimeEvents;

        //public List<SavedEvent> events;

        public SavedTemplate(SeasonTemplate temp)
        {
            seasonName = temp.nameSeason;

            tribes = temp.Tribes.ConvertAll(x => new SavedTeam(x));
            mergeAt = temp.mergeAt;
            juryMembers = temp.jury;
            final = temp.final;
            tiebreaker = temp.Tiebreaker;
            returningPlayers = temp.ReturningPlayers;

            mergeTribe.teamName = temp.MergeTribeName;
            mergeTribe.teamColor = ColorUtility.ToHtmlStringRGB(temp.MergeTribeColor);

            swaps = temp.swaps.ConvertAll(x => new SavedSwap(x));

            immChallenges = temp.ImmunityChallenges.ConvertAll(x => new SavedChallenge(x));
            reChallenges = temp.RewardChallenges.ConvertAll(x => new SavedChallenge(x));
            reSkips = temp.rewardSkips;
            exileIsland = temp.ExileIslandd; redemptionIsland = temp.RedemptionIsland; outcasts = temp.Outcasts; medallionOfPower = temp.MedallionOfPower; forcedFireMaking = temp.forcedFireMaking; idolsInPlay = temp.idolsInPlay; NoRewards = temp.NoRewards;
            OneWorld = temp.OneWorld; HavesVsHaveNots = temp.HavesVsHaveNots;
            MOPExpire = temp.MOPExpire; OWExpire = temp.OWExpire; idolLimit = temp.idolLimit;
            twoParts = temp.twoParts; threeParts = temp.threeParts;
            islandType = temp.IslandType;

            mergeHiddenAdvantages = temp.mergeHiddenAdvantages.ConvertAll(x => new SavedHiddenAdvantage(x));
            twistHiddenAdvantages = temp.twistHiddenAdvantages.ConvertAll(x => new SavedHiddenAdvantage(x));
            islandHiddenAdvantages = temp.islandHiddenAdvantages.ConvertAll(x => new SavedHiddenAdvantage(x));


            twist = new SavedTwist(temp.Twists);

            oneTimeEvents = temp.oneTimeEvents.ConvertAll(x => new SavedOneTimeEvent(x));

            //mergeColor = ColorUtility.ToHtmlStringRGB(temp.MergeTribeColor);
        }
    }

    public class SavedEpisodeSetting
    {
        public string name;
        public string nickname;
        public float con;
        public bool merged;
        //public SavedSwap swap = new SavedSwap();
        //public SavedEPExile exileIsland = new SavedEPExile();
        public List<string> events = new List<string>();
        public bool elimAllButTwo = false;
        //public SavedOneTimeEvent Event = new SavedOneTimeEvent();
    }

    [System.Serializable]
    public class SavedHiddenAdvantage
    {
        public string name;
        public SavedAdvantage advantageType;
        public int hideAt;
        public bool reHidden;
        public bool hidden;
        public bool linkedToExile;
        public int length;
        public bool temp;
        public bool giveAway;
        public string IOILesson;
        public List<SavedHiddenAdvantage> options;
        public bool IOISweetened;
        public int hiddenChance = 25;

        public SavedHiddenAdvantage(HiddenAdvantage adv)
        {
            name = adv.name;
            advantageType = new SavedAdvantage(adv.advantage);
            hideAt = adv.hideAt;
            reHidden = adv.reHidden;
            hidden = adv.hidden;
            linkedToExile = adv.linkedToExile;
            length = adv.length;
            temp = adv.temp;
            giveAway = adv.giveAway;

            IOILesson = adv.IOILesson;
            options = adv.options.ConvertAll(x => new SavedHiddenAdvantage(x));
            IOISweetened = adv.IOISweetened;

        }
    }

    [System.Serializable]
    public class SavedAdvantage
    {
        public string nickname;
        public string type;
        public int expiresAt;
        public int length;
        public List<int> onlyUsable = new List<int>();
        public bool temp;
        public bool playOnOthers;
        public string usedWhen;
        public string description;
        public string activate = "";
        public bool activated = false;

        public SavedAdvantage(Advantage adv)
        {
            nickname = adv.nickname;
            type = adv.type;
            expiresAt = adv.expiresAt;
            length = adv.length;
            onlyUsable = adv.onlyUsable;
            temp = adv.temp;
            playOnOthers = adv.playOnOthers;
            usedWhen = adv.usedWhen;
            description = adv.description;
            activate = adv.activate;
            activated = adv.activated;
        }
    }

    [System.Serializable]
    public class SavedEPExile
    {
        public bool on = false;
        public string reason;
        public bool ownTribe = false;
        public string exileEvent;
        public string challenge;
        public bool skipTribal = false;
        public bool two = false;
        public bool both = false;

        public SavedEPExile(Exile exile)
        {
            on = true;
            reason = exile.reason;
            ownTribe = exile.ownTribe;
            exileEvent = exile.exileEvent;
            challenge = exile.challenge;
            skipTribal = exile.skipTribal;
            two = exile.two;
            both = exile.both;
        }
    }
    public class SavedExileFormat
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
    public class SavedChallenge
    {
        public string challName;
        public string challDescript;
        public List<int> challStats;
        public List<string> rewards = new List<string>();
        public int rewardStamina;
        public bool sitout;

        public SavedChallenge(Challenge c)
        {
            challName = c.challengeName;
            challDescript = c.description;
            challStats = c.stats.ConvertAll(x => (int)x);
            rewards = c.rewards;
            rewardStamina = c.rewardStamina;
            sitout = c.sitout;
        }
    }

    [System.Serializable]
    public class SavedTeam
    {
        public string teamName;
        public string teamColor;
        public int teamSize;

        public List<SavedHiddenAdvantage> teamAdvantages;

        public List<SimLoader.SavedContestant> contestants;

        public SavedTeam(Team team)
        {
            teamName = team.name;
            teamColor = ColorUtility.ToHtmlStringRGB(team.tribeColor);
            teamSize = team.members.Count;
            teamAdvantages = team.hiddenAdvantages.ConvertAll(x => new SavedHiddenAdvantage(x));
            contestants = new List<SimLoader.SavedContestant>();
        }
        public SavedTeam()
        {
            teamName = "";
            teamColor = "";
            contestants = new List<SimLoader.SavedContestant>();
        }
    }

    [System.Serializable]
    public class SavedTwist
    {
        public int expireAt;
        public string expires;
        public List<int> epsSkipE = new List<int>();
        public List<int> epsSpecialE = new List<int>();
        public List<int> epsSkipRI = new List<int>();
        public List<int> epsSpecialRI = new List<int>();
        public List<SavedEPExile> SpecialEx = new List<SavedEPExile>();
        public SavedEPExile preMergeEIsland;
        public SavedEPExile MergeEIsland;
        public SavedTeam IOI;
        public List<SavedHiddenAdvantage> EOEAdvantages;

        public SavedTwist(Twist twists)
        {
            expireAt = twists.expireAt;
            expires = twists.expires;
            epsSkipE = twists.epsSkipE;
            epsSpecialE = twists.epsSpecialE;
            epsSkipRI = twists.epsSkipRI;
            epsSpecialRI = twists.epsSpecialRI;

            SpecialEx = twists.SpecialEx.ConvertAll(x => new SavedEPExile(x));
            preMergeEIsland = new SavedEPExile(twists.preMergeEIsland);
            MergeEIsland = new SavedEPExile(twists.MergeEIsland);

            EOEAdvantages = twists.EOEAdvantages.ConvertAll(x => new SavedHiddenAdvantage(x));

            if (twists.IOI.members.Count > 0)
            {
                IOI = new SavedTeam(twists.IOI);
                IOI.teamSize = 0;
                List<Texture> textures = GameManager.GetAllInstances<Texture>().ToList();
                for (int i = 0; i < twists.IOI.members.Count; i++)
                {

                    SimLoader.SavedContestant mentor = new SimLoader.SavedContestant { id = twists.IOI.members[i].simID, fullname = twists.IOI.members[i].fullname, nickname = twists.IOI.members[i].nickname };
                    if (!textures.Contains(twists.IOI.members[i].image.texture))
                    {
                        if (twists.IOI.members[i].imageUrl != "")
                        {
                            mentor.spriteUrl = twists.IOI.members[i].imageUrl;
                        }
                    }
                    else
                    {
                        mentor.spritePath = twists.IOI.members[i].season + "/" + twists.IOI.members[i].image.texture.name;
                        //Debug.Log(contestant.spritePath);
                    }
                    IOI.contestants.Add(mentor);
                }
            }
        }
    }

    [System.Serializable]
    public class SavedOneTimeEvent
    {
        public string type = "";
        public string context = "";
        public int round;
        public int elim;

        public SavedOneTimeEvent(OneTimeEvent ote)
        {
            type = ote.type;
            context = ote.context;
            round = ote.round;
            elim = ote.elim;
        }
    }

    [System.Serializable]
    public class SavedSwap
    {
        public bool on = false;
        public float swapAt;
        public int type;
        public List<SavedTeam> newTribes;
        public string text;
        public bool ResizeTribes;
        public float numberSwap;
        public string leaderReason;
        public string pickingRules;
        public SavedEPExile exileIsland;
        public bool exile;
        public bool redIs;
        public bool orderBySize;

        public SavedSwap(TribeSwap swap)
        {
            on = true;
            swapAt = swap.swapAt;
            type = (int)swap.type;
            newTribes = swap.newTribes.ConvertAll(x => new SavedTeam(x));
            text = swap.text;
            ResizeTribes = swap.ResizeTribes;
            numberSwap = swap.numberSwap;
            leaderReason = swap.leaderReason;
            pickingRules = swap.pickingRules;
            exileIsland = new SavedEPExile(swap.exileIsland);
            exile = swap.exile; redIs = swap.redIs; orderBySize = swap.orderBySize;
        } 
    }
}
