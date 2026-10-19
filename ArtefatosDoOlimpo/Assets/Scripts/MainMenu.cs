using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public FadeTransition fade;

    public AudioSource audioSource;
    public AudioClip cliqueSom;

    public void Jogar()
    {
        audioSource.PlayOneShot(cliqueSom);

        fade.TrocarCena("SelecaoFases");
    }

    public void Sair()
    {
        audioSource.PlayOneShot(cliqueSom);

        Application.Quit();

        Debug.Log("Saiu do jogo");
    }
}