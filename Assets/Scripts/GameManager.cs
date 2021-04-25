using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

enum Phase
{
    Menu,
    Names,
    Cities,
    Votes
}

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
    public GameObject nextButton;

    private List<Country> playerList = new List<Country>();

    private int currentPIndex;
    private VoteManager currentVote = new VoteManager();
    private Phase currentPhase = Phase.Menu;
    private double TotalDamage = 1000.0f;
    private bool P1Ani = false;

    private double treatyCost = -5000;
    private double emissionsChangePct = -0.05;


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
        nameEntry.GetComponent<TMP_InputField>().text = "";
        Country newPlayer = new Country(name);
        playerList.Add(newPlayer);
        if (playerList.Count == 4)
        {
            clearMenuUI();
            initializeNames();
            startCitiesPhase();
        }
        else
        {
            int current = playerList.Count + 1;
            prompt.text = "Player "+current+"\nEnter your country's name!";
        }
    }

    private void clearMenuUI()
    {
        nameEntry.SetActive(false);
        submitButton.SetActive(false);
        prompt.GetComponent<Animator>().Play("hide_prompt");
        StartCoroutine(HideTextAfterSeconds(1, prompt));
    }

    private void startCitiesPhase()
    {
        if (currentPhase == Phase.Menu)
        {
            StartCoroutine(LoadYourAsyncScene(true, "Countries"));
        }
        nextButton.SetActive(true);
        currentPhase = Phase.Cities;
        nextButton.GetComponent<Animator>().Play("show_next");
        leaderboard.GetComponent<Animator>().Play("show_leader");
    }

    private void initializeNames()
    {
        TextMeshProUGUI[] nameList = leaderboard.transform.Find("Names").GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < 4; i++)
        {
            nameList[i].text = playerList[i].Name;
        }
    }

    private void setupVoteUI()
    {
        yesButton.SetActive(true);
        noButton.SetActive(true);
        StartCoroutine(ColorLerp(new Color(1, 1, 1, 1), 2));
        yesButton.GetComponent<Animator>().Play("show_agree");
        noButton.GetComponent<Animator>().Play("show_decline");
        nextButton.GetComponent<Animator>().Play("hide_next");
        description.GetComponent<Animator>().Play("show_desc");
        prompt.GetComponent<Animator>().Play("show_prompt");
        leaderboard.GetComponent<Animator>().Play("hide_leader");
        StartCoroutine(RemoveAfterSeconds(2, nextButton));

    }

    public void startVotePhase()
    {
        currentPhase = Phase.Votes;
        prompt.text = GetPromptText();
        setupVoteUI();
        currentPIndex = 0;
        leaderboard.GetComponent<Animator>().Play("show_P" + (currentPIndex + 1));
        currentVote = new VoteManager();
        currentVote.clearVotes();
    }

    private void clearVoteUI()
    {
        leaderboard.GetComponent<Animator>().Play("hide_P4");
        yesButton.GetComponent<Animator>().Play("hide_agree");
        noButton.GetComponent<Animator>().Play("hide_decline");
        description.GetComponent<Animator>().Play("hide_desc");
        prompt.GetComponent<Animator>().Play("hide_prompt");
        StartCoroutine(RemoveAfterSeconds(2, yesButton));
        StartCoroutine(RemoveAfterSeconds(2, noButton));
        StartCoroutine(HideTextAfterSeconds(2, description));
        StartCoroutine(HideTextAfterSeconds(2, prompt));
        StartCoroutine(ColorLerp(new Color(0, 0, 0, 0), 2));
    }

    public void agree()
    {
        var player = playerList[currentPIndex];
        player.adjustGDP(treatyCost);
        player.adjustEmissions(emissionsChangePct);
        currentVote.AcceptVotes += 1;
        playerList[currentPIndex].Agree();
        currentPIndex = currentVote.sumVotes();
        if (currentVote.sumVotes() < 4)
        {
            leaderboard.GetComponent<Animator>().Play("show_P" + (currentPIndex + 1));
        }
        if (currentVote.sumVotes() == 4) enactVotes();
    }

    public void decline()
    {
        currentVote.DeclineVotes += 1;
        playerList[currentPIndex].Decline();
        currentPIndex = currentVote.sumVotes();
        if (currentVote.sumVotes() < 4)
        {
            leaderboard.GetComponent<Animator>().Play("show_P" + (currentPIndex + 1));
        }
        if (currentVote.sumVotes() == 4) enactVotes();
    }

    public void enactVotes()
    {
        AdjustCountries();
        updateLeaderboard();
        clearVoteUI();
        startCitiesPhase();
    }

    private void updateLeaderboard()
    {
        TextMeshProUGUI[] gdpList = leaderboard.transform.Find("GDPs").GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] emmList = leaderboard.transform.Find("Emissions").GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < 4; i++)
        {
            string new_gdp = playerList[i].GDP.ToString("C2");
            string new_emm = playerList[i].Emissions.ToString();
            gdpList[i].text = new_gdp;
            emmList[i].text = new_emm;
        }
    }

    private void AdjustCountries()
    {
        int numAgreed = 0;
        playerList.ForEach(p => { if (p.HaveAgreed) { numAgreed++;} });
        double damageGrowthMultiplier = 2.0f - numAgreed * 0.4f;
        double damageThisRound = TotalDamage * damageGrowthMultiplier;
        TotalDamage += damageThisRound;
        AdjustGDPEmissionDamage(damageThisRound, 0.1f, 0.4f);
    }

    private void AdjustGDPEmissionDamage(double totalMoney, double agreeMultiplier, double declineMultiplier)
    {
        playerList.ForEach(p =>
        {
            if (p.HaveAgreed)
            { p.adjustGDP(totalMoney * agreeMultiplier); }
            else
            { p.adjustGDP(totalMoney * declineMultiplier); }
        });
    }

    private string GetPromptText()
    {
        string toReturn = "";
        double emissionDecPerc = emissionsChangePct * -100;
        toReturn += $"Agree to a treaty with your fellow countries to reduce " +
            $"your emissions by {emissionDecPerc}% in one turn of the game?";
        return toReturn;
    }

}
