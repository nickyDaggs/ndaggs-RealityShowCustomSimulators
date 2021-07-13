using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeasonParts;
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
    
}
