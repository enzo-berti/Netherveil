using Map;
using Map.Generation;
using PrefabLightMapBaker;
using System.IO;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;

namespace Tool
{
    public class RoomGeneratorWindow : EditorWindow
    {
        RoomType roomType = RoomType.Normal;
        string roomName = "";
        GameObject houdiniRoom;
        GameObject bakedRoom;

        [UnityEditor.MenuItem("Tools/Room/Create")]
        public static void CreateRoom()
        {
            EditorWindow.GetWindow(typeof(RoomGeneratorWindow));
        }

        private void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            houdiniRoom = EditorGUILayout.ObjectField("Houdini Room", houdiniRoom, typeof(GameObject), true) as GameObject;
            bakedRoom = EditorGUILayout.ObjectField("Baked Room", bakedRoom, typeof(GameObject), true) as GameObject;
            roomName = EditorGUILayout.TextField("Prefab Name", roomName);
            roomType = (RoomType)EditorGUILayout.EnumPopup("Type of room", roomType);

            if (GUILayout.Button("Generate Room"))
            {
                GenerateRoomPrefab();
            }

            EditorGUILayout.EndVertical();
        }

        private GameObject CreateRoomGameObject()
        {
            Object source = Resources.Load("RoomPrefab");
            GameObject roomGO = (GameObject)PrefabUtility.InstantiatePrefab(source);
            roomGO.name = roomName;
            roomGO.GetComponent<Room>().type = roomType;

            return roomGO;
        }

        private void SaveRoomGameObject(GameObject room)
        {
            string typeRoomPath = "/SampleSceneAssets/Levels/Prefabs/Map/Room/" + roomType.ToString();
            string roomFolderPath = typeRoomPath + "/" + roomName;
            string roomPrefabPath = roomFolderPath + "/" + roomName + ".prefab";
            if (!Directory.Exists(UnityEngine.Application.dataPath + roomFolderPath))
            {
                AssetDatabase.CreateFolder("Assets" + typeRoomPath, roomName);
            }
            PrefabUtility.SaveAsPrefabAsset(room, UnityEngine.Application.dataPath + roomPrefabPath);
        }

        private void GenerateRoomPrefab()
        {
            GameObject roomGO = CreateRoomGameObject();
            Room room = roomGO.GetComponent<Room>();

            //GameObject room = Instantiate(bakedRoom);
            //GameObject roomPrefab = new GameObject(prefabName == "" ? bakedRoom.name : prefabName);
            //roomPrefab.AddComponent<PrefabBaker>();
            //
            //GameObject skeleton = room.transform.GetChild(1).transform.GetChild(0).gameObject;
            //skeleton.gameObject.name = "Skeleton";
            //skeleton.transform.parent = roomPrefab.transform;
            //skeleton.layer = LayerMask.NameToLayer("Map");
            //BoxCollider boxCollider = skeleton.AddComponent<BoxCollider>();
            //boxCollider.isTrigger = true;
            //MeshCollider collisionPlayer = skeleton.AddComponent<MeshCollider>();
            //collisionPlayer.includeLayers = -1;
            //skeleton.AddComponent<RoomEvents>();
            //
            //GameObject arrows = room.transform.GetChild(0).gameObject;
            //arrows.gameObject.name = "Doors";
            //arrows.transform.parent = skeleton.transform;
            //DoorsGenerator generator = arrows.AddComponent<DoorsGenerator>();
            //generator.GeneratePrefab();
            //
            //GameObject staticProps = room.transform.GetChild(1).gameObject;
            //staticProps.gameObject.name = "StaticProps";
            //staticProps.transform.parent = skeleton.transform;
            //
            //GameObject lights = new GameObject("Lights");
            //lights.transform.parent = skeleton.transform;
            //
            //GameObject roomPreset = new GameObject("RoomPreset");
            //roomPreset.transform.parent = roomPrefab.transform;
            ////roomGenerator.AddComponent<RoomGenerator>();
            //
            //GameObject roomSeed1 = new GameObject("Room1");
            //roomSeed1.transform.parent = roomPreset.transform;
            //roomSeed1.AddComponent<NavMeshSurface>();
            //
            //GameObject traps = new GameObject("Traps");
            //traps.transform.parent = roomSeed1.transform;
            //GameObject enemies = new GameObject("Enemies");
            //enemies.transform.parent = roomSeed1.transform;
            //GameObject props = new GameObject("Props");
            //props.transform.parent = roomSeed1.transform;
            //GameObject treasures = new GameObject("Treasures");
            //treasures.transform.parent = roomSeed1.transform;
            //GameObject npcs = new GameObject("Npcs");
            //npcs.transform.parent = roomSeed1.transform;

            SaveRoomGameObject(roomGO);
            
            // destroy garbage in scene
            DestroyImmediate(roomGO);
        }
    }
}