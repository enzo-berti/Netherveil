using Map;
using System;
using UnityEngine;

public class DungeonsLimits : Quest
{
    float currentNumber = 0f;
    int COMPLETION_POURCENTAGE = 0;
    public override void AcceptQuest()
    {
        base.AcceptQuest();

        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                COMPLETION_POURCENTAGE = 70;
                break;
            case QuestDifficulty.MEDIUM:
                COMPLETION_POURCENTAGE = 85;
                Datas.CorruptionModifierValue += 5;
                break;
            case QuestDifficulty.HARD:
                COMPLETION_POURCENTAGE = 100;
                Datas.CorruptionModifierValue += 10;
                break;
        }

        progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/{COMPLETION_POURCENTAGE} %";
        RoomUtilities.onEnter += UpdateCount;
    }

    protected override bool IsQuestFinished()
    {
        return currentNumber >= COMPLETION_POURCENTAGE;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        RoomUtilities.onEnter -= UpdateCount;
    }

    private void UpdateCount()
    {
        if (IsQuestFinished())
            return;

        Debug.Log("NB ENTER ROOM : " + RoomUtilities.NbEnterRoom);
        currentNumber = (int)((float)RoomUtilities.NbEnterRoom/ RoomUtilities.NbRoom * 100f);
        Debug.Log("Current Number : " + currentNumber);
        progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/{COMPLETION_POURCENTAGE} %";
        QuestUpdated();
    }
}
