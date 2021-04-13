using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
    public Country C;
    public string Header;
    public string Desc;
    public double Cost;
    public double CostPerTurn;
    public double Emissions;
    public double EmissionsPerTurn;
    
    public Event(Country c)
	{
        C = c;
	}

    public void accept()
	{
        C.adjustGDP(Cost);
        C.adjustGrowth(CostPerTurn);
        C.adjustEmissions(EmissionsPerTurn);
	}
}
