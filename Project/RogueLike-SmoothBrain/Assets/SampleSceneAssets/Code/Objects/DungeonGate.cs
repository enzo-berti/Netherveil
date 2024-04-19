using Map;
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
        RoomUtilities.enterEvents += Close;
        RoomUtilities.allEnemiesDeadEvents += Open;
    }

    private void OnDestroy()
    {
        RoomUtilities.enterEvents -= Close;
        RoomUtilities.allEnemiesDeadEvents -= Open;
    }

    private void Open()
    {
        boxCollider.enabled = false;
        DisolveGate();
    }

    private void Close()
    {
        if (RoomUtilities.roomData.enemies.Count <= 0)
        {
            return;
        }

        boxCollider.enabled = true;
        AppearGate();
    }

    async void AppearGate()
    {
        float disolveMat = material.GetFloat("_Dissolve");
        while ((disolveMat - 1f) < 0.05f)
        {
            disolveMat += Time.deltaTime;

            if (material == null)
            {
                return;
            }
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

            if (material == null)
            {
                return;
            }
            material.SetFloat("_Dissolve", disolveMat);

            await Task.Yield();
        }

        material.SetFloat("_Dissolve", 0f);
    }
}
