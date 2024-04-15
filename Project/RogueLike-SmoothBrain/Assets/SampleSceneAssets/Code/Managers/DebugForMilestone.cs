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
    }
}
