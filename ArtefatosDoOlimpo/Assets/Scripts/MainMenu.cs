using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }

    [Header("Transição")]
    public FadeTransition fade;

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

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.spatialBlend = 0f;
        }

        // Cria uma fonte de áudio separada para os cliques.
        if (efeitosSource == null)
        {
            efeitosSource = gameObject.AddComponent<AudioSource>();
        }

        efeitosSource.playOnAwake = false;
        efeitosSource.loop = false;
        efeitosSource.spatialBlend = 0f;
        efeitosSource.volume = 1f;
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
    }

    private void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        AtualizarMusica(cena.name);
    }

    private void AtualizarMusica(string nomeCena)
    {
        if (audioSource == null)
        {
            Debug.LogWarning("MainMenu: Audio Source da música não configurado.");
            return;
        }

        bool deveTocarMusica =
            nomeCena == "TelaInicial" ||
            nomeCena == "SelecaoFases";

        if (deveTocarMusica)
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
        TocarSomBotao();

        if (fade == null)
        {
            Debug.LogError("MainMenu: o campo Fade não está configurado.");
            return;
        }

        fade.TrocarCena("SelecaoFases");
    }

    public void Sair()
    {
        TocarSomBotao();
        Debug.Log("Saindo do jogo.");

        Application.Quit();
    }

    public void TocarSomBotao()
    {
        if (cliqueSom == null)
        {
            Debug.LogWarning("MainMenu: o campo Clique Som está vazio.");
            return;
        }

        if (efeitosSource == null)
        {
            Debug.LogWarning("MainMenu: a fonte de áudio dos efeitos está vazia.");
            return;
        }

        efeitosSource.PlayOneShot(cliqueSom, volumeClique);

        Debug.Log("Som do botão reproduzido.");
    }
}