using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public DetectEnime detectEnime;
    public PlayerController pcontroler;
    public bool isAttackEnemy = false;
    private EnemyScript enemy;
    public Rigidbody rig;
    [Header("Combo")]
    public int comboStep = 0;
    public float comboCooldown = 1.0f;
    private float lastAttackTime = 0;


    private void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttackEnemy)
        {
            Vector3 moveDirection = (enemy.transform.position - transform.position).normalized;
            // moveDirection.y = 0;

            // Faz o player olhar na direção do inimigo
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10.0f * Time.deltaTime);

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            // has um currentEnime
            if (detectEnime.GetCurrentEnime() != null)
            {
                enemy = detectEnime.GetCurrentEnime();
                // atacar inimigo
                if (!isAttackEnemy)
                {
                    isAttackEnemy = true;

                    float dist = Vector3.Distance(transform.position, enemy.transform.position);
                    if (dist > 1) // inimigo muito longe ir para sima dele 
                    {



                        //mover para inimigo
                        StartCoroutine(MoveToEnemy(enemy, 0.9f));

                        //iniciar a animação de jump punch
                        pcontroler.StartAniPunch(0);
                    }
                    else if (dist <= 1)
                    {
                        // Está em alcance para iniciar o combo
                        isAttackEnemy = true;
                        ExecuteCombo();
                    }
                }
            }
        }
    }
    void ExecuteCombo()
    {
        if (isAttackEnemy)
        {
            // Verifica se está dentro do tempo para continuar o combo
            if (Time.time - lastAttackTime <= comboCooldown)
            {
                comboStep++;
                if (comboStep > 3) // Limite de combos
                {
                    comboStep = 1; // Reinicia o combo
                }
                pcontroler.StartAniPunch(comboStep);
            }
            else
            {
                comboStep = 1; // Reseta o combo se passar do cooldown
                pcontroler.StartAniPunch(comboStep);
            }

            lastAttackTime = Time.time; // Atualiza o tempo do último ataque
            Invoke("DesativeAttack", 1);
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
        DesativeAttack();
        // rig.constraints = RigidbodyConstraints.None;

    }
    void DesativeAttack()
    {
        isAttackEnemy = false;
        pcontroler.FinishAniPunch();
    }
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Colide");
        EnemyScript enimeColider = other.gameObject.GetComponent<EnemyScript>();
        if (enimeColider != null)
        {
            if (enimeColider == detectEnime.GetCurrentEnime())
            {
                Debug.Log("Colider how enime");
                // rig.constraints = RigidbodyConstraints.FreezePositionY;
                if (isAttackEnemy) enimeColider.Hit();
            }
        }
    }
    private void OnCollisionExit(Collision other)
    {
        Debug.Log("exit Colide");
        EnemyScript enimeColider = other.gameObject.GetComponent<EnemyScript>();
        if (enimeColider != null)
        {
            if (enimeColider == detectEnime.GetCurrentEnime())
            {
                rig.velocity = Vector3.zero;
            }
        }
    }
}
