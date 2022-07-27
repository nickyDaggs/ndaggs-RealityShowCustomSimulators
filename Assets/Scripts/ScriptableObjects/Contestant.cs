using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeasonParts;
using System.Linq;
[CreateAssetMenu(fileName = "newContestant", menuName = "Contestant")]
public class Contestant : ScriptableObject
{
    [Header("Editable")]
    //Contains the info for a contestant
    public string fullname;
    public string nickname;
    public string gender;
    public string team;
    public int Age;
    public Sprite image;
    public Stats stats;
    public List<Relationship> Relationships;
    [Header("Viewable")]
    public int votesGotten;
    public int immunityWins;
    public int rewardWins;
    public string voteReason;
    public string placement;
    [Header("Simulation Only")]
    public Contestant target;
    public int targetValue;
    public List<Contestant> altVotes = new List<Contestant>();
    public List<Contestant> halfIdols = new List<Contestant>();
    public bool inTie;
    public bool challengeAdvantage;
    public bool combineIdol;
    public int safety;
    public int votes;
    public string IOIEvent;
    public List<Advantage> advantages;
    public List<Color> teams = new List<Color>();
    public HiddenAdvantage savedAdv;
    public Relationship GetRelationship(Contestant num)
    {
        foreach (Relationship friend in Relationships)
        {
            if(friend.person = num)
            return friend;
        }
        return new Relationship();
    }

    public Contestant PersonalTarget(List<Contestant> tribe)
    {
        List<float> statsNeeded = new List<float>();
        //List<Contestant> possibleTargets = new List<Contestant>();
        List<Contestant> targets = new List<Contestant>(tribe);
        if(tribe.Contains(this))
        {
            //Debug.Log("Targets:" + string.Join(",", targets.Select(x => x.nickname)));
        }
        targets.Remove(this);
        foreach (Alliance alliance in GameManager.Instance.Alliances)
        {
            int intersect = tribe.Intersect(alliance.members).ToList().Count;
            if (alliance.members.Contains(this) && intersect > 1 && intersect != tribe.Count)
            {
                foreach(Contestant num in alliance.members)
                {
                    targets.Remove(num);
                }
            }
        }
        if(targets.Count < 1)
        {
            //Debug.Log("fadfssffsasfa");
            targets = new List<Contestant>(tribe);
            targets.Remove(this);
            if(targets.Count < 1)
            {
                Debug.Log(this);
            }
        }
        
        targets = targets.OrderByDescending(x => value(x)).ToList();
        //Debug.Log("Episode:" + GameManager.Instance.curEp+"targetSize:"+ targets.Count+"TribeSize:"+tribe.Count);
        target = targets[0];
        targetValue = value(targets[0]);
        if(targets[0] == null)
        {
            Debug.Log("aaaaaa");
        }
        return targets[0];
        
    }

    public Contestant MakeVote(List<Contestant> targets, List<Contestant> team)
    {
        if(targets.Contains(this))
        {
            //Debug.Log("Targets:" + string.Join(",", targets.Select(x => x.nickname)));
        }
        targets.Remove(this);
        if (targets.Count < 1)
        {
            //Debug.Log("Targets:" + string.Join(",", targets.Select(x => x.nickname)));
        }

        voteReason = "They voted their personal target.";
        if ( Random.Range(1, 101) <= targetValue && Random.Range(1, 6) > stats.Strategic && !targets.Contains(target))
        {
            if (target == null)
            {
                Debug.Log("AAAAAA");
            }
            return target;
        }
        
        target = targets.OrderByDescending(x => value(x)).ToList()[0];

        targetValue = value(target);

        List<Alliance> alliances = new List<Alliance>(GameManager.Instance.Alliances);
        alliances = alliances.OrderByDescending(x => ContestantEvents.Instance.GetLoyalty(this, x.members)).ToList();

        foreach (Alliance alliance in GameManager.Instance.Alliances)
        {
            int intersect = team.Except(GameManager.Instance.immune).Intersect(alliance.members).ToList().Count;
            if (alliance.members.Contains(this) && intersect > 1 && intersect != team.Except(GameManager.Instance.immune).ToList().Count)
            {
                int loyalty = ContestantEvents.Instance.GetLoyalty(this, alliance.members);
                if(stats.Loyalty < 3)
                {
                    loyalty -= 3 - stats.Loyalty;
                } else if(stats.Loyalty > 3)
                {
                    loyalty += stats.Loyalty - 3;
                }
                if(loyalty < 1)
                {
                    loyalty = 1;
                } else if (loyalty > 9)
                {
                    loyalty = 9;
                }
                //&& Random.Range(1, 7) <= 6 - stats.Boldness
                if ((targets.Count > 1 && Random.Range(1, 11) <= loyalty) || alliance.mainTargets.Contains(target) || (alliance.altTargets.Contains(target) && alliance.splitVoters.Contains(this)))
                {
                    //Debug.Log("gggggggg");
                    voteReason = "They voted with " + alliance.name;
                    if (alliance.mainTargets.Find(x => team.Contains(x)) == null)
                    {
                        Debug.Log("Episode:" +(GameManager.instance.curEp + 1) +alliance.name);
                    } else
                    {
                        if(alliance.splitVoters.Contains(this))
                        {
                            return alliance.altTargets.Find(x => team.Contains(x));
                        } else
                        {
                            return alliance.mainTargets.Find(x => team.Contains(x));
                        }
                    }
                }
            }
            //targets.Remove(alliance.mainTargets.Find(x => team.Contains(x)));
        }
        
        Debug.Assert(!targets.Contains(this), "Contains contestant");
        if (target == null)
        {
            Debug.Log("AAAAAA");
        }
        
        voteReason = "They voted based on personal preference.";

        targets = targets.OrderByDescending(x => value(x)).ToList();

        foreach(Contestant targett in targets)
        {
            if (Random.Range(1, 101) <= value(targett))
            {
                return targett;
            }
        }
        //Debug.Log("Target");
        return target;
    }

