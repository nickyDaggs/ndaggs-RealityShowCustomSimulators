using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeasonParts;
using System.Linq;
using UnityEngine.EventSystems;

public class SwapsMenu : MonoBehaviour
{
    public GameObject CurSwap;

    public static SwapsMenu instance;
    public static SwapsMenu Instance { get { return instance; } }

    [HideInInspector] public List<string> types = new List<string>() { "Shuffle", "Challenge Dissolve", "Dissolve Least Members", "Schoolyard Pick" };



    public GameObject tribeSizeParent;
    public RectTransform editorParent;
    public LayoutElement parent;

    public Dropdown swapAt;
    public Dropdown swapType;

    public Toggle exileSwap;

    public Button delSwap;

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Update()
    {
        if(SeasonMenuManager.Instance.swapParent.transform.childCount > 0)
        {
            delSwap.interactable = true;
        } else
        {
            delSwap.interactable = false;
        }

        if(CurSwap == null && SeasonMenuManager.Instance.swapParent.transform.childCount > 0)
        {
            DeleteSwap();
        }
    }

    public void SubmitSwapContestants(string aa)
    {
        /*Dropdown swapAt = CurSwap.transform.GetChild(0).GetComponent<Dropdown>();
        Dropdown swapType = CurSwap.transform.GetChild(1).GetComponent<Dropdown>();
        InputField s = EventSystem.current.currentSelectedGameObject.GetComponent<InputField>();
        List<int> t = GetTribesAt(swapAt, swapType).Select(x => x.members.Count).ToList();
        float least = t.Min();

        List<Team> curTribes = new List<Team>();

        float limit = least - (SeasonMenuManager.Instance.contestants - int.Parse(swapAt.options[swapAt.value].text));

        float swapNum = int.Parse(aa);
        if(swapNum > limit)
        {
            //s.text = limit.ToString();
        }*/
    }

    public void SubmitSwapTA(string aa)
    {
        InputField ss = EventSystem.current.currentSelectedGameObject.GetComponent<InputField>();

        //Dropdown swapAt = CurSwap.transform.GetChild(0).GetComponent<Dropdown>();
        //Dropdown swapType = CurSwap.transform.GetChild(1).GetComponent<Dropdown>();

        if (aa != "")
        {
            float newY = editorParent.sizeDelta.y;
            float contestants = int.Parse(swapAt.options[swapAt.value].text);//GetTribesAt(int.Parse(swapAt.options[swapAt.value].text), swapType.options[swapType.value].text).Select(x => x.members.Count).Sum();
            float sum = 0;
            int num = int.Parse(aa);
            if(exileSwap.gameObject.activeSelf && exileSwap.isOn)
            {
                contestants = contestants - 1;
            }

            if (num == 0)
            {
                //customSeason.Tribes = new List<Team>();
            }
            if (num > contestants)
            {
                num = (int)contestants;
                ss.GetComponent<InputField>().text = contestants.ToString();
            }
            else if (num < 2)
            {
                num = 2;
                ss.GetComponent<InputField>().text = "2";
            }
            float teamSize = Mathf.Round(contestants / num);

            float extra = contestants % num;

            //tribes = num;
            for (int i = 0; i < num; i++)
            {
                if (i + 1 > tribeSizeParent.transform.childCount)
                {
                    GameObject size = Instantiate(SeasonMenuManager.Instance.tribeSizer);
                    size.transform.SetParent(tribeSizeParent.transform);

                    float ySize = size.GetComponent<RectTransform>().sizeDelta.y;
                    if (i == num - 1)
                    {
                        ySize += 10;
                    }
                    //editorParent.sizeDelta = new Vector2(editorParent.sizeDelta.x, newY + ySize);
                    parent.preferredHeight += ySize;
                    //newY = editorParent.sizeDelta.y;
                }
            }

            for (int i = 0; i < tribeSizeParent.transform.childCount; i++)
            {
                GameObject size = tribeSizeParent.transform.GetChild(i).gameObject;
                if (i < num)
                {
                    size.transform.GetChild(3).GetComponent<InputField>().text = teamSize.ToString();
                    int s = (int)teamSize;
                    if (0 > contestants - teamSize * (i + 1))
                    {
                        size.transform.GetChild(3).GetComponent<InputField>().text = (contestants - teamSize * (i + 1)).ToString();
                        s = (int)contestants - (int)teamSize * (i + 1);
                    }
                    sum += s;
                }
                else
                {
                    float ySize = size.GetComponent<RectTransform>().sizeDelta.y;

                    //editorParent.sizeDelta = new Vector2(editorParent.sizeDelta.x, newY - ySize);
                    parent.preferredHeight -= ySize;
                    //newY = editorParent.sizeDelta.y;
                    Destroy(size);
                }
                if(i == tribeSizeParent.transform.childCount - 1)
                {
                    if(sum != contestants)
                    {
                        extra = contestants - sum;
                        size.transform.GetChild(3).GetComponent<InputField>().text = (int.Parse(size.transform.GetChild(3).GetComponent<InputField>().text) + extra).ToString();
                    } 
                }
            }
            if (num - tribeSizeParent.transform.childCount > 0)
            {
                //editorParent.sizeDelta = new Vector2(editorParent.sizeDelta.x, newY + 10);
                parent.preferredHeight += 10;
            }
            StartCoroutine(ABC());
        }
        
    }

