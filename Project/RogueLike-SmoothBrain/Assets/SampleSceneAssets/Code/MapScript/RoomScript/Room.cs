using Map.Component;
using Map.Generation;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class Room : MonoBehaviour
    {
        [SerializeField] public RoomType type = RoomType.Normal;

        [field: SerializeField] public Skeleton Skeleton { get; private set; } = null;
        [field: SerializeField] public DoorsGenerator DoorsGenerator { get; private set; } = null;
        [field: SerializeField] public StaticProps StaticProps { get; private set; } = null;
        [field: SerializeField] public Lights Lights { get; private set; } = null;
        [field: SerializeField] public RoomUI RoomUI { get; private set; }

        [field: SerializeField] public RoomPresets RoomPresets { get; private set; } = null;
        public RoomEnemies RoomEnemies // Need to be updated when roomPresets destroyed other rooms (work for now but not optimised)
        {
            get
            {
                return RoomPresets.GetComponentInChildren<RoomEnemies>(true);
            }
        }

        readonly public List<Room> neighbor = new List<Room>();

        private void OnValidate()
        {
            Skeleton = transform.GetComponentInChildren<Skeleton>(true);
            DoorsGenerator = transform.GetComponentInChildren<DoorsGenerator>(true);
            StaticProps = transform.GetComponentInChildren<StaticProps>(true);
            Lights = transform.GetComponentInChildren<Lights>(true);
            RoomUI = transform.GetComponentInChildren<RoomUI>(true);
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