
using UnityEngine;
using System.Collections;

public class PressurePlate : MonoBehaviour
{
    [Header("Objetos do enigma")]
    public Transform pillar;
    public GameObject warningText;

    [Header("Movimento do pilar")]
    public float riseHeight = 3f;
    public float riseDuration = 4f;

    private bool activated = false;
    private Vector3 pillarStartPosition;

    private void Start()
    {
        if (pillar != null)
        {
            pillarStartPosition = pillar.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (activated)
            return;

        activated = true;

        // Mostra o aviso por 3 segundos.
        if (warningText != null)
        {
            warningText.SetActive(true);
            StartCoroutine(HideText());
        }

        // Toca o som da placa de pressão.
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarPlacaPressao();
        }

        // Faz o pilar subir lentamente.
        if (pillar != null)
        {
            StartCoroutine(RaisePillar());
        }
        else
        {
            Debug.LogWarning(
                "PressurePlate: arraste o pilar para o campo Pillar!"
            );
        }
    }

    private IEnumerator RaisePillar()
    {
        Vector3 startPosition = pillarStartPosition;

        Vector3 targetPosition = pillarStartPosition
            + Vector3.up * riseHeight;

        float elapsed = 0f;

        while (elapsed < riseDuration)
        {
            if (pillar == null)
                yield break;

            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / riseDuration);

            float smoothProgress = Mathf.SmoothStep(
                0f,
                1f,
                progress
            );

            pillar.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                smoothProgress
            );

            yield return null;
        }

        if (pillar != null)
        {
            pillar.position = targetPosition;
        }
    }

    private IEnumerator HideText()
    {
        yield return new WaitForSeconds(3f);

        if (warningText != null)
        {
            warningText.SetActive(false);
        }
    }
}
