using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5.0f;
    public float autoRotationSpeed = 10.0f;
    [Header("Animação")]
    public Animator ani;
    [Header("Combate")]
    public CombatController combatControl;

    void Start(){

    }

    private void Update()
    {
        // Pega as entradas do teclado
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Verifica se há movimento
        if ((horizontal != 0 || vertical != 0) && !combatControl.isAttackEnemy)
        {
            // Calcula a direção do movimento com base na câmera
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0; // Ignora a rotação vertical da câmera
            cameraForward.Normalize();

            Vector3 cameraRight = Camera.main.transform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();

            Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

            // Faz o player olhar na direção do movimento
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, autoRotationSpeed * Time.deltaTime);


            // Move o player
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            //Animação de andando
            ani.SetFloat("MoveVertical",vertical);
        }
    }
}
