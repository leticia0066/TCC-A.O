
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

        alpha = 0f;
        transicionando = false;

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
        alpha = 0f;

        try
        {
            // ESCURECER
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

            // CARREGAR CENA
            SceneManager.LoadScene(nomeCena);

            yield return null;

            Debug.Log(
                "Cena carregada: " +
                SceneManager.GetActiveScene().name
            );

            // CLAREAR
            while (alpha > 0f)
            {
                alpha = Mathf.MoveTowards(
                    alpha,
                    0f,
                    velocidadeFade * Time.unscaledDeltaTime
                );

                yield return null;
            }

            alpha = 0f;

            Debug.Log("Transição finalizada.");
        }
        finally
        {
            alpha = 0f;
            transicionando = false;
        }
    }

    // NOVO: escurece a tela e mostra um painel.
    public void MostrarPainelComTransicao(GameObject painel)
    {
        if (transicionando)
        {
            Debug.LogWarning("Já existe uma transição em andamento.");
            return;
        }

        if (painel == null)
        {
            Debug.LogError(
                "TransicaoCenas: o painel recebido está vazio!"
            );
            return;
        }

        StartCoroutine(FazerTransicaoParaPainel(painel));
    }

    private IEnumerator FazerTransicaoParaPainel(GameObject painel)
    {
        transicionando = true;
        alpha = 0f;

        try
        {
            // 1. ESCURECER A TELA.
            while (alpha < 1f)
            {
                alpha = Mathf.MoveTowards(
                    alpha,
                    1f,
                    velocidadeFade * Time.unscaledDeltaTime
                );

                yield return null;
            }

            // 2. MOSTRAR O PAINEL COM A TELA ESCURA.
            painel.SetActive(true);

            // 3. PAUSAR O JOGO.
            Time.timeScale = 0f;

            yield return null;

            // 4. CLAREAR A TELA E REVELAR O PAINEL.
            while (alpha > 0f)
            {
                alpha = Mathf.MoveTowards(
                    alpha,
                    0f,
                    velocidadeFade * Time.unscaledDeltaTime
                );

                yield return null;
            }

            alpha = 0f;

            Debug.Log("Painel exibido com transição!");
        }
        finally
        {
            alpha = 0f;
            transicionando = false;
        }
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
