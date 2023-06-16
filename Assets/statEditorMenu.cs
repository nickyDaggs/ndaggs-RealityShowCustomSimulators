using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class statEditorMenu : MonoBehaviour
{
    public GameObject mainStats;
    public Transform statParent;
    public Dropdown minDropdown;
    public Dropdown maxDropdown;
    public Dropdown targetDropdown;
    List<Dropdown.OptionData> minOptions;
    List<Dropdown.OptionData> maxOptions;
    [HideInInspector] public int curMin = 0;
    [HideInInspector] public int curMax = 4;
    // Start is called before the first frame update
    public void StartStats()
    {
        for(int i = 0; i < statParent.childCount; i++)
        {
            int num = i;
            statParent.GetChild(i).GetComponentInChildren<Button>().onClick.AddListener(() => randomizeContestant(num));
        }
        minOptions = new List<Dropdown.OptionData>(minDropdown.options);
        maxOptions = new List<Dropdown.OptionData>(maxDropdown.options);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void insertMin(int min)
    {
        curMin = minDropdown.value;

    }

    public void insertMax(int max)
    {
        curMax = maxDropdown.value;
    }

    public void randomizeContestant(int index)
    {
        for (int i = 0; i < 12; i++)
        {
            
            statParent.GetChild(index).GetChild(1).GetChild(i).GetComponent<Dropdown>().value = Random.Range(curMin, curMax + 1);
            //Debug.Log(statParent.GetChild(index).GetChild(1).GetChild(i).GetComponent<Dropdown>().value);
        }
        //Debug.Log((curMin + 1) + " " + (curMax + 1));
    }

    public void setRandom()
    {
        switch(targetDropdown.value)
        {
            default:
                for (int i = 0; i < statParent.childCount; i++)
                {
                    int num = i;
                    statParent.GetChild(i).GetComponentsInChildren<Dropdown>()[targetDropdown.value].value = Random.Range(curMin, curMax + 1);
                }
                break;
            case 12:
                for (int i = 0; i < statParent.childCount; i++)
                {
                    int num = i;
                    randomizeContestant(num);
                }
                break;
            case 13:
                for (int i = 0; i < statParent.childCount; i++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        //Debug.Log(statParent.GetChild(i).GetChild(k));
                        statParent.GetChild(i).GetChild(1).GetChild(k).GetComponent<Dropdown>().value = Random.Range(curMin, curMax + 1);
                        //Debug.Log(statParent.GetChild(index).GetChild(1).GetChild(i).GetComponent<Dropdown>().value);
                    }
                }
                break;
            case 14:
                for (int i = 0; i < statParent.childCount; i++)
                {
                    for (int k = 4; k < 8; k++)
                    {
                        statParent.GetChild(i).GetChild(1).GetChild(k).GetComponent<Dropdown>().value = Random.Range(curMin, curMax + 1);
                        //Debug.Log(statParent.GetChild(index).GetChild(1).GetChild(i).GetComponent<Dropdown>().value);
                    }
                }
                break;
            case 15:
                for (int i = 0; i < statParent.childCount; i++)
                {
                    for (int k = 8; k < 12; k++)
                    {
                        statParent.GetChild(i).GetChild(1).GetChild(k).GetComponent<Dropdown>().value = Random.Range(curMin, curMax + 1);
                        //Debug.Log(statParent.GetChild(index).GetChild(1).GetChild(i).GetComponent<Dropdown>().value);
                    }
                }
                break;
        }
    }

    public void ResetStats()
    {
        for (int i = 0; i < statParent.childCount; i++)
        {
            //int num = i;
            for (int k = 0; k < 12; k++)
            {
                statParent.GetChild(i).GetChild(1).GetChild(k).GetComponent<Dropdown>().value = 2;
                //Debug.Log(statParent.GetChild(index).GetChild(1).GetChild(i).GetComponent<Dropdown>().value);
            }
        }
    }
}
