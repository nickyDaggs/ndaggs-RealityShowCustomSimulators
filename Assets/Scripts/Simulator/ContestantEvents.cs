using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeasonParts;
using System.Linq;

public class ContestantEvents : MonoBehaviour
{
    private static ContestantEvents instance;
    public static ContestantEvents Instance { get { return instance; } }
    public bool join = false;
    public List<ContestantEvent> allianceEvents;
    public List<ContestantEvent> staminaEvents;
    public List<ContestantEvent> relationshipEvents;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void EventsChances(Team team, GameObject Status)
    {
        bool eventt = false;

        int eventCap = 0;

        int eventsNum = Random.Range(1, 6);
        if (eventsNum == 4)
        {
            eventsNum += Random.Range(0, 3);
        }
        if (GameManager.Instance.merged)
        {
            eventsNum = Random.Range(2, 6);
            if (eventsNum == 5)
            {
                eventsNum += Random.Range(0, 2);
            }
        }

        /*if(OW)
        {
            eventCap = -1;
        }*/


        if (eventsNum > 0)
        {
            int eventTimes = 10;
            List<Contestant> tribe = new List<Contestant>(team.members);
            bool waht = false;
            foreach (ContestantEvent Event in allianceEvents)
            {
                waht = false;
                eventTimes = 10;
                switch (Event.allianceEvent)
                {
                    case AllianceEventType.Create:
                        eventTimes = 1;
                        List<Alliance> AddAlliance = new List<Alliance>();
                        tribe = tribe.OrderByDescending(x => ChallengeScript.Instance.GetPoints(x, Event.stats)).ToList();
                        foreach (Contestant num in tribe)
                        {
                            waht = false;
                            ContestantEvents.Instance.join = false;
                            Alliance newAlliance = new Alliance();
                            newAlliance.members.Add(num);
                            num.Relationships = num.Relationships.OrderByDescending(x => x.Type).ThenByDescending(x => (int)x.Status * 10 + x.Extra).Where(x => tribe.Contains(x.person)).ToList();
                            foreach (Relationship re in num.Relationships)
                            {
                                if (team.members.Contains(re.person) && !newAlliance.members.Contains(re.person) && re.person != num && Random.Range(0, newAlliance.members.Count - 1) == 0)
                                {
                                    int stat = ContestantEvents.Instance.GetLoyalty(num, new List<Contestant>() { re.person });
                                    if (Random.Range(1, 11) <= stat)
                                    {
                                        newAlliance.members.Add(re.person);
                                    }
                                }
                            }
                            if (eventCap < eventsNum && Random.Range(0, 15 + eventCap) == 0 && newAlliance.members.Count > 1 && AddAlliance.Count < 3 && Random.Range(0, team.allianceCount + 1) == 0)
                            {
                                if (AddAlliance.Count > 0)
                                {

                                }

                                waht = true;
                            }
                            Contestant main = new Contestant();
                            foreach (Alliance alliance in GameManager.Instance.Alliances)
                            {
                                if (alliance.members.All(newAlliance.members.Contains) && alliance.members.Count == newAlliance.members.Count)
                                {
                                    waht = false;
                                }
                                else if (alliance.members.All(newAlliance.members.Contains) && newAlliance.members.Count == alliance.members.Count + 1)
                                {
                                    foreach (Alliance all in GameManager.Instance.Alliances)
                                    {
                                        if (all.members.All(newAlliance.members.Contains) && all.members.Count == newAlliance.members.Count && all != newAlliance)
                                        {
                                            waht = false;
                                        }
                                    }
                                    ContestantEvents.Instance.join = true;
                                    newAlliance.members = newAlliance.members.OrderByDescending(x => alliance.members.Contains(x)).ToList();
                                    main = newAlliance.members[newAlliance.members.Count - 1];
                                    alliance.members = newAlliance.members;
                                    newAlliance.name = alliance.name;
                                }
                            }
                            if (ContestantEvents.Instance.EventChance(Event, newAlliance.members, num) == true && waht)
                            {
                                if (!ContestantEvents.Instance.join)
                                {
                                    //Debug.Log("ALLIANCE");
                                    AddAlliance.Add(newAlliance);
                                    team.allianceCount++;
                                    if (GameManager.Instance.OW)
                                    {
                                        newAlliance.name = "Alliance #" + team.allianceCount;
                                    }
                                    else
                                    {
                                        newAlliance.name = team.name + " Alliance #" + team.allianceCount;

                                    }
                                }
                                //Debug.Log("Episode: " + curEp);
                                if (!eventt)
                                {
                                    GameManager.Instance.MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), Status.transform.GetChild(0).GetChild(0), -10);
                                    eventt = true;
                                }
                                ContestantEvents.Instance.DoEvent(Event, newAlliance.members, newAlliance, main, Status.transform.GetChild(0).GetChild(0));
                                //Debug.Log(eventTimes < 3);//Debug.Log(eventTimes);
                                eventTimes += eventTimes; eventCap++;
                            }
                        }
                        foreach (Alliance alliance in AddAlliance)
                        {
                            alliance.teams.Add(team.name);
                            GameManager.Instance.Alliances.Add(alliance);
                        }
                        break;
                    case AllianceEventType.Dissolve:
                        List<Alliance> RemoveAlliance = new List<Alliance>();
                        foreach (Alliance alliance in GameManager.Instance.Alliances)
                        {
                            if (alliance.teams.Contains(team.name))
                            {
                                if (eventCap < eventsNum)
                                {
                                    waht = true;
                                }
                                if (ContestantEvents.Instance.EventChance(Event, alliance.members, null) == true && Random.Range(0, eventTimes) == 0 && waht)
                                {
                                    if (!eventt)
                                    {
                                        GameManager.Instance.MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), Status.transform.GetChild(0).GetChild(0), -10);
                                        eventt = true;
                                    }
                                    ContestantEvents.Instance.DoEvent(Event, alliance.members, alliance, new Contestant(), Status.transform.GetChild(0).GetChild(0));
                                    eventTimes += eventTimes; eventCap++;
                                    RemoveAlliance.Add(alliance);
                                }
                            }
                        }
                        foreach (Alliance ads in RemoveAlliance)
                        {
                            GameManager.Instance.Alliances.Remove(ads);
                        }
                        break;
                    case AllianceEventType.Leave:
                        foreach (Alliance alliance in GameManager.Instance.Alliances)
                        {
                            if (alliance.teams.Contains(team.name))
                            {
                                List<Contestant> remove = new List<Contestant>();
                                bool removed = false;
                                foreach (Contestant num in alliance.members)
                                {
                                    if (eventCap < 7 && Random.Range(0, 5 + eventCap) == 0)
                                    {
                                        waht = true;
                                    }
                                    if (ContestantEvents.Instance.EventChance(Event, alliance.members, num) == true && Random.Range(0, eventTimes) == 0 && waht & !removed)
                                    {
                                        if (!eventt)
                                        {
                                            GameManager.Instance.MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), Status.transform.GetChild(0).GetChild(0), -10);
                                            eventt = true;
                                        }
                                        ContestantEvents.Instance.DoEvent(Event, alliance.members, alliance, num, Status.transform.GetChild(0).GetChild(0));
                                        eventTimes += eventTimes; eventCap++;
                                        remove.Add(num);
                                        removed = true;
                                    }
                                }
                                foreach (Contestant num in remove)
                                {
                                    alliance.members.Remove(num);
                                }
                            }
                        }
                        break;
                }
            }
            foreach (ContestantEvent Event in staminaEvents)
            {
                waht = false;
                eventTimes = 3;
                if (Event.staminaAffect > 0)
                {
                    tribe = tribe.OrderBy(x => x.stats.Stamina).ToList();
                }
                else
                {
                    tribe = tribe.OrderByDescending(x => x.stats.Stamina).ToList();
                }
                foreach (Contestant num in tribe)
                {
                    waht = false;
                    if (eventCap < eventsNum)
                    {
                        waht = true;
                    }
                    //&& !Event.type.Contains("Alliance")
                    //bool what = ContestantEvents.Instance.EventChance(Event, null, num);
                    if (EventChance(Event, new List<Contestant>(), num) == true && Random.Range(0, eventTimes) == 0 && eventCap < eventsNum && waht)
                    {
                        if (!eventt)
                        {
                            GameManager.Instance.MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), Status.transform.GetChild(0).GetChild(0), -10);
                            eventt = true;
                        }
                        DoEvent(Event, null, null, num, Status.transform.GetChild(0).GetChild(0));
                        eventTimes += eventTimes;
                        eventCap++;
                    }
                }
            }
            foreach (ContestantEvent Event in relationshipEvents)
            {
                waht = false;
                eventTimes = 3;
                if (Event.relationshipAffect > 0)
                {
                    tribe = tribe.OrderBy(x => ChallengeScript.Instance.GetPoints(x, Event.stats)).ToList();
                }
                else
                {
                    tribe = tribe.OrderByDescending(x => ChallengeScript.Instance.GetPoints(x, Event.stats)).ToList();
                }
                //eventTimes = 50;
                if (Event.overall)
                {
                    foreach (Contestant num in tribe)
                    {
                        waht = false;
                        if (eventCap < eventsNum)
                        {
                            waht = true;
                        }
                        //&& !Event.type.Contains("Alliance")
                        //bool what = ContestantEvents.Instance.EventChance(Event, null, num);

                        if (ContestantEvents.Instance.EventChance(Event, new List<Contestant>(), num) == true && Random.Range(0, eventTimes) == 0 && waht)
                        {
                            if (!eventt)
                            {
                                GameManager.Instance.MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), Status.transform.GetChild(0).GetChild(0), -10);
                                eventt = true;
                            }
                            ContestantEvents.Instance.DoEvent(Event, tribe, null, num, Status.transform.GetChild(0).GetChild(0));
                            eventTimes += 100;
                            eventCap++;
                        }
                    }
                }
                else
                {
                    foreach (Contestant num in tribe)
                    {
                        waht = false;
                        if (eventCap < eventsNum)
                        {
                            waht = true;
                        }
                        if (Event.relationshipAffect < 0)
                        {
                            num.Relationships = num.Relationships.OrderBy(x => x.Type).ThenByDescending(x => (int)x.Status * 10 + x.Extra).Where(x => tribe.Contains(x.person)).ToList();
                        }
                        else
                        {
                            num.Relationships = num.Relationships.OrderByDescending(x => x.Type).ThenByDescending(x => (int)x.Status * 10 + x.Extra).Where(x => tribe.Contains(x.person)).ToList();
                        }
                        foreach (Relationship re in num.Relationships)
                        {
                            if (ContestantEvents.Instance.EventChance(Event, new List<Contestant>(), num) == true && Random.Range(0, eventTimes) == 0 && waht && tribe.Contains(re.person))
                            {
                                if (!eventt)
                                {
                                    GameManager.Instance.MakeGroup(false, null, "", "<b>Events</b>", "", new List<Contestant>(), Status.transform.GetChild(0).GetChild(0), -10);
                                    eventt = true;
                                }
                                ContestantEvents.Instance.DoEvent(Event, new List<Contestant>() { re.person }, null, num, Status.transform.GetChild(0).GetChild(0));
                                eventTimes += 100;
                                eventCap++;
                            }
                        }
                    }
                }

            }
        }

        if (!eventt)
        {
            GameManager.Instance.MakeGroup(false, null, "", "No events occured.", "", new List<Contestant>(), Status.transform.GetChild(0).GetChild(0), -10);
        }
        List<Alliance> removee = new List<Alliance>();
        foreach (Alliance alliance in GameManager.Instance.Alliances)
        {
            if (alliance.members.Count < 2)
            {
                removee.Add(alliance);
            }
            foreach (Alliance all in GameManager.Instance.Alliances)
            {
                if (!all.members.Except(alliance.members).Any() && !alliance.members.Except(all.members).Any() && all.members.Count == alliance.members.Count && all != alliance)
                {
                    if (!removee.Contains(alliance))
                    {
                        alliance.name += "-" + all.name;
                        removee.Add(all);
                    }
                }
            }
        }
        foreach (Alliance alliance in removee)
        {
            GameManager.Instance.Alliances.Remove(alliance);
        }
    }

    public void DoEvent(ContestantEvent Event, List<Contestant> nums, Alliance alliance, Contestant main, Transform parent)
    {
        if (GameManager.instance.Episodes[GameManager.instance.curEp].merged)
        {
            //Debug.Log("Merge");
        } else
        {
            //Debug.Log("Pre-Merge");
        }
        string text = "";
        text = Event.eventText.Replace("Player1", main.nickname);
        switch (Event.type)
        {
            case EventType.Relationship:

                bool lasting = false;
                foreach (Relationship re in main.Relationships)
                {
                    if (nums.Contains(re.person) && re.person != main)
                    {
                        if (re.Type == RelationshipType.Dislike)
                        {
                            if (Event.relationshipAffect < 0)
                            {
                                re.Extra -= Event.relationshipAffect;
                            } else
                            {
                                re.Extra += Event.relationshipAffect;
                            }
                        } else if (re.Type == RelationshipType.Like)
                        {
                            re.Extra += Event.relationshipAffect;
                        }
                        else if (re.Type == RelationshipType.Neutral)
                        {
                            if (Event.relationshipAffect < 0)
                            {
                                re.Type--;
                            }
                            else
                            {
                                re.Type++;
                            }
                            re.Extra += Event.relationshipAffect;
                        }

                        if (re.Extra >= 10)
                        {
                            if ((int)re.Status < 4)
                            {
                                re.Status++;
                                re.Extra = re.Extra - 10;
                            }
                            else
                            {
                                re.Extra = 9;
                            }
                        }
                        else if (re.Extra < 0)
                        {
                            if (re.Status > 0)
                            {
                                re.Status--;
                                re.Extra = 10 + re.Extra;
                            }
                            else
                            {
                                re.changeChance = 10;
                                re.Type = RelationshipType.Neutral;
                                re.Extra = 0;
                            }
                        }
                        int e = Event.relationshipAffect;
                        if (Mathf.Abs(Event.relationshipAffect) > 15)
                        {
                            e = 15;
                        }
                        if (Mathf.Abs(e) <= Random.Range(1, 21) && (Event.relationshipAffect < 0 && re.Type == RelationshipType.Dislike || Event.relationshipAffect > 0 && re.Type == RelationshipType.Like))
                        {
                            re.changeChance += 5;
                            lasting = true;
                        }
                    }
                }
                if (Event.overall)
                {
                    GameManager.Instance.MakeGroup(false, null, "", "", text, new List<Contestant>() { main }, parent, 0);
                }
                else
                {
                    
                    nums.Add(main);
                    
                    if(nums.Count > 2)
                    {
                        text = Event.eventText.Replace("Player1", string.Join(", ", nums.ConvertAll(x => x.nickname))).Replace(", " + nums[0].nickname, " and " + nums[0].nickname);
                    } else
                    {
                        text = Event.eventText.Replace("Player1", string.Join(" ", nums.ConvertAll(x => x.nickname))).Replace(nums[nums.Count - 1].nickname, "and " + nums[nums.Count - 1].nickname);
                    }
                    if(lasting)
                    {
                        text += "\n\nIt has lasting impact";
                    }
                    nums.Reverse();
                    GameManager.Instance.MakeGroup(false, null, "", "", text, nums, parent, 0);
                }
                break;
            case EventType.Alliance:
                string strength = " ("+ Mathf.Round((float)alliance.members.ConvertAll(x => GetLoyalty(x, alliance.members)).Average()) + " Strength)";
                if(Mathf.Round((float)alliance.members.ConvertAll(x => GetLoyalty(x, alliance.members)).Average()) < 1)
                {
                    strength = " (1 Strength)";
                }
                text = Event.eventText.Replace("Alliance", alliance.name);
                if (Event.allianceEvent == AllianceEventType.Create)
                {
                    //Debug.Log("Gaming" + (GameManager.instance.curEp + 1));
                }
                GameManager.instance.all = true;
                if (Event.allianceEvent == AllianceEventType.Leave || join)
                {
                    if(join)
                    {
                        text = main.nickname + " joins the alliance.";
                        //Debug.Log(Event.allianceEvent);
                        GameManager.Instance.MakeGroup(false, null, "name", "<b>" + alliance.name + strength + "</b>", "", nums.Except(new List<Contestant>() { main}).ToList(), parent, 0);
                        //Debug.Log("Join" + (GameManager.instance.curEp + 1));
                    }  else
                    {
                        text = Event.eventText.Replace("Player1", main.nickname);
                        GameManager.Instance.MakeGroup(false, null, "name", "<b>" + alliance.name + strength + "</b>", "", nums, parent, 0);
                    }
                    GameManager.instance.all = false;
                    GameManager.Instance.MakeGroup(false, null, "", "", text, new List<Contestant>() { main}, parent, 0);
                    
                } else
                {
                    GameManager.Instance.MakeGroup(false, null, "name", "<b>" + alliance.name + strength + "</b>", text, nums, parent, 0);
                }
                GameManager.instance.all = false;
                break;
            case EventType.Stamina:
                
                if (Event.overall)
                {
                    foreach (Contestant num in nums)
                    {
                        num.stats.Stamina += Event.staminaAffect;
                    }
                } else
                {
                    //Debug.Log("Stat:");
                    
                    GameManager.Instance.MakeGroup(false, null, "", "", text, new List<Contestant>() { main}, parent, 0);
                    main.stats.Stamina += Event.staminaAffect;
                }
                break;
        }
        join = false;
    }
    public bool EventChance(ContestantEvent Event, List<Contestant> cons, Contestant main)
    {
        bool chance = false;
        int stat = 0;
        if(main != null && Event.stats.Count > 0)
        {
            stat = ChallengeScript.Instance.GetPoints(main, Event.stats);
        }
        int st = 6 - stat;
        int sum = 0;
        switch (Event.type)
        {
            case EventType.Relationship:
                // && stat <= Event.limit
                if (Event.relationshipAffect > 0)
                {
                    if (stat <= Random.Range(1, 7) )
                    {
                        chance = true;
                    }
                }
                else
                {
                    if (st <= Random.Range(1, 7))
                    {
                        chance = true;
                    }
                }
                
                break;
            case EventType.Alliance:
                
                if (Event.allianceEvent == AllianceEventType.Create || Event.allianceEvent == AllianceEventType.Dissolve)
                {
                    sum = cons.ConvertAll(x => GetLoyalty(x, cons)).Sum();

                    int overall = 10 * cons.Count;
                    
                    if(Event.allianceEvent == AllianceEventType.Dissolve)
                    {
                        sum = overall - sum;
                        if (Random.Range(1, overall + 1) <= sum)
                        {
                            chance = true;
                        }
                    } else
                    {
                        
                        overall = 2 * cons.Count;
                        foreach (Contestant num in cons)
                        {
                            if(GetLoyalty(num, cons) == 5)
                            {
                                //sum -= 1;
                            } else if (GetLoyalty(num, cons) < 5)
                            {
                                sum -= 1;
                            } else if (GetLoyalty(num, cons) > 5)
                            {
                                sum += 1;
                            }
                        }
                        if (Random.Range(1, overall + 1) <= sum && Random.Range(1, 7) <= stat)
                        {
                            chance = true;
                        }
                    }
                    
                } else
                {
                    if(Event.allianceEvent == AllianceEventType.Leave)
                    {
                        sum = GetLoyalty(main, cons) + stat;
                        if (Random.Range(1, 16) > sum)
                        {
                            chance = true;
                        }
                    }
                }

                break;
            case EventType.Stamina:
                stat = stat * 10;
                if(Event.staminaAffect > 0)
                {
                    if(stat > Random.Range(1, 101) && stat <= Event.limit)
                    {
                        chance = true;
                    }
                } else
                {
                    if (stat < Random.Range(1, 101) && stat >= Event.limit)
                    {
                        chance = true;
                    }
                }
                break;
        }
        return chance;
    }
    public int GetLoyalty(Contestant num, List<Contestant> alliance)
    {
        List<float> statsNeeded = new List<float>();
        foreach(Relationship friend in num.Relationships)
        {
            if (alliance.Contains(friend.person))
            {
                float value = 0;
                if (friend.Type == RelationshipType.Like)
                {
                    value = (50 + (float)friend.Status * 10 + friend.Extra) / 10;
                } else if (friend.Type == RelationshipType.Dislike)
                {
                    value = (50 - (float)friend.Status * 10 + friend.Extra) / 10;
                } else if(friend.Type == RelationshipType.Neutral)
                {
                    value = 5;
                }
                //((float)friend.Status * 10 + friend.Extra) / 10
                statsNeeded.Add(value);
            }
        }
        float numb = 0;
        if (statsNeeded.Count > 0)
        {
            numb = Mathf.Round(statsNeeded.Average());
        }

        /*if (num.stats.Loyalty < 3)
        {
            numb -= 3 - num.stats.Loyalty;
        } else if(num.stats.Loyalty > 3)
        {
            numb += num.stats.Loyalty - 3;
        }*/

        if (numb > 9)
        {
            numb = 9;
        }

        if (numb < 1)
        {
            numb = 1;
        }

        return (int)numb;
    }
    public Contestant PersonalTarget(Contestant main, List<Contestant> tribe)
    {
        List<float> statsNeeded = new List<float>();
        
        List<Contestant> possibleTargets = new List<Contestant>();
        List<Contestant> targets = new List<Contestant>(tribe);
        targets.Remove(main);
        targets = targets.OrderBy(x => main.GetRelationship(x)).ToList();

        return targets[0];
    }
    public void UpdateRelationships(Contestant num, List<Contestant> tribe)
    {
        int stat = ChallengeScript.Instance.GetPoints(num, new List<StatChoice>() { StatChoice.SocialSkills });
        int st = 6 - stat;
        int startRelationship = tribe.Count - num.Relationships.Where(x => x.Type != RelationshipType.Neutral && tribe.Contains(x.person)).ToList().Count;
        /*if(num.Relationships.Count == 0)
        {
            foreach(Team team in GameManager.Instance.Tribes)
            {
                foreach(Contestant con in team.members)
                {
                    if(con != num)
                    num.Relationships.Add(new Relationship() {person=con, Type=RelationshipType.Neutral });
                }
            }
        }*/
        foreach (Relationship re in num.Relationships)
        {
            if(tribe.Contains(re.person) && re.person != num && !re.perm)
            {
                if(re.Type == RelationshipType.Neutral)
                {
                    if (Random.Range(0, startRelationship) == 0)
                    {
                        if (Random.Range(1, 6) <= stat)
                        {
                            re.Type++;
                        }
                        else
                        {
                            re.Type--;
                        }
                        
                        startRelationship++;
                    }
                    else
                    {
                        if(GameManager.Instance.curEp == 0)
                        {
                            startRelationship--;
                        }
                    }
                    re.Extra = Random.Range(0, 6);
                    if (re.person.GetRelationship(num).Type == RelationshipType.Neutral)
                    {
                        re.person.GetRelationship(num).Type = re.Type;
                        re.person.GetRelationship(num).Extra = re.Extra;
                    }
                } else
                {
                    if (re.Type == RelationshipType.Like)
                    {
                        if (Random.Range(1, 7) <= stat)
                        {
                            re.Extra += Random.Range(5, 16);
                        } else
                        {
                            
                            if (Random.Range(1, re.changeChance) <= st && Random.Range(0, 5) == 0)
                            {
                                re.Extra -= 6 - Random.Range(stat, 6);
                            }
                        }
                        re.Extra += Random.Range(0, stat + 1);
                    }
                    else
                    {
                        if (Random.Range(1, 7) <= st)
                        {
                            re.Extra += Random.Range(5, 16);
                        }
                        else
                        {
                            if (Random.Range(1, re.changeChance) <= stat && Random.Range(0, 5) == 0)
                            {
                                re.Extra -= Random.Range(1, stat + 1);
                            }
                        }
                        re.Extra += 6 - Random.Range(stat, 7);
                    }
                    if (re.Extra >= 10)
                    {
                        if((int)re.Status < 4)
                        {
                            re.Status++;
                            re.Extra = re.Extra - 10;
                        } else
                        {
                            re.Extra = 9;
                        }
                    } else if(re.Extra < 0)
                    {
                        if(re.Status > 0)
                        {
                            re.Status--;
                            re.Extra = 10 - re.Extra;
                        } else
                        {
                            re.Type = RelationshipType.Neutral;
                            re.Extra = 0;
                            re.changeChance = 10;
                        }
                    }
                }
                
            } 
        }
    }
}
