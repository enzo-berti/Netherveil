using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public static InGameManager current;

    public Transition publicFade;

    private void Awake()
    {
        ItemAltar.altarCount = 0;
        current = this;
    }
}