
using System.Collections;
using UnityEngine;

public class UITransition : MonoBehaviour
{
    [Header("Painel animado")]
    public GameObject panel;

    [Header("Configuração da animação")]
    [Min(0.01f)]
    public float duration = 0.25f;

    [Range(0.1f, 1f)]
    public float initialScale = 0.85f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Coroutine transitionCoroutine;

    private void Awake()
    {
        if (panel == null)
            panel = gameObject;

        canvasGroup = panel.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = panel.AddComponent<CanvasGroup>();

        rectTransform = panel.GetComponent<RectTransform>();
    }

    private void Start()
    {
        if (panel == null)
            return;

        canvasGroup.alpha = 0f;
        rectTransform.localScale = Vector3.one * initialScale;
    }

    public void Show()
    {
        if (panel == null)
            return;

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        panel.SetActive(true);

        transitionCoroutine = StartCoroutine(
            AnimarPainel(1f, 1f)
        );
    }

    public void Hide()
    {
        if (panel == null)
            return;

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        if (!panel.activeSelf)
            return;

        transitionCoroutine = StartCoroutine(
            EsconderPainel()
        );
    }

    private IEnumerator EsconderPainel()
    {
        yield return AnimarPainel(initialScale, 0f);

        panel.SetActive(false);
        transitionCoroutine = null;
    }

    private IEnumerator AnimarPainel(
        float escalaFinal,
        float alphaFinal)
    {
        float tempo = 0f;

        Vector3 escalaInicial = rectTransform.localScale;
        float alphaInicial = canvasGroup.alpha;

        while (tempo < duration)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(tempo / duration);

            // Transição suave.
            float suave = progresso * progresso *
                          (3f - 2f * progresso);

            rectTransform.localScale = Vector3.Lerp(
                escalaInicial,
                Vector3.one * escalaFinal,
                suave
            );

            canvasGroup.alpha = Mathf.Lerp(
                alphaInicial,
                alphaFinal,
                suave
            );

            yield return null;
        }

        rectTransform.localScale = Vector3.one * escalaFinal;
        canvasGroup.alpha = alphaFinal;

        transitionCoroutine = null;
    }
}
