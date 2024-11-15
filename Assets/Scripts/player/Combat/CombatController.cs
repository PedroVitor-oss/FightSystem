using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public DetectEnime detectEnime;
    public bool isAttackEnemy = false;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))    
        {
            // has um currentEnime
            if(detectEnime.GetCurrentEnime() != null)
            {
                EnemyScript enemy = detectEnime.GetCurrentEnime();
                // va para cima do inimigo
                if(!isAttackEnemy){
                    StartCoroutine(MoveToEnemy(enemy,0.7f));
                    isAttackEnemy = true;
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
