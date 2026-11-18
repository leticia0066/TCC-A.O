
using UnityEngine;

public class Machado : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 8f;

    [Header("Rotacao")]
    [Tooltip("Graus de giro por segundo. 500 significa mais de uma volta completa por segundo.")]
    public float rotationSpeed = 500f;

    [Header("Tempo de vida")]
    public float tempoDeVida = 5f;

    private Vector2 direction;
    private bool lancado;

    private void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        lancado = true;
    }

    private void Update()
    {
        if (!lancado)
            return;

        transform.position +=
            (Vector3)(direction * speed * Time.deltaTime);

        // Gira continuamente durante o voo.
        transform.Rotate(
            0f,
            0f,
            rotationSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!lancado)
            return;

        if (collision.CompareTag("Player"))
        {
            PlayerHealth vida =
                collision.GetComponentInParent<PlayerHealth>();

            if (vida != null)
                vida.TakeDamage(1);

            Destroy(gameObject);
            return;
        }

        // Ignora o próprio Minotauro.
        if (collision.GetComponentInParent<Minotauro>() != null)
            return;

        // Desaparece ao atingir o cenário ou outro obstáculo.
        Destroy(gameObject);
    }
}
