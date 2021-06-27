using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newAdvantage", menuName = "Advantage")]
public class Advantage : ScriptableObject
{
    //Contains info for Advantages
    public string nickname;
    public string type;
    public int expiresAt;
    public int length;
    public List<int> onlyUsable;
    public bool temp;
    public bool playOnOthers;
    public string usedWhen;
    public string description;
    public Contestant otherIdol;
}
