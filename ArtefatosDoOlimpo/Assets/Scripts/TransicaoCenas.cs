
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransicaoCenas : MonoBehaviour
{
    public static TransicaoCenas Instance;

    [Header("Configuração da transição")]
    [Min(0.1f)]
    public float velocidadeFade = 2f;

    private Texture2D texturaPreta;
    private float alpha = 0f;
    private bool transicionando = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("TransicaoCenas duplicado. Destruindo cópia.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        texturaPreta = new Texture2D(1, 1);
        texturaPreta.SetPixel(0, 0, Color.black);
        texturaPreta.Apply();

        Debug.Log("TransicaoCenas inicializado corretamente.");
    }

    private void OnGUI()
    {
        if (alpha <= 0f || texturaPreta == null)
            return;

        GUI.color = new Color(0f, 0f, 0f, alpha);

        GUI.DrawTexture(
            new Rect(0, 0, Screen.width, Screen.height),
            texturaPreta
        );

        GUI.color = Color.white;
    }

    public void TrocarCena(string nomeCena)
    {
        Debug.Log("Pedido de transição recebido: " + nomeCena);

        if (transicionando)
        {
            Debug.LogWarning("Já existe uma transição em andamento.");
            return;
        }

        if (string.IsNullOrEmpty(nomeCena))
        {
            Debug.LogError("TransicaoCenas: nome da cena vazio!");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(nomeCena))
        {
            Debug.LogError(
                "TransicaoCenas: cena não encontrada no Build Profile: " +
                nomeCena
            );
            return;
        }

        StartCoroutine(FazerTransicao(nomeCena));
    }

    private IEnumerator FazerTransicao(string nomeCena)
    {
        transicionando = true;

        Debug.Log("Escurecendo a tela...");

        while (alpha < 1f)
        {
            alpha = Mathf.MoveTowards(
                alpha,
                1f,
                velocidadeFade * Time.unscaledDeltaTime
            );

            yield return null;
        }

        Debug.Log("Carregando cena: " + nomeCena);

        SceneManager.LoadScene(nomeCena);

        yield return null;

        Debug.Log("Cena carregada: " + SceneManager.GetActiveScene().name);

        while (alpha > 0f)
        {
            alpha = Mathf.MoveTowards(
                alpha,
                0f,
                velocidadeFade * Time.unscaledDeltaTime
            );

            yield return null;
        }

        transicionando = false;

        Debug.Log("Transição finalizada.");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (texturaPreta != null)
        {
            Destroy(texturaPreta);
        }
    }
}
