using Map;
using System.Collections;
using UnityEngine;

public class BeastHunter : Quest
{
    int currentNumber = 0;
    int MAX_NUMBER;
    float timeToFinishQuest;
    Coroutine timeManagerRoutine = null;

    public override void AcceptQuest()
    {
        base.AcceptQuest();

        MAX_NUMBER = 3;

        switch (difficulty)
        {
            case QuestDifficulty.EASY:
                timeToFinishQuest = 600f;
                break;
            case QuestDifficulty.MEDIUM:
                timeToFinishQuest = 450f;
                CorruptionModifierValue += 5;
                break;
            case QuestDifficulty.HARD:
                timeToFinishQuest = 10f;
                CorruptionModifierValue += 10;
                break;
        }
        progressText = $"NB BEASTS KILLED : {currentNumber}/{MAX_NUMBER}";
        Hero.OnKill += UpdateCount;
        timeManagerRoutine = CoroutineManager.Instance.StartCoroutine(TimeToFinishRoutine());
    }

    protected override bool IsQuestFinished()
    {
        return currentNumber >= MAX_NUMBER;
    }

    private void UpdateCount(IDamageable damageable)
    {
        if (!IsQuestFinished() && damageable as IGlorb != null)
        {
            currentNumber++;
            progressText = $"NB BEASTS KILLED : {currentNumber}/{MAX_NUMBER}";
        }
    }

    private IEnumerator TimeToFinishRoutine()
    {
        CurrentQuestTimer = timeToFinishQuest;
        while(CurrentQuestTimer > 0)
        {
            CurrentQuestTimer -= Time.deltaTime;
            QuestUpdated();
            yield return null;
        }
        QuestLost();
        yield break;
    }

    protected override void ResetQuestValues()
    {
        Hero.OnKill -= UpdateCount;
        if (timeManagerRoutine != null)
            CoroutineManager.Instance.StopCoroutine(timeManagerRoutine);

        timeManagerRoutine = null;
    }
}
