using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
// using System.Threading.Tasks.Dataflow;
using UnityEngine;

public class ClimbManager : MonoBehaviour
{
    public PlayerStateControler playerStateControler;
    public PlayerController playerMoviment;
    public LayerMask wallLayer;
    public float wallRunAngleThreshold = 0.3f; // Ajuste esse valor
    public float speed = 100;
    // Start is called before the first frame update
    private string positionWall;

    private Rigidbody rig;
    void Start()
    {
        playerStateControler = GetComponent<PlayerStateControler>();
        playerMoviment = GetComponent<PlayerController>();
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (playerStateControler.CheckState(PlayerState.RunWall))
        {
            MoveRunWall();
        }

        if (playerStateControler.CheckState(PlayerState.Climb))
        {
            MoveClimb();
        }
    }
    private void MoveClimb()
    {
        RaycastHit hitForward;
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(transform.up * verticalInput* 5.0f * Time.deltaTime);

        bool hitWallFoward = Physics.Raycast(
        transform.position,
        transform.forward,
        out hitForward,
        1f,
        wallLayer
    );
        if (!hitWallFoward)
        {
            rig.velocity = transform.forward * 5 + transform.up * 5f;
            EndClimbi();
        }
    }
    private void MoveRunWall()
    {
        Vector3 forceForward = new Vector3(0, 0, 1);
        transform.Translate(forceForward * speed * Time.deltaTime);
        RaycastHit hitRight;
        RaycastHit hitLeft;
        bool hitWallRight = Physics.Raycast(
        transform.position,
        transform.right,
        out hitRight,
        1f,
        wallLayer
    );
        bool hitWallLeft = Physics.Raycast(
            transform.position,
            -transform.right,
            out hitLeft,
            1f,
            wallLayer
        );

        if (!hitWallLeft && !hitWallRight)
        {
            rig.velocity = forceForward * speed;
            StopRunWall();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            EditorApplication.isPaused = true;
            Debug.Log("forceForward " + forceForward);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // rig.AddForce((transform.left + forceForward + Vector3.up) * forceJump);
            int direction = positionWall == "right" ? 1 : -1;
            rig.velocity = forceForward * speed + (transform.right * direction * 3.0f) + Vector3.up * 10;

            StopRunWall();
        }
    }
    public void CheckWallCollision()
    {
        RaycastHit hitRight;
        RaycastHit hitLeft;
        RaycastHit hitForward;
        RaycastHit mainHit;

        if (!playerStateControler.CheckState(PlayerState.Fall))
            return;

        bool hitWallFoward = Physics.Raycast(
            transform.position,
            transform.forward,
            out hitForward,
            1f,
            wallLayer
        );
        bool hitWallRight = Physics.Raycast(
            transform.position,
            transform.right,
            out hitRight,
            1f,
            wallLayer
        );
        bool hitWallLeft = Physics.Raycast(
            transform.position,
            -transform.right,
            out hitLeft,
            1f,
            wallLayer
        );

        if (hitWallFoward && !hitWallLeft && !hitWallRight)//CLIMB
        {
            Debug.Log("[ClimbManage] colision for climb");
            mainHit = hitForward;

            float wallVerticality = Mathf.Abs(mainHit.normal.y);
            if (wallVerticality > 0.2f)
                return; // não é parede
            //rotacionar olhar para parede
            RotateForClimb(mainHit);
            StartClimb();

        }
        else if (hitWallLeft || hitWallRight)
        {
            Debug.Log("[ClimbManage] colision for runwall");
            mainHit = hitWallRight ? hitRight : hitLeft;
            RaycastHit hit = hitWallRight ? hitRight : hitLeft;
            string wallSide = hitWallRight ? "right" : "left";
            Debug.Log("Hit na parede do lado: " + wallSide);
            float wallVerticality = Mathf.Abs(mainHit.normal.y);
            if (wallVerticality > 0.2f)
                return; // não é parede

            RotateForWallRun(mainHit);
            StartRunWall();


        }
    }

    void RotateForClimb(RaycastHit hit)
    {
        Vector3 dir = -hit.normal;
        dir.y = 0f;
        transform.rotation = Quaternion.LookRotation(dir);
    }
    void RotateForWallRun(RaycastHit hit)
    {
        Vector3 dir = Vector3.Cross(Vector3.up, hit.normal);

        if (Vector3.Dot(dir, transform.forward) < 0)
            dir = -dir;

        dir.y = 0f;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    void DefinePositionWall(string side)
    {
        Debug.Log("Definindo posição na parede do lado: " + side);
        positionWall = side;
    }

    public void StartRunWall()
    {
        Debug.Log("[ClimbManage] StartRunWall");
        playerStateControler.ChangeState(PlayerState.RunWall);
        playerMoviment.AniamationCLimb();

        // PointFollowCam.ToPoint(0);
        // playerMoviment.movimentInGround = false;
        // runWall = true;
        playerMoviment.disableFisic();
    }

    public void StartClimb()
    {
        Debug.Log("[ClimbManage] StartClimb");
        playerStateControler.ChangeState(PlayerState.Climb);
        playerMoviment.AniamationCLimb();
        // PointFollowCam.ToPoint(2);
        // playerMoviment.movimentInGround = false;
        // inClimbi = true;
        playerMoviment.disableFisic();

    }
    public void EndClimbi()
    {
        // state = fall;
        playerMoviment.EndAniamationCLimb();
        playerStateControler.ChangeState(PlayerState.Fall);
        // PointFollowCam.ToPoint(0);
        // playerMoviment.movimentInGround = true;
        // inClimbi = false;
        playerMoviment.activateFisic();
    }
    public void StopRunWall()
    {
        // state = fall;
        Debug.Log("[ClimbControler] StopRunWall");
        playerMoviment.EndAniamationCLimb();
        playerStateControler.ChangeState(PlayerState.Fall);

        // PointFollowCam.ToPoint(0);
        // playerMoviment.movimentInGround = true;
        // inClimbi = false;
        playerMoviment.activateFisic();
    }

    public void StartRotateInWall()
    {
        // playerMoviment.StartAniTunrWall();
        // StopTempClimb();
        // Invoke("StartClimb", 1); //tempo da animação
    }
    // Método auxiliar para desenhar debug rays
    void OnDrawGizmosSelected()
    {
        // Ray para baixo
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, Vector3.down * 1f);

        // Rays laterais
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.right * 1f);
        Gizmos.DrawRay(transform.position, -transform.right * 1f);
    }
}
