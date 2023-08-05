using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using SeasonParts;

public class OneWorld : MonoBehaviour
{
    public void TribeStatus(List<Team> teams)
    {
        GameManager.Instance.owStatus = true;
        GameObject EpisodeStatus = Instantiate(GameManager.Instance.Prefabs[0]);
        EpisodeStatus.transform.parent = GameManager.Instance.Canvas.transform;
        EpisodeStatus.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStatus.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStatus.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStatus.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStatus.name = "Camp Status";

        Team tribe = new Team() { tribeColor = Color.white };

        for(int i = 0; i < teams.Count; i++)
        {
            tribe.members.Concat(teams[i].members);
            tribe.name += "\n" + "<color=#" + ColorUtility.ToHtmlStringRGBA(teams[i].tribeColor) + ">" + teams[i].name + "</color>";
            //tribe.hiddenAdvantages.Concat(teams[i].hiddenAdvantages);
        }
        GameManager.Instance.MakeGroup(true, tribe, "", "", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), 0);
        bool adv = false;
        float ed = 0;

        foreach(Team team in teams)
        {
            foreach (HiddenAdvantage hid in team.hiddenAdvantages)
            {
                if (hid.hideAt <= GameManager.Instance.curEp + 1 && GameManager.Instance.currentContestants >= hid.advantage.expiresAt)
                {
                    if (!adv)
                    {
                        adv = true;
                        GameManager.Instance.MakeGroup(false, null, "", "<b>Advantages</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                    }
                    string nam = hid.name;
                    if (!hid.name.Contains("Immunity Idol"))
                    {
                        nam = "secret advantage";
                    }
                    string atext = "The " + nam + " is currently hidden.";
                    if (!hid.hidden)
                    {
                        if (hid.reHidden)
                        {
                            adv = true;
                            atext = "The " + nam + " is not currently hidden.";
                        }
                        else
                        {

                            atext = "";
                        }
                    }
                    if (atext != "")
                    {
                        GameManager.Instance.MakeGroup(false, null, "", atext, "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                    }
                    foreach (Contestant num in tribe.members)
                    {
                        if (hid.hidden)
                        {
                            int ran = Random.Range(0, 10 - num.stats.Strategic);
                            if (ran == 1)
                            {
                                Advantage av = Instantiate(hid.advantage);
                                av.nickname = hid.name;
                                if (hid.temp)
                                {
                                    av.temp = true;
                                    av.length = hid.length;
                                }
                                hid.hidden = false;
                                List<Contestant> n = new List<Contestant>() { num };


                                if (team.members.Contains(num))
                                {
                                    
                                    if (hid.advantage.type == "HalfIdol")
                                    {
                                        num.halfIdols.Add(num);
                                    }
                                    else
                                    {
                                        num.advantages.Add(av);
                                    }
                                    GameManager.Instance.MakeGroup(false, null, "", "", num.nickname + " finds the " + hid.name + "\n\n" + av.description, n, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);

                                } else
                                {
                                    GameManager.Instance.MakeGroup(false, null, "", "", num.nickname + " finds the " + hid.name + "\n\n" + av.description + "\n\nThis advantage needs to be given to someone who's on the tribe it belongs to.", n, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);

                                    Contestant gamer = team.members[Random.Range(0, team.members.Count)];
                                    n.Add(gamer);
                                    n.Reverse();
                                    if (hid.advantage.type == "HalfIdol")
                                    {
                                        gamer.halfIdols.Add(gamer);
                                    }
                                    else
                                    {
                                        gamer.advantages.Add(av);
                                    }
                                    GameManager.Instance.MakeGroup(false, null, "", "", num.nickname + " gives the " + hid.name + " to " + gamer.name + ".", n, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);

                                }

                            }
                        }
                    }
                }
            }

        }

        List<Contestant> u = new List<Contestant>();
        foreach (Contestant num in tribe.members)
        {
            ContestantEvents.Instance.UpdateRelationships(num, tribe.members);
            List<Contestant> w = new List<Contestant>() { num };
            foreach (Advantage advantage in num.advantages)
            {
                if (!adv)
                {
                    adv = true;
                    GameManager.Instance.MakeGroup(false, null, "", "<b>Advantages</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                }
                string extra = "";
                if (advantage.temp)
                {
                    if (advantage.length > 1)
                    {
                        extra = "\n \nThis can be used at the next " + advantage.length + " tribal councils.";
                    }
                    else
                    {
                        extra = "\n \nThis can be used at the next tribal council.";
                    }
                }
                if (GameManager.Instance.currentContestants == advantage.expiresAt)
                {
                    extra = "\n \nThis is the last round to use it.";
                }
                int a = 0;
                if (advantage.onlyUsable.Count > 0)
                {
                    extra = "\n \nIt can't be used this round.";
                    foreach (int numb in advantage.onlyUsable)
                    {
                        if (GameManager.Instance.currentContestants == numb)
                        {
                            a = numb;
                        }
                    }
                }
                else
                {
                    a = 0;
                }
                if (a != 0)
                {
                    if (a != advantage.onlyUsable[advantage.onlyUsable.Count - 1])
                    {
                        extra = "\n \nIt can be used this round.";
                    }
                    else
                    {
                        extra = "\n \nIt can be used this round.\n \nThis is the last round to use it.";
                    }
                }
                GameManager.Instance.MakeGroup(false, null, "", "", num.nickname + " has the " + advantage.nickname + extra, w, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
            }
            int comb = 0;
            foreach (Contestant half in num.halfIdols)
            {
                u = new List<Contestant>() { half };
                if (num.halfIdols.Count > 1)
                {
                    if (tribe.members.Contains(half))
                    {
                        comb++;
                    }
                }
                else
                {
                    GameManager.Instance.MakeGroup(false, null, "", "", num.nickname + " has the Half Idol.", u, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                }
                adv = true;
            }
            if (num.halfIdols.Count > 1)
            {
                if (comb == 2)
                {
                    foreach (Contestant half in num.halfIdols)
                    {
                        u = new List<Contestant>() { half };
                        GameManager.Instance.MakeGroup(false, null, "", "", num.nickname + " has the Half Idol that is ready to be combined into a full idol.", u, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        num.halfIdols.Reverse();
                    }

                    GameManager.Instance.MakeGroup(false, null, "", "", num.halfIdols[1].nickname + " lets " + num.halfIdols[0].nickname + " have the Combined Hidden Immunity Idol.", num.halfIdols, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                    Advantage av = Instantiate(GameManager.Instance.HiddenIdol);
                    av.nickname = "Combined Hidden Immunity Idol";
                    num.halfIdols[0].advantages.Add(av);
                    if (tribe.members.IndexOf(num.halfIdols[0]) < tribe.members.IndexOf(num) || tribe.members.IndexOf(num.halfIdols[0]) == tribe.members.IndexOf(num))
                    {
                        List<Contestant> ww = new List<Contestant>() { num.halfIdols[0] };
                        GameManager.Instance.MakeGroup(false, null, "", "", num.halfIdols[0].nickname + " has the " + av.nickname, ww, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                    }
                    num.halfIdols = new List<Contestant>();
                }
                else
                {
                    num.halfIdols = new List<Contestant>();
                }
            }

            if (num.halfIdols.Count == 1)
            {
                List<Contestant> TribeV = new List<Contestant>(tribe.members);
                TribeV.Remove(num);
                num.halfIdols.Add(TribeV[Random.Range(0, TribeV.Count)]);
                List<Contestant> ex = new List<Contestant>();
                num.halfIdols.Reverse();
                GameManager.Instance.MakeGroup(false, null, "", "", num.nickname + " transfers the half idol to " + num.halfIdols[0].nickname, num.halfIdols, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                num.halfIdols.Reverse();
            }
        }
        if (!adv && GameManager.Instance.advant)
        {
            GameManager.Instance.MakeGroup(false, null, "", "There are no secret advantages.", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
        }
        foreach (Alliance alliance in GameManager.Instance.Alliances)
        {
            if (alliance.teams.Contains(tribe.name))
            {
                ed++;
            }
        }
        float d = 0;
        GameManager.Instance.MakeGroup(false, null, "", "<b>Alliances</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
        GameManager.Instance.all = true;
        foreach (Alliance alliance in GameManager.Instance.Alliances)
        {
            if (alliance.teams.Contains(tribe.name))
            {
                float ee = 0;
                foreach (Contestant num in tribe.members)
                {
                    if (alliance.members.Contains(num))
                    {
                        ee++;
                    }
                }
                if (ee == 0)
                {
                    alliance.teams.Remove(tribe.name);

                }
            }

            if (alliance.teams.Contains(tribe.name))
            {
                float strength = Mathf.Round((float)alliance.members.ConvertAll(x => ContestantEvents.Instance.GetLoyalty(x, alliance.members)).Average());
                if (strength < 1)
                {
                    strength = 1;
                }
                GameManager.Instance.MakeGroup(false, null, "name", alliance.name + " (" + strength + " Strength)", "", alliance.members, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);
                if (ed < 2 && !adv && tribe.hiddenAdvantages.Count < 1)
                {
                    EpisodeStatus.transform.GetChild(0).GetComponent<VerticalLayoutGroup>().spacing = -90;
                }
                else
                {

                }

                d++;
            }
        }
        GameManager.Instance.all = false;
        if (d == 0)
        {
            GameManager.Instance.MakeGroup(false, null, "", "There are no alliances", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), 0);
        }
        GameManager.Instance.AddGM(EpisodeStatus, true, 0);
        GameManager.Instance.owStatus = false;

    }

    public void TribeEvents(List<Team> teams)
    {
        GameManager.Instance.owStatus = true;

        GameObject EpisodeStatus = Instantiate(GameManager.Instance.Prefabs[0]);
        EpisodeStatus.transform.parent = GameManager.Instance.Canvas.transform;
        EpisodeStatus.GetComponent<RectTransform>().offsetMax = new Vector2(0, EpisodeStatus.GetComponent<RectTransform>().offsetMax.y);
        EpisodeStatus.GetComponent<RectTransform>().offsetMax = new Vector2(EpisodeStatus.GetComponent<RectTransform>().offsetMin.x, 0);
        EpisodeStatus.name = "Camp Status";

        Team tribe = new Team() {tribeColor=Color.white };

        for (int i = 0; i < teams.Count; i++)
        {
            tribe.members.AddRange(teams[i].members);
            tribe.name += "\n" + "<color=#" + ColorUtility.ToHtmlStringRGBA(teams[i].tribeColor) + ">" + teams[i].name + "</color>";
            //tribe.hiddenAdvantages.Concat(teams[i].hiddenAdvantages);
        }
        GameManager.Instance.MakeGroup(true, tribe, "", "", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), 0);
        bool adv = false;
        float ed = 0;


        foreach (Team team in teams)
        {
            foreach (HiddenAdvantage hid in team.hiddenAdvantages)
            {
                if (hid.hideAt <= GameManager.Instance.curEp + 1 && GameManager.Instance.currentContestants >= hid.advantage.expiresAt)
                {
                    if (!adv)
                    {
                        adv = true;
                        GameManager.Instance.MakeGroup(false, null, "", "<b>Advantages</b>", "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                    }
                    string nam = hid.name;
                    if (!hid.name.Contains("Immunity Idol"))
                    {
                        nam = "secret advantage";
                    }
                    string atext = "The " + nam + " is currently hidden.";
                    if (!hid.hidden)
                    {
                        if (hid.reHidden)
                        {
                            adv = true;
                            atext = "The " + nam + " is not currently hidden.";
                        }
                        else
                        {

                            atext = "";
                        }
                    }
                    if (atext != "")
                    {
                        GameManager.Instance.MakeGroup(false, null, "", atext, "", new List<Contestant>(), EpisodeStatus.transform.GetChild(0).GetChild(0), -10);
                    }
                    foreach (Contestant num in tribe.members)
                    {
                        if (hid.hidden)
                        {
                            int ran = Random.Range(0, 10 - num.stats.Strategic);
                            if (ran == 1)
                            {
                                Advantage av = Instantiate(hid.advantage);
                                av.nickname = hid.name;
                                if (hid.temp)
                                {
                                    av.temp = true;
                                    av.length = hid.length;
                                }
                                hid.hidden = false;
                                List<Contestant> n = new List<Contestant>() { num };


                                if (team.members.Contains(num))
                                {

                                    if (hid.advantage.type == "HalfIdol")
                                    {
                                        num.halfIdols.Add(num);
                                    }
                                    else
                                    {
                                        num.advantages.Add(av);
                                    }
                                    GameManager.Instance.MakeGroup(false, null, "", "", num.nickname + " finds the " + hid.name + "\n\n" + av.description, n, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);

                                }
                                else
                                {
                                    GameManager.Instance.MakeGroup(false, null, "", "", num.nickname + " finds the " + hid.name + "\n\n" + av.description + "\n\nThis advantage needs to be given to someone who's on the tribe it belongs to.", n, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);

                                    Contestant gamer = team.members[Random.Range(0, team.members.Count)];
                                    n.Add(gamer);
                                    n.Reverse();
                                    if (hid.advantage.type == "HalfIdol")
                                    {
                                        gamer.halfIdols.Add(gamer);
                                    }
                                    else
                                    {
                                        gamer.advantages.Add(av);
                                    }
                                    GameManager.Instance.MakeGroup(false, null, "", "", num.nickname + " gives the " + hid.name + " to " + gamer.nickname + ".", n, EpisodeStatus.transform.GetChild(0).GetChild(0), 10);

                                }

                            }
                        }
                    }
                }
            }

        }

        GameManager.Instance.EventsChances(tribe, EpisodeStatus);

        foreach(Team team in teams)
        {
            if(GameManager.Instance.LosingTribes.Contains(team))
            {
                GameManager.Instance.TribeTargeting(team);
            }
        }
        GameManager.Instance.AddGM(EpisodeStatus, true, 0);
        GameManager.Instance.owStatus = false;

    }
}
