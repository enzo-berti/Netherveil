using System.Collections;
using UnityEngine;

public class DungeonGate : MonoBehaviour
{
    private Material material;
    private void Awake()
    {
        material = GetComponent<Renderer>().material;
        RoomUtilities.EnterEvents += Close;
    }

    private void Close()
    {
        StartCoroutine(SetDisolve(1f));
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
