using Map;
using Map.Generation;
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
                CorruptionModifierValue += 5;
                break;
            case QuestDifficulty.HARD:
                COMPLETION_POURCENTAGE = 100;
                CorruptionModifierValue += 10;
                break;
        }

        currentNumber = (int)((float)MapUtilities.NbEnterRoom / MapUtilities.NbRoom * 100f);

        progressText = $"EXPLORE THIS FLOOR : {currentNumber}%/{COMPLETION_POURCENTAGE} %";
        MapUtilities.onEarlyEnter += UpdateCount;
        MapUtilities.onFinishStage += LoseQuest;
        UpdateCount();
    }

    public override bool IsQuestFinished()
    {
        return currentNumber >= COMPLETION_POURCENTAGE;
    }

    protected override void ResetQuestValues()
    {
        MapUtilities.onEarlyEnter -= UpdateCount;
        MapUtilities.onFinishStage -= LoseQuest;
    }

    private void LoseQuest()
    {
        questLost = true;
        ResetQuestValues();
        CheckQuestFinished();
    }

    private void UpdateCount()
    {
        if (!IsQuestFinished())
        {
            currentNumber = (int)((float)MapUtilities.NbEnterRoom / MapUtilities.NbRoom * 100f);
            progressText = $"EXPLORE THIS FLOOR : {currentNumber}%/{COMPLETION_POURCENTAGE} %";
        }

        QuestUpdated();
    }
}
