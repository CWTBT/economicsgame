using System.Collections;
using System.Collections.Generic;

public class Country
{
    public string Name { get; }
    public double GDP { set; get; }
    public double Growth { set; get; }
    public double Emissions { set; get; }
    public bool HaveAgreed { get; set; }
    public double PercentageOfTotalEmissions { get; set; }

    public Country(string name)
    {
        Name = name;
        GDP = 20000;
        Growth = 0.2f;
        Emissions = 0.5f;
        HaveAgreed = false;
        PercentageOfTotalEmissions = 0.0f;
    }

    public void adjustGDP(double d)
    {
        GDP += d;
    }

    public void adjustGrowth(double d)
    {
        Growth += d;
    }

    public void adjustEmissions(double d)
    {
        Emissions -= d;
    }

    public void Agree()
    {
        HaveAgreed = true;
    }

    public void Decline()
    {
        HaveAgreed = false;
    }

    public void UpdatePercentageOfEmissions(double totalEmissions)
    {
        PercentageOfTotalEmissions = Emissions / totalEmissions;
    }
}
