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
    public bool temp;
    public bool playOnOthers;
    public string usedWhen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
