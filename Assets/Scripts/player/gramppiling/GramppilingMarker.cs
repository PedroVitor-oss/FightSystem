using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GramppilingMarker : MonoBehaviour
{
    // Start is called before the first frame update
    public Image marker;
public Transform target;
public float maxDistance = 50f; // Distância máxima para redução de tamanho
public float minScale = 0.3f; // Escala mínima do marcador

private float initialScale;
private RectTransform markerRectTransform;

void Start()
{
    if (marker != null)
    {
        markerRectTransform = marker.GetComponent<RectTransform>();
        initialScale = markerRectTransform.localScale.x; // Ou use .localScale se for Vector3
        Debug.Log("[GramppilingMaker 14] Escala inicial: " + initialScale);
    }
    else
    {
        Debug.LogError("Marker não está atribuído!");
    }
}

void Update()
{
    if (marker == null || target == null || markerRectTransform == null)
        return;
    
    // Calcular distância
    float dist = Vector3.Distance(transform.position, target.position);
      if (dist < maxDistance)
    {
        marker.gameObject.SetActive(true);
        // return; // Sai do Update para não processar o resto
    }
    else
    {
        // Garantir que o marcador está ativo
        if (marker.gameObject.activeSelf)
            marker.gameObject.SetActive(false);
    }

    // Atualizar posição do marcador
    marker.transform.position = Camera.main.WorldToScreenPoint(target.position);
    
    // Ajustar escala baseado na distância
    float scaleMultiplier = CalculateScaleByDistance(dist);
    markerRectTransform.localScale = Vector3.one * scaleMultiplier;
}

float CalculateScaleByDistance(float distance)
{
    // Limitar a distância ao máximo definido
    float clampedDistance = Mathf.Clamp(distance, 0, maxDistance);
    
    // Calcular a escala baseado na distância (inverso: mais perto = maior, mais longe = menor)
    // Usando interpolação linear inversa
    float t = clampedDistance / maxDistance;
    float scale = Mathf.Lerp(initialScale, initialScale * minScale, t);
    
    return scale;
}

    public void DefineTarget(Transform newtarget){
        target = newtarget;
    }
}
