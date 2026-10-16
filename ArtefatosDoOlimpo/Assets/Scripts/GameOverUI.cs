
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void ReiniciarGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Fase1_Aress");
    }

    public void SairDoJogo()
    {
        Debug.Log("SAIR CLICADO");

        Time.timeScale = 1f;
        Application.Quit();
    }
}
