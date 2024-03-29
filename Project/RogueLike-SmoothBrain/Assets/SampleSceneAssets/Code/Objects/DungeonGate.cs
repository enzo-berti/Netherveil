using System.Collections;
using UnityEngine;

public class DungeonGate : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private BoxCollider boxCollider;

    private void Awake()
    {
        // set value to default
        material.SetFloat("_Dissolve", 0f);
    }

    private void Start()
    {
        RoomUtilities.EnterEvents += Close;
        RoomUtilities.allEnemiesDeadEvents += Open;
    }

    private void Open()
    {
        StartCoroutine(SetDisolve(0f));
        boxCollider.enabled = false;
    }

    private void Close()
    {
        if (RoomUtilities.roomData.Type == RoomType.Lobby)
        {
            return;
        }

        StartCoroutine(SetDisolve(1f));
        boxCollider.enabled = true;
    }

    IEnumerator SetDisolve(float desiredDisolve)
    {
        float disolveMat = material.GetFloat("_Dissolve");
        while (disolveMat - desiredDisolve < 0.05f)
        {
            disolveMat += Time.deltaTime;
            material.SetFloat("_Dissolve", disolveMat);

            yield return null;
        }

        material.SetFloat("_Dissolve", desiredDisolve);
    }
}
