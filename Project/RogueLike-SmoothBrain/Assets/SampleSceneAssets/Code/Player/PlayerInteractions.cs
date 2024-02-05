using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Hero))]
public class PlayerInteractions : MonoBehaviour
{
    [SerializeField] private Hero hero;

    void Update()
    {
        // TODO : Add UI to understand that we can press a touch to take an object
    }

    public void Interract(InputAction.CallbackContext ctx)
    {
        IInterractable[] interactables = Physics.OverlapSphere(transform.position, hero.Stats.GetValueStat(Stat.CATCH_RADIUS))
            .Select(x => x.GetComponent<IInterractable>())
            .Where(x => x != null)
            .ToArray();

        foreach (IInterractable interactable in interactables)
        {
            interactable.Interract();
        }
    }

    public void RetreivedConsommable()
    {
        var colliders =  Physics.OverlapSphere(this.transform.position, hero.Stats.GetValueStat(Stat.CATCH_RADIUS));
        foreach(var collider in colliders.Where(x => x.TryGetComponent<IConsommable>(out _)))
        {
            collider.GetComponent<IConsommable>().OnRetreived();
            Destroy(collider.gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        Handles.color = new Color(1, 1, 0, 0.25f);
        //Handles.DrawSolidDisc(transform.position, Vector3.up, hero.Stats.GetValueStat(Stat.CATCH_RADIUS));
    }
}
