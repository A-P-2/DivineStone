using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    public void ChooseMove()
    {
        StartCoroutine("ChooseMoveWithTimer");
    }

    private IEnumerator ChooseMoveWithTimer()
    {
        while (FindObjectOfType<UIFight>().actionTextTimerActive) yield return null;
        yield return new WaitForSeconds(1.2f);

        FightManager fightManager = GetComponent<FightManager>();
        NPCFighter npcFighter = DataTransfer.npcFighter;

        bool canChoose = true;
        if (fightManager.enemy.effectName == "Обморожение")
        {
            if (Random.Range(0, 2) == 0)
            {
                FindObjectOfType<UIFight>().AddInfoToActionText(
                    "Персонаж \"" + DataTransfer.enemyName + "\" хотел что-то сделать, но не смог из-за обморожения");
                fightManager.NextTurn();
                canChoose = false;
            }
        }

        if (canChoose)
        {
            /*
             0 - Физ. атака
             1 - Маг. атака
             2 - Физ. буст
             3 - Маг. буст
             4 - Хил
             */
            int[] movePriority = { 1, 1, 1, 1, 1 };

            if (npcFighter.maxPhysDmg == 0) movePriority[0] = 0;
            if (npcFighter.maxMagicDmg == 0) movePriority[1] = 0;
            if (npcFighter.canBoostPhys == 0 || fightManager.enemy.physDmgBoost != 0) movePriority[2] = 0;
            if (npcFighter.canBoostMagic == 0 || fightManager.enemy.magDmgBoost != 0) movePriority[3] = 0;
            if (npcFighter.canHeal == 0 ||
                npcFighter.HP == npcFighter.maxHP) movePriority[4] = 0;

            if (movePriority[1] == 0) movePriority[0] = 10;
            else if (movePriority[0] == 0) movePriority[1] = 10;
            else if (movePriority[0] != 0 && movePriority[1] != 0)
            {
                int temp = (int)(10.0 * npcFighter.maxPhysDmg / (npcFighter.maxPhysDmg + npcFighter.maxMagicDmg));
                movePriority[0] = temp;
                movePriority[1] = 10 - temp;
            }

            if (movePriority[2] > 0 && movePriority[0] > 0)
                movePriority[2] = (int)(0.2 * npcFighter.HP / npcFighter.maxHP * npcFighter.canBoostPhys);

            if (movePriority[3] > 0 && movePriority[1] > 0)
                movePriority[3] = (int)(0.2 * npcFighter.HP / npcFighter.maxHP * npcFighter.canBoostMagic);

            if (movePriority[4] > 0)
                movePriority[4] = (int)(20.0 * (npcFighter.maxHP - npcFighter.HP) / npcFighter.maxHP * npcFighter.canHeal / npcFighter.maxHP);

            int prioritySum = 0;
            foreach (int i in movePriority) prioritySum += i;

            if (prioritySum > 0)
            {
                int decision = Random.Range(1, prioritySum + 1);
                int index = 0;
                while (decision > 0)
                {
                    while (movePriority[index] == 0) index++;
                    decision--;
                    movePriority[index]--;
                }

                switch (index)
                {
                    case 0:
                        fightManager.SimpleAttack(true, false);
                        break;
                    case 1:
                        fightManager.SimpleAttack(false, false);
                        break;
                    case 2:
                        fightManager.EnemyBoostPhysDmg();
                        break;
                    case 3:
                        fightManager.EnemyBoostMagDmg();
                        break;
                    case 4:
                        fightManager.HealEnemy();
                        break;
                    default:
                        Debug.LogError("ИИ пошёл против системы и выбрал что-то своё! Да начнётся восстание!");
                        break;
                }
            }
            else
            {
                fightManager.NextTurn();
            }
        }
    }
}
