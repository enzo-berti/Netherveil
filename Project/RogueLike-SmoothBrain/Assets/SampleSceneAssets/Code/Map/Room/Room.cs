using Map.Component;
using UnityEngine;

namespace Map
{
    public class Room : MonoBehaviour
    {
        [SerializeField] public RoomType type = RoomType.Normal;

        [field: SerializeField] public Skeleton Skeleton { get; private set; } = null;
        [field: SerializeField] public StaticProps StaticProps { get; private set; } = null;
        [field: SerializeField] public Lights Lights { get; private set; } = null;

        [field: SerializeField] public RoomPresets RoomPresets { get; private set; } = null;
        public RoomEnemies RoomEnemies // Need to be updated when roomPresets destroyed other rooms (work for now but not optimised)
        {
            get
            {
                return RoomPresets.GetComponentInChildren<RoomEnemies>(true);
            }
        }

        private void OnValidate()
        {
            Skeleton = transform.GetComponentInChildren<Skeleton>(true);
            StaticProps = transform.GetComponentInChildren<StaticProps>(true);
            Lights = transform.GetComponentInChildren<Lights>(true);
            RoomPresets = transform.GetComponentInChildren<RoomPresets>(true);
        }

        /// <summary>
        /// Make the room marked has "cleared" in game
        /// </summary>
        public void ClearPreset()
        {
            RoomEnemies.Clear();
            Skeleton.GetComponent<RoomEvents>().Clear();
        }
    }
}