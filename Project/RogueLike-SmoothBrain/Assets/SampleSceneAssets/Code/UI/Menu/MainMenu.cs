using Cinemachine;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        if (Camera.main.GetComponent<CinemachineBrain>().IsBlending) return;
        StartCoroutine(StartEnumerator());
    }

    public void ExitGame()
    {
        if (Camera.main.GetComponent<CinemachineBrain>().IsBlending) return;
        StartCoroutine(ExitEnumerator());
    }

    public IEnumerator StartEnumerator()
    {
        yield return new WaitForEndOfFrame();
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        yield return new WaitWhile(() => brain.IsBlending);

        // TODO : Scene loader
        //SceneManager.LoadSceneAsync();
        Debug.Log("Game started");
    }

    public IEnumerator ExitEnumerator()
    {
        yield return new WaitForEndOfFrame();
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        yield return new WaitWhile(() => brain.IsBlending);

#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
#else
        Application.Quit();
#endif
    }
}
