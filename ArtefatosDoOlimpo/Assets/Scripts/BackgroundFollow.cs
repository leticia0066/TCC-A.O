using UnityEngine;



public class BackgroundFollow : MonoBehaviour
{
    [Header("Câmera")]
    public Transform cameraTransform;



    [Header("Intensidade do Parallax")]
    [Range(0f, 1f)]
    public float seguirX = 0.2f;



    [Range(0f, 1f)]
    public float seguirY = 0.1f;



    private Vector3 posicaoInicialCamera;
    private Vector3 posicaoInicialFundo;



    private void Start()
    {
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;



            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
        }



        if (cameraTransform == null)
        {
            Debug.LogError(
                "BackgroundFollow: não foi possível encontrar a câmera."
            );



            enabled = false;
            return;
        }



        posicaoInicialCamera = cameraTransform.position;
        posicaoInicialFundo = transform.position;
    }



    private void LateUpdate()
    {
        if (cameraTransform == null)
        {
            return;
        }



        Vector3 deslocamentoCamera =
            cameraTransform.position - posicaoInicialCamera;



        float novoX =
            posicaoInicialFundo.x +
            deslocamentoCamera.x * seguirX;



        float novoY =
            posicaoInicialFundo.y +
            deslocamentoCamera.y * seguirY;



        transform.position = new Vector3(
            novoX,
            novoY,
            posicaoInicialFundo.z
        );
    }
}