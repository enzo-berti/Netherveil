using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugForMilestone : MonoBehaviour
{

    // Update is called once per frame
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
                enemy.GetComponentInChildren<IDamageable>().Death();
            }
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
                GameObject.FindWithTag("Player").GetComponent<Hero>().Death();
        }
    }
}
