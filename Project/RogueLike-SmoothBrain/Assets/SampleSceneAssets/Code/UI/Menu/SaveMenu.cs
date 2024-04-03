using MeshUI;
using UnityEngine;

public class SaveMenu : MenuHandler
{
    [SerializeField] private MeshButton[] booksButton;
    private new Collider collider;
    private MainMenu mainMenu;
    private bool isToggle = true;

    private void Start()
    {
        mainMenu = FindObjectOfType<MainMenu>();
        collider = GetComponent<Collider>();
    }

    public void Toggle()
    {
        isToggle = !isToggle;

        collider.enabled = isToggle;
        mainMenu.ToggleMeshButton(isToggle);
        mainMenu.FadeFloatingTexts(isToggle);
        foreach (MeshButton book in booksButton)
        {
            book.enabled = !isToggle;
        }
    }
}
