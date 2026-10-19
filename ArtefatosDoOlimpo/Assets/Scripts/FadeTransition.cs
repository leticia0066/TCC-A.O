using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeTransition : MonoBehaviour
{
    public Image fadeImage;

    private void Start()
    {
        Color cor = fadeImage.color;
        cor.a = 0f;
        fadeImage.color = cor;
    }

    public void TrocarCena(string nomeCena)
    {
        StartCoroutine(FazerFade(nomeCena));
    }

    IEnumerator FazerFade(string nomeCena)
    {
        float alpha = 0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * 2f;

            Color cor = fadeImage.color;
            cor.a = alpha;
            fadeImage.color = cor;

            yield return null;
        }

        SceneManager.LoadScene(nomeCena);
    }
}