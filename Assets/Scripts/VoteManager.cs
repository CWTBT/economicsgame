using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoteManager
{
    public int AcceptVotes { set; get; }
    public int DeclineVotes { set; get; }

    public void clearVotes()
    {
        AcceptVotes = 0;
        DeclineVotes = 0;
    }

    public int sumVotes()
    {
        return AcceptVotes + DeclineVotes;
    }

}
