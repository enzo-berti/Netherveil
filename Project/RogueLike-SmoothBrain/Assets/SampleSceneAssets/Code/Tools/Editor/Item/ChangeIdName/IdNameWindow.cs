using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IdNameWindow : EditorWindow
{
    public static void OpenWindow()
    {
        IdNameWindow wnd = GetWindow<IdNameWindow>();
        wnd.titleContent = new GUIContent("Item creator");
    }
}
