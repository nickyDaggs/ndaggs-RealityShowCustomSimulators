using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeasonParts; 

public class ChallengeScript : MonoBehaviour
{
    private static ChallengeScript instance;
    public static ChallengeScript Instance { get { return instance; } }
    // Start is called before the first frame update
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else if(instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void TribeChallenge(List<Team> tribes, List<string> stats)
    {

    }

    public void IndividualChallenge(Team tribe, List<string> stats)
    {

    }
    private int GetPoints(Contestant num, string stat)
    {
        return 0;
    }
}
