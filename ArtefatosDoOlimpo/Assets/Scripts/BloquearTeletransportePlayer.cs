
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BloquearTeleportePlayer : MonoBehaviour
{
    [Header("Proteção contra teleporte")]
    public float distanciaMaximaPorFrame = 0.75f;

    private Rigidbody2D rb;
    private PlayerHealth vida;

    private Vector2 ultimaPosicao;
    private bool inicializado;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        vida = GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        ultimaPosicao = rb.position;
        inicializado = true;

        Debug.Log("PROTEÇÃO CONTRA TELEPORTE ATIVADA!");
    }

    private void FixedUpdate()
    {
        if (!inicializado || rb == null || vida == null)
            return;

        if (vida.currentHealth <= 0 || vida.EstaMorrendo)
        {
            ultimaPosicao = rb.position;
            return;
        }

        Vector2 posicaoAtual = rb.position;

        float distancia = Vector2.Distance(
            ultimaPosicao,
            posicaoAtual
        );

        if (distancia > distanciaMaximaPorFrame)
        {
            Debug.LogWarning(
                "TELEPORTE INDEVIDO BLOQUEADO! Distância: "
                + distancia
            );

            rb.position = ultimaPosicao;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            transform.position = new Vector3(
                ultimaPosicao.x,
                ultimaPosicao.y,
                transform.position.z
            );

            return;
        }

        ultimaPosicao = posicaoAtual;
    }

    // Permite teletransportes legítimos, como checkpoints e saves.
    public void AutorizarTeleporte(Vector3 destino)
    {
        Vector2 destino2D = new Vector2(
            destino.x,
            destino.y
        );

        rb.position = destino2D;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.position = destino;

        ultimaPosicao = destino2D;

        Debug.Log("TELEPORTE AUTORIZADO: " + destino);
    }
}
