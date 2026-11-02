
using UnityEngine;
using UnityEngine.UI;

public class ArtefatosUI : MonoBehaviour
{
    [Header("Imagens dos artefatos")]
    public Image imagemEspada;
    public Image imagemEscudo;
    public Image imagemRaio;

    [Header("Cores")]
    public Color corSilhueta = Color.black;
    public Color corColetado = Color.white;

    private void Start()
    {
        AtualizarInterface();
    }

    public void AtualizarInterface()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("ArtefatosUI: GameManager não encontrado!");
            return;
        }

        AtualizarImagem(
            imagemEspada,
            GameManager.instance.espadaColetada
        );

        AtualizarImagem(
            imagemEscudo,
            GameManager.instance.escudoColetado
        );

        AtualizarImagem(
            imagemRaio,
            GameManager.instance.raioColetado
        );
    }

    private void AtualizarImagem(Image imagem, bool coletado)
    {
        if (imagem == null)
            return;

        imagem.color = coletado ? corColetado : corSilhueta;
    }
}
