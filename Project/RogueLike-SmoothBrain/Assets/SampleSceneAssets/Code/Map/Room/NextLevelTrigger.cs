using Map.Generation;
using UnityEngine;

namespace Map
{
    public class NextLevelTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                MapUtilities.onFinishStage?.Invoke();

                MapGenerator mapGen = FindObjectOfType<MapGenerator>();

                if (mapGen.stage == 2)
                {
                    FindObjectOfType<LevelLoader>().LoadScene("endScreen", true);
                    return;
                }

                mapGen.DestroyMap();
                mapGen.generate = true;
            }
        }
    }
}