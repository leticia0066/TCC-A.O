
using System.Collections;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Dano")]
    [Min(1)]
    public int damage = 1;

    [Header("Movimento dos espinhos")]
    public float altura = 2f;
    public float velocidade = 4f;

    [Header("Tempo de espera")]
    public float tempoEscondido = 2f;
    public float tempoLevantado = 1.5f;

    [Header("Inicializacao")]
    public bool iniciarEscondido = true;

    private Vector3 posicaoSuperior;
    private Vector3 posicaoInferior;

    private Collider2D colisor;

    private void Awake()
    {
        colisor = GetComponent<Collider2D>();

        if (colisor == null)
        {
            Debug.LogError(
                "SpikeTrap precisa de um Collider2D!"
            );
            return;
        }

        // Os espinhos detectam o jogador, mas não o empurram.
        colisor.isTrigger = true;
    }

    private void Start()
    {
        posicaoSuperior = transform.position;

        posicaoInferior = posicaoSuperior -
            Vector3.up * altura;

        if (iniciarEscondido)
            transform.position = posicaoInferior;

        StartCoroutine(CicloEspinhos());
    }

    private IEnumerator CicloEspinhos()
    {
        while (true)
        {
            yield return new WaitForSeconds(tempoEscondido);

            yield return StartCoroutine(
                MoverEspinhos(posicaoSuperior)
            );

            yield return new WaitForSeconds(tempoLevantado);

            yield return StartCoroutine(
                MoverEspinhos(posicaoInferior)
            );
        }
    }

    private IEnumerator MoverEspinhos(Vector3 destino)
    {
        while (Vector3.Distance(transform.position, destino) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destino,
                velocidade * Time.deltaTime
            );

            yield return null;
        }

        transform.position = destino;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth player =
            collision.GetComponentInParent<PlayerHealth>();

        if (player == null)
            return;

        Debug.Log("Espinhos atingiram o jogador!");

        player.TakeDamage(damage);
    }
}
