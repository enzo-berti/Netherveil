using Map;
using System;
using UnityEngine;

public class DungeonsLimits : Quest
{
    float currentNumber = 0f;
    readonly int EASY_POURCENTAGE = 70;
    readonly int MEDIUM_POURCENTAGE = 85;
    readonly int HARD_POURCENTAGE = 100;

    public override void AcceptQuest()
    {
        base.AcceptQuest();

        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/{EASY_POURCENTAGE}%";
                break;
            case QuestDifficulty.MEDIUM:
                Datas.CorruptionModifierValue += 5;
                progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/{MEDIUM_POURCENTAGE}%";
                break;
            case QuestDifficulty.HARD:
                Datas.CorruptionModifierValue += 10;
                progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/{HARD_POURCENTAGE} %";
                break;
        }
        RoomUtilities.enterEvents += UpdateCount;
    }

    protected override bool IsQuestFinished()
    {
        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                return currentNumber >= EASY_POURCENTAGE;
            case QuestDifficulty.MEDIUM:
                return currentNumber >= MEDIUM_POURCENTAGE;
            case QuestDifficulty.HARD:
                return currentNumber >= HARD_POURCENTAGE;
            default:
                break;
        }
        return false;
    }

    protected override void QuestFinished()
    {
        base.QuestFinished();
        RoomUtilities.enterEvents -= UpdateCount;
    }

    private void UpdateCount()
    {
        if (IsQuestFinished())
            return;

        currentNumber = RoomUtilities.nbEnterRoomByType[RoomType.Normal] / RoomUtilities.nbRoomByType[RoomType.Normal] * 100f;

        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/{EASY_POURCENTAGE} %";
                break;
            case QuestDifficulty.MEDIUM:
                progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/{MEDIUM_POURCENTAGE} %";
                break;
            case QuestDifficulty.HARD:
                progressText = $"EXPLORE THE DUNGEON : {currentNumber}%/{HARD_POURCENTAGE} %";
                break;
        }
        QuestUpdated();
    }
}
