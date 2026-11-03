
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

        playerEntered = true;

        // Registra a conclusão da fase.
        ProgressoJogo.ConcluirFase(numeroDaFase);

        Debug.Log("FASE " + numeroDaFase + " CONCLUÍDA!");

        // Fase 3: vitória final.
        if (numeroDaFase == 3)
        {
            if (GameManager.instance.TodosArtefatosColetados())
            {
                if (painelVitoria != null)
                {
                    painelVitoria.SetActive(true);
                    Time.timeScale = 0f;
                    Debug.Log("VITÓRIA! TODOS OS ARTEFATOS FORAM COLETADOS!");
                }
                else
                {
                    Debug.LogWarning(
                        "Configure o Painel de Vitória no Inspector!"
                    );
                }
            }

            return;
        }

        // Fases 1 e 2: volta para a seleção.
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
