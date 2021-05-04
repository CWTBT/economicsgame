using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Various Variables
    public GameObject canvas;
    //private GameObject resetCanvas;
    public GameObject events;
    public AudioClip buttonClick;
    public AudioSource BackgroundAmbience;
    public TextMeshProUGUI title;
    public GameObject startButton;
    public TextMeshProUGUI prompt;
    public GameObject backgroundImage;
    public TextMeshProUGUI description;
    public GameObject submitButton;
    public GameObject leaderboard;
    public GameObject nextButton;
    public GameObject backButton;
    // Credits Stuff
    public GameObject creditsText;
    public GameObject creditsButton;
    // Tutorial stuff
    public GameObject howToPlayButton;
    public GameObject tutorialNextButton;
    public GameObject tutorialTextBox;
    public TextMeshProUGUI tutorialText;
    public GameObject tutorialBackground;
    private int tutorialCount;
    // Game Designer controlled "upgrade" values
    public double pollutionUpgrade1;
    public double pollutionUpgrade2;
    public double cityUpgrade1;
    public double cityUpgrade2;
    public double cityUpgrade3;
    public double eMulti;
    // Country Related | Camera Panning
    public GameObject nameEntry;
    private List<Country> playerList = new List<Country>();
    List<GameObject> CountryList = new List<GameObject>();
    private GameObject mainCamera;
    public bool botMode = false;
    string[] names = { "", "", "" };
    private Phase currentPhase = Phase.Menu;
    public GameObject nextCountryButton;
    public GameObject lastCountryButton;
    public GameObject currentCountry;
    // Treaty | Votes | Rounds | End
    private double treatyCost = 1000f;
    private double emissionsChangePct = -0.20;
    private VoteManager currentVote = new VoteManager();
    public GameObject yesButton;
    public GameObject noButton;
    private int currentPIndex = 0;
    private int completedVotes = 0;
    public int maxTurns = 5;
    private Evaluator eval;
    private List<List<Accolades>> evaluation;

    // Start is called before the first frame update
    void Start()
    {
        //resetCanvas = canvas;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (currentPhase == Phase.Names ) { submit(); }
        }
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
    public void start()
    {
        //StartCoroutine(HideTextAfterSeconds(1, title));
        nameEntry.GetComponent<TMP_InputField>().text = "";
        playerList = new List<Country>();
        StartCoroutine(RemoveAfterSeconds(1, startButton));
        prompt.text = "Player 1\nEnter your country's name!";
        nameEntry.SetActive(true);
        submitButton.SetActive(true);
        backButton.SetActive(true);
        currentPhase = Phase.Names;
    }

    // Tutorial
    public void TutorialStart()
    {
        tutorialTextBox.SetActive(true);
        tutorialText.text = "there are two numbers you need to consider in this game: GDP and Emissions. \n \n your GDP number is a measure of the total economic productivity in your country in dollars. \n \n your Emissions number tracks how much your country pollutes in gigatons of carbon.";
        tutorialNextButton.SetActive(true);
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
            backButton.GetComponent<Animator>().Play("back_show");
            //display treaty screen and start button
        }
    }

    // Credits
    public void credits()
    {
        clearMenuUI();
        backButton.SetActive(true);
        creditsText.SetActive(true);

    }

    // Go back to Main Menu
    public void Back()
    {
        //this can probably go through the 'clear menu UI function'
        StartCoroutine(RemoveAfterSeconds(30, backButton));

        //game start off
        prompt.GetComponent<Animator>().Play("hide_prompt");
        submitButton.SetActive(false);
        nameEntry.SetActive(false);

        //tutorial off
        tutorialBackground.SetActive(false);
        tutorialNextButton.SetActive(false);
        tutorialText.text = "";
        tutorialTextBox.SetActive(false);

        //credits off
        creditsText.SetActive(false);

        //reset main menu
        startButton.SetActive(true);
        title.text = "Climate Goes Political";
    }

    public void submit()
    {
        int current = playerList.Count+1;
        if(current == 1)
		{
            string[] names = { "", "", "" };
        }
        string name = nameEntry.GetComponentsInChildren<TextMeshProUGUI>()[1].text;
        if (names[0] == name || names[1] == name || names[2] == name)
        {
            prompt.text = "Player " + current + "\nEnter your country's name!\nPlease enter an available name!";
        }
        else if (name.Length > 9)
        {
            prompt.text = "Player " + current + "\nEnter your country's name!\nPlease enter a name less than 9 characters!";
        }
        else if (name.Length > 1)
        {
            nameEntry.GetComponent<TMP_InputField>().text = "";
            Country newPlayer = new Country(name);
            playerList.Add(newPlayer);
            if (playerList.Count == 4 || botMode)
            {
                ClickButton();
                clearMenuUI();
                initializeNames();
                updateLeaderboard();
                startCitiesPhase();
            }
            else
            {
                names[current - 1] = name;
                current = playerList.Count + 1;
                prompt.text = "Player " + current + "\nEnter your country's name!";
                nameEntry.GetComponent<TMP_InputField>().Select();
                nameEntry.GetComponent<TMP_InputField>().ActivateInputField();
            }
        }
        
        
    }

    private void clearMenuUI()
    {
        nameEntry.SetActive(false);
        submitButton.SetActive(false);
        backButton.SetActive(false);
        prompt.GetComponent<Animator>().Play("hide_prompt");
        StartCoroutine(HideTextAfterSeconds(1, prompt));
    }

    private void startCitiesPhase()
    {
        if (currentPhase == Phase.Names)
        {
            StartCoroutine(LoadYourAsyncScene(true, "Countries"));
            playerList.ForEach(p => p.adjustScore(eMulti));
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
        currentCountry.GetComponentInChildren<TextMeshProUGUI>().text = playerList[currentPIndex].Name + "\nScore: " + playerList[currentPIndex].Score;
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
    
    // Voting Related
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
        if (currentPhase == Phase.Cities)
        {
            if (completedVotes == maxTurns) startResultsPhase();
            else startVotePhase();
        }
        else if (currentPhase == Phase.Results) NextResult();
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

    private string GetPromptText()
    {
        string toReturn = "";
        double emissionDecPerc = emissionsChangePct * -100;
        toReturn += $"Agree to a treaty with your fellow countries to reduce " +
            $"your emissions by {emissionDecPerc}% in one turn of the game?";
        return toReturn;
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
        currentVote.DeclineVotes += 1;
        playerList[currentPIndex].Decline();
        currentPIndex = currentVote.sumVotes();
        if (currentVote.sumVotes() < 4)
        {
            leaderboard.GetComponent<Animator>().Play("show_P" + (currentPIndex + 1));
        }
        if (botMode && currentPIndex == 1) BotVote();
        if (currentVote.sumVotes() == 4) enactVotes();
    }

    private void startResultsPhase()
    {
        mainCamera.GetComponent<CameraPanning>().CurrentCity = 3;
        mainCamera.GetComponent<CameraPanning>().OnRightButtonPress();
        currentCountry.GetComponent<Animator>().Play("hide_current");
        nextCountryButton.SetActive(false);
        lastCountryButton.SetActive(false);
        currentPhase = Phase.Results;
        currentPIndex = 0;
        eval = new Evaluator(playerList);
        evaluation = eval.evaluate();
        StartCoroutine(ColorLerp(new Color(0, 0, 0, 0.5f), 0.75f));
        prompt.text = playerList[currentPIndex].Name + "'s Achievements:";
        description.text = PrintAccolades(0);
        leaderboard.GetComponent<Animator>().Play("hide_leader");
        description.GetComponent<Animator>().Play("show_desc");
        prompt.GetComponent<Animator>().Play("show_prompt");
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
            
            //Environment Related
            if(currL > prevL)
			{
                Debug.Log("Oh no! " + playerList[i].Name + " became more polluted!");
			}
            if(currL < prevL)
			{
                Debug.Log("Yay! " + playerList[i].Name + " became less polluted!");
            }
            CountryList[i].transform.Find("Land " + prevL).gameObject.SetActive(false);
            CountryList[i].transform.Find("Land " + currL).gameObject.SetActive(true);
            
            //City Related
            if (currC > prevC)
            {
                Debug.Log("Yay! " + playerList[i].Name + " grew!");
            }
            if (currC < prevC)
            {
                Debug.Log("Oh No! " + playerList[i].Name + " shrunk!");
            }
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
        double totalEmissions = 0f;
        playerList.ForEach(p => 
        { 
            if (p.HaveAgreed) 
            {
                numAgreed++;
                p.GDP -= treatyCost;
                p.Emissions -= p.Emissions * (1.0f / 5.0f);
            }
            else
            {
                p.Emissions *= (5.0f / 4.0f);
            }
            totalEmissions += p.Emissions;
        });
        double growthIncrease = 5 - (totalEmissions * 0.002f);
        playerList.ForEach(p => p.ActivateGDPGrowth());
        playerList.ForEach(p => p.Growth += (growthIncrease / 100f));
        playerList.ForEach(player =>
        {
            player.adjustCity(cityUpgrade1, cityUpgrade2, cityUpgrade3);
            player.adjustEnvironment(pollutionUpgrade1, pollutionUpgrade2);
            player.adjustScore(eMulti);
            Debug.Log(player.Name + "'s score is: " + (int)player.Score);
        });
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

    // Camera Panning Related
    public void OnRightButton()
	{
        mainCamera.GetComponent<CameraPanning>().OnRightButtonPress();
        if (currentPIndex < 3) currentPIndex++;
        else currentPIndex = 0;
        currentCountry.GetComponentInChildren<TextMeshProUGUI>().text = playerList[currentPIndex].Name + "\nScore: " + playerList[currentPIndex].Score;
	}
    public void OnLeftButton()
    {
        mainCamera.GetComponent<CameraPanning>().OnLeftButtonPress();
        if (currentPIndex > 0) currentPIndex--;
        else currentPIndex = 3;
        currentCountry.GetComponentInChildren<TextMeshProUGUI>().text = playerList[currentPIndex].Name + "\nScore: " + playerList[currentPIndex].Score;
    }
    // Sounds
    public void ClickButton()
    {
        AudioSource audio = gameObject.GetComponent<AudioSource>();

        audio.PlayOneShot(buttonClick);
    }

    // End
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
            prompt.text = playerList[currentPIndex].Name + "'s Achievements:";
            string newStr = PrintAccolades(currentPIndex);
            description.text = newStr;
        }
        mainCamera.GetComponent<CameraPanning>().OnRightButtonPress();
    }

    private string PrintAccolades(int pIndex)
    {
        string accStr = "";
        foreach (Accolades a in evaluation[pIndex])
        {
            if (a == Accolades.TopGDP) accStr += "You ended with the highest GDP! Congrats!";
            if (a == Accolades.BotEmi) accStr += "Nice! You had the lowest carbon emissions!";
            if (a == Accolades.TopEmi) accStr += "You had the highest carbon emissions.";
            if (a == Accolades.AllAgree) accStr += "You demonstrated remarkable concern for the environment by accepting every treaty.";
            if (a == Accolades.AllDecline) accStr += "You didn't agree to a single climate treaty.";
            accStr += "\n";
        }
        return accStr;
    }

    // Coroutine Related
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
            text.color = Color.Lerp(start, end, time / 2);
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

}
