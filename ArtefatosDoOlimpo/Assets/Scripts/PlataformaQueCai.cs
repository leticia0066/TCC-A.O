
using System.Collections;
using UnityEngine;

public class PlataformaQueCai : MonoBehaviour
{
    [Header("Tempo para cair")]
    public float tempoAntesDeCair = 1f;

    [Header("Queda")]
    public float velocidadeQueda = 8f;

    [Header("Tremor")]
    public float duracaoTremor = 0.5f;
    public float intensidadeTremor = 0.05f;

    [Header("Retorno")]
    public float tempoParaVoltar = 3f;
    public float velocidadeRetorno = 5f;

    private Rigidbody2D rb;
    private Vector3 posicaoInicial;
    private Quaternion rotacaoInicial;

    private bool ativada;
    private bool caindo;
    private bool retornando;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        posicaoInicial = transform.position;
        rotacaoInicial = transform.rotation;

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (ativada || retornando)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            ativada = true;
            StartCoroutine(SequenciaQueda());
        }
    }

    private IEnumerator SequenciaQueda()
    {
        yield return new WaitForSeconds(tempoAntesDeCair);

        // Tremida antes da queda.
        float tempo = 0f;

        while (tempo < duracaoTremor)
        {
            tempo += Time.deltaTime;

            Vector2 tremor = Random.insideUnitCircle *
                intensidadeTremor;

            transform.position = posicaoInicial + new Vector3(
                tremor.x,
                tremor.y,
                0f
            );

            yield return null;
        }

        transform.position = posicaoInicial;

        // Ativa a gravidade e deixa a plataforma cair.
        caindo = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        rb.linearVelocity = Vector2.down * velocidadeQueda;

        yield return new WaitForSeconds(tempoParaVoltar);

        // Prepara o retorno.
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        caindo = false;
        retornando = true;

        // Volta suavemente ao lugar inicial.
        while (Vector2.Distance(
            transform.position,
            posicaoInicial
        ) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                posicaoInicial,
                velocidadeRetorno * Time.deltaTime
            );

            yield return null;
        }

        transform.position = posicaoInicial;
        transform.rotation = rotacaoInicial;

        rb.linearVelocity = Vector2.zero;

        ativada = false;
        retornando = false;
    }
}
