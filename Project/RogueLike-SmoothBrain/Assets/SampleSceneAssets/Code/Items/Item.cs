using System.Reflection;
using System.Linq;
using UnityEngine;

public class Item : MonoBehaviour, IInterractable
{
    [SerializeField] ItemData data;
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
