using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;

public class RoomToolWindow : EditorWindow 
{
    public enum TypeRoom
    {
        Lobby,
        Normal,
        Treasure,
        Boss,
    }
    TypeRoom typeRoom = TypeRoom.Normal;
    string prefabName = "";
    GameObject roomObj;

    [UnityEditor.MenuItem("Tools/Room/Create")]
    public static void CreateRoom()
    {
        EditorWindow.GetWindow(typeof(RoomToolWindow));
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        roomObj = EditorGUILayout.ObjectField("Room prefab", roomObj, typeof(GameObject), false) as GameObject;
        prefabName = EditorGUILayout.TextField("Prefab Name", prefabName);
        typeRoom = (TypeRoom)EditorGUILayout.EnumPopup("Type of room", typeRoom);

        if (GUILayout.Button("Generate Room"))
        {
            GenerateRoomPrefab();
        }


        EditorGUILayout.EndVertical();
    }

    void GenerateRoomPrefab()
    {
        GameObject room = Instantiate(roomObj);
        GameObject roomPrefab = new GameObject(prefabName == "" ? roomObj.name : prefabName);

        //GameObject skeleton = room.transform.GetChild(1).transform.GetChild(0).gameObject;
        //skeleton.gameObject.name = "Skeleton";
        //skeleton.transform.parent = roomPrefab.transform;
        //skeleton.layer = LayerMask.NameToLayer("Map");
        //skeleton.AddComponent<NavMeshSurface>();
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
        //GameObject roomGenerator = new GameObject("RoomGenerator");
        //roomGenerator.transform.parent = roomPrefab.transform;
        //roomGenerator.AddComponent<RoomGenerator>();
        //
        //GameObject roomSeed1 = new GameObject("Room1");
        //roomSeed1.transform.parent = roomGenerator.transform;
        //
        //GameObject traps = new GameObject("Traps");
        //traps.transform.parent = roomSeed1.transform;
        //GameObject enemies = new GameObject("Enemies");
        //enemies.transform.parent = roomSeed1.transform;
        //GameObject props = new GameObject("Props");
        //props.transform.parent = roomSeed1.transform;
        //GameObject treasures = new GameObject("Treasures");
        //treasures.transform.parent = roomSeed1.transform;

        AssetDatabase.CreateFolder("/SampleSceneAssets/Levels/Prefabs/Map/Room/" + typeRoom.ToString() + "/" + prefabName, prefabName);
        PrefabUtility.SaveAsPrefabAsset(roomPrefab, UnityEngine.Application.dataPath + "/SampleSceneAssets/Levels/Prefabs/Map/Room/" + typeRoom.ToString() + "/" + prefabName + "/" + ".prefab");

        // destroy garbage in scene
        DestroyImmediate(room);
        DestroyImmediate(roomPrefab);
    }
}
