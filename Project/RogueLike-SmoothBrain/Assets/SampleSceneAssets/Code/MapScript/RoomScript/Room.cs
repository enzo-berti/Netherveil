using Map.Component;
using Map.Generation;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class Room : MonoBehaviour
    {
        [SerializeField] public RoomType type = RoomType.Normal;

        [field: SerializeField, HideInInspector] public Skeleton Skeleton { get; private set; } = null;
        [field: SerializeField, HideInInspector] public DoorsGenerator DoorsGenerator { get; private set; } = null;
        [field: SerializeField, HideInInspector] public StaticProps StaticProps { get; private set; } = null;
        [field: SerializeField, HideInInspector] public Lights Lights { get; private set; } = null;
        [field: SerializeField, HideInInspector] public RoomUI RoomUI { get; private set; } = null;
        [field: SerializeField, HideInInspector] public RoomEvents RoomEvents { get; private set; } = null;

        [field: SerializeField, HideInInspector] public RoomPresets RoomPresets { get; private set; } = null;
        public RoomEnemies RoomEnemies // Need to be updated when roomPresets destroyed other rooms (work for now but not optimised)
        {
            get
            {
                return RoomPresets.GetComponentInChildren<RoomEnemies>(true);
            }
        }

        readonly public List<Room> neighbors = new List<Room>();

        private void OnValidate()
        {
            Skeleton = transform.GetComponentInChildren<Skeleton>(true);
            DoorsGenerator = transform.GetComponentInChildren<DoorsGenerator>(true);
            StaticProps = transform.GetComponentInChildren<StaticProps>(true);
            Lights = transform.GetComponentInChildren<Lights>(true);
            RoomUI = transform.GetComponentInChildren<RoomUI>(true);
            RoomPresets = transform.GetComponentInChildren<RoomPresets>(true);
            RoomEvents = transform.GetComponentInChildren<RoomEvents>(true);
        }

        public void Unclear()
        {
            RoomEnemies.gameObject.SetActive(false);
            foreach (var c in GetComponentsInChildren<MapLayer>(true))
            {
                c.Unset();
            }

            var roomUI = GetComponentInChildren<RoomUI>(true);
            if (roomUI)
            {
                roomUI.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Make the room marked has "cleared" in game
        /// </summary>
        public void ClearPreset()
        {
            RoomEnemies.Clear();
            RoomEvents.Clear();
        }
    }
}