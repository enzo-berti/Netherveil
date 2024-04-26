using Map.Component;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class Room : MonoBehaviour
    {
        [SerializeField] GameObject[] roomObjects;
        [SerializeField] public RoomType type = RoomType.Normal;

        public List<Room> neighbours = new List<Room>();

        private void OnValidate()
        {
            roomObjects = new GameObject[gameObject.transform.childCount];

            for (int i = 0; i < roomObjects.Length; i++)
            {
                roomObjects[i] = gameObject.transform.GetChild(i).gameObject;
            }
        }

        public T Get<T>() where T : UnityEngine.Component
        {
            foreach (var go in roomObjects)
            {
                if (go.TryGetComponent(out T component))
                {
                    return component;
                }
            }

            Debug.LogError("Can't find " + typeof(T).Name + " in " + gameObject.name, gameObject);
            return null;
        }

        public GameObject Skeleton
        {
            get
            {
                return Get<Skeleton>().gameObject;
            }
        }

        public GameObject StaticProps
        {
            get
            {
                return Get<StaticProps>().gameObject;
            }
        }

        public GameObject Lights
        {
            get
            {
                return Get<Lights>().gameObject;
            }
        }

        public GameObject RoomPresets
        {
            get
            {
                return Get<RoomPresets>().gameObject;
            }
        }
    }
}