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
    private GameObject resetCanvas;
    public AudioClip buttonClick;
    public AudioSource BackgroundAmbience;
    public AudioClip GoodEmissions;
    public AudioClip BadEmissions;
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
    public GameObject backButton;
    public GameObject nextCountryButton;
    public GameObject lastCountryButton;
    public GameObject currentCountry;
    public GameObject creditsText;
    public GameObject creditsButton;
    public GameObject howToPlayButton;
    public GameObject tutorialNextButton;
    public GameObject tutorialTextBox;
    public TextMeshProUGUI tutorialText;
    public GameObject tutorialBackground;
    private int tutorialCount;

    public bool botMode = false;

    //Game Designer controlled "upgrade" values
    public double pollutionUpgrade1;
    public double pollutionUpgrade2;
    public double cityUpgrade1;
    public double cityUpgrade2;
    public double cityUpgrade3;

    public double eMulti;

    private List<Country> playerList = new List<Country>();
    List<GameObject> CountryList = new List<GameObject>();
    private GameObject mainCamera;

    private int currentPIndex = 0;
    private VoteManager currentVote = new VoteManager();
    private Phase currentPhase = Phase.Menu;
    private double TotalDamage = 1250.0f;

    private double treatyCost = -500;
    private double emissionsChangePct = -0.05;

    private int completedVotes = 0;
    public int maxTurns = 5;

    private Evaluator eval;
    private List<List<Accolades>> evaluation;


    // Start is called before the first frame update
    void Start()
    {
        resetCanvas = canvas;
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
            DontDestroyOnLoad(BackgroundAmbience);
            
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

    IEnumerator TextLerp(bool agreed, TextMeshProUGUI text)
    {
        float time = 0;

        Color start;
        Color end = Color.white;

        if (agreed) start = Color.green;
        else start = Color.red;
        text.color = start;
        while (time < 2)
        {
            text.color= Color.Lerp(start, end, time / 2);
            time += Time.deltaTime;
            yield return null;
        }
        text.color = end;
    }

    IEnumerator LoadYourAsyncScene(bool lerp, string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        if (lerp) { StartCoroutine(ColorLerp(new Color(0, 0, 0, 0), 0.75f)); }
        else StartCoroutine(ColorLerp(new Color(1, 1, 1, 1), 0.75f)); // reverse
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

    IEnumerator Wait(int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    public void start()
    {
        StartCoroutine(HideTextAfterSeconds(1, title));
        StartCoroutine(RemoveAfterSeconds(1, startButton));
        prompt.text = "Player 1\nEnter your country's name!";
        nameEntry.SetActive(true);
        submitButton.SetActive(true);
        backButton.SetActive(true);
        howToPlayButton.SetActive(false);
        creditsButton.SetActive(false);
        GoodAmbientSound();
    }

    public void Back()
    {
        //this can probably go through the 'clear menu UI function'
        StartCoroutine(RemoveAfterSeconds(0, backButton));

        //game start off
        prompt.GetComponent<Animator>().Play("hide_prompt");
        submitButton.SetActive(false);
        nameEntry.SetActive(false);

        //tutorial off
        tutorialBackground.SetActive(false);
        tutorialNextButton.SetActive(false);
        tutorialText.text = "";
        tutorialTextBox.SetActive(false);
        howToPlayButton.SetActive(true);

        //credits off
        creditsText.SetActive(false);
        creditsButton.SetActive(true);

        //reset main menu
        startButton.SetActive(true);
        title.text = "Climate Goes Political";
    }

    public void credits()
    {
        clearMenuUI();
        backButton.SetActive(true);
        creditsText.SetActive(true);
        StartCoroutine(HideTextAfterSeconds(0, title));
        StartCoroutine(RemoveAfterSeconds(0, startButton));

    }

    public void submit()
    {
        string name = nameEntry.GetComponentsInChildren<TextMeshProUGUI>()[1].text;
        if (name.Length > 1 && name.Length <= 9)
        {
            nameEntry.GetComponent<TMP_InputField>().text = "";
            Country newPlayer = new Country(name);
            playerList.Add(newPlayer);
            if (playerList.Count == 4 || botMode)
            {
                ClickButton();
                clearMenuUI();
                initializeNames();
                startCitiesPhase();
                
            }
            else
            {
                int current = playerList.Count + 1;
                prompt.text = "Player " + current + "\nEnter your country's name!";
            }
        }
    }

    private void clearMenuUI()
    {
        nameEntry.SetActive(false);
        submitButton.SetActive(false);
        backButton.SetActive(false);
        howToPlayButton.SetActive(false);
        creditsButton.SetActive(false);
        prompt.GetComponent<Animator>().Play("hide_prompt");
        StartCoroutine(HideTextAfterSeconds(1, prompt));
    }

    private void startCitiesPhase()
    {
        if (currentPhase == Phase.Menu)
        {
            StartCoroutine(LoadYourAsyncScene(true, "Countries"));
        }
        else
        {
            mainCamera.transform.position = new Vector3(-9.7f, 4.9f, -1f);
            mainCamera.GetComponent<CameraPanning>().CurrentCity = 0;
            TextMeshProUGUI[] nameList = leaderboard.transform.Find("Names").GetComponentsInChildren<TextMeshProUGUI>();
            for (int i = 0; i < playerList.Count; i++)
            {
                StartCoroutine(TextLerp(playerList[i].HaveAgreed, nameList[i])); 
            }
            currentPIndex = 0;
        }
        nextButton.SetActive(true);
        nextCountryButton.SetActive(true);
        lastCountryButton.SetActive(true);
        currentPhase = Phase.Cities;
        nextButton.GetComponent<Animator>().Play("show_next");
        leaderboard.GetComponent<Animator>().Play("show_leader");
        currentCountry.GetComponentInChildren<TextMeshProUGUI>().text = playerList[currentPIndex].Name;
        currentCountry.GetComponent<Animator>().Play("show_current");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //Connects the Cities Phase objects to the GameManager
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
        CountryList.Add(GameObject.Find("Country1"));
        CountryList.Add(GameObject.Find("Country2"));
        CountryList.Add(GameObject.Find("Country3"));
        CountryList.Add(GameObject.Find("Country4"));
        mainCamera = GameObject.Find("Main Camera");
        Debug.Log("Main Camera Set");
    }

    private void initializeNames()
    {
        if (botMode)
        {
            for (int i = 1; i < 4; i++)
            {
                string name = "Player " + (i + 1);
                Country newPlayer = new Country(name);
                playerList.Add(newPlayer);
            }
        }

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
        StartCoroutine(ColorLerp(new Color(0.61f, 0.83f, 0.89f, 1), 0.75f));
        yesButton.GetComponent<Animator>().Play("show_agree");
        noButton.GetComponent<Animator>().Play("show_decline");
        nextButton.GetComponent<Animator>().Play("hide_next");
        description.GetComponent<Animator>().Play("show_desc");
        prompt.GetComponent<Animator>().Play("show_prompt");
        leaderboard.GetComponent<Animator>().Play("hide_leader");
        currentCountry.GetComponent<Animator>().Play("hide_current");
        //StartCoroutine(RemoveAfterSeconds(2, nextButton));
        nextCountryButton.SetActive(false);
        lastCountryButton.SetActive(false);

    }

    public void Next()
    {
        BackgroundAmbience.Stop();
        if (currentPhase == Phase.Cities)
        {
            if (completedVotes == maxTurns) startResultsPhase();
            else startVotePhase();
        }
        else if (currentPhase == Phase.Results) NextResult();
    }

    private void NextResult()
    {
        if (currentPIndex == 3)
        {
            //back to main menu
            Destroy(gameObject);
            Destroy(canvas);
            Destroy(events);
            SceneManager.LoadScene("MainMenu");
            //canvas = resetCanvas;
            Debug.Log("that's all folks");
        }
        else
        {
            currentPIndex++;
            leaderboard.GetComponent<Animator>().Play("show_P" + (currentPIndex + 1));
            prompt.text = "Player " + (currentPIndex + 1) + " Results";
            string newStr = PrintAccolades(currentPIndex);
            description.text = newStr;
        }
    }

    private string PrintAccolades(int pIndex)
    {
        string accStr = "";
        foreach (Accolades a in evaluation[pIndex])
        {
            if (a == Accolades.TopGDP) accStr += "You ended with the highest GDP! Congrats!";
            if (a == Accolades.BotEmi) accStr += "Nice! You had the lowest carbon emissions!";
            if (a == Accolades.TopEmi) accStr += "You had the highest carbon emissions.";
            accStr += "\n";
        }
        return accStr;
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

    private void startResultsPhase()
    {
        currentPhase = Phase.Results;
        currentPIndex = 0;
        eval = new Evaluator(playerList);
        evaluation = eval.evaluate();
        StartCoroutine(ColorLerp(new Color(0, 0, 0, 0.5f), 0.75f));
        prompt.text = "Player " + (currentPIndex + 1) + " Results";
        description.text = PrintAccolades(0);
        leaderboard.GetComponent<Animator>().Play("hide_leader");
        leaderboard.GetComponent<Animator>().Play("show_P" + (currentPIndex + 1));
        description.GetComponent<Animator>().Play("show_desc");
        prompt.GetComponent<Animator>().Play("show_prompt");
    }

    private void clearVoteUI()
    {
        StartCoroutine(ColorLerp(new Color(0, 0, 0, 0), 0.75f));
        leaderboard.GetComponent<Animator>().Play("hide_P4");
        yesButton.GetComponent<Animator>().Play("hide_agree");
        noButton.GetComponent<Animator>().Play("hide_decline");
        description.GetComponent<Animator>().Play("hide_desc");
        prompt.GetComponent<Animator>().Play("hide_prompt");
        //StartCoroutine(RemoveAfterSeconds(1, yesButton));
        //StartCoroutine(RemoveAfterSeconds(1, noButton));
        //StartCoroutine(HideTextAfterSeconds(1, description));
        //StartCoroutine(HideTextAfterSeconds(1, prompt));
    }

    private void BotVote()
    {
        for (int i = 0; i < 3; i++)
        {
            int choice = Random.Range(0, 2);
            if (choice == 0) agree();
            else decline();
        }
    }

    public void agree()
    {
        var player = playerList[currentPIndex];
        player.Growth = 0.1f;
        player.ActivateGDPGrowth();
        player.adjustGDP(treatyCost);
        player.adjustEmissions(emissionsChangePct);
        currentVote.AcceptVotes += 1;
        playerList[currentPIndex].Agree();
        currentPIndex = currentVote.sumVotes();
        if (currentVote.sumVotes() < 4)
        {
            leaderboard.GetComponent<Animator>().Play("show_P" + (currentPIndex + 1));
        }
        if (botMode && currentPIndex == 1) BotVote();
        if (currentVote.sumVotes() == 4) enactVotes();
    }

    public void decline()
    {
        var player = playerList[currentPIndex];
        player.Growth = 0.15f;
        player.ActivateGDPGrowth();
        currentVote.DeclineVotes += 1;
        player.adjustEmissions(-emissionsChangePct);
        playerList[currentPIndex].Decline();
        currentPIndex = currentVote.sumVotes();
        if (currentVote.sumVotes() < 4)
        {
            leaderboard.GetComponent<Animator>().Play("show_P" + (currentPIndex + 1));
        }
        if (botMode && currentPIndex == 1) BotVote();
        if (currentVote.sumVotes() == 4) enactVotes();
    }

    public void enactVotes()
    {
        completedVotes++;
        AdjustCountries();
        updateLeaderboard();
        clearVoteUI();
        //Updates the map
        for (int i = 0; i < 4; i++)
        {
            int prevL = playerList[i].previousE;
            int currL = playerList[i].environment;
            int prevC = playerList[i].previousC;
            int currC = playerList[i].city;
                
            CountryList[i].transform.Find("Land " + prevL).gameObject.SetActive(false);
            CountryList[i].transform.Find("Land " + currL).gameObject.SetActive(true);
            CountryList[i].transform.Find("City " + prevC).gameObject.SetActive(false);
            CountryList[i].transform.Find("City " + currC).gameObject.SetActive(true);
 

        }
        startCitiesPhase();
    }

    private void updateLeaderboard()
    {
        TextMeshProUGUI[] gdpList = leaderboard.transform.Find("GDPs").GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] emmList = leaderboard.transform.Find("Emissions").GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < 4; i++)
        {
            string new_gdp = playerList[i].GDP.ToString("C2");
            string new_emm = playerList[i].Emissions.ToString("F2");
            gdpList[i].text = "GDP: " + new_gdp;
            emmList[i].text = "EMISSIONS: " + new_emm + "GT";
        }
    }

    private void AdjustCountries()
    {
        int numAgreed = 0;
        playerList.ForEach(p => { if (p.HaveAgreed) { numAgreed++; } });
        double damageGrowthMultiplier = 2.0f - numAgreed * 0.45f;
        double damageThisRound = TotalDamage * damageGrowthMultiplier;
        TotalDamage += damageThisRound;
        AdjustGDPEmissionDamage(damageThisRound, 0.05f, 0.4f);
        playerList.ForEach(player =>
        {
            player.adjustCity(cityUpgrade1, cityUpgrade2, cityUpgrade3);
            player.adjustEnvironment(pollutionUpgrade1, pollutionUpgrade2);
            player.adjustScore(eMulti);
            Debug.Log(player.Name + "'s score is: " + (int)player.Score);
        });
    }

    private void AdjustGDPEmissionDamage(double totalMoney, double agreeMultiplier, double declineMultiplier)
    {
        playerList.ForEach(p =>
        {
            if (p.HaveAgreed)
            { p.adjustGDP(-totalMoney * agreeMultiplier); }
            else
            { p.adjustGDP(-totalMoney * declineMultiplier); }
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

    //Camera Panning Related
    public void OnRightButton()
	{
        mainCamera.GetComponent<CameraPanning>().OnRightButtonPress();
        if (currentPIndex < 3) currentPIndex++;
        else currentPIndex = 0;
        currentCountry.GetComponentInChildren<TextMeshProUGUI>().text = playerList[currentPIndex].Name;
    
        CameraPanning cameraPanning = mainCamera.GetComponent<CameraPanning>();
        if (playerList[cameraPanning.CurrentCity].Emissions >= pollutionUpgrade1)
        {
            BadAmbientSound();
        } else
        {
            GoodAmbientSound();
        }
	}
    public void OnLeftButton()
    {
        mainCamera.GetComponent<CameraPanning>().OnLeftButtonPress();
        if (currentPIndex > 0) currentPIndex--;
        else currentPIndex = 3;
        currentCountry.GetComponentInChildren<TextMeshProUGUI>().text = playerList[currentPIndex].Name;
    
        CameraPanning cameraPanning = mainCamera.GetComponent<CameraPanning>();
        if (playerList[cameraPanning.CurrentCity].Emissions >= pollutionUpgrade1)
        {
            BadAmbientSound();
        }
        else
        {
            GoodAmbientSound();
        }
    }
    //Sounds
    public void ClickButton()
    {
        AudioSource audio = gameObject.GetComponent<AudioSource>();

        audio.PlayOneShot(buttonClick);
    }

    public void GoodAmbientSound()
    {
        BackgroundAmbience.Stop();
        BackgroundAmbience.PlayOneShot(GoodEmissions);
    }

    public void BadAmbientSound()
    {
        BackgroundAmbience.Stop();
        BackgroundAmbience.PlayOneShot(BadEmissions);
    }
    //Tutorial
    public void TutorialStart()
    {
        StartCoroutine(HideTextAfterSeconds(0, title));
        StartCoroutine(RemoveAfterSeconds(0, startButton));
        tutorialTextBox.SetActive(true);
        tutorialText.text = "there are two numbers you need to consider in this game: GDP and Emissions. \n \n your GDP number is a measure of the total economic productivity in your country in dollars. \n \n your Emissions number tracks how much your country pollutes in gigatons of carbon.";
        tutorialNextButton.SetActive(true);
        howToPlayButton.SetActive(false);
        creditsButton.SetActive(false);
        GoodAmbientSound();
        tutorialCount = 0;
        tutorialBackground.SetActive(true);
        leaderboard.GetComponent<Animator>().Play("show_P1");
    }

    public void TutorialNext()
    {
        tutorialCount++;

        if (tutorialCount == 1)
        {
            tutorialText.text = "As your GDP grows, your country will expand";
            //fade in city level 2 to show city growth
            //point to GDP on panel
        }
        else if (tutorialCount == 2)
        {
            tutorialText.text = "Your GDP will grow faster if you emit more carbon, but the environment will be affected, such as rising sea levels \n \n Watch out! Harming the envrionment will have repercussions in the long run";
            // fade in environment level 2 to show rising sea levels
            //point to emissions on panel
        }
        else if (tutorialCount == 3)
        {
            leaderboard.GetComponent<Animator>().Play("hide_P1");
            tutorialText.text = "Consider the treaty each turn carefully to balance your GDP growth and environmental impact \n \n Remember, other countries' decisions will have an impact on you";
            //startButton.SetActive(true);
            tutorialNextButton.SetActive(false);
            backButton.SetActive(true);
            //display treaty screen and start button
        }
    }
}
