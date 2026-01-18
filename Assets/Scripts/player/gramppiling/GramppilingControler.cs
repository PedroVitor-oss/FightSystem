using System.Collections;
using System.Collections.Generic;

// using System.Numerics;
using UnityEngine;

public class GramppilingControler : MonoBehaviour
{
    // Start is called before the first frame update
    public GramppilingMarker gramppilingMarker;
    public PlayerStateControler playerStateControler;
    public PlayerController playerMoviment;

    public LayerMask layerMask;

    private Vector3 best;

    public float distMaxPoint = 10;
    public float force = 10;

    public Transform hand1;
    public Transform hand2;

    private Rigidbody rig;
    private LineRenderer lineRenderer;
    private bool statAni = false;
    
    private Vector3 normalToRotation;
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        playerMoviment = GetComponent<PlayerController>();
        lineRenderer = GetComponent<LineRenderer>();

        //config line renderer
        // Set the material
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Set the color
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;

        // Set the width
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;

        // Set the number of vertices
        lineRenderer.positionCount = 3;

    }

    // Update is called once per frame
    void Update()
    {
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");

        GetPoint();
        if (Input.GetMouseButtonDown(2) && statAni == false)
        {
            Debug.Log("Use Gramppiling");
            StartGramppliling();
        }
        if (statAni)
        {
            lineRenderer.SetPosition(0, hand1.position);
            lineRenderer.SetPosition(2, hand2.position);
        }
        else
        {
            Vector3 p1 =lineRenderer.GetPosition(0);
            Vector3 p2 =lineRenderer.GetPosition(2);
            p1.y-= 2*Time.deltaTime;
            p2.y-= 2*Time.deltaTime;
            lineRenderer.SetPosition(0, p1);
            lineRenderer.SetPosition(2, p2);

        }
    }
    void Gramppiling()
    {
        best.y += 1;
        Vector3 direction = best - transform.position;
        rig.AddForce(direction * force, ForceMode.Impulse);
        statAni = false;
        playerStateControler.ChangeState(PlayerState.Fall);
    }
    void desativeGramppiling()
    {
         lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
        lineRenderer.SetPosition(2, Vector3.zero);
    }

    void StartGramppliling()
    {
        float dist = Vector3.Distance(transform.position, best);
        if (dist < distMaxPoint)
            if (playerStateControler.ChangeState(PlayerState.Gramppiling))
            {
                RotateForGramppiling();
                playerMoviment.AniGramppliling();
                Invoke("desativeGramppiling", 3f);
                Invoke("Gramppiling", 0.5f);
                Invoke("DrawLine",0.165f);
                /*
                    021 - 5 - 100
                    007 - x - 33
                    700/21 = 

                */
            }
    }
    void DrawLine()
    {
                        statAni = true;

        lineRenderer.SetPosition(0, hand1.position);
        lineRenderer.SetPosition(1, best);
        lineRenderer.SetPosition(2, hand2.position);
    }
    void RotateForGramppiling()
    {
        Vector3 dir = -normalToRotation;
        dir.y = 0f;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    void GetPoint()
    {
        Camera camera = Camera.main;
        Vector3 forward = camera.transform.forward;
        RaycastHit hit;
        if (Physics.SphereCast(camera.transform.position, 3f, forward, out hit, distMaxPoint, layerMask))
        {
            gramppilingMarker.DefineTarget(hit.transform);
            best = hit.transform.position;
            normalToRotation = hit.normal;
        }

    }
}
