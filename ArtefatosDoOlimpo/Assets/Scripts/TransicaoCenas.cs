
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransicaoCenas : MonoBehaviour
{
    public static TransicaoCenas Instance;

    [Header("Configuração da transição")]
    public float velocidadeFade = 2f;

    private Texture2D texturaPreta;
    private float alpha = 0f;
    private bool transicionando = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        texturaPreta = new Texture2D(1, 1);
        texturaPreta.SetPixel(0, 0, Color.black);
        texturaPreta.Apply();
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
        if (transicionando)
            return;

        if (string.IsNullOrEmpty(nomeCena))
        {
            Debug.LogError("TransicaoCenas: nome da cena vazio!");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(nomeCena))
        {
            Debug.LogError(
                "TransicaoCenas: a cena '" + nomeCena +
                "' não está no Build Profile."
            );
            return;
        }

        StartCoroutine(FazerTransicao(nomeCena));
    }

    private IEnumerator FazerTransicao(string nomeCena)
    {
        transicionando = true;

        // Escurece a tela.
        while (alpha < 1f)
        {
            alpha = Mathf.MoveTowards(
                alpha,
                1f,
                velocidadeFade * Time.unscaledDeltaTime
            );

            yield return null;
        }

        // Carrega a próxima cena.
        SceneManager.LoadScene(nomeCena);

        // Aguarda a cena ser carregada.
        yield return null;

        // Clareia a tela.
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
