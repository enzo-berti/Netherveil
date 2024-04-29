using UnityEngine;
using UnityEngine.UI;

public class BuffHUD : MonoBehaviour
{
    [SerializeField] private GameObject buffIconPrefab;
    [SerializeField] private Transform buffTransform;

    public void AddBuffIcon(Texture texture)
    {
        GameObject buff = Instantiate(buffIconPrefab, buffTransform);
        buff.GetComponent<RawImage>().texture = texture;
    }
}
