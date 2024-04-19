using UnityEngine;

namespace Map
{
    public class NextLevelTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                RoomUtilities.onFinishStageEvents?.Invoke();

                LevelLoader.current.LoadScene("InGame");
            }
        }
    }
}