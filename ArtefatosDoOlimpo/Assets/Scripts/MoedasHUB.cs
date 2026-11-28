
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoedasHUD : MonoBehaviour
{
    [Header("Interface das moedas")]
    public Image iconeMoeda;
    public TMP_Text textoQuantidade;

    private bool inscritoNoContador;

    private void OnEnable()
    {
        ConectarAoContador();
    }

    private void Start()
    {
        ConectarAoContador();
    }

    private void Update()
    {
        if (!inscritoNoContador)
        {
            ConectarAoContador();
        }
    }

    private void OnDisable()
    {
        DesconectarDoContador();
    }

    private void ConectarAoContador()
    {
        if (inscritoNoContador)
            return;

        if (ContadorMoedas.Instance == null)
            return;

        ContadorMoedas.Instance.AoAtualizarMoedas += AtualizarTexto;

        inscritoNoContador = true;

        AtualizarTexto(
            ContadorMoedas.Instance.QuantidadeAtual
        );

        Debug.Log(
            "HUD conectado ao contador. Total: " +
            ContadorMoedas.Instance.QuantidadeAtual
        );
    }

    private void DesconectarDoContador()
    {
        if (!inscritoNoContador)
            return;

        if (ContadorMoedas.Instance != null)
        {
            ContadorMoedas.Instance.AoAtualizarMoedas -= AtualizarTexto;
        }

        inscritoNoContador = false;
    }

    private void AtualizarTexto(int quantidade)
    {
        if (textoQuantidade != null)
        {
            textoQuantidade.text = quantidade.ToString();
        }
    }
}
