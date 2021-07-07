using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SeasonMenuManager : MonoBehaviour
{
    [System.Serializable]
    public class Option
    {
        public GameObject button;
        public string option;
        public string optionBool;
    }
    public bool cineTribal, absorb;
    public List<SeasonTemplate> seasons;
    public List<Cast> casts;
    List<Transform> buttons;
    public GameObject buttonParent;
    public static SeasonMenuManager instance;
    public static SeasonMenuManager Instance { get { return instance; } }

    public List<Option> options;

    public SeasonTemplate curSeason;
    public Cast curCast;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < buttonParent.transform.childCount; i++)
        {
            int num = i;
            buttonParent.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => StartSeason(num));
            buttonParent.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = seasons[num].nameSeason;
        }
    }

    // Update is called once per frame
    public void ChangeOption()
    {
        foreach (Option opt in options)
        {
            if(opt.button == UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject)
            {
                string last = opt.button.GetComponentInChildren<Text>().text;
                opt.button.GetComponentInChildren<Text>().text = opt.option;
                opt.option = last;
            }
        }
    }
    public void StartSeason(int season)
    {
        foreach (Option opt in options)
        {
            if (opt.button.GetComponentInChildren<Text>().text.Contains("On"))
            {
                if(opt.optionBool == "CT")
                {
                    cineTribal = true;
                } else if (opt.optionBool == "absorb")
                {
                    absorb = true;
                }
            }
        }
        curSeason = seasons[season];
        curCast = casts[season];
        SceneManager.LoadScene(1);
    }
}
