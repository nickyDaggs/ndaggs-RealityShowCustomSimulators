using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeasonParts;
using System.Linq;

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
    public void TribeChallenge(List<Team> tribes, List<StatChoice> stats, int winner)
    {
        stats.Add(StatChoice.Stamina);
        List<Team> Tribes = new List<Team>(tribes);
        Dictionary<Team, int> teamSum = new Dictionary<Team, int>();
        int Overall = 0;
        Tribes = Tribes.OrderBy(x => x.members.Count).ToList();
        foreach (Team tribe in Tribes)
        {
            int sum = 0;
            List<int> sumMembers = new List<int>();
            foreach (Contestant num in tribe.members)
            {
                //Get an average from a player's stats
                sumMembers.Add(GetPoints(num, stats));
                num.stats.Stamina -= 3;
            }
            //Get average of all players in the team
            sum = (int)Mathf.Round((float)sumMembers.Average());
            //+ (float)sumMembers.Average()
            if(GameManager.instance.sea.HavesVsHaveNots)
            {
                if(tribe.environment == Environment.Chaotic)
                {
                    sum = (int)Mathf.Round((float)sum / 2);
                }
            }
            
            teamSum.Add(tribe, Overall + sum);
            Overall += sum;
            
        }
        teamSum = teamSum.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        //Randomize a number then go through each team and see if their average is greater than or equal to the random number
        for (int i = 0; i < winner; i++)
        {
            bool found = false;
            int ran = Random.Range(1, Overall + 1);
            int lastNum = 0;
            foreach (KeyValuePair<Team, int> tribe in teamSum)
            {

                if (!found && Tribes.Contains(tribe.Key))
                {
                    if (lastNum < ran && ran <= tribe.Value)
                    {

                        Tribes.Remove(tribe.Key);

                    }
                    lastNum = tribe.Value;
                }
            }

        }
        //Debug.Log("End");
        
        GameManager.instance.LosingTribes = new List<Team>(Tribes);
    }
    public void IndividualChallenge(Team tribe, List<StatChoice> stats, int winner)
    {
        stats.Add(StatChoice.Stamina);
        Dictionary<Contestant, int> members = new Dictionary<Contestant, int>();
        int Overall = 0; 
        foreach (Contestant num in tribe.members)
        {
            List<int> sumMembers = new List<int>();
            //Get an average from a player's stats and add it to dictionary
            members.Add(num, Overall + GetPoints(num, stats));
            Overall += GetPoints(num, stats);
            num.stats.Stamina -= 3;
        }
        members = members.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        for (int i = 0; i < winner; i++)
        {
            bool found = false;
            int ran = Random.Range(1, Overall);
            int rolls = 0;
            while (!found)
            {
                int lastNum = 0;
                foreach (KeyValuePair<Contestant, int> num in members)
                {
                    if (!found)
                    {
                        if (lastNum < ran && ran <= num.Value)
                        {
                            GameManager.instance.immune.Add(num.Key);
                            num.Key.advantages.Add(GameManager.instance.ImmunityNecklace);
                            //LogStats(num.Key, stats);
                            //Debug.Log("Stat Average:" + GetPoints(num.Key, stats));
                            found = true;
                        }
                        lastNum = num.Value;
                        if (rolls > 10)
                        {
                            lastNum = 0;
                            //Debug.Log("ffff");
                        }
                        ran = Random.Range(1, Overall);
                    }
                }
                rolls++;
            }
        }
    }
    public int GetPoints(Contestant num, List<StatChoice> stats)
    {
        List<float> statsNeeded = new List<float>();
        foreach (StatChoice stat in stats)
        {
            switch (stat)
            {
                case StatChoice.Physical:
                    statsNeeded.Add(num.stats.Physical);
                    break;
                case StatChoice.Mental:
                    statsNeeded.Add(num.stats.Mental);
                    break;
                case StatChoice.Endurance:
                    statsNeeded.Add(num.stats.Endurance);
                    break;
                case StatChoice.SocialSkills:
                    statsNeeded.Add(num.stats.SocialSkills);
                    break;
                case StatChoice.Temperament:
                    statsNeeded.Add(num.stats.Temperament);
                    break;
                case StatChoice.Strategic:
                    statsNeeded.Add(num.stats.Strategic);
                    break;
                case StatChoice.Loyalty:
                    statsNeeded.Add(num.stats.Loyalty);
                    break;
                case StatChoice.Forgivingness:
                    statsNeeded.Add(num.stats.Forgivingness);
                    break;
                case StatChoice.Boldness:
                    statsNeeded.Add(num.stats.Boldness);
                    break;
                case StatChoice.Influence:
                    statsNeeded.Add(num.stats.Influence);
                    break;
                case StatChoice.Intuition:
                    statsNeeded.Add(num.stats.Intuition);
                    break;
                case StatChoice.Stamina:
                    statsNeeded.Add(num.stats.Stamina / 10);
                    break;
            }
        }
        float numb = statsNeeded.Average();

        return (int)Mathf.Round(statsNeeded.Average());
    }
    public void RandomizeStats(Contestant num)
    {
        num.stats.Physical = Random.Range(1, 6);
        num.stats.Endurance = Random.Range(1, 6);
        num.stats.Mental = Random.Range(1, 6);
        num.stats.Stamina = Random.Range(1, 6);
        num.stats.SocialSkills = Random.Range(1, 6);
        num.stats.Temperament = Random.Range(1, 6);
        num.stats.Strategic = Random.Range(1, 6);
        num.stats.Loyalty = Random.Range(1, 6);
        num.stats.Forgivingness = Random.Range(1, 6);
        num.stats.Boldness = Random.Range(1, 6);
        num.stats.Influence = Random.Range(1, 6);
        num.stats.Intuition = Random.Range(1, 6);
    }
    public void LogStats(Contestant num, List<StatChoice> stats)
    {
        string log = "";
        foreach (StatChoice stat in stats)
        {
            switch (stat)
            {
                case StatChoice.Physical:
                    log += "Physical:" + num.stats.Physical + " ";
                    break;
                case StatChoice.Mental:
                    log += "Mental:" + num.stats.Mental + " ";
                    break;
                case StatChoice.Endurance:
                    log += "Endurance:" + num.stats.Endurance + " ";
                    break;
            }
        }
        Debug.Log(log);
    }
}
