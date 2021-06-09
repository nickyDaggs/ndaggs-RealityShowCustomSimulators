﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SeasonParts
{
    [System.Serializable]
    public class Alliance
    {
        public string name;
        public List<string> teams = new List<string>();
        public List<Contestant> members = new List<Contestant>();
        public Contestant target;
        public List<Contestant> altTargets = new List<Contestant>();
    }
    [System.Serializable]
    public class Team
    {
        public string name;
        public Color tribeColor;
        public List<Contestant> members = new List<Contestant>();
        public bool remove;
        //public List<Alliance> alliances = new List<Alliance>();
    }
    [System.Serializable]
    public class EpisodeSetting
    {
        public string name;
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
        public string type;
        public List<Team> newTribes;
        public string text;
        public bool ResizeTribes;
        public float numberSwap;
        public string leaderReason;
        public string pickingRules;
        public Exile exileIsland;
        public bool exile;
        public bool redIs;
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
        public bool ownTribe;
        public string exileEvent;
        public string challenge;
        public bool skipTribal;
        public bool two;
    }
    [System.Serializable]
    public class Twist
    {
        public int expireAt;
        public List<int> epsSkip = new List<int>();
        public List<int> epsSpecialE = new List<int>();
        public List<int> epsSkipRI = new List<int>();
        public List<int> epsSpecialRI = new List<int>();
        public List<Exile> SpecialEx = new List<Exile>();
        public Exile preMergeEIsland;
        public Exile MergeEIsland;
    }
    [System.Serializable]
    public class Page
    {
        public GameObject obj;
        public List<Contestant> Vote;
        public List<Contestant> VotesRead;
        public List<GameObject> VoteObjs = new List<GameObject>();
    }
    [System.Serializable]
    public class Advantage
    {
        public string name;
        public string type;
        public int expiresAt;
        public int length;
        public bool temp;
        public string usedWhen;
    }
}
public class SimulatorParts : MonoBehaviour
{

}