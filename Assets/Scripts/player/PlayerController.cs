using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject prefabball;
    [Header("Move")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;
    public float autoRotationSpeed = 10.0f;
    private Rigidbody rig;
    [SerializeField] LayerMask groundLayer;

    float horizontal;
    float vertical;
    Vector3 moveDirection;

    [Header("State PLayer")]
    public PlayerStateControler playerStateControler = new PlayerStateControler();
    public bool injump;
    public float jumpTime=0.5f;
    // public bool inGround;
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

        if (Input.GetKeyDown(KeyCode.Space))// && !combatControl.isAttackEnemy && Mathf.Abs(rig.velocity.y) < 0.01f)
        {
            if (playerStateControler.ChangeState(PlayerState.Jump) && playerStateControler.CheckInGround())
                StartAniJump();
        }

        if (playerStateControler.CheckState(PlayerState.Fight))
        {


            Vector3 moveDirection = detectEnime.GetCurrentEnime().gameObject.transform.position - transform.position;

            // Faz o player olhar na direção do inimigo
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, autoRotationSpeed * Time.deltaTime);

        }
        
        if ((horizontal != 0 || vertical != 0))
        {
            //verifica se pode se mover
                ani.SetFloat("MoveVertical", Mathf.Clamp(Mathf.Abs(vertical) + Mathf.Abs(horizontal), -1, 1));
            if (playerStateControler.ChangeState(PlayerState.Move))
            {

                // Verifica se há movimento

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
                ani.SetBool("punch", false);
            }
        }
        else
        {
            playerStateControler.ChangeState(PlayerState.Idle);
            ani.SetFloat("MoveVertical", 0);

        }

    }
    private void FixedUpdate()
    {
        if (playerStateControler.CheckState(PlayerState.Move))
        {
            float vy = rig.velocity.y;
            rig.velocity = moveDirection * moveSpeed * Time.deltaTime;
            rig.velocity = new Vector3(rig.velocity.x, vy, rig.velocity.z);
        }
        if(playerStateControler.CheckState(PlayerState.Idle))
        {
            rig.velocity = new Vector3(0, rig.velocity.y, 0);
        }
        if(playerStateControler.CheckState(PlayerState.Jump) && rig.velocity.y < 0)
        {
            playerStateControler.ChangeState(PlayerState.Fall);
        }

    }
    public void StartAniJump()
    {
        ani.SetTrigger("jump");
        ani.SetFloat("MoveVertical", 0);
        if (playerStateControler.CheckState(PlayerState.Move))
        {
            Invoke("Jump", 0.1f);
        }
        else
        {
            Invoke("Jump",jumpTime);
        }
    }
    public void Jump()
    {
        rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        playerStateControler.ExitGround();
        injump = true;
        ani.SetBool("inGround", playerStateControler.CheckInGround());
        // Debug.Log("Jump");
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            
            //verificar se o chao esta embaixo do player
            RaycastHit hit = new RaycastHit();
            // Debug.Log(hit);
            //intanciar um primitiva aqui
                Debug.Log(" colidiu com o chao");
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f , Vector3.down, out hit, 1f,groundLayer))
            {
                playerStateControler.ColisionGround();
                injump = false;
                ani.SetBool("inGround", playerStateControler.CheckInGround());
                Debug.Log("pode andar colidiu com o chao da forma certa");
            }
        }
    }
}
