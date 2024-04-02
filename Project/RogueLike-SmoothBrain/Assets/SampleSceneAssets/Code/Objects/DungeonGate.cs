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
        DisolveGate();
        boxCollider.enabled = false;
    }

    private void Close()
    {
        if (RoomUtilities.roomData.Type == RoomType.Lobby)
        {
            return;
        }

        AppearGate();
        boxCollider.enabled = true;
    }

    async void AppearGate()
    {
        float disolveMat = material.GetFloat("_Dissolve");
        while ((disolveMat - 1f) < 0.05f)
        {
            disolveMat += Time.deltaTime;
            material.SetFloat("_Dissolve", disolveMat);

            await Task.Yield();
        }

        material.SetFloat("_Dissolve", 1f);
    }

    async void DisolveGate()
    {
        float disolveMat = material.GetFloat("_Dissolve");
        while (disolveMat > 0.05f)
        {
            disolveMat -= Time.deltaTime;
            material.SetFloat("_Dissolve", disolveMat);

            await Task.Yield();
        }

        material.SetFloat("_Dissolve", 0f);
    }
}
