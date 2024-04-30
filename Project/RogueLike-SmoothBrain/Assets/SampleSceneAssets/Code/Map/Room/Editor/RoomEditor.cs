using System.IO;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Map
{
    [CustomEditor(typeof(Room))]
    public class RoomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(4);

            Room script = target as Room;

            if (GUILayout.Button("Bake rooms"))
            {
                Transform roomPreset = script.RoomPresets.transform;

                foreach (Transform t in roomPreset.transform)
                {
                    NavMeshSurface navMesh = t.GetComponent<NavMeshSurface>();

                    if (navMesh != null)
                    {
                        navMesh.BuildNavMesh();

                        NavMeshData navMeshData = navMesh.navMeshData;
                        if (navMeshData != null)
                        {
                            string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(script.gameObject);
                            string directory = Path.GetDirectoryName(prefabPath);
                            string filePath = Path.Combine(directory, $"{script.gameObject.name}-{t.name}.asset");

                            AssetDatabase.CreateAsset(navMeshData, filePath);
                            AssetDatabase.SaveAssets();
                        }
                        else
                        {
                            Debug.LogError("NavMesh data is null!");
                        }
                    }
                }
            }
        }
    }
}