
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneIntroManager : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject painelHistoria;
    public GameObject painelTutorial;

    [Header("Textos")]
    public TMP_Text textoHistoria;
    public TMP_Text textoTutorial;

    [Header("Imagem dos deuses")]
    public Image imagemDeus;

    public Sprite imagemAres;
    public Sprite imagemAtena;
    public Sprite imagemZeus;

    [Header("Configuração da digitação")]
    [Min(0.001f)]
    public float velocidadeDigitacao = 0.09f;

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip somDigitacao;
    public AudioClip somCliqueEspaco;

    [Range(0f, 1f)]
    public float volumeDigitacao = 0.15f;

    [Range(0f, 1f)]
    public float volumeClique = 0.8f;

    [Range(1, 10)]
    public int letrasEntreSons = 5;

    [Header("Mensagens automáticas")]
    public string mensagemPular = "PRESSIONE ESPAÇO PARA PULAR";
    public string mensagemContinuar = "PRESSIONE ESPAÇO PARA CONTINUAR";
    public string mensagemComecar = "PRESSIONE ESPAÇO PARA COMEÇAR";

    private string historiaCompleta;
    private Coroutine digitacaoCoroutine;

    private bool digitando;
    private bool historiaTerminada;
    private bool tutorialAtivo;
    private bool iniciandoFase;

    private int contadorLetras;

    private void Start()
    {
        Time.timeScale = 1f;

        if (!ReferenciasValidas())
        {
            enabled = false;
            return;
        }

        string fase = SelecaoFases.NomeFaseSelecionada;

        historiaCompleta = EscolherHistoria(fase);
        EscolherImagem(fase);

        painelHistoria.SetActive(true);
        painelTutorial.SetActive(false);

        textoHistoria.text = "";

        textoTutorial.text =
            "COMO JOGAR\n\n" +
            "A / D — Mover para os lados\n" +
            "ESPAÇO — Pular\n" +
            "Z — Atirar\n" +
            "P — Abrir configurações\n\n" +
            mensagemComecar;

        contadorLetras = 0;

        digitacaoCoroutine = StartCoroutine(DigitarHistoria());
    }

    private string EscolherHistoria(string fase)
    {
        switch (fase)
        {
            case "Fase1_Aress":

                return
                    "CAPÍTULO I — A PROVAÇÃO DE ARES\n\n" +
                    "O equilíbrio do Olimpo foi ameaçado, e os antigos " +
                    "artefatos foram espalhados pelo mundo.\n\n" +
                    "Sua primeira missão é encontrar a Espada do Olimpo, " +
                    "protegida no templo de Ares, o deus da guerra.\n\n" +
                    "Enfrente os perigos e prove sua coragem para conquistar " +
                    "o primeiro artefato.";

            case "Fase2_Atena":

                return
                    "CAPÍTULO II — O JULGAMENTO DE ATENA\n\n" +
                    "Com a Espada recuperada, sua jornada continua.\n\n" +
                    "Agora você precisa encontrar o Escudo do Olimpo, " +
                    "protegido por Atena, deusa da sabedoria.\n\n" +
                    "Supere os desafios de seu templo e prove que possui " +
                    "inteligência para conquistar o segundo artefato.";

            case "Fase3_Zeus":

                return
                    "CAPÍTULO III — A IRA DE ZEUS\n\n" +
                    "A Espada e o Escudo foram recuperados. " +
                    "Resta apenas um artefato.\n\n" +
                    "No alto do Olimpo, Zeus protege o poderoso Raio.\n\n" +
                    "Enfrente a última provação e conquiste o artefato " +
                    "capaz de restaurar o equilíbrio entre os deuses.";

            default:

                Debug.LogError("História não encontrada para: " + fase);

                return "Uma nova jornada começa no mundo dos deuses.";
        }
    }

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
                Debug.LogError("Imagem não encontrada para: " + fase);
                break;
        }

        imagemDeus.preserveAspect = true;

        if (imagemDeus.sprite == null)
        {
            Debug.LogError(
                "A imagem do deus não foi configurada para: " + fase
            );
        }
    }

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

        if (digitando)
        {
            MostrarHistoriaCompleta();
            return;
        }

        if (historiaTerminada)
        {
            MostrarTutorial();
        }
    }

    private bool ReferenciasValidas()
    {
        if (painelHistoria == null ||
            painelTutorial == null ||
            textoHistoria == null ||
            textoTutorial == null ||
            imagemDeus == null)
        {
            Debug.LogError(
                "CutsceneIntroManager: confira os campos da interface no Inspector."
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

    private IEnumerator DigitarHistoria()
    {
        digitando = true;
        historiaTerminada = false;
        contadorLetras = 0;

        string textoAtual = "";

        foreach (char letra in historiaCompleta)
        {
            textoAtual += letra;

            textoHistoria.text =
                textoAtual + "\n\n" + mensagemPular;

            if (!char.IsWhiteSpace(letra))
            {
                contadorLetras++;

                if (contadorLetras >= letrasEntreSons)
                {
                    TocarSomDigitacao();
                    contadorLetras = 0;
                }
            }

            yield return new WaitForSecondsRealtime(
                velocidadeDigitacao
            );
        }

        digitando = false;
        historiaTerminada = true;
        digitacaoCoroutine = null;

        textoHistoria.text =
            historiaCompleta + "\n\n" + mensagemContinuar;
    }

    private void TocarSomDigitacao()
    {
        if (audioSource == null || somDigitacao == null)
            return;

        audioSource.PlayOneShot(
            somDigitacao,
            volumeDigitacao
        );
    }

    private void TocarSomClique()
    {
        if (audioSource == null || somCliqueEspaco == null)
            return;

        audioSource.PlayOneShot(
            somCliqueEspaco,
            volumeClique
        );
    }

    private void MostrarHistoriaCompleta()
    {
        if (digitacaoCoroutine != null)
        {
            StopCoroutine(digitacaoCoroutine);
            digitacaoCoroutine = null;
        }

        textoHistoria.text =
            historiaCompleta + "\n\n" + mensagemContinuar;

        digitando = false;
        historiaTerminada = true;
    }

    private void MostrarTutorial()
    {
        painelHistoria.SetActive(false);
        painelTutorial.SetActive(true);

        textoTutorial.text =
            "COMO JOGAR\n\n" +
            "A / D — Mover para os lados\n" +
            "ESPAÇO — Pular\n" +
            "Z — Atirar\n" +
            "P — Abrir configurações\n\n" +
            mensagemComecar;

        tutorialAtivo = true;

        Debug.Log("Tutorial exibido.");
    }

    private void ComecarFase()
    {
        if (iniciandoFase)
            return;

        iniciandoFase = true;

        Time.timeScale = 1f;

        string faseEscolhida =
            SelecaoFases.NomeFaseSelecionada;

        Debug.Log(
            "Cutscene concluída. Indo para: " + faseEscolhida
        );

        if (TransicaoCenas.Instance != null)
        {
            TransicaoCenas.Instance.TrocarCena(faseEscolhida);
        }
        else
        {
            Debug.LogError("TransicaoCenas não encontrada!");
            iniciandoFase = false;
        }
    }

    private void OnDisable()
    {
        if (digitacaoCoroutine != null)
        {
            StopCoroutine(digitacaoCoroutine);
            digitacaoCoroutine = null;
        }
    }
}
