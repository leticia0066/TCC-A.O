
using UnityEngine;
using UnityEngine.SceneManagement;

public class PainelVitoria : MonoBehaviour
{
    public void Continuar()
    {
        Time.timeScale = 1f;

        if (TransicaoCenas.Instance != null)
        {
            TransicaoCenas.Instance.TrocarCena("TelaInicial");
        }
        else
        {
            SceneManager.LoadScene("TelaInicial");
        }
    }
}
