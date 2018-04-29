using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoader : MonoBehaviour
{
    public Animator anim;

    public Action LoadingCallBack;

    void Start()
    {
        LoadScene("Home");
    }

    void LoadScene(int scene)
    {
        StartCoroutine(AsyncLoad(SceneManager.GetSceneByBuildIndex(scene).name));
    }

    public void LoadScene(int scene, Action LoadingCallBack)
    {
        this.LoadingCallBack = LoadingCallBack;
        StartCoroutine(AsyncLoad(SceneManager.GetSceneByBuildIndex(scene).name));
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(AsyncLoad(sceneName));
    }

    IEnumerator AsyncLoad(string sceneName)
    {
        /*if (LoadGroup)
        {
            LoadGroup.SetActive(true);
        }
        yield return new WaitForSeconds(0.2f);*/

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        for(int i=0;i<100;i++)
        {
            if (anim)
            {
                var value = Mathf.Clamp01(i/100.0f); ;

                anim.SetFloat("Progress", value);
            }

            yield return new WaitForSeconds(0.01f);
        }

        if (operation.progress >= 0.9f && !operation.allowSceneActivation)
        {
            operation.allowSceneActivation = true;
        }

        //if (LoadGroup) LoadGroup.SetActive(false);
        if (LoadingCallBack != null)
        {
            LoadingCallBack();
        }
    }

    IEnumerator AsyncLoad2(string sceneName)
    {
        /*if (LoadGroup)
        {
            LoadGroup.SetActive(true);
        }
        yield return new WaitForSeconds(0.2f);*/

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            if (anim)
            {
                var value = Mathf.Clamp01(operation.progress / 0.9f); ;

                anim.SetFloat("Progress", value);
            }

            if (operation.progress >= 0.9f && !operation.allowSceneActivation)
            {
                operation.allowSceneActivation = true;
            }
            yield return new WaitForSeconds(0.1f);
        }

        //if (LoadGroup) LoadGroup.SetActive(false);
        if (LoadingCallBack != null)
        {
            LoadingCallBack();
        }
    }

}
