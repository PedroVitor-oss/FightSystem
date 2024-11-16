using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public DetectEnime detectEnime;
    public PlayerController pcontroler;
    public bool isAttackEnemy = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // has um currentEnime
            if (detectEnime.GetCurrentEnime() != null)
            {
                EnemyScript enemy = detectEnime.GetCurrentEnime();
                // va para cima do inimigo
                if (!isAttackEnemy)
                {
                    //olhar para o inimifo

                    float dist = Vector3.Distance(transform.position, enemy.transform.position);
                    if (dist > 1)
                    {

                        Vector3 moveDirection = (enemy.transform.position - transform.position).normalized;
                        // moveDirection.y = 0;

                        // Faz o player olhar na direção do movimento
                        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10.0f * Time.deltaTime);


                        //mover para inimigo
                        StartCoroutine(MoveToEnemy(enemy, (dist * 1.5f) / 10.0f));
                        isAttackEnemy = true;

                        //iniciar a animação de jump punch
                        pcontroler.StartAniPunch();
                    }
                }
            }
        }
    }
    IEnumerator MoveToEnemy(EnemyScript enemy, float time)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = enemy.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            // Calcula a fração do tempo entre a posição inicial e o alvo
            float progress = elapsedTime / time;
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Garante que o player chegue exatamente na posição do inimigo no final
        transform.position = targetPosition;
        isAttackEnemy = false;
    }

}
