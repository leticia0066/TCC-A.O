
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
        ConectarBotaoJogar();
    }

    private void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        AtualizarMusica(cena.name);

        if (cena.name == "TelaInicial")
        {
            ConectarBotaoJogar();
        }
    }

    private void ConectarBotaoJogar()
    {
        GameObject objetoBotao = GameObject.Find("Jogar");

        if (objetoBotao == null)
        {
            Debug.LogWarning("PlayButton não encontrado na TelaInicial!");
            return;
        }

        Button botao = objetoBotao.GetComponent<Button>();

        if (botao == null)
        {
            Debug.LogWarning("O objeto PlayButton não possui componente Button!");
            return;
        }

        botao.onClick.RemoveListener(Jogar);
        botao.onClick.AddListener(Jogar);

        Debug.Log("Botão JOGAR conectado ao MainMenu!");
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
                Debug.Log("Música do menu iniciada.");
            }
        }
        else
        {
            audioSource.Stop();
            Debug.Log("Música do menu parada na cena: " + nomeCena);
        }
    }

    public void Jogar()
    {
        Debug.Log("INICIANDO NOVA PARTIDA!");

        Time.timeScale = 1f;

        TocarSomBotao();

        if (GameManager.instance != null)
        {
            GameManager.instance.ResetarArtefatos();
            GameManager.instance.ResetToStart();
        }
        else
        {
            PlayerPrefs.DeleteKey("Artefato_Espada");
            PlayerPrefs.DeleteKey("Artefato_Escudo");
            PlayerPrefs.DeleteKey("Artefato_Raio");
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
            Debug.LogWarning("TransicaoCenas não encontrada!");
            SceneManager.LoadScene("SelecaoFases");
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

        Debug.Log("Som do botão reproduzido.");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
