using UnityEngine;

public class PlatformPassenger : MonoBehaviour
{
    private Transform plataformaAtual;
    private Transform paiOriginal;

    private void Awake()
    {
        paiOriginal = transform.parent;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("MovingPlatform"))
            return;

        if (collision.contactCount == 0)
            return;

        foreach (ContactPoint2D contato in collision.contacts)
        {
            // Confirma que o jogador está pousando sobre a plataforma.
            if (contato.normal.y > 0.5f)
            {
                plataformaAtual = collision.transform;
                transform.SetParent(plataformaAtual);

                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (plataformaAtual == null)
            return;

        if (collision.transform != plataformaAtual)
            return;

        // Evita tentar alterar o pai durante a desativação dos objetos.
        if (!gameObject.activeInHierarchy)
            return;

        transform.SetParent(paiOriginal);
        plataformaAtual = null;
    }

    private void OnDisable()
    {
        // Limpa a referência sem tentar alterar a hierarquia
        // enquanto o objeto está sendo desativado.
        plataformaAtual = null;
    }
}