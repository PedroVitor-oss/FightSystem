using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform player; // Referência ao player
    public float rotationSpeed = 5.0f; // Velocidade de rotação da câmera
    public float followDistance = 5.0f; // Distância da câmera ao player
    public float followHeight = 2.0f; // Altura da câmera em relação ao player
    public float maxVerticalAngle = 60.0f; // Ângulo máximo para cima
    public float minVerticalAngle = -10.0f; // Ângulo mínimo para baixo

    private float currentVerticalAngle = 0.0f; // Ângulo vertical atual

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Posiciona a câmera inicialmente na posição correta
        if (player != null)
        {
            transform.position = player.position - player.forward * followDistance + Vector3.up * followHeight;
            transform.LookAt(player);
        }
    }

    private void Update()
    {
        if (player == null) return;

        // Entrada de rotação do mouse
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        // Rotaciona a câmera horizontalmente em torno do player
        transform.RotateAround(player.position, Vector3.up, horizontalInput * rotationSpeed);

        // Calcula o ângulo vertical e limita-o
        currentVerticalAngle -= verticalInput * rotationSpeed;
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);

        // Aplica a rotação vertical mantendo a posição correta
        Quaternion verticalRotation = Quaternion.Euler(currentVerticalAngle, transform.eulerAngles.y, 0);
        Vector3 direction = verticalRotation * Vector3.forward;
        transform.position = player.position - direction * followDistance + Vector3.up * followHeight;

        // A câmera deve olhar para o player
        transform.LookAt(player);
    }
}
