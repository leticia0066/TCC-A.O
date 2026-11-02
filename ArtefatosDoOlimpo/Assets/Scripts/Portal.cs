
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Próxima cena")]
    public string nextSceneName;

    private bool playerEntered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerEntered)
            return;

        if (!collision.CompareTag("Player"))
            return;

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("Portal: o nome da próxima cena não foi configurado!");
            return;
        }

        if (TransicaoCenas.Instance == null)
        {
            Debug.LogError("Portal: Controlador TransicaoCenas não encontrado!");
            return;
        }

        playerEntered = true;

        Debug.Log("Portal ativado! Indo para: " + nextSceneName);

        TransicaoCenas.Instance.TrocarCena(nextSceneName);
    }
}
