
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

    [Header("Artefatos")]
    public RectTransform painelArtefatos;

    [Header("Posicionamento")]
    public float margemEsquerda = 35f;
    public float margemSuperior = 35f;
    public float margemDireita = 35f;

    [Header("Espacamento")]
    public float espacamentoCoracoes = 15f;

    private RectTransform canvasRect;

    private void Start()
    {
        canvasRect = GetComponentInParent<Canvas>()
            .GetComponent<RectTransform>();

        if (canvasRect == null)
        {
            Debug.LogError("Canvas não encontrado!");
            return;
        }

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

        float larguraTotal = 0f;

        foreach (RectTransform coracao in coracoes)
        {
            if (coracao != null)
            {
                larguraTotal += coracao.rect.width;
            }
        }

        larguraTotal += espacamentoCoracoes * 2f;

        float inicioX =
            -canvasRect.rect.width / 2f + margemEsquerda;

        float topoY =
            canvasRect.rect.height / 2f - margemSuperior;

        foreach (RectTransform coracao in coracoes)
        {
            if (coracao == null)
                continue;

            coracao.anchorMin = new Vector2(0.5f, 0.5f);
            coracao.anchorMax = new Vector2(0.5f, 0.5f);
            coracao.pivot = new Vector2(0f, 1f);

            coracao.anchoredPosition =
                new Vector2(inicioX, topoY);

            inicioX +=
                coracao.rect.width + espacamentoCoracoes;
        }
    }


    private void PosicionarMoedas()
    {
        if (iconeMoeda == null ||
            textoQuantidadeMoedas == null)
            return;

        float direita =
            canvasRect.rect.width / 2f - margemDireita;

        float topo =
            canvasRect.rect.height / 2f - margemSuperior;

        iconeMoeda.anchorMin = new Vector2(0.5f, 0.5f);
        iconeMoeda.anchorMax = new Vector2(0.5f, 0.5f);
        iconeMoeda.pivot = new Vector2(1f, 1f);

        iconeMoeda.anchoredPosition =
            new Vector2(direita, topo);

        textoQuantidadeMoedas.anchorMin =
            new Vector2(0.5f, 0.5f);

        textoQuantidadeMoedas.anchorMax =
            new Vector2(0.5f, 0.5f);

        textoQuantidadeMoedas.pivot =
            new Vector2(1f, 1f);

        textoQuantidadeMoedas.anchoredPosition =
            new Vector2(
                direita - iconeMoeda.rect.width - 10f,
                topo
            );
    }

    

    private void PosicionarArtefatos()
    {
        if (painelArtefatos == null)
            return;

        float direita =
            canvasRect.rect.width / 2f - margemDireita;

        float topo =
            canvasRect.rect.height / 2f -
            margemSuperior -
            75f;

        painelArtefatos.anchorMin =
            new Vector2(0.5f, 0.5f);

        painelArtefatos.anchorMax =
            new Vector2(0.5f, 0.5f);

        painelArtefatos.pivot =
            new Vector2(1f, 1f);

        painelArtefatos.anchoredPosition =
            new Vector2(direita, topo);
    }
}
