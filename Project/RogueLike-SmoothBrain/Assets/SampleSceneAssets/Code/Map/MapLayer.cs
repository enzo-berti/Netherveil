using UnityEngine;

public class MapLayer : MonoBehaviour
{
    public void Set()
    {
        gameObject.layer = LayerMask.NameToLayer("Map");
    }

    public void Unset()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
}