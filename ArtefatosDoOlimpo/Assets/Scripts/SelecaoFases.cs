using UnityEngine;
using UnityEngine.SceneManagement;

public class SelecaoFases : MonoBehaviour
{
    public void JogarFase1()
    {
        SceneManager.LoadScene("Fase1_Aress");
    }

    public void JogarFase2()
    {
        SceneManager.LoadScene("Fase2_Atena");
    }

    public void JogarFase3()
    {
        SceneManager.LoadScene("Fase3_Zeus");
    }

    public void Voltar()
    {
        SceneManager.LoadScene("TelaInicial");
    }
}