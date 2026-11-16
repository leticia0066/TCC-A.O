
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneIntroManager : MonoBehaviour
{
    [Header("Paineis")]
    public GameObject painelHistoria;
    public GameObject painelTutorial;

    [Header("Textos antigos")]
    public TMP_Text textoHistoria;
    public TMP_Text textoTutorial;

    [Header("Imagem dos deuses")]
    public Image imagemDeus;
    public Sprite imagemAres;
    public Sprite imagemAtena;
    public Sprite imagemZeus;

    [Header("Barra de dialogo")]
    public Sprite arteBarraDialogo;

    [Range(60, 200)]
    public int alturaBarra = 110;

    [Range(12, 35)]
    public int tamanhoTexto = 23;

    [Header("Som do espaco")]
    public AudioSource audioSource;
    public AudioClip somCliqueEspaco;

    [Range(0f, 1f)]
    public float volumeClique = 0.8f;

    [Header("Mensagens")]
    public string mensagemContinuar =
        "ESPAÇO — CONTINUAR";

    public string mensagemComecar =
        "ESPAÇO — COMEÇAR";

    private Canvas canvas;
    private GameObject barraDialogo;

    private TMP_Text textoBarra;
    private TMP_Text textoAjuda;

    private bool tutorialAtivo;
    private bool iniciandoFase;

    private void Start()
    {
        Time.timeScale = 1f;

        if (!ReferenciasValidas())
        {
            enabled = false;
            return;
        }

        string fase = SelecaoFases.NomeFaseSelecionada;

        EscolherImagem(fase);

        painelHistoria.SetActive(true);
        painelTutorial.SetActive(false);

        // Desativa os textos antigos.
        textoHistoria.gameObject.SetActive(false);
        textoTutorial.gameObject.SetActive(false);

        CriarBarraDialogo();

        MostrarHistoria(fase);
    }

    // =====================================
    // VALIDAR REFERENCIAS
    // =====================================

    private bool ReferenciasValidas()
    {
        if (painelHistoria == null ||
            painelTutorial == null ||
            textoHistoria == null ||
            textoTutorial == null ||
            imagemDeus == null)
        {
            Debug.LogError(
                "CutsceneIntroManager: confira os campos no Inspector."
            );

            return false;
        }

        string fase = SelecaoFases.NomeFaseSelecionada;

        if (string.IsNullOrEmpty(fase) ||
            !Application.CanStreamedLevelBeLoaded(fase))
        {
            Debug.LogError(
                "A fase selecionada não foi encontrada no Build Profile!"
            );

            return false;
        }

        return true;
    }

    // =====================================
    // IMAGEM DOS DEUSES
    // =====================================

    private void EscolherImagem(string fase)
    {
        switch (fase)
        {
            case "Fase1_Aress":
                imagemDeus.sprite = imagemAres;
                break;

            case "Fase2_Atena":
                imagemDeus.sprite = imagemAtena;
                break;

            case "Fase3_Zeus":
                imagemDeus.sprite = imagemZeus;
                break;

            default:
                Debug.LogError(
                    "Imagem não encontrada: " + fase
                );
                break;
        }

        imagemDeus.preserveAspect = true;
    }

    // =====================================
    // CRIAR BARRA AUTOMATICAMENTE
    // =====================================

    private void CriarBarraDialogo()
    {
        canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
        }

        if (canvas == null)
        {
            Debug.LogError(
                "Nenhum Canvas foi encontrado!"
            );

            return;
        }

        // Cria o objeto da barra.
        barraDialogo = new GameObject(
            "BarraDialogo",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        barraDialogo.transform.SetParent(
            canvas.transform,
            false
        );

        RectTransform barraRect =
            barraDialogo.GetComponent<RectTransform>();

        // Posicionamento na parte inferior.
        barraRect.anchorMin = new Vector2(0.08f, 0f);
        barraRect.anchorMax = new Vector2(0.92f, 0f);

        barraRect.pivot = new Vector2(0.5f, 0f);

        barraRect.sizeDelta = new Vector2(
            0f,
            alturaBarra
        );

        barraRect.anchoredPosition = new Vector2(
            0f,
            28f
        );

        // Aplica a arte da barra.
        Image imagemBarra =
            barraDialogo.GetComponent<Image>();

        imagemBarra.sprite = arteBarraDialogo;
        imagemBarra.type = Image.Type.Simple;
        imagemBarra.preserveAspect = false;
        imagemBarra.color = Color.white;

        // Cria o texto principal.
        textoBarra = CriarTexto(
            "TextoHistoria",
            barraDialogo.transform,
            new Vector2(0.08f, 0.28f),
            new Vector2(0.92f, 0.88f),
            tamanhoTexto,
            TextAlignmentOptions.Center
        );

        // Cria a instrução de espaço.
        textoAjuda = CriarTexto(
            "TextoContinuar",
            barraDialogo.transform,
            new Vector2(0.08f, 0.02f),
            new Vector2(0.92f, 0.27f),
            15,
            TextAlignmentOptions.Center
        );

        textoAjuda.color = new Color(
            1f,
            0.85f,
            0.55f,
            1f
        );

        barraDialogo.transform.SetAsLastSibling();
    }

    // =====================================
    // CRIAR TEXTOS
    // =====================================

    private TMP_Text CriarTexto(
        string nome,
        Transform pai,
        Vector2 anchorMin,
        Vector2 anchorMax,
        int fonte,
        TextAlignmentOptions alinhamento)
    {
        GameObject objeto = new GameObject(
            nome,
            typeof(RectTransform)
        );

        objeto.transform.SetParent(
            pai,
            false
        );

        RectTransform rect =
            objeto.GetComponent<RectTransform>();

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;

        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        TMP_Text texto =
            objeto.AddComponent<TextMeshProUGUI>();

        texto.fontSize = fonte;
        texto.alignment = alinhamento;

        texto.color = Color.black;

        texto.enableAutoSizing = true;
        texto.fontSizeMin = 13;
        texto.fontSizeMax = fonte;

        texto.textWrappingMode =
            TextWrappingModes.Normal;

        texto.margin = new Vector4(
            5f,
            2f,
            5f,
            2f
        );

        return texto;
    }

    // =====================================
    // HISTORIA
    // =====================================

    private void MostrarHistoria(string fase)
    {
        string historia = "";

        switch (fase)
        {
            case "Fase1_Aress":

                historia =
                    "O equilíbrio do Olimpo foi ameaçado! " +
                    "Encontre a Espada do Olimpo, protegida por Ares, " +
                    "e prove sua coragem.";

                break;

            case "Fase2_Atena":

                historia =
                    "A jornada continua! Recupere o Escudo do Olimpo, " +
                    "protegido por Atena, e prove sua sabedoria.";

                break;

            case "Fase3_Zeus":

                historia =
                    "Resta apenas um artefato! Encontre o Raio de Zeus " +
                    "e restaure o equilíbrio do Olimpo.";

                break;
        }

        textoBarra.text = historia;
        textoAjuda.text = mensagemContinuar;
    }

    // =====================================
    // TUTORIAL
    // =====================================

    private void MostrarTutorial()
    {
        tutorialAtivo = true;

        painelHistoria.SetActive(false);
        painelTutorial.SetActive(true);

        textoBarra.text =
            "A / D — Mover     |     ESPAÇO — Pular\n" +
            "Z — Atacar     |     P — Configurações";

        textoAjuda.text = mensagemComecar;
    }

    // =====================================
    // CONTROLES
    // =====================================

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        TocarSomClique();

        if (tutorialAtivo)
        {
            ComecarFase();
            return;
        }

        MostrarTutorial();
    }

    // =====================================
    // SOM DO ESPACO
    // =====================================

    private void TocarSomClique()
    {
        if (audioSource == null ||
            somCliqueEspaco == null)
        {
            return;
        }

        audioSource.PlayOneShot(
            somCliqueEspaco,
            volumeClique
        );
    }

    // =====================================
    // INICIAR FASE
    // =====================================

    private void ComecarFase()
    {
        if (iniciandoFase)
            return;

        iniciandoFase = true;

        Time.timeScale = 1f;

        string faseEscolhida =
            SelecaoFases.NomeFaseSelecionada;

        if (TransicaoCenas.Instance != null)
        {
            TransicaoCenas.Instance.TrocarCena(
                faseEscolhida
            );
        }
        else
        {
            Debug.LogError(
                "TransicaoCenas não encontrada!"
            );

            iniciandoFase = false;
        }
    }
}
