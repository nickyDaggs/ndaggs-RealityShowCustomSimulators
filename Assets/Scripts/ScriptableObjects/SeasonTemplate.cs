﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeasonParts;
[CreateAssetMenu(fileName = "newSeasonTemp", menuName = "SeasonTemplate")]
public class SeasonTemplate : ScriptableObject
{
    //Template used to set up what twists and options will be used in a season.
    public string nameSeason;
    public Sprite seasonImage;
    public List<Team> Tribes = new List<Team>();    
    public float mergeAt;
    public float jury;
    public float final;
    public List<Episode> Episodes;
    public string Tiebreaker;
    public string ReturningPlayers;
    public Color MergeTribeColor;
    public string MergeTribeName;
    public List<Challenge> ImmunityChallenges;
    public List<Challenge> RewardChallenges;
    public List<int> rewardSkips;
    public bool NoRewards;
    public List<HiddenAdvantage> mergeHiddenAdvantages;
    public List<HiddenAdvantage> twistHiddenAdvantages;
    public List<TribeSwap> swaps;
    public bool ExileIslandd;
    public string IslandType;
    public List<HiddenAdvantage> islandHiddenAdvantages;
    public bool RedemptionIsland;
    public bool EdgeOfExtinction;
    public bool Outcasts;
    public bool MedallionOfPower;
    public bool OneWorld;
    public bool HavesVsHaveNots;
    public bool advantages;
    public bool forcedFireMaking;
    public bool idolsInPlay;
    public int MOPExpire;
    public int OWExpire;
    public int idolLimit;
    public List<int> twoParts;
    public List<int> threeParts;
    public Twist Twists;
    public List<OneTimeEvent> oneTimeEvents;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