    public void SubmitSwapAt(Dropdown change)
    {
        //Dropdown swapAt = CurSwap.transform.GetChild(0).GetComponent<Dropdown>();
        //Dropdown swapType = CurSwap.transform.GetChild(1).GetComponent<Dropdown>();
        List<Team> t = GetTribesAt(swapAt);

        

        //Debug.Log(t.Count);

        if (t.Count > 2)
        {
            List<Dropdown.OptionData> swapOpt = new List<Dropdown.OptionData>(SeasonMenuManager.Instance.swapOptions);
            swapOpt.RemoveAt(6);
            float least = t.Select(x => x.members.Count).ToList().Min();

            float limit = SeasonMenuManager.Instance.contestants - least + 1;

            float con = int.Parse(change.options[change.value].text);
            if (con < limit && swapType.options.Count == swapOpt.Count)
            {
                swapOpt.RemoveAt(3);
                swapOpt.RemoveAt(3);
                swapType.options = swapOpt;
                
                //Debug.Log("dsads");
            }
            if (con >= limit && swapType.options.Count != swapOpt.Count)
            {
                
                swapType.options = swapOpt;
                //Debug.Log("fdsaf");
            }
            
            //Debug.Log("SwapOpt:" + swapOpt.Count + " options:" + swapType.options.Count);
        }
        
        SeasonMenuManager.Instance.SubmitSwapType(swapType);
    }

    public List<Team> GetTribesAt(Dropdown swapAt)
    {
        float con = int.Parse(swapAt.options[swapAt.value].text);

        Transform swapParent = SeasonMenuManager.Instance.swapParent.transform;

        Transform previousTribes = null;

        string type = "";
        float lastSwap = 0;

        foreach (Transform child in swapParent)
        {
            type = child.GetChild(1).GetComponent<Dropdown>().options[child.GetChild(1).GetComponent<Dropdown>().value].text;
            if (child.childCount > 0)
            {
                if(child.GetComponentInChildren<Dropdown>() != null)
                {
                    if (types.Contains(type) && !child.GetChild(0).GetComponent<Dropdown>().interactable || types.Contains(type) && SeasonMenuManager.Instance.spEvMenu.RoundsClone.Count > 0)
                    {
                        
                        float prevSwap = int.Parse(child.GetChild(0).GetComponent<Dropdown>().options[child.GetChild(0).GetComponent<Dropdown>().value].text);
                        if (prevSwap >= con && (prevSwap < lastSwap || lastSwap == 0))
                        {
                            previousTribes = child.GetChild(4).GetChild(1);
                        }
                        lastSwap = prevSwap;
                    }
                }
            }
        }

        if (previousTribes != null)
        {
            //Debug.Log(previousTribes.name);
            List<Team> swappedTeams = new List<Team>();

            foreach (Transform child in previousTribes)
            {
                
                Team tribe = new Team() { members = new List<Contestant>(new Contestant[int.Parse(child.GetChild(3).GetComponent<InputField>().text)]) };
                //Debug.Log("Tribe:" + tribe.name + ColorUtility.ToHtmlStringRGBA(tribe.tribeColor));
                swappedTeams.Add(tribe);
            }
            return swappedTeams;
        }

        return SeasonMenuManager.Instance.customSeason.Tribes;
    }

