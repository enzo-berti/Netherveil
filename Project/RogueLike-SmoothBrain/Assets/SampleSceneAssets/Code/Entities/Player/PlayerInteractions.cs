using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Hero))]
public class PlayerInteractions : MonoBehaviour
{
    private Hero hero;
    [SerializeField] Material outlineMaterial;
    public List<IInterractable> interactablesInRange { get; private set; } = new List<IInterractable>();


    private void Start()
    {
        hero = GetComponent<Hero>();
    }

    void Update()
    {
        RetrievedConsommable();
        // TODO : Add UI to understand that we can press a button to take an object

        SelectClosestItem();
    }

    private void SelectClosestItem()
    {
        Vector3 tmp = (Camera.main.transform.forward * transform.position.z + Camera.main.transform.right * transform.position.x);
        Vector2 playerPos = new Vector2(tmp.x, tmp.z);

        IInterractable[] interactables = interactablesInRange.OrderBy(x =>
        {
            tmp = Camera.main.transform.forward * (x as MonoBehaviour).transform.position.z +
            Camera.main.transform.right * (x as MonoBehaviour).transform.position.x;
            Vector2 itemPos = new Vector2(tmp.x, tmp.z);

            return Vector2.Distance(playerPos, itemPos);
        }
        ).ToArray();

        if (interactables.Length > 0)
        {
            MeshRenderer meshRenderer;
            List<Material> finalMaterial;

            for (int i = 1; i < interactables.Length; i++)
            {
                meshRenderer = (interactables[i] as MonoBehaviour).gameObject.GetComponentInChildren<MeshRenderer>();
                (interactables[0] as MonoBehaviour).gameObject.GetComponent<ItemDescription>().TogglePanel(false);
                if (meshRenderer.materials.Length > 1)
                {
                    finalMaterial = new()
                    {
                        meshRenderer.material
                    };
                    meshRenderer.SetMaterials(finalMaterial);
                }
            }

            meshRenderer = (interactables[0] as MonoBehaviour).gameObject.GetComponentInChildren<MeshRenderer>();
            (interactables[0] as MonoBehaviour).gameObject.GetComponent<ItemDescription>().TogglePanel(true);
            finalMaterial = new()
                {
                    meshRenderer.material,
                    outlineMaterial
                };
            meshRenderer.SetMaterials(finalMaterial);
        }
    }

    public void Interract(InputAction.CallbackContext ctx)
    {
        Vector3 playerPos = (Camera.main.transform.forward * transform.position.z + Camera.main.transform.right * transform.position.x);
        IInterractable closestInteractable = interactablesInRange.OrderBy(x =>
        {
            Vector3 itemPos = Camera.main.transform.forward * (x as MonoBehaviour).transform.position.z +
            Camera.main.transform.right * (x as MonoBehaviour).transform.position.x;
            return Vector2.Distance(playerPos, itemPos);
        }
        )
        .FirstOrDefault();

        if (closestInteractable != null)
        {
            closestInteractable.Interract();
        }
    }

    public void RetrievedConsommable()
    {
        IConsumable[] consumables = Physics.OverlapSphere(this.transform.position, hero.Stats.GetValue(Stat.CATCH_RADIUS))
            .Where(x => x.gameObject.TryGetComponent<IConsumable>(out var consommable) && consommable.CanBeRetrieved)
            .Select(x => x.gameObject.GetComponent<IConsumable>())
            .ToArray();

        foreach (IConsumable consumable in consumables)
        {
            consumable.OnRetrieved();
        }
    }
    private void OnDrawGizmos()
    {
        //Handles.color = new Color(1, 1, 0, 0.25f);
        //Handles.DrawSolidDisc(transform.position, Vector3.up, hero.Stats.GetValueStat(Stat.CATCH_RADIUS));
    }
}
