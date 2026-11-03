
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SistemaInteracao : MonoBehaviour
{
    [Header("Jogador")]
    public string tagJogador = "Player";

    [Header("Guardião")]
    public string nomeGuardiao = "MINOTAURO";

    [TextArea(3, 6)]
    public string mensagem =
        "Você ousa entrar em meu território? Prepare-se para enfrentar minha força!";

    [Header("Teclas")]
    public KeyCode teclaInteragir = KeyCode.E;
    public KeyCode teclaFechar = KeyCode.Space;

    [Header("Animação")]
    public float duracaoAnimacao = 0.25f;

    [Header("Cores")]
    public Color corFundo = new Color(0.035f, 0.075f, 0.13f, 0.97f);
    public Color corDourada = new Color(0.85f, 0.65f, 0.25f);
    public Color corTexto = new Color(0.96f, 0.92f, 0.82f);

    private bool jogadorPerto;
    private bool dialogoAberto;

    private Canvas canvas;
    private GameObject avisoInteracao;
    private GameObject painelDialogo;

    private CanvasGroup grupoAviso;
    private CanvasGroup grupoDialogo;

    private RectTransform rectAviso;
    private RectTransform rectDialogo;

    private Coroutine animacaoAtual;

    private void Start()
    {
        CriarInterface();
    }

    private void Update()
    {
        if (canvas == null)
            return;

        if (dialogoAberto)
        {
            if (Input.GetKeyDown(teclaFechar))
                FecharDialogo();

            return;
        }

        if (jogadorPerto && Input.GetKeyDown(teclaInteragir))
            AbrirDialogo();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(tagJogador))
            return;

        jogadorPerto = true;

        if (!dialogoAberto && avisoInteracao != null)
            avisoInteracao.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(tagJogador))
            return;

        jogadorPerto = false;

        if (avisoInteracao != null)
            avisoInteracao.SetActive(false);

        if (dialogoAberto)
            FecharDialogo();
    }

    private void CriarInterface()
    {
        // Cria o Canvas automaticamente.
        GameObject canvasObjeto = new GameObject(
            "Canvas_Interacao_" + nomeGuardiao,
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        canvas = canvasObjeto.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = canvasObjeto.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // Cria o aviso de interação.
        avisoInteracao = CriarPainel(
            "AvisoInteracao",
            canvasObjeto.transform,
            new Vector2(0.5f, 0.18f),
            new Vector2(280, 65),
            corFundo
        );

        Image imagemAviso = avisoInteracao.GetComponent<Image>();
        AdicionarContorno(imagemAviso);

        TMP_Text textoAviso = CriarTexto(
            "TextoAviso",
            avisoInteracao.transform,
            "[ E ]  INTERAGIR",
            25,
            corDourada,
            FontStyles.Bold
        );

        AjustarTextoCompleto(textoAviso);

        grupoAviso = avisoInteracao.AddComponent<CanvasGroup>();
        rectAviso = avisoInteracao.GetComponent<RectTransform>();

        avisoInteracao.SetActive(false);

        // Cria o painel principal.
        painelDialogo = CriarPainel(
            "PainelDialogo",
            canvasObjeto.transform,
            new Vector2(0.5f, 0.16f),
            new Vector2(850, 250),
            corFundo
        );

        Image imagemPainel = painelDialogo.GetComponent<Image>();
        AdicionarContorno(imagemPainel);

        grupoDialogo = painelDialogo.AddComponent<CanvasGroup>();
        rectDialogo = painelDialogo.GetComponent<RectTransform>();

        // Cria o título.
        TMP_Text titulo = CriarTexto(
            "TituloGuardiao",
            painelDialogo.transform,
            nomeGuardiao,
            34,
            corDourada,
            FontStyles.Bold
        );

        RectTransform tituloRect = titulo.GetComponent<RectTransform>();
        tituloRect.anchorMin = new Vector2(0.05f, 0.72f);
        tituloRect.anchorMax = new Vector2(0.95f, 0.95f);
        tituloRect.offsetMin = Vector2.zero;
        tituloRect.offsetMax = Vector2.zero;

        // Cria a mensagem.
        TMP_Text textoMensagem = CriarTexto(
            "TextoMensagem",
            painelDialogo.transform,
            mensagem,
            27,
            corTexto,
            FontStyles.Normal
        );

        textoMensagem.enableWordWrapping = true;
        textoMensagem.enableAutoSizing = true;
        textoMensagem.fontSizeMin = 18;
        textoMensagem.fontSizeMax = 27;

        RectTransform mensagemRect =
            textoMensagem.GetComponent<RectTransform>();

        mensagemRect.anchorMin = new Vector2(0.07f, 0.20f);
        mensagemRect.anchorMax = new Vector2(0.93f, 0.70f);
        mensagemRect.offsetMin = Vector2.zero;
        mensagemRect.offsetMax = Vector2.zero;

        // Instrução para fechar.
        TMP_Text rodape = CriarTexto(
            "TextoFechar",
            painelDialogo.transform,
            "PRESSIONE ESPAÇO PARA FECHAR",
            16,
            corDourada,
            FontStyles.Bold
        );

        RectTransform rodapeRect = rodape.GetComponent<RectTransform>();
        rodapeRect.anchorMin = new Vector2(0.05f, 0.03f);
        rodapeRect.anchorMax = new Vector2(0.95f, 0.16f);
        rodapeRect.offsetMin = Vector2.zero;
        rodapeRect.offsetMax = Vector2.zero;

        painelDialogo.SetActive(false);

        Debug.Log("Interface de interação criada para " + nomeGuardiao);
    }

    private GameObject CriarPainel(
        string nome,
        Transform pai,
        Vector2 posicao,
        Vector2 tamanho,
        Color cor
    )
    {
        GameObject objeto = new GameObject(
            nome,
            typeof(RectTransform),
            typeof(Image)
        );

        objeto.transform.SetParent(pai, false);

        RectTransform rect = objeto.GetComponent<RectTransform>();

        rect.anchorMin = posicao;
        rect.anchorMax = posicao;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = tamanho;

        objeto.GetComponent<Image>().color = cor;

        return objeto;
    }

    private TMP_Text CriarTexto(
        string nome,
        Transform pai,
        string conteudo,
        float tamanho,
        Color cor,
        FontStyles estilo
    )
    {
        GameObject objeto = new GameObject(
            nome,
            typeof(RectTransform),
            typeof(TextMeshProUGUI)
        );

        objeto.transform.SetParent(pai, false);

        TMP_Text texto = objeto.GetComponent<TMP_Text>();

        texto.text = conteudo;
        texto.fontSize = tamanho;
        texto.color = cor;
        texto.fontStyle = estilo;
        texto.alignment = TextAlignmentOptions.Center;
        texto.enableWordWrapping = true;

        RectTransform rect = objeto.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(10, 5);
        rect.offsetMax = new Vector2(-10, -5);

        return texto;
    }

    private void AjustarTextoCompleto(TMP_Text texto)
    {
        texto.alignment = TextAlignmentOptions.Center;
        texto.enableAutoSizing = true;
        texto.fontSizeMin = 16;
        texto.fontSizeMax = 25;
    }

    private void AdicionarContorno(Image imagem)
    {
        Outline outline = imagem.gameObject.AddComponent<Outline>();
        outline.effectColor = corDourada;
        outline.effectDistance = new Vector2(2, 2);
    }

    private void AbrirDialogo()
    {
        if (painelDialogo == null)
            return;

        dialogoAberto = true;

        // Atualiza a mensagem e o título.
        painelDialogo.transform.Find("TituloGuardiao")
            .GetComponent<TMP_Text>().text = nomeGuardiao;

        painelDialogo.transform.Find("TextoMensagem")
            .GetComponent<TMP_Text>().text = mensagem;

        if (avisoInteracao != null)
            avisoInteracao.SetActive(false);

        painelDialogo.SetActive(true);

        if (animacaoAtual != null)
            StopCoroutine(animacaoAtual);

        animacaoAtual = StartCoroutine(AnimarPainel(true));

        Debug.Log("Diálogo aberto: " + nomeGuardiao);
    }

    private void FecharDialogo()
    {
        if (!dialogoAberto)
            return;

        dialogoAberto = false;

        if (animacaoAtual != null)
            StopCoroutine(animacaoAtual);

        animacaoAtual = StartCoroutine(AnimarPainel(false));
    }

    private IEnumerator AnimarPainel(bool abrir)
    {
        float tempo = 0f;

        Vector3 escalaInicial = abrir
            ? Vector3.one * 0.80f
            : Vector3.one;

        Vector3 escalaFinal = abrir
            ? Vector3.one
            : Vector3.one * 0.80f;

        float alphaInicial = abrir ? 0f : 1f;
        float alphaFinal = abrir ? 1f : 0f;

        rectDialogo.localScale = escalaInicial;
        grupoDialogo.alpha = alphaInicial;

        while (tempo < duracaoAnimacao)
        {
            tempo += Time.unscaledDeltaTime;

            float progresso = Mathf.Clamp01(
                tempo / duracaoAnimacao
            );

            float suave = Mathf.SmoothStep(0f, 1f, progresso);

            rectDialogo.localScale = Vector3.Lerp(
                escalaInicial,
                escalaFinal,
                suave
            );

            grupoDialogo.alpha = Mathf.Lerp(
                alphaInicial,
                alphaFinal,
                suave
            );

            yield return null;
        }

        rectDialogo.localScale = escalaFinal;
        grupoDialogo.alpha = alphaFinal;

        if (!abrir)
        {
            painelDialogo.SetActive(false);

            if (jogadorPerto && avisoInteracao != null)
                avisoInteracao.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (canvas != null)
            Destroy(canvas.gameObject);
    }
}
