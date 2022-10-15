using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : csSingleton<SceneLoader>
{
    public string sceneNameToBeLoaded;

    public void LoadScene(string _sceneName)
    {
        sceneNameToBeLoaded = _sceneName;

        StartCoroutine(InitializeSceneLoading());
    }

    IEnumerator InitializeSceneLoading()
    {
        yield return SceneManager.LoadSceneAsync("Scene_Loading");

        StartCoroutine(LoadActualScene());
    }

    IEnumerator LoadActualScene()
    {
        var asyncSceneLoading =  SceneManager.LoadSceneAsync(sceneNameToBeLoaded);

        asyncSceneLoading.allowSceneActivation = false;

        while (!asyncSceneLoading.isDone)
        {
#if UNITY_EDITOR
            Debug.Log(asyncSceneLoading.progress);
#endif
            if (asyncSceneLoading.progress >= 0.9f)
            {
                asyncSceneLoading.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
