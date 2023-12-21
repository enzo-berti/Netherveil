using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoudiniEngineUnity;

public class HoudiniTest : MonoBehaviour
{

    private HEU_HoudiniAsset HoudiniAsset;
    void Start()
    {
        string ToolAssetPath = "Assets/SampleSceneAssets/Art/Houdini/Precedural_Room.hdalc";
        string ToolFullPath = HEU_AssetDatabase.GetAssetFullPath(ToolAssetPath);
        if (string.IsNullOrEmpty(ToolFullPath) )
        {
            HEU_Logger.LogErrorFormat("Unable to load Tool at path : {0}", ToolAssetPath);
            return;

        }

        HEU_SessionBase session = HEU_SessionManager.GetOrCreateDefaultSession();

        GameObject rootGO = HEU_HAPIUtility.InstantiateHDA(ToolFullPath, Vector3.zero, session, true);
        if (rootGO != null)
        {
            HEU_EditorUtility.SelectObject(rootGO);
        }
        HoudiniAsset = QueryHoudiniAsset(rootGO);
    }

    public static HEU_HoudiniAsset QueryHoudiniAsset(GameObject rootGO)
    {
        // First get the HEU_HoudiniAssetRoot which is the script at the root gameobject
        HEU_HoudiniAssetRoot heuRoot = rootGO.GetComponent<HEU_HoudiniAssetRoot>();
        if (heuRoot == null)
        {
            HEU_Logger.LogWarningFormat("Unable to get the HEU_HoudiniAssetRoot from gameobject: {0}. Not a valid HDA.", rootGO.name);
            return null;
        }

        // The HEU_HoudiniAssetRoot should have a reference to HEU_HoudiniAsset which is the main HEU asset script.
        if (heuRoot.HoudiniAsset == null)
        {
            HEU_Logger.LogWarningFormat("Unable to get the HEU_HoudiniAsset in root gameobject: {0}. Not a valid HDA.", rootGO.name);
            return null;
        }

        return heuRoot.HoudiniAsset;
    }
    public static void ChangeParmsAndCook(HEU_HoudiniAsset houdiniAsset)
    {
        List<HEU_ParameterData> parms = houdiniAsset.Parameters.GetParameters();
        if (parms == null || parms.Count == 0)
        {
            HEU_Logger.LogFormat("No parms found");
            return;
        }
        
    }

        // Update is called once per frame
        void Update()
    {
        if (Input.GetKeyUp(KeyCode.K))
        {
            Debug.Log("oui");
            foreach (HEU_ParameterData parmData in HoudiniAsset.Parameters.GetParameters())
            {
                parmData._floatValues[4] = Random.Range(0f, 200f);
            }
        }
    }
}
