using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader current;

    private AsyncOperation asyncOperation;
    private List<Transition> levelTransitions = new List<Transition>();

    private void Awake()
    {
        current = this;

        foreach (Transform transition in transform)
        {
            levelTransitions.Add(transition.GetComponent<Transition>());
        }
    }

    public void LoadScene(string sceneName, int transitionIndex)
    {
        int sceneIndex = GetIndexSceneByName(sceneName);

        LoadScene(sceneIndex, transitionIndex);
    }

    public void LoadScene(string sceneName, string transitionName)
    {
        int sceneIndex = GetIndexSceneByName(sceneName);
        int transitionIndex = GetIndexTransitionByName(transitionName);

        LoadScene(sceneIndex, transitionIndex);
    }

    public void LoadScene(int sceneIndex, string transitionName)
    {
        int transitionIndex = GetIndexTransitionByName(transitionName);

        LoadScene(sceneIndex, transitionIndex);
    }

    public void LoadScene(int sceneIndex, int transitionIndex)
    {
        if (transitionIndex < 0 || transitionIndex >= levelTransitions.Count)
        {
            Debug.LogWarning($"No transition with the index {transitionIndex} !");
            return;
        }
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"No scene with the index {sceneIndex} !");
            return;
        }

        StartCoroutine(LoadSceneRoutine(sceneIndex, transitionIndex));
    }

    public void LoadScene(string sceneName)
    {
        int sceneIndex = GetIndexSceneByName(sceneName);

        LoadScene(sceneIndex);
    }

    public void LoadScene(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"No scene with the index {sceneIndex} !");
            return;
        }

        StartCoroutine(LoadSceneRoutine(sceneIndex));
    }

    private int GetIndexSceneByName(string name)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneNameInBuild == name)
            {
                return i;
            }
        }

        return -1;
    }

    private int GetIndexTransitionByName(string name)
    {
        return levelTransitions.Select(x => x.name).ToList().IndexOf(name);
    }

    private IEnumerator LoadSceneRoutine(int sceneIndex)
    {
        //asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        //yield return new WaitWhile(() => asyncOperation.progress < 0.9f);
        SceneManager.LoadScene(sceneIndex);
        yield return null;
    }

    private IEnumerator LoadSceneRoutine(int sceneIndex, int transitionIndex)
    {
        //asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        //asyncOperation.allowSceneActivation = false;

        yield return levelTransitions[transitionIndex].ToggleTransition(true);
        //yield return new WaitWhile(() => asyncOperation.progress < 0.9f);

        //asyncOperation.allowSceneActivation = true;
        SceneManager.LoadScene(sceneIndex);
        levelTransitions[transitionIndex].ToggleTransition(false);
    }
}
