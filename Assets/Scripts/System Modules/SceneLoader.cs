using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : PersistentSingleton<SceneLoader>
{
    [SerializeField] UnityEngine.UI.Image transitionImage;
    [SerializeField] float fadeTime = 3.5f;

    Color color;

    const string GAMEPLAY = "GamePlay";
    const string Main_Menu = "MainMenu";
    const string SCORING = "Scoring";

    void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadingCoroutine(string sceneName)
    {
        var loadingOperation = SceneManager.LoadSceneAsync(sceneName);//异步加载，方法返回的是场景加载信息

        //allowSceneActivation属性：设置加载好的场景是否为激活状态
        loadingOperation.allowSceneActivation = false;//后台场景设为FALSE，不会影响到当前场景

        transitionImage.gameObject.SetActive(true);

        //Fade Out
        while (color.a < 1f)
        {
            color.a = Mathf.Clamp01(color.a + Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }

        //一直挂起等待到场景加载完(progress为0.9时就加载完场景了,只有当场景被启用时这个进度progress才会被设置为1)
        yield return new WaitUntil(() => loadingOperation.progress >= 0.9f);

        //Activate the new scene
        loadingOperation.allowSceneActivation = true;//激活后台场景，原场景自动摧毁

        //Fade in
        while (color.a > 0f)
        {
            color.a = Mathf.Clamp01(color.a - Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }
        transitionImage.gameObject.SetActive(false);

    }

    public void LoadGamePlayScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCoroutine(GAMEPLAY));
    }

    public void LoadMainMenuScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCoroutine(Main_Menu));
    }

    public void LoadScoringScene()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingCoroutine(SCORING));
    }

}
