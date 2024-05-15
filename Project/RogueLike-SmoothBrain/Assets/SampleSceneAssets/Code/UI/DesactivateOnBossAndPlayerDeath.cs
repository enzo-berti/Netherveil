using UnityEngine;

public class DesactivateOnBossAndPlayerDeath : MonoBehaviour
{
    [SerializeField] private Entity boss;

    void Start()
    {
        Utilities.Hero.OnDeath += Desactive;
        boss.OnDeath += Desactive;
    }

    private void Desactive(Vector3 _)
    {
        gameObject.SetActive(false);
    }
}
