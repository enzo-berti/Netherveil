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

    public void StartGame()
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

    public void FadeFloatingTexts(bool fadeIn)
    {
        StartCoroutine(FadeFloatingText(fadeIn));
    }

    private IEnumerator FadeFloatingText(bool fadeIn)
    {
        float elapsed = 0;
        float time = 0.5f;

        while (elapsed < time)
        {
            yield return null;
            elapsed = Mathf.Clamp(elapsed + Time.deltaTime, 0, time);

            foreach (TMP_Text textMesh in floatingTexts)
            {
                textMesh.alpha = fadeIn ? elapsed / time : 1f - elapsed / time;
            }
        }

        foreach (TMP_Text textMesh in floatingTexts)
        {
            textMesh.alpha = fadeIn ? 1f : 0f;
        }
    }
}
