using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

[Serializable]
public class Item : MonoBehaviour, IInterractable
{
    public string item;
    private void Start()
    {
        //effect = LoadDataClass();
    }
    private void Update()
    {
        if( Vector2.Distance(GameObject.FindWithTag("Player").transform.position, transform.position ) < 10 )
        {
            Interract();
        }
    }
    public void Interract()
    {
        //GameObject.FindWithTag("Player").GetComponent<Hero>().Inventory.AddItem(LoadDataClass());
        Destroy(this.gameObject);
    }

    //ItemEffect LoadDataClass()
    //{
    //    var classArray = data.effect.ToString().Split(' ').ToList();
    //    int index = classArray.IndexOf("class") + 1;

    //    return Assembly.GetExecutingAssembly().CreateInstance(classArray[index]) as ItemEffect;
    //}
}
[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    int selection;
    ItemDatabase database;
    List<string> names = new List<string>();
    Item itemTarget;
    private void OnEnable()
    {
        
        database = Resources.Load<ItemDatabase>("ItemDatabase");
        names = database.datas.Select(x => x.idName).ToList();
        itemTarget = (Item)target;
        selection = GetIndex(names, itemTarget.item);
    }
    public override void OnInspectorGUI()
    {
       serializedObject.Update();

        Item itemTarget = (Item)target;

        EditorGUILayout.BeginHorizontal();
        // Utiliser Popup pour afficher et éditer la chaîne sélectionnée
        EditorGUILayout.LabelField("idName : ");
        //selection = EditorGUILayout.Popup(selection, names.ToArray(), EditorStyles.popup);
        if(GUILayout.Button("test"))
        {
            ResearchItemWizard.CreateWizard();
        }
        EditorGUILayout.EndHorizontal();
        itemTarget.item = names[selection];
        serializedObject.ApplyModifiedProperties();
    }

    // Méthode pour obtenir l'index d'une chaîne dans un tableau de chaînes
    private int GetIndex(List<string> options, string selectedOption)
    {
        return options.IndexOf(selectedOption);
    }
}

public class ResearchItemWizard : ScriptableWizard
{
    public static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<ResearchItemWizard>("Create Light", "Create", "Apply");
        //If you don't want to use the secondary button simply leave it out:
        //ScriptableWizard.DisplayWizard<WizardCreateLight>("Create Light", "Create");
    }
}