    public void DeleteSwap()
    {
        
        //editorParent.sizeDelta = new Vector2(editorParent.sizeDelta.x, editorParent.sizeDelta.y - 150);
        
        if (SeasonMenuManager.Instance.swapParent.transform.childCount > 0)
        {
            if (!SeasonMenuManager.Instance.premergeRounds.Contains(swapAt.options[swapAt.value]))
            {
                SeasonMenuManager.Instance.premergeRounds.Add(swapAt.options[swapAt.value]);
                SeasonMenuManager.Instance.premergeRounds = SeasonMenuManager.Instance.premergeRounds.OrderBy(x => int.Parse(x.text)).ToList();
            }
            Destroy(CurSwap);
            GameObject swap = SeasonMenuManager.Instance.swapParent.transform.GetChild(SeasonMenuManager.Instance.swapParent.transform.childCount - 1).gameObject;
            /*swap.transform.GetChild(0).GetComponent<Dropdown>().options.Clear();
            for (int i = (int)customSeason.mergeAt + 1; i < contestants; i++)
            {
                swap.transform.GetChild(0).GetComponent<Dropdown>().options.Add(new Dropdown.OptionData { text = i.ToString() });
                //Debug.Log(i);
            }
            swap.transform.GetChild(0).GetComponent<Dropdown>().options = premergeRounds;
            swap.transform.GetChild(0).GetComponent<Dropdown>().onValueChanged.AddListener(delegate { SwapsMenu.Instance.SubmitSwapAt(swap.transform.GetChild(0).GetComponent<Dropdown>()); });
            swap.transform.GetChild(1).GetComponent<Dropdown>().onValueChanged.AddListener(delegate { SubmitSwapType(swap.transform.GetChild(1).GetComponent<Dropdown>()); });
            
            List<Dropdown.OptionData> swapOpt = new List<Dropdown.OptionData>(swapOptions);

            List<Team> t = SwapsMenu.Instance.GetTribesAt(swap.transform.GetChild(0).GetComponent<Dropdown>(), swap.transform.GetChild(1).GetComponent<Dropdown>());

            if (t.Count < 3)
            {
                swapOpt.RemoveAt(3);
                swapOpt.RemoveAt(3);
            }
            if (t.Count > 2)
            {
                swapOpt.RemoveAt(6);
            }*/
            //ogSwapY = editorParent.sizeDelta.y;
            //swap.transform.GetChild(1).GetComponent<Dropdown>().options = swapOpt;
            CurSwap = swap;
            swapAt = swap.transform.GetChild(0).GetComponent<Dropdown>();
            swapType = swap.transform.GetChild(1).GetComponent<Dropdown>();
            parent = swap.GetComponent<LayoutElement>();
            exileSwap = swap.transform.GetChild(1).GetComponent<Toggle>();
            //SubmitSwapAt(swap.transform.GetChild(0).GetComponent<Dropdown>());

            swapAt.interactable = true;
            swapType.interactable = true;

            foreach (InputField inputField in CurSwap.transform.GetChild(4).GetComponentsInChildren<InputField>())
            {
                if (inputField.transform.childCount > 0)
                {
                    foreach (InputField inputFieldd in inputField.GetComponentsInChildren<InputField>())
                    {
                        inputFieldd.interactable = true;
                    }
                }
                inputField.interactable = true;
            }
            
            StartCoroutine(ABC());
        }
        
    }

    IEnumerator ABC()
    {

        //returning 0 will make it wait 1 frame
        editorParent.gameObject.SetActive(!editorParent.gameObject.activeSelf);
        yield return 0;
        editorParent.gameObject.SetActive(!editorParent.gameObject.activeSelf);
        yield return 0;
        editorParent.gameObject.SetActive(!editorParent.gameObject.activeSelf);
        yield return 0;
        editorParent.gameObject.SetActive(true);
        
    }
}