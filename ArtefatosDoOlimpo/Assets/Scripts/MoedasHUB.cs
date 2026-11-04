
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoedasHUD : MonoBehaviour
{
    [Header("Interface das moedas")]
    public Image iconeMoeda;
    public TMP_Text textoQuantidade;

    private void OnEnable()
    {
        if (ContadorMoedas.Instance != null)
        {
            ContadorMoedas.Instance.AoAtualizarMoedas += AtualizarTexto;

            AtualizarTexto(
                ContadorMoedas.Instance.QuantidadeAtual
            );
        }
    }

    private void OnDisable()
    {
        if (ContadorMoedas.Instance != null)
        {
            ContadorMoedas.Instance.AoAtualizarMoedas -= AtualizarTexto;
        }
    }

    private void AtualizarTexto(int quantidade)
    {
        if (textoQuantidade != null)
        {
            textoQuantidade.text = quantidade.ToString();
        }
    }
}
