using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoringUIController : MonoBehaviour
{
    [Header("----BACKGROUND----")]
    [SerializeField] Image background;
    [SerializeField] Sprite[] backgroundImages;

    [Header("----SCORING SCREEN----")]
    [SerializeField] Canvas scoringScreenCanvas;
    [SerializeField] Text playerScoreText;
    [SerializeField] Button buttonMainMenu;
    [SerializeField] Transform highScoreLeaderboardContainer;

    [Header("----HIGH SCORE SCREEN----")]
    [SerializeField] Canvas newHighScoreScreenCanvas;
    [SerializeField] Button buttonCancel;
    [SerializeField] Button buttonSubmit;
    [SerializeField] InputField playerNameInputField;


    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ShowRandomBackground();

        if (ScoreManager.Instance.HasNewHighScore)
        {
            ShowNewHighScoreScreen();
        }
        else
        {
            ShowScoringScreen();
        }

        ButtonPressedBehavior.buttonFunctionTable.Add(buttonMainMenu.gameObject.name, OnButtonMainMenuClicked);
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonSubmit.gameObject.name, OnBUttonSubmitClicked);
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonCancel.gameObject.name, HideNewHighScoreScreen);


        GameManager.GameState = GameState.Scoring;
    }

    private void OnDisable()
    {
        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }

    void ShowRandomBackground()
    {
        background.sprite = backgroundImages[Random.Range(0, backgroundImages.Length)];
    }

    private void ShowNewHighScoreScreen()
    {
        newHighScoreScreenCanvas.enabled = true;
        UIInput.Instance.SelectUI(buttonCancel);
    }

    private void HideNewHighScoreScreen()
    {
        newHighScoreScreenCanvas.enabled = false;
        ScoreManager.Instance.SavePlayerScoreData();
        ShowRandomBackground();
        ShowScoringScreen();

    }

    void ShowScoringScreen()
    {
        scoringScreenCanvas.enabled = true;
        playerScoreText.text = ScoreManager.Instance.Score.ToString();
        UIInput.Instance.SelectUI(buttonMainMenu);

        UpdateHighScoreLeaderboard();
    }

    void UpdateHighScoreLeaderboard()
    {
        var playerScoreList = ScoreManager.Instance.LoadPlayerScoreData().list;

        for (int i = 0; i < highScoreLeaderboardContainer.childCount; i++)
        {
            var child = highScoreLeaderboardContainer.GetChild(i);

            child.Find("Rank").GetComponent<Text>().text = (i + 1).ToString();
            child.Find("Score").GetComponent<Text>().text = playerScoreList[i].score.ToString();
            child.Find("Name").GetComponent<Text>().text = playerScoreList[i].playerName;
        }
    }

    void OnButtonMainMenuClicked()
    {
        scoringScreenCanvas.enabled = false;
        SceneLoader.Instance.LoadMainMenuScene();
    }


    void OnBUttonSubmitClicked()
    {
        if (!string.IsNullOrEmpty(playerNameInputField.text))
        {
            ScoreManager.Instance.SetPlayerName(playerNameInputField.text);
        }

        HideNewHighScoreScreen();
    }


}
