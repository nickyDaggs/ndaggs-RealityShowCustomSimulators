using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeasonParts;

public class ContestantEvents : MonoBehaviour
{
    public void DoEvent()
    {
        
    }
    public bool EventChance(ContestantEvent Event)
    {
        switch(Event.type)
        {
            case "Social":
                break;
            case "Alliance":
                break;
            case "Stamina":
                break;
        }
        return true;
    }
}
