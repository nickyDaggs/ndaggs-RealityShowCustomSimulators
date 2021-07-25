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
    public Contestant vote;
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

        List<Contestant> possibleTargets = new List<Contestant>();
        List<Contestant> targets = new List<Contestant>(tribe);
        targets.Remove(this);
        targets = targets.OrderBy(x => GetRelationship(x).Type).ThenByDescending(x => (int)GetRelationship(x).Status*10 + GetRelationship(x).Extra).ToList();
        return targets[0];
    }
}
