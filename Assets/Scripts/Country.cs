using System.Collections;
using System.Collections.Generic;

public class Country
{
    public string Name { get; }
    public double GDP { set; get; }
    public double Growth { set; get; }
    public double GrowthMod { set; get; }
    public double Emissions { set; get; }
    public bool HaveAgreed { get; set; }
    public double PercentageOfTotalEmissions { get; set; }
    public int Score { get; set; }
    public bool PerfectAgree { get; set; }
    public bool PerfectDisagree { get; set; }
    public int environment = 1;
    public int city = 1;
    public int previousE = 1;
    public int previousC = 1;

    public double DeltaEmissions { set; get; }
    public double DeltaGDP { set; get; }
    public double LastGDP { set; get; }
    public double LastEmissions { set; get; }

    public Country(string name)
    {
        Name = name;
        GDP = 25000;
        Growth = 0.05f;
        GrowthMod = 0f;
        Emissions = 500f;
        HaveAgreed = false;
        PercentageOfTotalEmissions = 0.0f;
        PerfectAgree = true;
        PerfectDisagree = true;
        DeltaGDP = 0;
        DeltaEmissions = 0;
    }

    public void adjustGDP(double d)
    {
        GDP += d;
    }

    public void adjustGrowth(double d)
    {
        Growth += d;
    }

    // "d" is a percentage by which to adjust emissions
    public void adjustEmissions(double d)
    {
        Emissions += (Emissions * d);
    }
    public void adjustScore(double eMulti)
	{
        Score = (int)(GDP - ((Emissions - 500) * (eMulti)))/10;
	}
    public void adjustEnvironment(double envi2, double envi3)
	{
        previousE = environment;
        if(Emissions < envi2)
		{
            environment = 1;
		} 
        else if(Emissions < envi3)
		{
            environment = 2;
		}
		else
		{
            environment = 3;
		}
	}
    public void adjustCity(double city2, double city3, double city4)
    {
        previousC = city;
        if (GDP < city2)
        {
            city = 1;
        }
        else if (GDP < city3)
        {
            city = 2;
        }
        else if (GDP < city4)
        {
            city = 3;
        }
        else
        {
            city = 4;
        }
    }

    public void Agree()
    {
        HaveAgreed = true;
        if (PerfectDisagree) PerfectDisagree = false;
    }

    public void Decline()
    {
        HaveAgreed = false;
        if (PerfectAgree) PerfectAgree= false;
    }

    public void ActivateGDPGrowth()
    {
        double growth = (Growth + GrowthMod) * GDP;
        adjustGDP(growth);
    }

    public void UpdatePercentageOfEmissions(double totalEmissions)
    {
        PercentageOfTotalEmissions = Emissions / totalEmissions;
    }

    public void UpdateDelta()
    {
        DeltaGDP = GDP - LastGDP;
        DeltaEmissions = Emissions - LastEmissions;
    }
}
