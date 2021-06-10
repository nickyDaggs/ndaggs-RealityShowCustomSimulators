using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SeasonMenuManager : MonoBehaviour
{
    public List<SeasonTemplate> seasons;
    public List<Cast> casts;
    List<Transform> buttons;
    public GameObject buttonParent;
    public static SeasonMenuManager instance;
    public static SeasonMenuManager Instance { get { return instance; } }

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
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartSeason(int season)
    {
        curSeason = seasons[season];
        curCast = casts[season];
        SceneManager.LoadScene(1);
    }
}
