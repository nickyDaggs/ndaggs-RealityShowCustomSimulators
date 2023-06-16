using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeasonParts;
[CreateAssetMenu(fileName = "newSeason", menuName = "Season")]
public class Season : ScriptableObject
{
    public SeasonTemplate template;
    public List<Episode> Episodes;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
