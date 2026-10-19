using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeTransition : MonoBehaviour
{
    public Image fadeImage;

    public void TrocarCena(string nomeCena)
    {
        StartCoroutine(Fade(nomeCena));
    }

    IEnumerator Fade(string nomeCena)
    {
        float alpha = 0;

        while(alpha < 1)
        {
            alpha += Time.deltaTime;

            Color cor = fadeImage.color;
            cor.a = alpha;

            fadeImage.color = cor;

            yield return null;
        }

        SceneManager.LoadScene(nomeCena);
    }
}