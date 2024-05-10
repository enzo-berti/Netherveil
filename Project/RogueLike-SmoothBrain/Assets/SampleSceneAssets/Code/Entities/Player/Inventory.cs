using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Inventory
{
    public static event Action OnAddOrRemoveBlood;

    public class BloodClass
    {
        private int value = 0;

        public int Value
        {
            get { return value; }
        }

        public static BloodClass operator +(BloodClass blood, int increment)
        {
            blood.value += increment;
            OnAddOrRemoveBlood?.Invoke();
            FloatingTextGenerator.CreateActionText(Utilities.Player.transform.position, $"+{increment} Blood", Color.red);
            return blood;
        }

        public static BloodClass operator -(BloodClass blood, int decrement)
        {
            blood.value -= decrement;
            OnAddOrRemoveBlood?.Invoke();
            FloatingTextGenerator.CreateActionText(Utilities.Player.transform.position, $"-{decrement} Blood", Color.red);
            return blood;
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
            for(int i = passiveItems.Count -1; i >= 0; --i)
            {
                RemoveItem(passiveItems[i]);
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
                if(item.gameObject.TryGetComponent<ItemInteractionMerchant>(out var itemInteraction) && itemInteraction.enabled)
                {
                    item.gameObject.AddComponent<ItemInteraction>();
                    itemInteraction.enabled = false;
                }
                ActiveItemGameObject = GameObject.Instantiate(item.gameObject);
                ActiveItemGameObject.name = item.idItemName;
                ActiveItemGameObject.SetActive(false);
            }
            else
            {
                var go = GameObject.Instantiate(ActiveItemGameObject, item.gameObject.transform.position, Quaternion.identity);
                go.SetActive(true);
                go.GetComponent<Item>().idItemName = ActiveItemGameObject.name;
                go.GetComponentInChildren<ItemDescription>().RemovePriceText();
                go.GetComponent<Item>().CreateItem();
                go.GetComponent<Item>().ItemEffect.HasBeenRetreived = true;
                go.GetComponent<Item>().ItemEffect.CurrentEnergy = (activeItem as ItemEffect).CurrentEnergy;
                go.name = "item";
                GameObject.Destroy(ActiveItemGameObject);
                if (item.gameObject.TryGetComponent<ItemInteractionMerchant>(out var itemInteraction) && itemInteraction.enabled)
                {
                    item.gameObject.AddComponent<ItemInteraction>();
                    itemInteraction.enabled = false;
                }
                ActiveItemGameObject = GameObject.Instantiate(item.gameObject);
                ActiveItemGameObject.name = item.idItemName;
                ActiveItemGameObject.SetActive(false);
            }
            AddActiveItem(itemEffect as IActiveItem);

            if (!itemEffect.HasBeenRetreived)
            {
                itemEffect.CurrentEnergy = (itemEffect as IActiveItem).Cooldown;
            }
            else
            {
                if (itemEffect.CurrentEnergy < (itemEffect as IActiveItem).Cooldown)
                {
                    CoroutineManager.Instance.StartCustom((itemEffect as IActiveItem).WaitToUse());
                }
            }
        }
        else if (itemEffect as IPassiveItem != null)
        {
            AddPassiveItem(itemEffect as IPassiveItem);
        }
        (itemEffect as IPassiveItem)?.OnRetrieved();
        itemEffect.HasBeenRetreived = true;
    }
}