    public int value(Contestant num)
    {
        int challengeStats = ChallengeScript.Instance.GetPoints(num, new List<StatChoice>() { StatChoice.Endurance, StatChoice.Mental, StatChoice.Physical, StatChoice.Stamina });
        int v = 50;
        if (GetRelationship(num).Type == RelationshipType.Dislike)
        {
            v += (int)GetRelationship(num).Status * 10 + GetRelationship(num).Extra;
            if (GetRelationship(num).Status >= RelationshipStatus.Medium)
            {
                v += 4 - stats.Forgivingness;
            }
        }
        else if (GetRelationship(num).Type == RelationshipType.Like)
        {
            v -= (int)GetRelationship(num).Status * 10 + GetRelationship(num).Extra;
        }
        else if (GetRelationship(num).Type == RelationshipType.Neutral)
        {
            v += Random.Range(0, 4);
        }

        if (GameManager.Instance.MergedTribe.members.Count < 0)
        {
            v += (3 - challengeStats) * 10;
        }
        else
        {
            v += (3 - challengeStats) * 10;
        }

        return v;
    }

    public int goodValue(Contestant num)
    {
        int v = 0;
        if (GetRelationship(num).Type == RelationshipType.Dislike)
        {
            v += (int)GetRelationship(num).Status + GetRelationship(num).Extra / 10;
            if (GetRelationship(num).Status >= RelationshipStatus.Medium)
            {
                v += 4 - stats.Forgivingness;
            }
        }
        else if (GetRelationship(num).Type == RelationshipType.Like)
        {
            v -= 50 - (int)GetRelationship(num).Status + GetRelationship(num).Extra / 10;
        }
        else if (GetRelationship(num).Type == RelationshipType.Neutral)
        {
            v += Random.Range(0, 4);
        }

        return v;
    }

    public int GetPoints(List<StatChoice> Stats)
    {
        List<float> statsNeeded = new List<float>();
        foreach (StatChoice stat in Stats)
        {
            switch (stat)
            {
                case StatChoice.Physical:
                    statsNeeded.Add(stats.Physical);
                    break;
                case StatChoice.Mental:
                    statsNeeded.Add(stats.Mental);
                    break;
                case StatChoice.Endurance:
                    statsNeeded.Add(stats.Endurance);
                    break;
                case StatChoice.SocialSkills:
                    statsNeeded.Add(stats.SocialSkills);
                    break;
                case StatChoice.Temperament:
                    statsNeeded.Add(stats.Temperament);
                    break;
                case StatChoice.Strategic:
                    statsNeeded.Add(stats.Strategic);
                    break;
                case StatChoice.Loyalty:
                    statsNeeded.Add(stats.Loyalty);
                    break;
                case StatChoice.Forgivingness:
                    statsNeeded.Add(stats.Forgivingness);
                    break;
                case StatChoice.Boldness:
                    statsNeeded.Add(stats.Boldness);
                    break;
                case StatChoice.Influence:
                    statsNeeded.Add(stats.Influence);
                    break;
                case StatChoice.Intuition:
                    statsNeeded.Add(stats.Intuition);
                    break;
                case StatChoice.Stamina:
                    statsNeeded.Add(stats.Stamina / 10);
                    break;
            }
        }
        float numb = statsNeeded.Average();

        return (int)Mathf.Round(statsNeeded.Average());
    }
}
