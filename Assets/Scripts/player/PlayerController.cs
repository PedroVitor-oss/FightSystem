using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5.0f;
    public float autoRotationSpeed = 10.0f;
    private Rigidbody rig;
    float horizontal;
    float vertical;
    Vector3 moveDirection;
    [Header("Animação")]
    public Animator ani;
    [Header("Combate")]
    public CombatController combatControl;
    public DetectEnime detectEnime;
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Pega as entradas do teclado
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        moveDirection = Vector3.zero;

        //verifica se pode se mover
        if (!combatControl.isAttackEnemy)
        {
            // Verifica se há movimento
            if ((horizontal != 0 || vertical != 0))
            {
                // Calcula a direção do movimento com base na câmera
                Vector3 cameraForward = Camera.main.transform.forward;
                cameraForward.y = 0; // Ignora a rotação vertical da câmera
                cameraForward.Normalize();

                Vector3 cameraRight = Camera.main.transform.right;
                cameraRight.y = 0;
                cameraRight.Normalize();

                moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

                // Faz o player olhar na direção do movimento
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, autoRotationSpeed * Time.deltaTime);


                // Move o player
                // transform.position += moveDirection * moveSpeed * Time.deltaTime;


                //Animação de andando
                ani.SetFloat("MoveVertical", Mathf.Clamp(vertical + horizontal, -1, 1));
                ani.SetBool("punch", false);
            }
        }
        else
        {
            Vector3 moveDirection = detectEnime.GetCurrentEnime().gameObject.transform.position - transform.position;

            // Faz o player olhar na direção do inimigo
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, autoRotationSpeed * Time.deltaTime);

        }
    }
    private void FixedUpdate()
    {
        if (!combatControl.isAttackEnemy)
        {
            rig.velocity = moveDirection * moveSpeed * Time.deltaTime;
        }

    }
    public void StartAniPunch(int number)
    {
        ani.SetTrigger("punch");
        ani.SetInteger("punchnum", number);


        ani.SetFloat("MoveVertical", 0);
    }
    public void FinishAniPunch()
    {
        ani.SetBool("punch", false);
    }
}
