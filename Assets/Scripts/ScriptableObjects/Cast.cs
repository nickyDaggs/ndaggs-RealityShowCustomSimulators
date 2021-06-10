using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newCast", menuName = "Cast")]
public class Cast : ScriptableObject
{
    //List of contestants that is inputed into a season. Separate from a season itself so you can use the same cast in different templates
    public string nameS;
    public List<Contestant> cast;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
