using Codice.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

public class IdNameWindow : EditorWindow
{
    private string idName;
    private string newId = string.Empty;
    private ItemDatabase itemDatabase;
    public static void OpenWindow(string _idName, ItemDatabase database)
    {
        IdNameWindow wnd = GetWindow<IdNameWindow>();
        wnd.titleContent = new GUIContent("Item creator");
        wnd.idName = _idName;
        wnd.itemDatabase = database;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Old id : ", idName);
        newId = EditorGUILayout.TextField("New id : ", newId);
        GUI.backgroundColor = Color.green;
        if(GUILayout.Button("Save"))
        {

        }
        GUI.backgroundColor = Color.white;
    }

    private void ChangeName()
    {

    }
}
