using System.Collections;
using System.Collections.Generic;

public class Country
{
    public string Name { get; }
    public int GDP { set; get; }

    public Country(string name)
    {
        Name = name;
        GDP = 20000;
    }

    public void adjustGDP(int d)
    {
        GDP += d;
    }
}
