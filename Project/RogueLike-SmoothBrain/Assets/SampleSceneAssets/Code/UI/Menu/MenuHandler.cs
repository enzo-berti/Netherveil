using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private List<MenuPart> menuItems = new List<MenuPart>();

    private void Start()
    {
        CloseAllMenus();
    }

    private void CloseAllMenus()
    {
        menuItems.ForEach(m => m.CloseMenu());
    }

    public void OpenMenu(MenuPart item)
    {
        CloseAllMenus();
        item.OpenMenu();
    }
}
