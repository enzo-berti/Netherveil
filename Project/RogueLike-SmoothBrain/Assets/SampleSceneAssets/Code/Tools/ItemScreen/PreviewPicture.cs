using UnityEditor;
using UnityEngine;

public class PreviewPicture : MonoBehaviour
{
    [SerializeField] private string spritePath = "Assets/SampleSceneAssets/Art/Sprites/Items";
    [SerializeField] private string fileName = "NewItem.png";
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private Transform mainCameraTransform;
    [SerializeField] private Transform objectTransform;
    [SerializeField] private Transform pivotTransform;
    private MeshRenderer meshRenderer => objectTransform.GetComponent<MeshRenderer>();
    private MeshFilter meshFilter => objectTransform.GetComponent<MeshFilter>();
    private string completePath => $"{spritePath}/{fileName}";

    [Header("Parameters")]
    [SerializeField, Range(0.5f, 2.0f)] private float zoom;
    [SerializeField] private Vector3 objectPosition;
    [SerializeField, Range(0.0f, 360.0f)] private float rotationX;
    [SerializeField, Range(0.0f, 360.0f)] private float rotationY;
    [Space]
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material[] materials;

    private void OnValidate()
    {
        mainCameraTransform.localPosition = -Vector3.forward * zoom;
        objectTransform.localPosition = objectPosition;
        pivotTransform.localEulerAngles = new Vector3(rotationX, rotationY, 0.0f);

        meshRenderer.materials = materials;
        meshFilter.mesh = mesh;
    }

    public void TakePicture()
    {
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();

        System.IO.File.WriteAllBytes(completePath, bytes);
        AssetDatabase.Refresh();
    }
}
