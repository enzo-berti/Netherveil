using UnityEngine;

namespace Map
{
    public class NextLevelTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                LevelLoader.current.LoadScene("InGame");
            }
        }
    }
}