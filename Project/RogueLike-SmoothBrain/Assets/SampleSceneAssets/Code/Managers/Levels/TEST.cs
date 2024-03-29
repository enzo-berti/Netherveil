using UnityEngine;

public class TEST : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindObjectOfType<LevelLoader>().LoadScene("MainMenu", "Fade");
        }
    }
}
