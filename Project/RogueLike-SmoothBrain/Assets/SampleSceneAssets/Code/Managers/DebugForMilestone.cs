using Map;
using UnityEngine;

public class DebugForMilestone : MonoBehaviour
{
    public float corruptionDelta = 1;
    public float benedictionDelta = 1;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            Utilities.Hero.Stats.IncreaseValue(Stat.HP, 10);
        }
        if(Input.GetKeyDown(KeyCode.Keypad2))
        {
            foreach(var enemy in RoomUtilities.roomData.enemies)
            {
                if (enemy != null)
                {
                    enemy.GetComponentInChildren<IDamageable>().Death();
                }
            }
            RoomUtilities.roomData.enemies.Clear();
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Utilities.Hero.Death();
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            Utilities.Hero.Stats.IncreaseValue(Stat.CORRUPTION, corruptionDelta);
            Utilities.Hero.DebugCallLaunchUpgrade();
            Utilities.Hero.ChangeStatsBasedOnAlignment();
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            Utilities.Hero.Stats.IncreaseValue(Stat.CORRUPTION, 25);
            Utilities.Hero.DebugCallLaunchUpgrade();
            Utilities.Hero.ChangeStatsBasedOnAlignment();
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            Utilities.Hero.Stats.IncreaseValue(Stat.CORRUPTION, 100);
            Utilities.Hero.DebugCallLaunchUpgrade();
            Utilities.Hero.ChangeStatsBasedOnAlignment();
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            Utilities.Hero.Stats.DecreaseValue(Stat.CORRUPTION, benedictionDelta);
            Utilities.Hero.DebugCallLaunchUpgrade();
            Utilities.Hero.ChangeStatsBasedOnAlignment();
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            Utilities.Hero.Stats.DecreaseValue(Stat.CORRUPTION, 50);
            Utilities.Hero.DebugCallLaunchUpgrade();
            Utilities.Hero.ChangeStatsBasedOnAlignment();
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            Utilities.Hero.Stats.DecreaseValue(Stat.CORRUPTION, 100);
            Utilities.Hero.DebugCallLaunchUpgrade();
            Utilities.Hero.ChangeStatsBasedOnAlignment();
        }
        if(Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            Utilities.Hero.CurrentQuest = Quest.LoadClass(Quest.GetRandomQuestName(), QuestTalker.TalkerType.CLERIC, QuestTalker.TalkerGrade.BOSS);
        }
    }
}
