using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void ReiniciarGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Fase1");
    }

    public void SairDoJogo()
    {
        Debug.Log("SAIR CLICADO");

        Application.Quit();
    }
}