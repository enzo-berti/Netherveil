#if UNITY_EDITOR
using System.Collections;
using UnityEditor;
using UnityEngine;

public class PreviewPicture : MonoBehaviour
{
    [SerializeField] private string spritePath = "Assets/SampleSceneAssets/Art/Sprites/Items";
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private Transform mainCameraTransform;
    [SerializeField] private Transform objectTransform;
    [SerializeField] private Transform pivotTransform;

    [SerializeField] private GameObject[] toPictures;

    public void TakePicture()
    {
        StartCoroutine(TakePictureRoutine());
    }

    private IEnumerator TakePictureRoutine()
    {
        DisableAllToPictures();
        GameObject lastObject = null;

        foreach (var p in toPictures)
        {
            if (lastObject != null)
                lastObject.SetActive(false);

            p.SetActive(true);
            lastObject = p;

            Picture(spritePath, p.name);

            yield return new WaitForEndOfFrame();
        }
    }

    private void DisableAllToPictures()
    {
        foreach (var p in toPictures)
        {
            p.SetActive(false);
        }
    }

    private void Picture(string path, string fileName)
    {
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();

        System.IO.File.WriteAllBytes($"{path}/{fileName}.png", bytes);
        AssetDatabase.Refresh();
    }
}
#endif