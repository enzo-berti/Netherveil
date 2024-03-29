using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelLoader
{
    private static AsyncOperation asyncOperation;
    private static Transition[] levelTransitions;

    static LevelLoader()
    {
        levelTransitions = Resources.LoadAll<Transition>("");
    }

    public static async Task LoadScene(string sceneName, int transitionIndex)
    {
        int sceneIndex = GetIndexSceneByName(sceneName);

        await LoadScene(sceneIndex, transitionIndex);
    }

    public static async Task LoadScene(string sceneName, string transitionName)
    {
        int sceneIndex = GetIndexSceneByName(sceneName);
        int transitionIndex = GetIndexTransitionByName(transitionName);

        await LoadScene(sceneIndex, transitionIndex);
    }

    public static async Task LoadScene(int sceneIndex, string transitionName)
    {
        int transitionIndex = GetIndexTransitionByName(transitionName);

        await LoadScene(sceneIndex, transitionIndex);
    }

    public static async Task LoadScene(int sceneIndex, int transitionIndex)
    {
        if (transitionIndex < 0 || transitionIndex >= levelTransitions.Length)
        {
            Debug.LogWarning($"No transition with the index {transitionIndex} !");
            return;
        }

        // Transition
        Transition instance = GameObject.Instantiate(levelTransitions[transitionIndex]);
        await instance.PlayTransitionAsync();

        // Scene Loading
        await LoadScene(sceneIndex);
    }

    public static async Task LoadScene(string sceneName)
    {
        int sceneIndex = GetIndexSceneByName(sceneName);

        await LoadScene(sceneIndex);
    }

    public static async Task LoadScene(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"No scene with the index {sceneIndex} !");
            return;
        }

        asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        asyncOperation.allowSceneActivation = false;

        await WaitForSceneLoad();

        asyncOperation.allowSceneActivation = true;
    }

    private static async Task WaitForSceneLoad()
    {
        while (asyncOperation.progress < 0.9f)
        {
            await Task.Delay(100);
        }
    }

    private static int GetIndexSceneByName(string name)
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

    private static int GetIndexTransitionByName(string name)
    {
        return System.Array.IndexOf(levelTransitions.Select(x => x.name).ToArray(), name);
    }
}
