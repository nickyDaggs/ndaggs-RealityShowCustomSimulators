using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EulerCalculator : MonoBehaviour
{
    public float deltaX;
    public float initalValue;
    public int subdivisions;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < subdivisions; i++)
        {
            float change = (initalValue * initalValue) * deltaX;
            initalValue += change;
            Debug.Log("sub = " + (i*deltaX) + ", New Y:" + initalValue + ", Change:" + change);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
