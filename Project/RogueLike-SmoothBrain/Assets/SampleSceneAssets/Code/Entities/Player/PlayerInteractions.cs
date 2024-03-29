using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    [SerializeField] Material outlineMaterial;
    public List<IInterractable> InteractablesInRange { get; private set; } = new List<IInterractable>();

    void Update()
    {
        SelectClosestItem();
    }

    private void SelectClosestItem()
    {
        Vector3 tmp = (Camera.main.transform.forward * transform.position.z + Camera.main.transform.right * transform.position.x);
        Vector2 playerPos = new(tmp.x, tmp.z);

        InteractablesInRange = InteractablesInRange.OrderBy(x =>
        {
            tmp = Camera.main.transform.forward * (x as MonoBehaviour).transform.position.z +
            Camera.main.transform.right * (x as MonoBehaviour).transform.position.x;
            Vector2 itemPos = new(tmp.x, tmp.z);

            return Vector2.Distance(playerPos, itemPos);
        }
        ).ToList();

        if (InteractablesInRange.Count > 0)
        {
            MeshRenderer meshRenderer;
            List<Material> finalMaterial;

            for (int i = 1; i < InteractablesInRange.Count; i++)
            {
                meshRenderer = (InteractablesInRange[i] as MonoBehaviour).gameObject.GetComponentInChildren<MeshRenderer>();
                if (meshRenderer.materials.Length > 1)
                {
                    finalMaterial = new()
                    {
                        meshRenderer.material
                    };
                    meshRenderer.SetMaterials(finalMaterial);
                }
            }

            meshRenderer = (InteractablesInRange[0] as MonoBehaviour).gameObject.GetComponentInChildren<MeshRenderer>();
            (InteractablesInRange[0] as MonoBehaviour).gameObject.GetComponent<ItemDescription>().TogglePanel(true);
            finalMaterial = new()
                {
                    meshRenderer.material,
                    outlineMaterial
                };
            meshRenderer.SetMaterials(finalMaterial);
        }
    }
}
