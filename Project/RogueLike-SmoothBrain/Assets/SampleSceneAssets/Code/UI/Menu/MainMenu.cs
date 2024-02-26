using Cinemachine;
using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private MeshButton[] meshButtons;
    [SerializeField] private TMP_Text[] floatingTexts;

    public void PlayGame()
    {
        if (Camera.main.GetComponent<CinemachineBrain>().IsBlending) return;
        StartCoroutine(EventEnumerator(LoadGame));
    }

    public IEnumerator EventEnumerator(Action eventMethod)
    {
        yield return new WaitForEndOfFrame();
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        yield return new WaitWhile(() => brain.IsBlending);

        eventMethod?.Invoke();
    }

    public void LoadGame()
    {
        // TODO : Scene loader
        //SceneManager.LoadSceneAsync();
        Debug.Log("Game started");
    }

    public void Quit()
    {
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
