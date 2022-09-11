using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIContoller : MonoBehaviour
{
    [Header("----CANVAS----")]
    [SerializeField] Canvas mainMenuCanvas;

    [Header("----BUTTONS----")]
    [SerializeField] Button buttonStart;
    [SerializeField] Button buttonOptions;
    [SerializeField] Button buttonQuit;

    private void OnEnable()
    {
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonStart.gameObject.name, OnButtonStartClick);
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonOptions.gameObject.name, OnButtonOptionsClicked);
        ButtonPressedBehavior.buttonFunctionTable.Add(buttonQuit.gameObject.name, OnButtonQuitClicked);
    }

    private void OnDisable()
    {
        ButtonPressedBehavior.buttonFunctionTable.Clear();
    }

    private void Start()
    {
        Time.timeScale = 1;
        GameManager.GameState = GameState.Playing;
        UIInput.Instance.SelectUI(buttonStart);
    }


    void OnButtonStartClick()
    {
        mainMenuCanvas.enabled = false;
        SceneLoader.Instance.LoadGamePlayScene();
    }

    void OnButtonOptionsClicked()
    {
        UIInput.Instance.SelectUI(buttonOptions);
    }

    void OnButtonQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
    
#endif
    }


}
