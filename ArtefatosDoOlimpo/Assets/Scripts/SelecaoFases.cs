
using UnityEngine;

public class SelecaoFases : MonoBehaviour
{
    [Header("Transição de cena")]
    public SceneTransition sceneTransition;

    public void JogarFase1()
    {
        CarregarCena("Fase1_Aress");
    }

    public void JogarFase2()
    {
        CarregarCena("Fase2_Atena");
    }

    public void JogarFase3()
    {
        CarregarCena("Fase3_Zeus");
    }

    public void Voltar()
    {
        CarregarCena("TelaInicial");
    }

    private void CarregarCena(string nomeCena)
    {
        if (sceneTransition == null)
        {
            Debug.LogError(
                "SelecaoFases: o campo Scene Transition está vazio! " +
                "Arraste o SceneTransitionManager para esse campo."
            );

            return;
        }

        Debug.Log("Solicitando transição para: " + nomeCena);

        sceneTransition.ChangeScene(nomeCena);
    }
}
