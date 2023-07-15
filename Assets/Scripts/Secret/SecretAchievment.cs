using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretAchievment : MonoBehaviour
{
    private int visits = 0;
    
    public void addVisit(bool alreadyVisited)
    {
        if (!(alreadyVisited))
        {
            visits++;
        }
    }

    private void Update()
    {
        if (visits == 3)
        {
            //>>Интерфейс - всплывающие подсказки
            Debug.Log("[Выводим надпись: «Поздравляем вы получили достижение «Межвидовой Рецензент»»]");
        }
    }
}
