using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }

    [Header("Transição")]
    public FadeTransition fade;

    [Header("Áudio")]
    public AudioSource audioSource;
    public AudioClip cliqueSom;

    private void Awake()
    {
        // Evita criar cópias do gerenciador de áudio.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Mantém a música disponível durante a troca de cenas.
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
            audioSource.loop = true;
        }
    }

    private void Start()
    {
        AtualizarMusica(SceneManager.GetActiveScene().name);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AoCarregarCena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AoCarregarCena;
    }

    private void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        AtualizarMusica(cena.name);
    }

    private void AtualizarMusica(string nomeCena)
    {
        if (audioSource == null)
        {
            Debug.LogWarning(
                "MainMenu: configure o Audio Source no Inspector."
            );

            return;
        }

        // A música de fundo toca somente no menu
        // e na seleção de fases.
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
            Debug.LogError(
                "MainMenu: o campo Fade não está configurado."
            );

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
        if (audioSource == null || cliqueSom == null)
        {
            Debug.LogWarning(
                "MainMenu: configure o Audio Source e o Clique Som."
            );

            return;
        }

        audioSource.PlayOneShot(cliqueSom);
    }
}