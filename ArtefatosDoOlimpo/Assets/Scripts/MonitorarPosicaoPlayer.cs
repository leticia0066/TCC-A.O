
using UnityEngine;

public class MonitorarPosicaoPlayer : MonoBehaviour
{
    [Header("Configuração")]
    public float distanciaMinimaParaDetectar = 2f;

    private Vector3 ultimaPosicao;

    private void Start()
    {
        ultimaPosicao = transform.position;

        Debug.Log(
            "MONITORAMENTO INICIADO! Posição inicial: "
            + ultimaPosicao
        );
    }

    private void LateUpdate()
    {
        Vector3 posicaoAtual = transform.position;

        float distancia = Vector3.Distance(
            ultimaPosicao,
            posicaoAtual
        );

        if (distancia >= distanciaMinimaParaDetectar)
        {
            Debug.LogWarning(
                "DESLOCAMENTO GRANDE DETECTADO!"
                + "\nPosição anterior: " + ultimaPosicao
                + "\nPosição atual: " + posicaoAtual
                + "\nDistância: " + distancia
                + "\nObjeto pai: "
                + (transform.parent != null
                    ? transform.parent.name
                    : "Nenhum")
            );
        }

        ultimaPosicao = posicaoAtual;
    }
}
