using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void Update()
    {
        transform.forward = Vector3.right;
        transform.Rotate(90f, 0f, 0);
    }
}
