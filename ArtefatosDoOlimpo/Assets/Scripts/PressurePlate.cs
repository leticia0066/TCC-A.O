
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
    public float pauseAtTop = 0.5f;
    public float descendDuration = 3f;

    [Header("Ativacao")]
    public bool returnToStart = true;

    private bool activated = false;
    private Vector3 pillarStartPosition;

    private void Start()
    {
        if (pillar != null)
        {
            pillarStartPosition = pillar.position;
        }
        else
        {
            Debug.LogWarning(
                "PressurePlate: arraste o pilar para o campo Pillar!"
            );
        }

        if (warningText != null)
        {
            warningText.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (activated)
            return;

        activated = true;

        // Som exclusivo de quando o jogador pisa na placa.
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarSomPlaca();
        }

        // Mostra o aviso por 3 segundos.
        if (warningText != null)
        {
            warningText.SetActive(true);
            StartCoroutine(HideText());
        }

        // Inicia a animacao do pilar.
        if (pillar != null)
        {
            StartCoroutine(MovePillar());
        }
    }

    private IEnumerator MovePillar()
    {
        Vector3 startPosition = pillarStartPosition;

        Vector3 topPosition = pillarStartPosition
            + Vector3.up * riseHeight;

        // O pilar sobe suavemente.
        yield return StartCoroutine(
            MoveBetweenPositions(
                startPosition,
                topPosition,
                riseDuration
            )
        );

        // Mantem o pilar no alto por um instante.
        if (pauseAtTop > 0f)
        {
            yield return new WaitForSeconds(pauseAtTop);
        }

        // Toca o som do movimento do pilar.
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarSubidaPilar();
        }

        // O pilar desce suavemente, se essa opcao estiver ativa.
        if (returnToStart)
        {
            yield return StartCoroutine(
                MoveBetweenPositions(
                    topPosition,
                    startPosition,
                    descendDuration
                )
            );
        }
    }

    private IEnumerator MoveBetweenPositions(
        Vector3 startPosition,
        Vector3 targetPosition,
        float duration
    )
    {
        if (duration <= 0f)
        {
            if (pillar != null)
            {
                pillar.position = targetPosition;
            }

            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (pillar == null)
                yield break;

            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

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
