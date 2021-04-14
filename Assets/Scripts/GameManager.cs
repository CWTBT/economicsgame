using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject canvas;
    public GameObject events;

    public TextMeshProUGUI title;
    public GameObject startButton;
    public TextMeshProUGUI prompt;
    public GameObject nameEntry;
    public GameObject yesButton;
    public GameObject noButton;
    public TextMeshProUGUI description;
    public GameObject submitButton;

    private List<Country> playerList = new List<Country>();

    private Event currentEvent;
    private VoteManager currentVote;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(canvas);
            DontDestroyOnLoad(events);

        }
        else
        {
            Destroy(gameObject);
            Destroy(canvas);
            Destroy(events);
        }
    }

    IEnumerator RemoveAfterSeconds(int seconds, GameObject obj)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }

    IEnumerator HideTextAfterSeconds(int seconds, TextMeshProUGUI t)
    {
        yield return new WaitForSeconds(seconds);
        t.text = "";
    }

    public void start()
    {
        StartCoroutine(HideTextAfterSeconds(1, title));
        StartCoroutine(RemoveAfterSeconds(1, startButton));
        prompt.text = "Player 1\nEnter your country's name!";
        nameEntry.SetActive(true);
        submitButton.SetActive(true);
    }

    public void submit()
    {
        string name = nameEntry.GetComponentsInChildren<TextMeshProUGUI>()[1].text;
        Country newPlayer = new Country(name);
        playerList.Add(newPlayer);
        if (playerList.Count == 4)
        {
            nameEntry.SetActive(false);
            submitButton.SetActive(false);
            // More here later
        }
        else
        {
            // Bug: Can't clear text field after hitting enter
            int current = playerList.Count + 1;
            prompt.text = "Player "+current+"\nEnter your country's name!";
            nameEntry.GetComponentsInChildren<TextMeshProUGUI>()[1].text = string.Empty;
        }
    }

    public void agree()
    {
        enactAgree();
        currentVote.AcceptVotes += 1;
        if (currentVote.sumVotes() == 4) enactVotes();
    }

    public void decline()
    {
        currentVote.DeclineVotes += 1;
        if (currentVote.sumVotes() == 4) enactVotes();
    }

    public void enactVotes()
    {
        
    }

    public void enactAgree()
	{
        Country currentPlayer = playerList[currentVote.sumVotes()];
        currentPlayer.adjustEmissions(currentEvent.EmissionsPerTurn);
        currentPlayer.adjustGDP(currentEvent.Cost);
        currentPlayer.adjustGrowth(currentEvent.CostPerTurn);

    }
}
