using System.Threading.Tasks;
using UnityEngine;

public class DungeonGate : MonoBehaviour
{
    private Material material;
    [SerializeField] private BoxCollider boxCollider;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;

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
        boxCollider.enabled = false;
        SetDisolve(0f);
    }

    private void Close()
    {
        if (RoomUtilities.roomData.Type == RoomType.Lobby)
        {
            return;
        }

        SetDisolve(1f);
        boxCollider.enabled = true;
    }

    async void SetDisolve(float desiredDisolve)
    {
        float disolveMat = material.GetFloat("_Dissolve");
        while (disolveMat - desiredDisolve < 0.05f)
        {
            Debug.Log(disolveMat + " " + (disolveMat - desiredDisolve));
            disolveMat += Time.deltaTime;
            material.SetFloat("_Dissolve", disolveMat);

            await Task.Yield();
        }

        material.SetFloat("_Dissolve", desiredDisolve);
    }
}
