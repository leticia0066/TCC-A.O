
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Número da fase")]
    [Range(1, 3)]
    public int numeroDaFase = 1;

    [Header("Painel de vitória - somente Fase 3")]
    public GameObject painelVitoria;

    private bool playerEntered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerEntered)
            return;

        if (!collision.CompareTag("Player"))
            return;

        if (GameManager.instance == null)
        {
            Debug.LogError("Portal: GameManager não encontrado!");
            return;
        }

        if (!VerificarArtefato())
        {
            Debug.LogWarning(
                "Você precisa coletar o artefato desta fase antes de sair!"
            );
            return;
        }

        // FASE 3: verificar todos os artefatos.
        if (numeroDaFase == 3)
        {
            if (!GameManager.instance.TodosArtefatosColetados())
            {
                Debug.LogWarning(
                    "Você ainda precisa recuperar todos os artefatos do Olimpo!"
                );
                return;
            }

            if (painelVitoria == null)
            {
                Debug.LogError(
                    "Portal: arraste o PainelVitoria para o campo no Inspector!"
                );
                return;
            }

            if (TransicaoCenas.Instance == null)
            {
                Debug.LogError(
                    "Portal: TransicaoCenas não foi encontrado!"
                );
                return;
            }

            playerEntered = true;

            ProgressoJogo.ConcluirFase(numeroDaFase);

            Debug.Log("VITÓRIA! TODOS OS ARTEFATOS FORAM COLETADOS!");

            // Escurece a tela antes de mostrar o painel.
            TransicaoCenas.Instance.MostrarPainelComTransicao(
                painelVitoria
            );

            return;
        }

        // FASES 1 E 2.
        playerEntered = true;

        ProgressoJogo.ConcluirFase(numeroDaFase);

        Debug.Log("FASE " + numeroDaFase + " CONCLUÍDA!");

        if (TransicaoCenas.Instance != null)
        {
            TransicaoCenas.Instance.TrocarCena("SelecaoFases");
        }
        else
        {
            Debug.LogError(
                "Portal: Controlador TransicaoCenas não encontrado!"
            );

            playerEntered = false;
        }
    }

    private bool VerificarArtefato()
    {
        switch (numeroDaFase)
        {
            case 1:
                return GameManager.instance.espadaColetada;

            case 2:
                return GameManager.instance.escudoColetado;

            case 3:
                return GameManager.instance.raioColetado;

            default:
                Debug.LogError("Número de fase inválido!");
                return false;
        }
    }
}
