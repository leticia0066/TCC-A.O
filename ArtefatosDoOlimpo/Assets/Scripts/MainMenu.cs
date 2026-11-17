
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

    private Button botaoContinuar;

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
        ConectarBotoes();
    }

    private void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        AtualizarMusica(cena.name);

        if (cena.name == "TelaInicial")
        {
            ConectarBotoes();
        }
    }

    private void ConectarBotoes()
    {
        // Botão de novo jogo.
        GameObject objetoJogar = GameObject.Find("Jogar");

        if (objetoJogar != null)
        {
            Button botaoJogar = objetoJogar.GetComponent<Button>();

            if (botaoJogar != null)
            {
                botaoJogar.onClick.RemoveListener(Jogar);
                botaoJogar.onClick.AddListener(Jogar);
            }
        }

        // Botão continuar.
        GameObject objetoContinuar = GameObject.Find("Continuar");

        if (objetoContinuar != null)
        {
            botaoContinuar = objetoContinuar.GetComponent<Button>();

            if (botaoContinuar != null)
            {
                botaoContinuar.onClick.RemoveListener(ContinuarJogo);
                botaoContinuar.onClick.AddListener(ContinuarJogo);

                botaoContinuar.interactable = SaveSystem.TemSave();

                // Esconde o botão se ainda não existe salvamento.
                objetoContinuar.SetActive(SaveSystem.TemSave());
            }
        }
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
        Debug.Log("INICIANDO NOVO JOGO!");

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
        if (!SaveSystem.TemSave())
        {
            Debug.LogWarning("Não existe jogo salvo para continuar.");
            return;
        }

        TocarSomBotao();

        if (GameManager.instance != null)
        {
            GameManager.instance.ContinuarJogo();
        }
        else
        {
            Debug.LogError(
                "GameManager não encontrado na TelaInicial!"
            );
        }
    }

    public void Sair()
    {
        TocarSomBotao();

        Debug.Log("Saindo do jogo.");

        Application.Quit();
    }

    public void TocarSomBotao()
    {
        if (cliqueSom == null || efeitosSource == null)
            return;

        efeitosSource.PlayOneShot(cliqueSom, volumeClique);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
