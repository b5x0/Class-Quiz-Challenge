using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// The central class that manages the overall game flow, including rounds,
/// scoring, timing, and UI updates.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Variables

    [Header("Game Configuration")]
    [Tooltip("The total number of rounds in a full game session.")]
    [SerializeField] private int totalRounds = 10;
    [Tooltip("The time limit for each round in seconds.")]
    [SerializeField] private float roundTime = 60f;

    [Header("Question Pool")]
    [Tooltip("The master list of all possible quiz questions for the game.")]
    [SerializeField] private List<QuizItem> allQuizItems;

    [Header("Component References")]
    [SerializeField] private LineManager lineManager;

    [Header("UI Element References")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Image[] pictureHolders;
    [SerializeField] private TextMeshProUGUI[] nameHolders;
    [SerializeField] private Button checkResultButton;


    [Header("UI Popups")]
    [SerializeField] private GameObject winPopup;
    [SerializeField] private GameObject losePopup;
    [SerializeField] private Button repeatButtonOnWinPopup;
    [SerializeField] private Image[] starImageComponents;
    [SerializeField] private Sprite filledStarSprite;
    [SerializeField] private Sprite emptyStarSprite;
    
    [Header("Game Over Screen")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    // --- Private State ---
    private int totalCoins;
    private int currentRoundNumber;
    private int attemptsThisRound;
    private float currentTime;
    private bool isRoundActive;
    
    private List<QuizItem> availableQuizItems;
    private List<QuizItem> usedQuizItems = new List<QuizItem>();
    private List<QuizItem> currentRoundItemPool;
    private List<QuizItem> currentShuffledNames;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        // Initialize the question pool from the master list
        availableQuizItems = new List<QuizItem>(allQuizItems);
        StartNewGame();
    }

    private void Update()
    {
        if (!isRoundActive) return;

        currentTime -= Time.deltaTime;
        UpdateTimerUI();

        if (currentTime <= 0)
        {
            TimeUp();
        }
    }

    #endregion

    #region Game Flow

    private void StartNewGame()
    {
        totalCoins = 0;
        currentRoundNumber = 1;
        StartNewRound();
    }

    private void StartNewRound()
    {
        isRoundActive = true;
        attemptsThisRound = 0;
        currentTime = roundTime;
        
        UpdateMainUI();
        SetupRoundContent();
    }

    private void SetupRoundContent()
    {
        // Ensure we have enough unique questions, recycle if needed.
        if (availableQuizItems.Count < 3)
        {
            availableQuizItems.AddRange(usedQuizItems);
            usedQuizItems.Clear();
        }
        
        // Select 3 random items for the current round.
        List<QuizItem> shuffledList = availableQuizItems.OrderBy(_ => Random.value).ToList();
        currentRoundItemPool = shuffledList.Take(3).ToList();

        // Move the selected items to the used pool.
        foreach (var item in currentRoundItemPool)
        {
            availableQuizItems.Remove(item);
            usedQuizItems.Add(item);
        }

        // Shuffle the names for display.
        currentShuffledNames = currentRoundItemPool.OrderBy(_ => Random.value).ToList();

        // Populate the UI elements.
        for (int i = 0; i < 3; i++)
        {
            pictureHolders[i].sprite = currentRoundItemPool[i].itemSprite;
            pictureHolders[i].GetComponent<LineConnector>().itemData = currentRoundItemPool[i];

            nameHolders[i].text = currentShuffledNames[i].itemName;
            nameHolders[i].transform.parent.GetComponent<LineConnector>().itemData = currentShuffledNames[i];
        }
    }

    private void TimeUp()
    {
        checkResultButton.interactable = false;
        isRoundActive = false;
        losePopup.SetActive(true);
    }
    
    private void EndGame()
    {
        isRoundActive = false;
        finalScoreText.text = "Final Score: " + totalCoins.ToString();
        gameOverScreen.SetActive(true);
    }
    
    #endregion

    #region UI Updaters

    private void UpdateMainUI()
    {
        progressBar.value = (float)(currentRoundNumber - 1) / totalRounds;
        coinsText.text = totalCoins.ToString();
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (currentTime < 0) currentTime = 0;
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timeText.text = $"{minutes:00}:{seconds:00}";
    }

    private void UpdateStarDisplay(int starCount)
    {
        for (int i = 0; i < starImageComponents.Length; i++)
        {
            starImageComponents[i].sprite = (i < starCount) ? filledStarSprite : emptyStarSprite;
        }
    }

    #endregion

    #region UI Button Handlers

    public void OnCheckResultButtonPressed()
    {
        if (!isRoundActive || lineManager.Connections.Count < 3) return;

        attemptsThisRound++;
        bool allAnswersCorrect = true;

        foreach (var pair in lineManager.Connections)
        {
            var startConnector = pair.Key.parent.GetComponent<LineConnector>();
            var endConnector = pair.Value.parent.GetComponent<LineConnector>();

            if (startConnector.itemData.itemName != endConnector.itemData.itemName)
            {
                allAnswersCorrect = false;
                break;
            }
        }

        if (allAnswersCorrect)
        {
            isRoundActive = false;
            HandleWinCondition();
        }
        else
        {
            lineManager.ClearAllLines();
            checkResultButton.interactable = false;
            isRoundActive = false;
            losePopup.SetActive(true);
        }
    }

    private void HandleWinCondition()
    {
        checkResultButton.interactable = false;
        int stars;
        if (attemptsThisRound == 1) { stars = 3; totalCoins += 100; }
        else if (attemptsThisRound < 4) { stars = 2; totalCoins += 60; }
        else if (attemptsThisRound < 6) { stars = 1; totalCoins += 30; }
        else { stars = 0; totalCoins += 10; }

        UpdateStarDisplay(stars);
        repeatButtonOnWinPopup.gameObject.SetActive(stars < 3);
        winPopup.SetActive(true);
    }

    public void OnNextButtonPressed()
    {
        winPopup.SetActive(false);
        lineManager.ClearAllLines();
        
        currentRoundNumber++;
        if (currentRoundNumber > totalRounds)
        {
            EndGame();
        }
        else
        {
            checkResultButton.interactable = true;
            StartNewRound();
        }
    }

    public void OnRepeatButtonPressed_FromWin()
    {
        checkResultButton.interactable = true;
        winPopup.SetActive(false);
        lineManager.ClearAllLines();
        
        isRoundActive = true;
        attemptsThisRound = 0;
        currentTime = roundTime;
        
        ReplayCurrentRound();
    }
    
    private void ReplayCurrentRound()
    {
        for (int i = 0; i < 3; i++)
        {
            pictureHolders[i].sprite = currentRoundItemPool[i].itemSprite;
            pictureHolders[i].GetComponent<LineConnector>().itemData = currentRoundItemPool[i];

            nameHolders[i].text = currentShuffledNames[i].itemName;
            nameHolders[i].transform.parent.GetComponent<LineConnector>().itemData = currentShuffledNames[i];
        }
    }

    public void OnRepeatButtonPressed_FromLose()
    {
        checkResultButton.interactable = true;
        losePopup.SetActive(false);
        lineManager.ClearAllLines();
        
        isRoundActive = true;
        currentTime = roundTime;
    }
    
    public void OnPlayAgainButtonPressed()
    {
        checkResultButton.interactable = true;
        gameOverScreen.SetActive(false);
        // Reset question pool to full
        availableQuizItems.AddRange(usedQuizItems);
        usedQuizItems.Clear();
        
        StartNewGame();
    }
    
    #endregion
}