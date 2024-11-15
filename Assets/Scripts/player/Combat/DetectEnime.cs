using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectEnime : MonoBehaviour
{
    public Vector3 inputDirection;
    public EnemyScript currentEnime;
    public float distMaxEnime = 10.0f;
    public LayerMask layerMask;
    public Transform marker;

    private void Update()
    {
        if(currentEnime!= null){
            marker.position = currentEnime.transform.position;
        }

        Camera camera = Camera.main;
        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        inputDirection = forward;

        RaycastHit hit;

        if (Physics.SphereCast(transform.position, 3f, inputDirection, out hit, distMaxEnime,layerMask))
        {
            if(hit.collider.transform.GetComponent<EnemyScript>()){
                currentEnime = hit.collider.transform.GetComponent<EnemyScript>();
            }
        }
    }
    public EnemyScript GetCurrentEnime()
    {
        return currentEnime;
    }
}
