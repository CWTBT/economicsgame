using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evaluator
{
    public List<Country> players;

    public Evaluator(List<Country> players)
    {
        this.players = players;
    }

    /* Returns a list of accolades that each
     * player achieved. Index in this list
     * corresponds to the index of the
     * relevent player in "players"
     */
    public List<List<Accolades>> evaluate()
    {
        List<List<Accolades>> accList = new List<List<Accolades>> {
            new List<Accolades>(),
            new List<Accolades>(),
            new List<Accolades>(),
            new List<Accolades>()
        };

        double topGDP = 0;
        double topEmi = 0;
        double botEmi = 999;

        List<int> topGDPs = new List<int>();
        List<int> topEmis = new List<int>();
        List<int> botEmis = new List<int>();
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GDP > topGDP)
            {
                topGDP = players[i].GDP;
                topGDPs = new List<int> { i };
            }
            else if (players[i].GDP == topGDP) topGDPs.Add(i);


            if (players[i].Emissions > topEmi)
            {
                topEmi = players[i].Emissions;
                topEmis = new List<int> { i };
            }
            else if (players[i].Emissions == topEmi) topEmis.Add(i);

            if (players[i].Emissions < botEmi)
            {
                botEmi = players[i].Emissions;
                botEmis = new List<int> { i };
            }
            else if (players[i].Emissions == botEmi) botEmis.Add(i);

            if (players[i].PerfectAgree) accList[i].Add(Accolades.AllAgree);
            else if (players[i].PerfectDisagree) accList[i].Add(Accolades.AllDecline);
        }

        for (int i = 0; i < topGDPs.Count; i++) accList[topGDPs[i]].Add(Accolades.TopGDP);
        for (int i = 0; i < topEmis.Count; i++) accList[topEmis[i]].Add(Accolades.TopEmi);
        for (int i = 0; i < botEmis.Count; i++) accList[botEmis[i]].Add(Accolades.BotEmi);

        return accList;
    }
}
