using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newContestant", menuName = "Contestant")]
public class Contestant : ScriptableObject
{
    //Contains the info for a contestant
    public string fullname;
    public string nickname;
    public string gender;
    public string team;
    public Sprite image;
    public Contestant vote;
    public List<Contestant> altVotes = new List<Contestant>();
    public int votesGotten;
    public int immunityWins;
    public int rewardWins;
    public List<Advantage> advantages;
    public int Age;
    public string voteReason;
    public string placement;
    public bool inTie;
    public List<Color> teams = new List<Color>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
