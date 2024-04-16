using Map;
using UnityEngine;

public class DebugForMilestone : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            GameObject.FindWithTag("Player").GetComponent<Hero>().Stats.IncreaseValue(Stat.HP, 10);
        }
        if(Input.GetKeyDown(KeyCode.Keypad2))
        {
            foreach(var enemy in RoomUtilities.roomData.enemies)
            {
                enemy.GetComponent<IDamageable>().Death();
            }
            RoomUtilities.roomData.enemies.Clear();
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            GameObject.FindWithTag("Player").GetComponent<Hero>().Death();
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            GameObject.FindWithTag("Player").GetComponent<Hero>().Stats.IncreaseValue(Stat.CORRUPTION, 10);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            GameObject.FindWithTag("Player").GetComponent<Hero>().Stats.IncreaseValue(Stat.CORRUPTION, 25);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            GameObject.FindWithTag("Player").GetComponent<Hero>().Stats.IncreaseValue(Stat.CORRUPTION, 100);
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            GameObject.FindWithTag("Player").GetComponent<Hero>().Stats.DecreaseValue(Stat.CORRUPTION, 10);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            GameObject.FindWithTag("Player").GetComponent<Hero>().Stats.DecreaseValue(Stat.CORRUPTION, 50);
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            GameObject.FindWithTag("Player").GetComponent<Hero>().Stats.DecreaseValue(Stat.CORRUPTION, 100);
        }
    }
}
