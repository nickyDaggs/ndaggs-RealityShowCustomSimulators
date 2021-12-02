using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeasonParts;
using System.Linq;

public class Swaps : MonoBehaviour
{
    public class Leader
    {
        Contestant con;
        int curTribe;
    }
    public void DoSwap(SwapType swapType)
    {
        switch (swapType)
        {
            case SwapType.RegularSwap:
                RegularSwap();
                break;
            case SwapType.RegularShuffle:
                RegularShuffle();
                break;
            case SwapType.ChallengeDissolve:
                ChallengeDissolve();
                break;
            case SwapType.DissolveLeastMembers:
                DissolveLeastMembers();
                break;
            case SwapType.SchoolyardPick:
                SchoolyardPick();
                break;
            case SwapType.SplitTribes:
                SplitTribes();
                break;
            case SwapType.TribeChiefs:
                TribeChiefs();
                break;
            case SwapType.Mutiny:
                Mutiny();
                break;
            case SwapType.CISchoolyardPick:
                CISchoolyardPick();
                break;
        }
    }
    void RegularSwap()
    {
        GameObject SwapContext = Instantiate(GameManager.instance.Prefabs[0]);
        SwapContext.transform.parent = GameManager.instance.Canvas.transform;
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(0, SwapContext.GetComponent<RectTransform>().offsetMax.y);
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(SwapContext.GetComponent<RectTransform>().offsetMin.x, 0);
        GameManager.instance.AddGM(SwapContext, true);
        List<List<Contestant>> swappedCon = new List<List<Contestant>>();
        List<Team> TribesDup = new List<Team>(GameManager.instance.Tribes);
        float swapNum = GameManager.instance.curSwap.numberSwap;
        foreach (Team tribe in GameManager.instance.Tribes)
        {
            List<Contestant> swapPlayers = new List<Contestant>();
            swappedCon.Add(swapPlayers);
        }
        string s = "s";
        if (swapNum < 2)
        {
            s = "";
        }
        string atext = "";
        if (GameManager.instance.curSwap.text == "Africa")
        {
            atext = "Each tribe is asked to pick " + GameManager.instance.curSwap.numberSwap + " representive" + s + ".";
        }
        else
        {
            atext = "Each tribe is asked to pick the " + GameManager.instance.curSwap.numberSwap + " strongest player" + s + " from another tribe.";
        }
        GameManager.instance.MakeGroup(false, null, "", atext, "", new List<Contestant>(), SwapContext.transform.GetChild(0).GetChild(0), 0);

        float limit = TribesDup.Select(x => x.members.Count).ToList().Min();

        if (swapNum > limit)
        {
            swapNum = limit;
        }

        for (int i = 0; i < TribesDup.Count; i++)
        {
            for (int j = 0; j < swapNum; j++)
            {
                Contestant num = TribesDup[i].members[Random.Range(0, TribesDup[i].members.Count)];
                num.team = TribesDup[i].name;
                swappedCon[i].Add(num);
                TribesDup[i].members.Remove(num);
            }
            string comma = "";
            if (swappedCon[i].Count > 2)
            {
                comma = ", ";
            }
            string etext = string.Join(comma, new List<Contestant>(swappedCon[i]).ConvertAll(u => u.nickname)) + " are picked for " + TribesDup[i].name;
            etext = etext.Replace(comma + swappedCon[i][swappedCon[i].Count -1].nickname, " and " + swappedCon[i][swappedCon[i].Count - 1].nickname);
            GameManager.instance.MakeGroup(false, null, "", "", etext, swappedCon[i], SwapContext.transform.GetChild(0).GetChild(0), 0);
        }
        GameManager.instance.MakeGroup(false, null, "", "", "Each group of " + GameManager.instance.curSwap.numberSwap + " players will actually be switching to another tribe.", new List<Contestant>(), SwapContext.transform.GetChild(0).GetChild(0), 0);
        /*
        if (swappedCon.Count > 2)
        {
            for (int i = swappedCon.Count - 1; i > 0; i--)
            {
                int swapIndex = Random.Range(0, i + 1);
                List<Contestant> currentCon = swappedCon[i];
                List<Contestant> conToSwap = swappedCon[swapIndex];
                swappedCon[i] = conToSwap;
                swappedCon[swapIndex] = currentCon;
            }
        } */
        
        for (int i = 0; i < swappedCon.Count; i++)
        {
            int gam = i + 1;
            if (gam > swappedCon.Count - 1)
            {
                gam = 0;
            }
            for (int j = 0; j < swappedCon[gam].Count; j++)
            {
                GameManager.instance.Tribes[gam].members.Remove(swappedCon[gam][j]);
            }
            for (int q = 0; q < swappedCon[i].Count; q++)
            {
                GameManager.instance.Tribes[gam].members.Add(swappedCon[i][q]);
                foreach (Alliance alliance in GameManager.instance.Alliances)
                {
                    if (alliance.members.Contains(swappedCon[i][q]))
                    {
                        if (!alliance.teams.Contains(GameManager.instance.Tribes[gam].name))
                        {
                            alliance.teams.Add(GameManager.instance.Tribes[gam].name);
                        }
                    }
                }
            }

        }
        foreach (Team tribe in GameManager.instance.Tribes)
        {
            foreach (Contestant num in tribe.members)
            {
                num.teams.Add(tribe.tribeColor);
            }
        }
    }
    void RegularShuffle()
    {
        GameObject SwapContext = Instantiate(GameManager.instance.Prefabs[0]);
        SwapContext.transform.parent = GameManager.instance.Canvas.transform;
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(0, SwapContext.GetComponent<RectTransform>().offsetMax.y);
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(SwapContext.GetComponent<RectTransform>().offsetMin.x, 0);
        GameManager.instance.AddGM(SwapContext, true);
        List<Contestant> curCast = new List<Contestant>();
        List<List<Contestant>> TribesDup = new List<List<Contestant>>();
        List<Team> NewTribes = new List<Team>(GameManager.instance.curSwap.newTribes);
        
        

        foreach (Team tribe in GameManager.instance.Tribes)
        {
            foreach (Contestant num in tribe.members)
            {
                curCast.Add(num);
            }
            TribesDup.Add(new List<Contestant>(tribe.members));
        }
        if (GameManager.instance.curSwap.text == "FakeFeast")
        {
            GameManager.instance.MakeGroup(false, null, "", "", "A feast occurs.\n\nAn immunity idol is hidden that can only be found now.", new List<Contestant>(), SwapContext.transform.GetChild(0).GetChild(0), 0);
            bool hidden = true;
            foreach (Contestant num in curCast)
            {
                if(hidden)
                {
                    if(Random.Range(0, 2) == 0)
                    {
                        Advantage av = Instantiate(GameManager.instance.HiddenIdol);
                        av.nickname = "Feast Hidden Immunity Idol";

                        hidden = false;
                        num.advantages.Add(av);
                        List<Contestant> n = new List<Contestant>() { num };
                        GameManager.instance.MakeGroup(false, null, "", "", num.nickname + " finds the " + av.nickname, n, SwapContext.transform.GetChild(0).GetChild(0), 10);
                    }
                }
            }
            if(hidden)
            {
                GameManager.instance.MakeGroup(false, null, "", "", "The idol is not found.", new List<Contestant>(), SwapContext.transform.GetChild(0).GetChild(0), 0);
            }
        }

        if(GameManager.instance.curSwap.ResizeTribes)
        {
            GameManager.instance.MakeGroup(false, null, "", "", "As part of a twist, new tribes are randomly assigned. \n \n There will be " + NewTribes.Count + " tribes.", new List<Contestant>(), SwapContext.transform.GetChild(0).GetChild(0), 0);

        } else
        {
            GameManager.instance.MakeGroup(false, null, "", "", "As part of a twist, new tribes are randomly assigned. \n \n There will be " + TribesDup.Count + " tribes.", new List<Contestant>(), SwapContext.transform.GetChild(0).GetChild(0), 0);
        }

        if (GameManager.instance.curSwap.exile == true)
        {
            int ran = Random.Range(0, curCast.Count);
            GameManager.instance.Exiled.Add(curCast[ran]);
            GameManager.instance.MakeGroup(false, null, "", "One castaway will be exiled.", curCast[ran].nickname + " is exiled!", GameManager.instance.Exiled, SwapContext.transform.GetChild(0).GetChild(0), 0);
            curCast.Remove(curCast[ran]);
        }
        for (int i = curCast.Count - 1; i > 0; i--)
        {
            int swapIndex = Random.Range(0, i + 1);
            Contestant currentCon = curCast[i];
            Contestant conToSwap = curCast[swapIndex];
            curCast[i] = conToSwap;
            curCast[swapIndex] = currentCon;
        }
        int con = 0;
        if (!GameManager.instance.curSwap.ResizeTribes)
        {
            for (int i = 0; i < GameManager.instance.Tribes.Count; i++)
            {
                for (int j = 0; j < GameManager.instance.Tribes[i].members.Count; j++)
                {
                    GameManager.instance.Tribes[i].members[j] = curCast[con];
                    con++;
                    foreach (Alliance alliance in GameManager.instance.Alliances)
                    {
                        if (alliance.members.Contains(GameManager.instance.Tribes[i].members[j]))
                        {
                            if (!alliance.teams.Contains(GameManager.instance.Tribes[i].name))
                            {
                                alliance.teams.Add(GameManager.instance.Tribes[i].name);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if(GameManager.instance.curSwap.orderBySize)
            {
                GameManager.instance.Tribes = GameManager.instance.Tribes.OrderByDescending(x => x.members.Count).ToList();
            }

            for (int i = 0; i < NewTribes.Count; i++)
            {
                if (NewTribes[i].name == "Same" || NewTribes[i].name == "same")
                {
                    NewTribes[i].tribeColor = GameManager.instance.Tribes[i].tribeColor;
                    NewTribes[i].name = GameManager.instance.Tribes[i].name;
                    
                }
                if(i < GameManager.instance.Tribes.Count)
                {
                    NewTribes[i].hiddenAdvantages = GameManager.instance.Tribes[i].hiddenAdvantages;
                }
            }
            for (int i = 0; i < NewTribes.Count; i++)
            {
                for (int j = 0; j < NewTribes[i].members.Count; j++)
                {
                    NewTribes[i].members[j] = curCast[curCast.Count - 1];
                    curCast.Remove(curCast[curCast.Count - 1]);
                    foreach (Alliance alliance in GameManager.instance.Alliances)
                    {
                        if (alliance.members.Contains(NewTribes[i].members[j]))
                        {
                            if (!alliance.teams.Contains(NewTribes[i].name))
                            {
                                alliance.teams.Add(NewTribes[i].name);
                            }
                        }
                    }
                }
            }
            
            GameManager.instance.Tribes = new List<Team>(NewTribes);
        }
        foreach (Team tribe in GameManager.instance.Tribes)
        {
            foreach (Contestant num in tribe.members)
            {
                num.teams.Add(tribe.tribeColor);
            }
        }
    }
    void ChallengeDissolve()
    {
        GameObject SwapContext = Instantiate(GameManager.instance.Prefabs[2]);
        SwapContext.transform.parent = GameManager.instance.Canvas.transform;
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(0, SwapContext.GetComponent<RectTransform>().offsetMax.y);
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(SwapContext.GetComponent<RectTransform>().offsetMin.x, 0);
        GameManager.instance.AddGM(SwapContext, true);
        List<List<Contestant>> swappedCon = new List<List<Contestant>>();
        List<List<Contestant>> TribesDup = new List<List<Contestant>>();
        float swapNum = GameManager.instance.curSwap.numberSwap;
        Team LosingTribee = new Team();
        Contestant exiled = new Contestant();
        List<int> SpaceLeft = new List<int>();
        int ran = Random.Range(0, GameManager.instance.Tribes.Count);
        LosingTribee = GameManager.instance.Tribes[ran];
        GameManager.instance.Tribes.Remove(GameManager.instance.Tribes[ran]);
        if (GameManager.instance.curSwap.exile == true)
        {
            int rann = Random.Range(0, LosingTribee.members.Count);
            GameManager.instance.Exiled.Add(LosingTribee.members[rann]);
            exiled = LosingTribee.members[rann];
            LosingTribee.members.Remove(LosingTribee.members[rann]);
        }
        List<Team> NewTribes = new List<Team>(GameManager.instance.curSwap.newTribes);
        foreach (Team tribe in GameManager.instance.Tribes)
        {
            GameManager.instance.MakeGroup(false, null, "", "", tribe.name + " wins reward.", tribe.members, SwapContext.transform.GetChild(0), 0);
            int r = 0;
            SpaceLeft.Add(r);
        }
        GameManager.instance.MakeGroup(false, null, "", "", "The losing tribe, " + LosingTribee.name + ", will be dissolved.", LosingTribee.members, SwapContext.transform.GetChild(0), 0);
        for (int i = 0; i < GameManager.instance.Tribes.Count; i++)
        {
            NewTribes[i].tribeColor = GameManager.instance.Tribes[i].tribeColor;
            NewTribes[i].name = GameManager.instance.Tribes[i].name;
            NewTribes[i].hiddenAdvantages = GameManager.instance.Tribes[i].hiddenAdvantages;
            for (int j = 0; j < GameManager.instance.Tribes[i].members.Count; j++)
            {
                NewTribes[i].members[j] = GameManager.instance.Tribes[i].members[j];
                foreach (Alliance alliance in GameManager.instance.Alliances)
                {
                    if (alliance.members.Contains(NewTribes[i].members[j]))
                    {
                        if (!alliance.teams.Contains(NewTribes[i].name))
                        {
                            alliance.teams.Add(NewTribes[i].name);
                        }
                    }
                }
                SpaceLeft[i]++;
            }
        } 
        //int curCon = 0;
        int curT = 0;
        for (int i = LosingTribee.members.Count - 1; i > 0; i--)
        {
            int swapIndex = Random.Range(0, i + 1);
            Contestant currentCon = LosingTribee.members[i];
            Contestant conToSwap = LosingTribee.members[swapIndex];
            LosingTribee.members[i] = conToSwap;
            LosingTribee.members[swapIndex] = currentCon;
        }
        for (int i = 0; i < LosingTribee.members.Count; i++)
        {
            bool g = false;
            while(!g)
            {
                if(SpaceLeft[curT] <= NewTribes[curT].members.Count - 1)
                {
                    g = true;
                } else
                {
                    curT++;
                    if (curT > NewTribes.Count - 1)
                    {
                        curT = 0;
                    }
                }
            }
            NewTribes[curT].members[SpaceLeft[curT]] = LosingTribee.members[i];
            List<Contestant> n = new List<Contestant>() { LosingTribee.members[i] };
            GameManager.instance.MakeGroup(false, null, "", "", NewTribes[curT].name + " selects " + LosingTribee.members[i].nickname + ".", n, SwapContext.transform.GetChild(0), 0);
            foreach (Alliance alliance in GameManager.instance.Alliances)
            {
                if (alliance.members.Contains(NewTribes[curT].members[SpaceLeft[curT]]))
                {
                    if (!alliance.teams.Contains(NewTribes[curT].name))
                    {
                        alliance.teams.Add(NewTribes[curT].name);
                    }
                }
            }
            SpaceLeft[curT]++;
            curT++;
            if (curT > NewTribes.Count - 1)
            {
                curT = 0;
            }
        }
        foreach (Team tribe in NewTribes)
        {
            foreach (Contestant num in tribe.members)
            {
                num.teams.Add(tribe.tribeColor);
            }
        }
        if (GameManager.instance.curSwap.exile)
        {
            GameManager.instance.MakeGroup(false, null, "", "", exiled.nickname + " is sent to exile!", GameManager.instance.Exiled, SwapContext.transform.GetChild(0), 0);
        }
        GameManager.instance.Tribes = new List<Team>(NewTribes);
    }
    void DissolveLeastMembers()
    {
        GameObject SwapContext = Instantiate(GameManager.instance.Prefabs[0]);
        SwapContext.transform.parent = GameManager.instance.Canvas.transform;
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(0, SwapContext.GetComponent<RectTransform>().offsetMax.y);
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(SwapContext.GetComponent<RectTransform>().offsetMin.x, 0);
        GameManager.instance.AddGM(SwapContext, true);
        List<int> SpaceLeft = new List<int>();
        List<Team> TribesDup = new List<Team>(GameManager.instance.Tribes);
        List<Team> Tie = new List<Team>();
        Contestant exiled = new Contestant();
        TribesDup = TribesDup.OrderBy(e => e.members.Count).ToList();
        foreach (Team tribe in TribesDup)
        {
            if (tribe.members.Count == TribesDup[0].members.Count)
            {
                Tie.Add(tribe);
            }
        }
        if (Tie.Count > 1)
        {
            int ran = Random.Range(0, Tie.Count);
            TribesDup[0] = Tie[ran];
        }
        List<List<Contestant>> swappedCon = new List<List<Contestant>>();
        float swapNum = GameManager.instance.curSwap.numberSwap;
        Team LosingTribee = new Team();
        List<Team> NewTribes = new List<Team>(GameManager.instance.curSwap.newTribes);
        //int ran = Random.Range(0, Tribes.Count);
        LosingTribee = TribesDup[0];
        GameManager.instance.Tribes.Remove(TribesDup[0]);
        GameManager.instance.MakeGroup(false, null, "", "", "The tribe with the least amount of members, " + LosingTribee.name + ", will be dissolved.", LosingTribee.members, SwapContext.transform.GetChild(0).GetChild(0), 0);
        if (GameManager.instance.curSwap.exile == true)
        {
            int rann = Random.Range(0, LosingTribee.members.Count);
            GameManager.instance.Exiled.Add(LosingTribee.members[rann]);
            exiled = LosingTribee.members[rann];
            LosingTribee.members.Remove(LosingTribee.members[rann]);
        }
        foreach (Team tribe in GameManager.instance.Tribes)
        {
            int r = 0;
            SpaceLeft.Add(r);
        }
        for (int i = 0; i < GameManager.instance.Tribes.Count; i++)
        {
            NewTribes[i].tribeColor = GameManager.instance.Tribes[i].tribeColor;
            NewTribes[i].name = GameManager.instance.Tribes[i].name;
            for (int j = 0; j < GameManager.instance.Tribes[i].members.Count; j++)
            {
                NewTribes[i].members[j] = GameManager.instance.Tribes[i].members[j];
                SpaceLeft[i]++;
            }
            NewTribes[i].hiddenAdvantages = GameManager.instance.Tribes[i].hiddenAdvantages;
        }
        int curT = 0;
        for (int i = 0; i < LosingTribee.members.Count; i++)
        {
            bool g = false;
            while (!g)
            {
                if (SpaceLeft[curT] <= NewTribes[curT].members.Count - 1)
                {
                    g = true;
                }
                else
                {
                    curT++;
                    if (curT > NewTribes.Count - 1)
                    {
                        curT = 0;
                    }
                }
            }
            NewTribes[curT].members[SpaceLeft[curT]] = LosingTribee.members[i];
            
            List<Contestant> n = new List<Contestant>() { LosingTribee.members[i] };
            GameManager.instance.MakeGroup(false, null, "", "", LosingTribee.members[i].nickname + " goes to " + NewTribes[curT].name + ".", n, SwapContext.transform.GetChild(0).GetChild(0), 0);
            foreach (Alliance alliance in GameManager.instance.Alliances)
            {
                if (alliance.members.Contains(NewTribes[curT].members[curT]))
                {
                    if (!alliance.teams.Contains(NewTribes[curT].name))
                    {
                        alliance.teams.Add(NewTribes[curT].name);
                    }
                }
            }
            SpaceLeft[curT]++;
            curT++;
            if (curT > NewTribes.Count - 1)
            {
                curT = 0;
            }
        }
        foreach (Team tribe in NewTribes)
        {
            foreach (Contestant num in tribe.members)
            {
                num.teams.Add(tribe.tribeColor);
            }
        }
        if(GameManager.instance.curSwap.exile)
        {
            GameManager.instance.MakeGroup(false, null, "", "", exiled.nickname + " is sent to exile!", GameManager.instance.Exiled, SwapContext.transform.GetChild(0).GetChild(0), 0);
        }
        GameManager.instance.Tribes = new List<Team>(NewTribes);
    }
    void SchoolyardPick()
    {
        GameObject SwapContext = Instantiate(GameManager.instance.Prefabs[0]);
        SwapContext.transform.parent = GameManager.instance.Canvas.transform;
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(0, SwapContext.GetComponent<RectTransform>().offsetMax.y);
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(SwapContext.GetComponent<RectTransform>().offsetMin.x, 0);
        GameManager.instance.AddGM(SwapContext, true);
        List<Contestant> Leaders = new List<Contestant>();
        List<Team> Groups = new List<Team>();
        Groups.Add(new Team()); Groups.Add(new Team());
        List<int> curTribe = new List<int>();
        List<int> curMem = new List<int>();
        List<Team> DupTribes = new List<Team>(GameManager.instance.Tribes);
        List<Team> Genders = new List<Team>();
        List<Team> LastTribe = new List<Team>(DupTribes);
        List<Team> NewTribes = new List<Team>(GameManager.instance.curSwap.newTribes); 
        List<Contestant> curCast = new List<Contestant>();
        int contestants = 0;
        int exile = 0;
        Team Males = new Team();
        Team Females = new Team();
        Genders.Add(Males); Genders.Add(Females);
        for (int i = 0; i < NewTribes.Count; i++)
        {
            if (NewTribes[i].name == "Same" || NewTribes[i].name == "same")
            {
                NewTribes[i].tribeColor = GameManager.instance.Tribes[i].tribeColor;
                NewTribes[i].name = GameManager.instance.Tribes[i].name;
            }
            if(i <= GameManager.instance.Tribes.Count - 1)
            {
                NewTribes[i].hiddenAdvantages = GameManager.instance.Tribes[i].hiddenAdvantages;
            }
        }
        foreach(Team tribe in DupTribes)
        {
            foreach(Contestant num in tribe.members)
            {
                if (GameManager.instance.curSwap.pickingRules == "altGender")
                {
                    if (num.gender == "M")
                    {
                        Genders[0].members.Add(num);
                    } else
                    {
                        Genders[1].members.Add(num);
                    }
                }
            }
        }
        if(GameManager.instance.curSwap.pickingRules == "altTribes" || GameManager.instance.curSwap.pickingRules == "Any")
        {
            for (int i = 0; i < NewTribes.Count; i++)
            {
                int r = 0;
                int a = 1;
                int ran = Random.Range(0, DupTribes[i].members.Count);
                Leaders.Add(DupTribes[i].members[ran]);
                NewTribes[i].members[0] = DupTribes[i].members[ran];
                curTribe.Add(r);
                curMem.Add(a);
                DupTribes[i].members.Remove(DupTribes[i].members[ran]);
                
            }
            foreach (Team tribe in DupTribes)
            {
                contestants += tribe.members.Count;
                foreach (Contestant num in tribe.members)
                {
                    curCast.Add(num);
                }
            }
        }
        else if (GameManager.instance.curSwap.pickingRules == "altGender")
        {
            for (int i = 0; i < NewTribes.Count; i++)
            {
                int r = 0;
                int a = 1;
                int ran = Random.Range(0, Genders[i].members.Count);
                Leaders.Add(Genders[i].members[ran]);
                NewTribes[i].members[0] = Genders[i].members[ran];
                curTribe.Add(r);
                curMem.Add(a);
                Genders[i].members.Remove(Genders[i].members[ran]);
                
                
            }
            foreach(Team tribe in Genders)
            {
                contestants += tribe.members.Count;
                foreach (Contestant num in tribe.members)
                {
                    curCast.Add(num);
                }
            }
        }
        else if (GameManager.instance.curSwap.pickingRules == "Panama")
        {
            //NewTribes = new List<Team>();
            //NewTribes.Add(new Team()); NewTribes.Add(new Team());
            for (int i = 0; i < NewTribes.Count; i++)
            {
                NewTribes[i].name = GameManager.instance.curSwap.newTribes[i].name;
                NewTribes[i].tribeColor = GameManager.instance.curSwap.newTribes[i].tribeColor;
            }
            int split = DupTribes.Count / 2;
            for (int i = 0; i < split; i++)
            {
                foreach (Contestant num in DupTribes[i].members)
                {
                    Groups[0].members.Add(num);
                }
                foreach (Contestant num in DupTribes[i + split].members)
                {
                    Groups[1].members.Add(num);
                }
            }
            for (int i = 0; i < Groups.Count; i++)
            {
                int r = 0;
                int a = 1;
                int ran = Random.Range(0, Groups[i].members.Count);
                Leaders.Add(Groups[i].members[ran]);
                NewTribes[i].members[0] = (Groups[i].members[ran]);
                Groups[i].members.Remove(Groups[i].members[ran]);
                curTribe.Add(r);
                curMem.Add(a);
                
            }
            foreach (Team tribe in Groups)
            {
                contestants += tribe.members.Count;
                foreach (Contestant num in tribe.members)
                {
                    curCast.Add(num);
                }
            }
        }
        string etext = string.Join(",", new List<Contestant>(Leaders).ConvertAll(i => i.nickname)) + " are chosen as captains";
        etext = etext.Replace("," + Leaders[Leaders.Count - 1].nickname, " and " + Leaders[Leaders.Count - 1].nickname);
        GameManager.instance.MakeGroup(false, null, "", "Tribes will now be mixed up with a schoolyard pick.", etext, Leaders, SwapContext.transform.GetChild(0).GetChild(0), 0);
        if (GameManager.instance.curSwap.exile)
        {
            exile = -1;
        }
        int rounds = (contestants + exile) / NewTribes.Count;
        switch (GameManager.instance.curSwap.pickingRules)
        {
            case "Any":
                for (int i = 0; i < rounds; i++)
                {
                    for (int j = 0; j < Leaders.Count; j++)
                    {
                        Contestant picked;
                        Contestant picker;
                        int ran = 0;
                        ran = Random.Range(0, curCast.Count);
                        NewTribes[j].members[curMem[j]] = curCast[ran];
                        if (GameManager.instance.curSwap.text == "Leader")
                        {
                            picker = Leaders[j];
                        }
                        else
                        {
                            if (curMem[j] - 1 > 0)
                            {
                                picker = NewTribes[j].members[curMem[j] - 1];
                            }
                            else
                            {
                                picker = Leaders[j];
                            }
                        }
                        picked = curCast[ran];
                        curCast.Remove(curCast[ran]);
                        curMem[j]++;

                        List<Contestant> n = new List<Contestant>() { picked, picker };
                        GameManager.instance.MakeGroup(false, null, "", "", picker.nickname + " chooses " + picked.nickname + " for the " + NewTribes[j].name, n, SwapContext.transform.GetChild(0).GetChild(0), 0);
                    }
                }
                if (curCast.Count > 0)
                {
                    foreach (Contestant num in curCast)
                    {
                        if (GameManager.instance.curSwap.exile == true)
                        {
                            GameManager.instance.MakeGroup(false, null, "", "", num.nickname + " is exiled!", GameManager.instance.Exiled, SwapContext.transform.GetChild(0).GetChild(0), 0);
                        }
                    }
                }
                break;
            default:
                switch(GameManager.instance.curSwap.pickingRules)
                {
                    case "altTribes":
                        LastTribe = new List<Team>(DupTribes);
                        break;
                    case "altGenders":
                        DupTribes = Genders;
                        LastTribe = new List<Team>(Genders);
                        break;
                    case "Panama":
                        DupTribes = Groups;
                        LastTribe = new List<Team>(Groups);
                        break;
                }
                for (int i = 0; i < rounds; i++)
                {
                    for (int j = 0; j < Leaders.Count; j++)
                    {
                        List<Team> TribesV = new List<Team>(DupTribes);
                        if (TribesV.Contains(LastTribe[j]))
                        {
                            TribesV.Remove(LastTribe[j]);
                        }
                        int ran = 0;
                        if (TribesV[curTribe[j]].members.Count > 0)
                        {
                            ran = Random.Range(0, TribesV[curTribe[j]].members.Count);
                        }
                        else
                        {
                            if (TribesV.Count > 1)
                            {
                                curTribe[j]++;
                                if (curTribe[j] > TribesV.Count)
                                {
                                    curTribe[j] = 0;
                                }
                            }
                            else
                            {
                                ran = Random.Range(0, LastTribe[j].members.Count);
                            }

                        }
                        Contestant picked;
                        Contestant picker;
                        if(GameManager.instance.curSwap.text == "Leader")
                        {
                            picker = Leaders[j];
                        } else
                        {
                            if(curMem[j] - 1 > 0)
                            {
                                picker = NewTribes[j].members[curMem[j] - 1];
                            } else
                            {
                                picker = Leaders[j];
                            }
                        }
                        if (curMem[j] > NewTribes[j].members.Count - 1)
                        {
                            curMem[j] = NewTribes[j].members.Count - 1;
                        }
                        if (TribesV[curTribe[j]].members.Count == 0)
                        {
                            if (TribesV.Count > 1)
                            {
                                NewTribes[j].members[curMem[j]] = TribesV[curTribe[j]].members[ran];
                                DupTribes[DupTribes.IndexOf(TribesV[curTribe[j]])].members.Remove(TribesV[curTribe[j]].members[ran]);
                            }
                            else
                            {
                                NewTribes[j].members[curMem[j]] = LastTribe[j].members[ran];
                                DupTribes[DupTribes.IndexOf(LastTribe[j])].members.Remove(LastTribe[j].members[ran]);
                            }
                        }
                        else
                        {
                            NewTribes[j].members[curMem[j]] = TribesV[curTribe[j]].members[ran];
                            DupTribes[DupTribes.IndexOf(TribesV[curTribe[j]])].members.Remove(TribesV[curTribe[j]].members[ran]);
                        }
                        curCast.Remove(NewTribes[j].members[curMem[j]]);
                        picked = NewTribes[j].members[curMem[j]];
                        List<Contestant> n = new List<Contestant>() { picked, picker };
                        GameManager.instance.MakeGroup(false, null, "", "", picker.nickname + " chooses " + picked.nickname + " for the " + NewTribes[j].name, n, SwapContext.transform.GetChild(0).GetChild(0), 0);
                        LastTribe[j] = DupTribes[DupTribes.IndexOf(TribesV[curTribe[j]])];
                        curTribe[j]++;
                        curMem[j]++;

                        if (curTribe[j] > TribesV.Count - 1)
                        {
                            curTribe[j] = 0;
                        }
                    }
                }
                if (curCast.Count > 0)
                {
                    foreach (Contestant num in curCast)
                    {
                        GameManager.instance.Exiled.Add(num);
                        if (GameManager.instance.curSwap.exile == true)
                        {
                            GameManager.instance.MakeGroup(false, null, "", "", num.nickname + " is exiled!", GameManager.instance.Exiled, SwapContext.transform.GetChild(0).GetChild(0), 0);
                        }
                    }
                }
                break;
        }
        
        
        GameManager.instance.Tribes = new List<Team>(NewTribes);
        foreach (Team tribe in GameManager.instance.Tribes)
        {
            foreach (Contestant num in tribe.members)
            {
                num.teams.Add(tribe.tribeColor);
                foreach (Alliance alliance in GameManager.instance.Alliances)
                {
                    if (alliance.members.Contains(num))
                    {
                        if (!alliance.teams.Contains(tribe.name))
                        {
                            alliance.teams.Add(tribe.name);
                        }
                    }
                }
            }
        }
    }
    void SplitTribes()
    {
        GameObject SwapContext = Instantiate(GameManager.instance.Prefabs[0]);
        SwapContext.transform.parent = GameManager.instance.Canvas.transform;
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(0, SwapContext.GetComponent<RectTransform>().offsetMax.y);
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(SwapContext.GetComponent<RectTransform>().offsetMin.x, 0);
        GameManager.instance.AddGM(SwapContext, true);
        List<Team> TribesDup = new List<Team>(GameManager.instance.Tribes);
        List<Team> NewTribes = new List<Team>();
        List<int> peopleToSwap = new List<int>();
        TribesDup = TribesDup.OrderBy(i => i.members.Count).ToList();
        TribesDup.Reverse();
        GameObject group = Instantiate(GameManager.instance.GroupPrefab);
        group.transform.parent = SwapContext.transform.GetChild(0).GetChild(0);
        group.GetComponent<UIGroup>().tribeName.enabled = false;
        string etext = "";
        if (GameManager.instance.curSwap.text == "Reward")
        {
            etext = "The tribes have a reward challenge that turns out to swap up the current tribes.";
        } else
        {
            etext = "The current tribes will now be split in half to create two new tribes";
        }
        GameManager.instance.MakeGroup(false, null, "", "", etext, GameManager.instance.Exiled, SwapContext.transform.GetChild(0).GetChild(0), 0);
        foreach (Team tribe in TribesDup)
        {
            if(tribe.members.Count % 2 == 0)
            {
                int swap = tribe.members.Count / 2;
                peopleToSwap.Add(swap);
            } else
            {
                int swap = (int)Mathf.Round(tribe.members.Count / 2);
                peopleToSwap.Add(swap);
            }
            for(int i = tribe.members.Count - 1; i > 0; i--)
            {
                int swapIndex = Random.Range(0, i + 1);
                Contestant currentCon = tribe.members[i];
                Contestant conToSwap = tribe.members[swapIndex];
                tribe.members[i] = conToSwap;
                tribe.members[swapIndex] = currentCon;
            }
            Team t = new Team();
            t.tribeColor = tribe.tribeColor;
            t.name = tribe.name;
            NewTribes.Add(t);
        }
        for(int i = 0; i < NewTribes.Count; i++)
        {
            int tri = i + 1;
            if(tri > NewTribes.Count - 1)
            {
                tri = 0;
            }
            for(int j = 0; j < peopleToSwap[i]; j++)
            {
                NewTribes[tri].members.Add(TribesDup[i].members[j]);
                TribesDup[i].members.Remove(TribesDup[i].members[j]);
            }
            NewTribes[i].hiddenAdvantages = GameManager.instance.Tribes[i].hiddenAdvantages;
        }
        for(int i = 0; i < NewTribes.Count; i++)
        {
            foreach(Contestant num in TribesDup[i].members)
            {
                NewTribes[i].members.Add(num);
            }
        }
        foreach (Team tribe in NewTribes)
        {
            foreach (Contestant num in tribe.members)
            {
                num.teams.Add(tribe.tribeColor);
            }
        }
        GameManager.instance.Tribes = NewTribes;
    }
    void TribeChiefs()
    {
        GameObject SwapContext = Instantiate(GameManager.instance.Prefabs[0]);
        SwapContext.transform.parent = GameManager.instance.Canvas.transform;
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(0, SwapContext.GetComponent<RectTransform>().offsetMax.y);
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(SwapContext.GetComponent<RectTransform>().offsetMin.x, 0);
        GameManager.instance.AddGM(SwapContext, true);
        List<Contestant> Leaders = new List<Contestant>();
        List<int> curTribe = new List<int>();
        List<int> curMem = new List<int>();
        List<Team> DupTribes = new List<Team>(GameManager.instance.Tribes);
        List<Team> LastTribe = new List<Team>(DupTribes);
        List<Team> NewTribes = new List<Team>(GameManager.instance.curSwap.newTribes);
        List<Contestant> curCast = new List<Contestant>();
        Contestant extraPerson = null;
        DupTribes = DupTribes.OrderBy(i => i.members.Count).ToList();
        int contestants = 0;
        int contestantss = 0;
        int exile = 0;

        for (int i = 0; i < NewTribes.Count; i++)
        {
            if (NewTribes[i].name == "Same" || NewTribes[i].name == "same")
            {
                NewTribes[i].tribeColor = GameManager.instance.Tribes[i].tribeColor;
                NewTribes[i].name = GameManager.instance.Tribes[i].name;
            }
            NewTribes[i].hiddenAdvantages = GameManager.instance.Tribes[i].hiddenAdvantages;
        }
        for (int i = 0; i < NewTribes.Count; i++)
        {
            int r = 0;
            int a = 0;
            int ran = Random.Range(0, DupTribes[i].members.Count);
            DupTribes[i].members[ran].team = DupTribes[i].name;
            Leaders.Add(DupTribes[i].members[ran]);
            curTribe.Add(r);
            curMem.Add(a);
            DupTribes[i].members.Remove(DupTribes[i].members[ran]);
        }
        foreach(Team tribe in DupTribes)
        {
            contestants += tribe.members.Count;
            foreach (Contestant num in tribe.members)
            {
                curCast.Add(num);
            }
        }
        foreach (Team tribe in NewTribes)
        {
            contestantss = tribe.members.Count;
        }
        Contestant Exiled = new Contestant();
        
        string etext = string.Join(",", new List<Contestant>(Leaders).ConvertAll(i => i.nickname)) + " are chosen as tribe chiefs. \n " + Leaders[Leaders.Count - 1].nickname + " will shuffle up the tribes.";
        etext = etext.Replace("," + Leaders[Leaders.Count - 1].nickname, " and " + Leaders[Leaders.Count - 1].nickname);
        GameManager.instance.MakeGroup(false, null, "", "", etext, Leaders, SwapContext.transform.GetChild(0).GetChild(0), 0);
        if (curCast.Count % NewTribes.Count != 0)
        {
            //exile = -1;
        }
        int rounds = contestants / NewTribes.Count + exile;
        LastTribe = new List<Team>(DupTribes);
        LastTribe.Reverse();
        Contestant picker = Leaders[Leaders.Count - 1];

        for (int i = 0; i < rounds; i++)
        {
            for (int j = 0; j < 1; j++)
            {
                List<Team> TribesV = new List<Team>(DupTribes);
                if (TribesV.Contains(LastTribe[j]))
                {
                    TribesV.Remove(LastTribe[j]);
                }
                for (int u = 0; u < NewTribes.Count; u++)
                {
                    int ran = 0;
                    if (TribesV[curTribe[j]].members.Count > 0)
                    {
                        ran = Random.Range(0, TribesV[curTribe[j]].members.Count);
                    }
                    else
                    {
                        if (TribesV.Count > 1)
                        {
                            curTribe[j]++;
                            if (curTribe[j] > TribesV.Count)
                            {
                                curTribe[j] = 0;
                            }
                        }
                        else
                        {
                            ran = Random.Range(0, LastTribe[j].members.Count);
                        }

                    }

                    if (curMem[j] > NewTribes[u].members.Count - 2)
                    {
                        curMem[j] = NewTribes[u].members.Count - 2;
                    }
                    if (TribesV[curTribe[j]].members.Count == 0)
                    {
                        if (TribesV.Count > 1)
                        {
                            NewTribes[u].members[curMem[j]] = TribesV[curTribe[j]].members[ran];
                            DupTribes[DupTribes.IndexOf(TribesV[curTribe[j]])].members.Remove(TribesV[curTribe[j]].members[ran]);
                        }
                        else
                        {
                            NewTribes[u].members[curMem[j]] = LastTribe[j].members[ran];
                            DupTribes[DupTribes.IndexOf(LastTribe[j])].members.Remove(LastTribe[j].members[ran]);
                        }
                    }
                    else
                    {
                        NewTribes[u].members[curMem[j]] = TribesV[curTribe[j]].members[ran];
                        DupTribes[DupTribes.IndexOf(TribesV[curTribe[j]])].members.Remove(TribesV[curTribe[j]].members[ran]);
                    }
                    Contestant picked = NewTribes[u].members[curMem[j]];
                    curCast.Remove(NewTribes[u].members[curMem[j]]);
                    List<Contestant> n = new List<Contestant>() { picked, picker };
                    GameManager.instance.MakeGroup(false, null, "", "", picker.nickname + " chooses " + picked.nickname + " for Tribe #" + (u + 1), n, SwapContext.transform.GetChild(0).GetChild(0), 0);
                }
                
                LastTribe[j] = DupTribes[DupTribes.IndexOf(TribesV[curTribe[j]])];
                curTribe[j]++;
                curMem[j]++;
                if (curTribe[j] > TribesV.Count - 1)
                {
                    curTribe[j] = 0;
                }
            }
        }
        int b = 0;
        for(int i = 0; i < Leaders.Count; i++)
        {
            GameObject Group = Instantiate(GameManager.instance.GroupPrefab);
            Group.transform.parent = SwapContext.transform.GetChild(0).GetChild(0);
            Group.GetComponent<UIGroup>().tribeName.enabled = false;
            int r = i + 1;
            if(r > Leaders.Count - 1)
            {
                r = 0;
            }
            if(r > 0)
            {
                int ran = Random.Range(0, NewTribes.Count);
                NewTribes[ran].members[NewTribes[ran].members.Count - 1] = Leaders[i];
                NewTribes[ran].name = Leaders[i].team;
                List<Contestant> n = new List<Contestant>() { Leaders[i] };
                GameManager.instance.MakeGroup(false, null, "", "", Leaders[i].nickname + " can choose which tribe goes to " + Leaders[i].team + " beach. \n" + Leaders[i].nickname + " chooses Tribe #" + (ran + 1), n, SwapContext.transform.GetChild(0).GetChild(0), 0);
                foreach (Team tribe in DupTribes)
                {
                    if(NewTribes[ran].name == tribe.name)
                    {
                        NewTribes[ran].tribeColor = tribe.tribeColor;
                    }
                }
                b = ran + 1;
                if(b > NewTribes.Count - 1)
                {
                    b = 0;
                }
            } else
            {
                NewTribes[b].members[NewTribes[b].members.Count - 1] = Leaders[i];
                NewTribes[b].name = Leaders[i].team;
                List<Contestant> n = new List<Contestant>() { Leaders[i] };
                GameManager.instance.MakeGroup(false, null, "", "", Leaders[i].nickname + " joins Tribe #" + (b + 1), n, SwapContext.transform.GetChild(0).GetChild(0), 0);
                foreach (Team tribe in DupTribes)
                {
                    if (NewTribes[b].name == tribe.name)
                    {
                        NewTribes[b].tribeColor = tribe.tribeColor;
                    }
                }
            }
        }
        if(curCast.Count > 0)
        {
            if (GameManager.instance.curSwap.exile == true)
            {
                int ran = Random.Range(0, curCast.Count);
                GameManager.instance.Exiled.Add(curCast[ran]);
                Exiled = curCast[ran];
                GameManager.instance.MakeGroup(false, null, "", "", Exiled.nickname + " is not chosen and sent to exile.", GameManager.instance.Exiled, SwapContext.transform.GetChild(0).GetChild(0), 0);
            }
            else
            {
                int ran1 = Random.Range(0, curCast.Count);
                extraPerson = curCast[ran1];
                int ran = Random.Range(0, NewTribes.Count);
                NewTribes[ran].members.Add(extraPerson);
                List<Contestant> n = new List<Contestant>() { extraPerson };
                GameManager.instance.MakeGroup(false, null, "", "", extraPerson.nickname + " is not picked. \n" + extraPerson.nickname + " can choose which tribe they join. \n" + "They choose " + NewTribes[ran].name, n, SwapContext.transform.GetChild(0).GetChild(0), 0);
            }
        }
        foreach (Team tribe in NewTribes)
        {
            foreach (Contestant num in tribe.members)
            {
                num.teams.Add(tribe.tribeColor);
                foreach (Alliance alliance in GameManager.instance.Alliances)
                {
                    if (alliance.members.Contains(num))
                    {
                        if (!alliance.teams.Contains(tribe.name))
                        {
                            alliance.teams.Add(tribe.name);
                        }
                    }
                }
            }
        }
        GameManager.instance.Tribes = new List<Team>(NewTribes);
    }
    void Mutiny()
    {
        GameObject SwapContext = Instantiate(GameManager.instance.Prefabs[0]);
        SwapContext.transform.parent = GameManager.instance.Canvas.transform;
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(0, SwapContext.GetComponent<RectTransform>().offsetMax.y);
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(SwapContext.GetComponent<RectTransform>().offsetMin.x, 0);
        GameManager.instance.AddGM(SwapContext, true);
        List<Team> DupTribes = new List<Team>(GameManager.instance.Tribes);
        int mutinyLimit = 0;
        GameManager.instance.MakeGroup(false, null, "", "", "Castaways are given the opportunity to mutiny to the other tribe.", new List<Contestant>(), SwapContext.transform.GetChild(0).GetChild(0), 0);
        List<Contestant> cantMut = new List<Contestant>();
        foreach (Team tribe in DupTribes)
        {
            for (int i = 0; i < tribe.members.Count; i++)
            {
                if(!cantMut.Contains(tribe.members[i]))
                {
                    int ran = Random.Range(0, 10);
                    if (mutinyLimit < 4 && ran == 0)
                    {
                        List<Team> TribesV = new List<Team>(DupTribes);
                        if (TribesV.Contains(tribe))
                        {
                            TribesV.Remove(tribe);
                        }
                        int ran2 = Random.Range(0, TribesV.Count);
                        DupTribes[DupTribes.IndexOf(TribesV[ran2])].members.Add(tribe.members[i]);
                        List<Contestant> n = new List<Contestant>() { tribe.members[i] };
                        GameManager.instance.MakeGroup(false, null, "", "", tribe.members[i].nickname + " mutinies from " + tribe.name + " to " + TribesV[ran2].name + ".", n, SwapContext.transform.GetChild(0).GetChild(0), 0);
                        cantMut.Add(tribe.members[i]);
                        tribe.members.Remove(tribe.members[i]);
                        mutinyLimit++;
                    }
                    else
                    {

                    }
                }
            }
        }
        if(mutinyLimit > 0)
        {
            foreach (Team tribe in DupTribes)
            {
                foreach (Contestant num in tribe.members)
                {
                    num.teams.Add(tribe.tribeColor);
                }
            }
        } else
        {
            GameManager.instance.MakeGroup(false, null, "", "", "No one mutinies.", new List<Contestant>(), SwapContext.transform.GetChild(0).GetChild(0), 0);
        }
        GameManager.instance.Tribes = DupTribes;
    }
    void CISchoolyardPick()
    {
        GameObject SwapContext = Instantiate(GameManager.instance.Prefabs[0]);
        SwapContext.transform.parent = GameManager.instance.Canvas.transform;
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(0, SwapContext.GetComponent<RectTransform>().offsetMax.y);
        SwapContext.GetComponent<RectTransform>().offsetMax = new Vector2(SwapContext.GetComponent<RectTransform>().offsetMin.x, 0);
        GameManager.instance.AddGM(SwapContext, true);
        List<Team> DupTribes = new List<Team>(GameManager.instance.Tribes);
        List<Team> Genders = new List<Team>() { new Team(), new Team()};
        Team Males = new Team();
        Team Females = new Team();
        List<Contestant> Cast = new List<Contestant>();
        List<Team> NewTribes = new List<Team>();
        List<List<Contestant>> Groups = new List<List<Contestant>>();
        Groups.Add(new List<Contestant>()); Groups.Add(new List<Contestant>());
        List<List<Team>> NewGroups = new List<List<Team>>();
        List<Team> ee = new List<Team>(); ee.Add(new Team()); ee.Add(new Team());
        List<Team> eee = new List<Team>(); eee.Add(new Team()); eee.Add(new Team());
        NewGroups.Add(ee); NewGroups.Add(eee);
        
        foreach (Team tribe in DupTribes)
        {
            foreach (Contestant num in tribe.members)
            {
                if (GameManager.instance.curSwap.pickingRules == "altGender")
                {
                    if (num.gender == "M")
                    {
                        Genders[0].members.Add(num);
                    }
                    else if (num.gender == "F")
                    {
                        Genders[1].members.Add(num);
                    }
                }
                num.team = tribe.name;
                Cast.Add(num);
            }
        }
        
        int split = Cast.Count / 2;
        for (int i = Cast.Count - 1; i > 0; i--)
        {
            int swapIndex = Random.Range(0, i + 1);
            Contestant currentCon = Cast[i];
            Contestant conToSwap = Cast[swapIndex];
            Cast[i] = conToSwap;
            Cast[swapIndex] = currentCon;
        }
        for (int i = 0; i < split; i++)
        {
            Groups[0].Add(Cast[i]);
            Groups[1].Add(Cast[i + split]);
        }
        GameObject group = Instantiate(GameManager.instance.GroupPrefab);
        group.transform.parent = SwapContext.transform.GetChild(0).GetChild(0);
        group.GetComponent<UIGroup>().tribeName.enabled = false;
        string etext = "The current tribes are no more. \n \n All of the castaways are asked to stand on two blank lines.";
        if (GameManager.instance.genderEqual)
        {
            //Debug.Log(Males.members.Count + " " + Females.members.Count);
            Groups = new List<List<Contestant>>();
            Groups.Add(Genders[0].members); Groups.Add(Genders[1].members);
            etext += "\n \n Each gender will stand together.";
        }
        etext += "\n \n Each group will have two randomly picked captains and a schoolyard pick.";
        GameManager.instance.MakeGroup(false, null, "", "", etext, new List<Contestant>(), SwapContext.transform.GetChild(0).GetChild(0), 0);
        foreach (List<Contestant> contestants in Groups)
        {
            string atext = "<b>Group " + (Groups.IndexOf(contestants) + 1) + "</b>";
            GameManager.instance.MakeGroup(false, null, "", atext, "", contestants, SwapContext.transform.GetChild(0).GetChild(0), 0);
        }
        for (int i = 0; i < Groups.Count; i++)
        {
            List<Contestant> Leaders = new List<Contestant>();
            int ran = Random.Range(0, Groups[i].Count);
            Leaders.Add(Groups[i][ran]);
            NewGroups[i][0].name = "Team " + Groups[i][ran].nickname;
            NewGroups[i][0].members.Add(Groups[i][ran]);
            Groups[i].Remove(Groups[i][ran]);
            int ran2 = Random.Range(0, Groups[i].Count);
            Leaders.Add(Groups[i][ran2]);
            NewGroups[i][1].name = "Team " + Groups[i][ran2].nickname;
            NewGroups[i][1].members.Add(Groups[i][ran2]);
            Groups[i].Remove(Groups[i][ran2]);
            List<Contestant> f = new List<Contestant>(Leaders);
            f.Reverse();
            GameManager.instance.MakeGroup(false, null, "", "", Leaders[0].nickname + " and " + Leaders[1].nickname + " are the captains of Group " + (i + 1), f, SwapContext.transform.GetChild(0).GetChild(0), 0);
            float rounds = Groups[i].Count;
            int curTribe = 0;
            for (int j = 0; j < rounds; j++)
            {
                if (Groups[i].Count > 0)
                {
                    Contestant picker = NewGroups[i][curTribe].members[NewGroups[i][curTribe].members.Count - 1];
                    Contestant picked;
                    List<Contestant> GroupV = new List<Contestant>(Groups[i]);
                    foreach(Contestant num in NewGroups[i][curTribe].members)
                    {
                        List<Contestant> remove = new List<Contestant>();
                        foreach(Contestant con in GroupV)
                        {
                            if(num.team == con.team)
                            {
                                remove.Add(con);
                            }
                        }
                        foreach (Contestant con in remove)
                        {
                            GroupV.Remove(con);
                        }
                    }
                    if(GroupV.Count < 1)
                    {
                        GroupV = Groups[i];
                    }

                    int ran3 = Random.Range(0, GroupV.Count);
                    picked = GroupV[ran3];

                    NewGroups[i][curTribe].members.Add(GroupV[ran3]);
                    Groups[i].Remove(GroupV[ran3]);
                    List<Contestant> r = new List<Contestant>() { picked, picker };
                    GameManager.instance.MakeGroup(false, null, "", "", picker.nickname + " chooses " + picked.nickname + " for " + NewGroups[i][curTribe].name, r, SwapContext.transform.GetChild(0).GetChild(0), 0);
                    curTribe++;
                    if (curTribe >= NewGroups[i].Count)
                    {
                        curTribe = 0;
                    }
                    
                }
            }

        }
        for (int i = 0; i < NewGroups.Count; i++)
        {
            NewGroups[i] = NewGroups[i].OrderBy(x => x.members.Count).ToList();
            foreach(Team tribe in NewGroups[i])
            {
                GameManager.instance.MakeGroup(false, null, "", "", tribe.name + " consists of " + tribe.members.Count + " members.", tribe.members, SwapContext.transform.GetChild(0).GetChild(0), 0);
            }
        }
        
        Team n = new Team();

        string eetext = "";
        List<Contestant> rr = new List<Contestant>();
        if (NewGroups[1][0].members.Count == NewGroups[1][1].members.Count)
        {
            int ran = Random.Range(0, NewGroups[1].Count);
            foreach (Contestant num in NewGroups[1][ran].members)
            {
                n.members.Add(num);
            }

            eetext += NewGroups[1][ran].name + " is paired with ";
            rr.Add(NewGroups[1][ran].members[0]);
            NewGroups[1].Remove(NewGroups[1][ran]);
        }
        else
        {
            foreach (Contestant num in NewGroups[1][1].members)
            {
                n.members.Add(num);
            }

            eetext += NewGroups[1][1].name + " and ";
            rr.Add(NewGroups[1][1].members[0]);
            NewGroups[1].Remove(NewGroups[1][1]);
        }

        foreach (Contestant num in NewGroups[0][0].members)
        {
            n.members.Add(num);
        }

        rr.Add(NewGroups[0][0].members[0]);
        eetext += NewGroups[0][0].name;
        NewGroups[0].Remove(NewGroups[0][0]);
        Team nn = new Team();
        List<Contestant> a = new List<Contestant>();
        a.Add(NewGroups[0][0].members[0]);
        string evtext = NewGroups[0][0].name + " and ";
        foreach (Contestant num in NewGroups[0][0].members)
        {
            nn.members.Add(num);
        }

        a.Add(NewGroups[1][0].members[0]);
        evtext += NewGroups[1][0].name;
        foreach (Contestant num in NewGroups[1][0].members)
        {
            nn.members.Add(num);
        }
        NewTribes.Add(n); NewTribes.Add(nn);
        for (int i = 0; i < NewTribes.Count; i++)
        {
            NewTribes[i].name = GameManager.instance.curSwap.newTribes[i].name;
            NewTribes[i].tribeColor = GameManager.instance.curSwap.newTribes[i].tribeColor;
        }
        for (int i = 0; i < NewTribes.Count; i++)
        {
            if (NewTribes[i].name == "Same" || NewTribes[i].name == "same")
            {
                NewTribes[i].tribeColor = GameManager.instance.Tribes[i].tribeColor;
                NewTribes[i].name = GameManager.instance.Tribes[i].name;
            }
            NewTribes[i].hiddenAdvantages = GameManager.instance.Tribes[i].hiddenAdvantages;
        }
        eetext += " form " + NewTribes[0].name; rr.Reverse();
        evtext += " form " + NewTribes[1].name; a.Reverse();
        GameManager.instance.MakeGroup(false, null, "", "", eetext, rr, SwapContext.transform.GetChild(0).GetChild(0), 0);
        GameManager.instance.MakeGroup(false, null, "", "", evtext, a, SwapContext.transform.GetChild(0).GetChild(0), 0);
        
        foreach (Team tribe in NewTribes)
        {
            foreach (Contestant num in tribe.members)
            {
                num.teams.Add(tribe.tribeColor);
                foreach (Alliance alliance in GameManager.instance.Alliances)
                {
                    if (alliance.members.Contains(num))
                    {
                        if (!alliance.teams.Contains(tribe.name))
                        {
                            alliance.teams.Add(tribe.name);
                        }
                    }
                }
            }
        }
        GameManager.instance.Tribes = NewTribes; 
    }
}
