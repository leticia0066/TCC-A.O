
using UnityEngine;

public class HUDPositionManager : MonoBehaviour
{
    [Header("Corações")]
    public RectTransform heart1;
    public RectTransform heart2;
    public RectTransform heart3;

    [Header("Moedas")]
    public RectTransform iconeMoeda;
    public RectTransform textoQuantidadeMoedas;

    [Header("Painel de Artefatos")]
    public RectTransform painelArtefatos;
    public RectTransform imagemespada;
    public RectTransform imagemescudo;
    public RectTransform imagemraio;

    [Header("Margens")]
    public float margemEsquerda = 35f;
    public float margemDireita = 35f;
    public float margemSuperior = 20f;

    [Header("Espaçamento")]
    public float espacamentoCoracoes = 15f;

    [Header("Posição das moedas")]
    public float centroMoedas = 0f;
    public float distanciaIconeMoeda = 45f;

    private RectTransform canvasRect;

    private void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("Canvas não encontrado!");
            return;
        }

        canvasRect = canvas.GetComponent<RectTransform>();

        PosicionarHUD();
    }

    private void OnRectTransformDimensionsChange()
    {
        if (canvasRect != null)
        {
            PosicionarHUD();
        }
    }

    private void PosicionarHUD()
    {
        PosicionarCoracoes();
        PosicionarMoedas();
        PosicionarArtefatos();
    }

    private void PosicionarCoracoes()
    {
        RectTransform[] coracoes =
        {
            heart1,
            heart2,
            heart3
        };

        float x = margemEsquerda;

        foreach (RectTransform coracao in coracoes)
        {
            if (coracao == null)
                continue;

            coracao.anchorMin = new Vector2(0f, 1f);
            coracao.anchorMax = new Vector2(0f, 1f);
            coracao.pivot = new Vector2(0f, 0.5f);

            coracao.anchoredPosition =
                new Vector2(x, -margemSuperior);

            x += coracao.rect.width + espacamentoCoracoes;
        }
    }

    private void PosicionarMoedas()
    {
        if (iconeMoeda == null ||
            textoQuantidadeMoedas == null)
            return;

        iconeMoeda.anchorMin = new Vector2(0.5f, 1f);
        iconeMoeda.anchorMax = new Vector2(0.5f, 1f);
        iconeMoeda.pivot = new Vector2(1f, 0.5f);

        iconeMoeda.anchoredPosition =
            new Vector2(
                centroMoedas + distanciaIconeMoeda,
                -margemSuperior
            );

        textoQuantidadeMoedas.anchorMin =
            new Vector2(0.5f, 1f);

        textoQuantidadeMoedas.anchorMax =
            new Vector2(0.5f, 1f);

        textoQuantidadeMoedas.pivot =
            new Vector2(1f, 0.5f);

        textoQuantidadeMoedas.anchoredPosition =
            new Vector2(centroMoedas, -margemSuperior);
    }

    private void PosicionarArtefatos()
    {
        if (painelArtefatos == null)
            return;

        // Posiciona o painel no canto superior direito
        painelArtefatos.anchorMin = new Vector2(1f, 1f);
        painelArtefatos.anchorMax = new Vector2(1f, 1f);
        painelArtefatos.pivot = new Vector2(1f, 0.5f);

        painelArtefatos.sizeDelta = new Vector2(170f, 50f);

        painelArtefatos.anchoredPosition =
            new Vector2(-margemDireita, -margemSuperior);

        // Organiza os três artefatos lado a lado
        PosicionarIconeArtefato(imagemespada, -145f);
        PosicionarIconeArtefato(imagemescudo, -85f);
        PosicionarIconeArtefato(imagemraio, -25f);
    }

    private void PosicionarIconeArtefato(
        RectTransform icone, float x)
    {
        if (icone == null)
            return;

        icone.anchorMin = new Vector2(0.5f, 0.5f);
        icone.anchorMax = new Vector2(0.5f, 0.5f);

        icone.pivot = new Vector2(0.5f, 0.5f);

        icone.sizeDelta = new Vector2(50f, 50f);

        icone.anchoredPosition = new Vector2(x, 0f);
    }
}
