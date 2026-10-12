
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Alvo da câmera")]
    public Transform target;

    [Header("Acompanhamento")]
    [Min(0.01f)]
    public float smoothSpeed = 5f;

    public Vector3 offset = new Vector3(0f, 1f, -10f);

    [Header("Zoom 2D")]
    public bool ajustarZoomAoIniciar = true;

    [Min(0.1f)]
    public float tamanhoCamera = 3.5f;

    private Camera cameraComponent;

    private void Awake()
    {
        cameraComponent = GetComponent<Camera>();

        if (cameraComponent.orthographic &&
            ajustarZoomAoIniciar)
        {
            cameraComponent.orthographicSize = tamanhoCamera;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;

        // Mantém a câmera na profundidade correta para um jogo 2D.
        desiredPosition.z = -10f;

        float fatorSuavidade =
            1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            fatorSuavidade
        );
    }
}
