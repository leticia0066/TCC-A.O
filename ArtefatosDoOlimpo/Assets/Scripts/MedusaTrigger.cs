
using UnityEngine;
using System.Collections;

public class MedusaTrigger : MonoBehaviour
{
    [Header("Boss")]
    public GameObject medusa;

    [Header("Pilar / Porta da arena")]
    public GameObject bossDoor;

    [Header("Tempo para ativar a Medusa")]
    public float tempoAntesDaMedusa = 2f;

    private bool fightStarted = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (fightStarted)
            return;

        StartCoroutine(StartFight());
    }

    private IEnumerator StartFight()
    {
        fightStarted = true;

        // Ativa a animação do pilar.
        if (bossDoor != null)
        {
            bossDoor.SetActive(true);

            // Toca o som no início do movimento.
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.TocarSubidaPilar();
            }
        }
        else
        {
            Debug.LogWarning("Pilar/porta não foi atribuído!");
        }

        // Aguarda antes de ativar a Medusa.
        yield return new WaitForSeconds(tempoAntesDaMedusa);

        if (medusa != null)
        {
            medusa.SetActive(true);
            Debug.Log("MEDUSA ATIVADA! BATALHA INICIADA!");
        }
        else
        {
            Debug.LogWarning("Medusa não foi atribuída no Inspector!");
        }
    }
}
