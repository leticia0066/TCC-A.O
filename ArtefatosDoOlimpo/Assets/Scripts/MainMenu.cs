
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }

    [Header("Música de fundo")]
    public AudioSource audioSource;

    [Header("Som dos botões")]
    public AudioClip cliqueSom;
    public AudioSource efeitosSource;

    [Range(0f, 1f)]
    public float volumeClique = 1f;

    private bool inicializado = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (efeitosSource == null)
            efeitosSource = gameObject.AddComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.spatialBlend = 0f;
        }

        efeitosSource.playOnAwake = false;
        efeitosSource.loop = false;
        efeitosSource.spatialBlend = 0f;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AoCarregarCena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AoCarregarCena;
    }

    private void Start()
    {
        AtualizarMusica(SceneManager.GetActiveScene().name);

        Invoke(nameof(ConectarBotoes), 0.1f);
    }

    private void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        AtualizarMusica(cena.name);

        if (cena.name == "TelaInicial")
        {
            inicializado = false;

            Invoke(nameof(ConectarBotoes), 0.1f);
        }
    }

    private void ConectarBotoes()
    {
        if (SceneManager.GetActiveScene().name != "TelaInicial")
            return;

        // NOVO JOGO
        GameObject objetoJogar = GameObject.Find("NovoJogo");

        if (objetoJogar != null)
        {
            Button botaoJogar = objetoJogar.GetComponent<Button>();

            if (botaoJogar != null)
            {
                botaoJogar.onClick.RemoveListener(Jogar);
                botaoJogar.onClick.AddListener(Jogar);
            }
        }
        else
        {
            Debug.LogWarning("Botão NovoJogo não encontrado!");
        }

        // CONTINUAR JOGO
        GameObject objetoContinuar = GameObject.Find("ContinuarJogo");

        if (objetoContinuar != null)
        {
            Button botaoContinuar = objetoContinuar.GetComponent<Button>();

            if (botaoContinuar != null)
            {
                botaoContinuar.onClick.RemoveListener(ContinuarJogo);
                botaoContinuar.onClick.AddListener(ContinuarJogo);

                bool existeSave = SaveSystem.TemSave();

                // O botão continua visível.
                objetoContinuar.SetActive(true);

                // Ativa ou desativa conforme o salvamento.
                botaoContinuar.interactable = existeSave;
            }
        }
        else
        {
            Debug.LogWarning("Botão ContinuarJogo não encontrado!");
        }

        // SAIR
        GameObject objetoSair = GameObject.Find("Sair");

        if (objetoSair != null)
        {
            Button botaoSair = objetoSair.GetComponent<Button>();

            if (botaoSair != null)
            {
                botaoSair.onClick.RemoveListener(Sair);
                botaoSair.onClick.AddListener(Sair);
            }
        }
        else
        {
            Debug.LogWarning("Botão Sair não encontrado!");
        }

        inicializado = true;

        Debug.Log("Botões do menu conectados corretamente!");
    }

    private void AtualizarMusica(string nomeCena)
    {
        if (audioSource == null)
            return;

        bool tocarMusica =
            nomeCena == "TelaInicial" ||
            nomeCena == "SelecaoFases";

        if (tocarMusica)
        {
            if (audioSource.clip != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }

    // NOVO JOGO
    public void Jogar()
    {
        if (!inicializado)
            return;

        Debug.Log("INICIANDO NOVO JOGO!");

        inicializado = false;

        Time.timeScale = 1f;

        TocarSomBotao();

        if (GameManager.instance != null)
        {
            GameManager.instance.NovoJogo();
        }
        else
        {
            SaveSystem.ApagarSave();
        }

        PlayerPrefs.DeleteKey("Fase1Concluida");
        PlayerPrefs.DeleteKey("Fase2Concluida");
        PlayerPrefs.DeleteKey("Fase3Concluida");
        PlayerPrefs.Save();

        if (TransicaoCenas.Instance != null)
        {
            TransicaoCenas.Instance.TrocarCena("SelecaoFases");
        }
        else
        {
            SceneManager.LoadScene("SelecaoFases");
        }
    }

    // CONTINUAR JOGO
    public void ContinuarJogo()
    {
        if (!inicializado)
            return;

        if (!SaveSystem.TemSave())
        {
            Debug.LogWarning("Não existe jogo salvo para continuar.");
            return;
        }

        Debug.Log("CONTINUANDO JOGO!");

        inicializado = false;

        Time.timeScale = 1f;

        TocarSomBotao();

        if (GameManager.instance != null)
        {
            GameManager.instance.ContinuarJogo();
        }
        else
        {
            Debug.LogError("GameManager não encontrado na TelaInicial!");

            inicializado = true;
        }
    }

    // SAIR
    public void Sair()
    {
        if (!inicializado)
            return;

        TocarSomBotao();

        Debug.Log("Saindo do jogo.");

        Application.Quit();
    }

    // SOM DOS BOTÕES
    public void TocarSomBotao()
    {
        if (cliqueSom == null || efeitosSource == null)
            return;

        efeitosSource.PlayOneShot(cliqueSom, volumeClique);
    }

    private void OnDestroy()
    {
        CancelInvoke();

        if (Instance == this)
        {
            Instance = null;
        }
    }
}
