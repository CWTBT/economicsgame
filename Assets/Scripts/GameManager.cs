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
    public GameObject backgroundImage;
    public TextMeshProUGUI description;
    public GameObject submitButton;
    public GameObject leaderboard;

    private List<Country> playerList = new List<Country>();

    private Event currentEvent;
    private int currentPIndex;
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

    IEnumerator ColorLerp(Color endValue, float duration)
    {
        float time = 0;
        Image sprite = backgroundImage.GetComponent<Image>();
        Color startValue = sprite.color;

        while (time < duration)
        {
            sprite.color = Color.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        sprite.color = endValue;
    }

    IEnumerator LoadYourAsyncScene(bool lerp, string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        if (lerp) { StartCoroutine(ColorLerp(new Color(0, 0, 0, 0), 2)); }
        else StartCoroutine(ColorLerp(new Color(1, 1, 1, 1), 2)); // reverse
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
            prompt.GetComponent<Animator>().Play("hide_prompt");
            StartCoroutine(HideTextAfterSeconds(1, prompt));
            initializeNames();
            StartCoroutine(LoadYourAsyncScene(true, "Countries"));
            leaderboard.GetComponent<Animator>().Play("show_leader");
        }
        else
        {
            // Bug: Can't clear text field after hitting enter

            int current = playerList.Count + 1;
            prompt.text = "Player "+current+"\nEnter your country's name!";
            nameEntry.GetComponentsInChildren<TextMeshProUGUI>()[1].text = string.Empty;
        }
    }

    private void initializeNames()
    {
        TextMeshProUGUI[] nameList = leaderboard.transform.Find("Names").GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < 4; i++)
        {
            nameList[i].text = playerList[i].Name;
        }
    }

    public void startVotePhase()
    {
        currentPIndex = 0;
        currentVote = new VoteManager();
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
        playerList[currentPIndex].adjustEmissions(currentEvent.EmissionsPerTurn);
        playerList[currentPIndex].adjustGDP(currentEvent.Cost);
        playerList[currentPIndex].adjustGrowth(currentEvent.CostPerTurn);
    }
}
