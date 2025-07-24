using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [Tooltip("Список шагов туториала")]
    public List<GameObject> TutorialSteps = new List<GameObject>();

    [Tooltip("Текущий шаг туториала")]
    public int TutorialStep = 0;

    [Tooltip("Ссылка на GameManager")]
    public GameManager gameManager;

    // Переход к следующему шагу туториала
    public void NextStep()
    {
        TutorialStep++;
        ShowOnlyCurrentStep();
    }

    // Показывает только текущий шаг, остальные скрывает
    public void ShowOnlyCurrentStep()
    {
        for (int i = 0; i < TutorialSteps.Count; i++)
        {
            if (TutorialSteps[i] != null)
            {
                TutorialSteps[i].SetActive(i == TutorialStep && TutorialStep < TutorialSteps.Count);
            }
        }
        // Если шагов больше нет, все скрываем
        if (TutorialStep >= TutorialSteps.Count)
        {
            foreach (var obj in TutorialSteps)
            {
                if (obj != null) obj.SetActive(false);
            }
            gameManager.isTutorial = false;
            gameManager.gameTimer.StartTimer();
        }
    }
} 