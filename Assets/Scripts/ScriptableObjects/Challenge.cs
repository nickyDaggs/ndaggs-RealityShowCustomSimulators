using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newChallenge", menuName = "Challenge")]
public class Challenge : ScriptableObject
{
    public string challengeName;
    public string description;
    public List<string> rewards = new List<string>();
    public int rewardStamina;
    public List<Contestant> Groups = new List<Contestant>();
    public bool sitout;
    public List<StatChoice> stats = new List<StatChoice>() { StatChoice.Stamina };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
