using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Inventory
{
    public class BloodClass
    {
        private int value = 0;

        public int Value
        {
            get { return value; }
        }

        // Override the += operator
        public static BloodClass operator +(BloodClass obj, int increment)
        {
            obj.value += increment;
            FloatingTextGenerator.CreateActionText(Utilities.Player.transform.position, $"+{increment} Blood", Color.red);
            return obj;
        }

        // Override the -= operator
        public static BloodClass operator -(BloodClass obj, int decrement)
        {
            obj.value -= decrement;
            FloatingTextGenerator.CreateActionText(Utilities.Player.transform.position, $"-{decrement} Blood", Color.red);
            return obj;
        }
    }


    public static GameObject ActiveItemGameObject;
    IActiveItem activeItem = null;
    List<IPassiveItem> passiveItems = new List<IPassiveItem>();
    public IActiveItem ActiveItem { get { return activeItem; } }
    public List<IPassiveItem> PassiveItems { get { return passiveItems; } }
    public List<IItem> AllItems
    {
        get
        {
            List<IItem> itemList = new List<IItem>
            {
                activeItem
            };
            itemList.AddRange(passiveItems);
            return itemList;
        }
    }
    public bool HasActiveItem
    {
        get => activeItem != null;
    }
    public BloodClass Blood = new BloodClass();

    public int Keys = 0;
    private void AddActiveItem(IActiveItem item)
    {
        if (activeItem != null)
        {
            (activeItem as IPassiveItem)?.OnRemove();
        }
        activeItem = item;
    }

    private void AddPassiveItem(IPassiveItem item)
    {
        passiveItems.Add(item);
    }

    public void RemoveItem(IPassiveItem item)
    {
        item.OnRemove();
        passiveItems.Remove(item);
    }

    public void RemoveAllItems(Vector3 _)
    {
        if(passiveItems.Count > 0)
        {
            foreach(var item in passiveItems)
            {
                RemoveItem(item);
            }
        }
        if(activeItem != null)
        {
            (activeItem as IPassiveItem)?.OnRemove();
            activeItem = null;
        }
    }
    public void AddItem(Item item)
    {
        ItemEffect itemEffect = item.ItemEffect;
        if ((itemEffect as IActiveItem) != null)
        {
            if (activeItem == null)
            {
                ActiveItemGameObject = GameObject.Instantiate(item.gameObject);
                ActiveItemGameObject.SetActive(false);
            }
            else
            {
                var go = GameObject.Instantiate(ActiveItemGameObject, item.gameObject.transform.position, Quaternion.identity);
                go.SetActive(true);
                go.GetComponent<Item>().ItemEffect.HasBeenRetreived = true;
                go.GetComponent<Item>().ItemEffect.CurrentEnergy = (activeItem as ItemEffect).CurrentEnergy;
                GameObject.Destroy(ActiveItemGameObject);
                ActiveItemGameObject = GameObject.Instantiate(item.gameObject);
                ActiveItemGameObject.SetActive(false);
            }
            AddActiveItem(itemEffect as IActiveItem);

            if (!itemEffect.HasBeenRetreived)
            {
                Debug.Log("First pick");
                itemEffect.CurrentEnergy = (itemEffect as IActiveItem).Cooldown;
            }
            else
            {
                if (itemEffect.CurrentEnergy < (itemEffect as IActiveItem).Cooldown)
                {
                    CoroutineManager.Instance.StartCustom((itemEffect as IActiveItem).WaitToUse());
                }
            }
            Debug.Log(itemEffect.CurrentEnergy);
        }
        else if (itemEffect as IPassiveItem != null)
        {
            AddPassiveItem(itemEffect as IPassiveItem);
        }
        (itemEffect as IPassiveItem)?.OnRetrieved();
        itemEffect.HasBeenRetreived = true;
    }
}
