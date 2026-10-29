using UnityEngine;
using TMPro;

public class SistemaInteracao : MonoBehaviour
{
    [Header("Referência do jogador")]
    public string tagJogador = "Player";

    [Header("Interface")]
    public GameObject avisoInteracao;
    public GameObject painelDialogo;
    public TMP_Text textoDialogo;

    [Header("Mensagem")]
    [TextArea(3, 6)]
    public string mensagem =
        "O guardião da espada está à frente. Tome cuidado!";

    [Header("Teclas")]
    public KeyCode teclaInteragir = KeyCode.E;
    public KeyCode teclaFechar = KeyCode.Space;

    private bool jogadorPerto;
    private bool dialogoAberto;

    private void Start()
    {
        if (avisoInteracao != null)
            avisoInteracao.SetActive(false);

        if (painelDialogo != null)
            painelDialogo.SetActive(false);

        if (textoDialogo == null)
        {
            Debug.LogError(
                "SistemaInteracao: o campo Texto Dialogo está vazio!",
                this
            );
        }
    }

    private void Update()
    {
        if (dialogoAberto)
        {
            if (Input.GetKeyDown(teclaFechar))
                FecharDialogo();

            return;
        }

        if (jogadorPerto && Input.GetKeyDown(teclaInteragir))
            AbrirDialogo();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(tagJogador))
            return;

        jogadorPerto = true;

        if (avisoInteracao != null)
            avisoInteracao.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(tagJogador))
            return;

        jogadorPerto = false;

        if (avisoInteracao != null)
            avisoInteracao.SetActive(false);

        if (dialogoAberto)
            FecharDialogo();
    }

    private void AbrirDialogo()
    {
        if (painelDialogo == null || textoDialogo == null)
        {
            Debug.LogError(
                "SistemaInteracao: configure o painel e o texto no Inspector!",
                this
            );

            return;
        }

        dialogoAberto = true;

        textoDialogo.text = mensagem;
        painelDialogo.SetActive(true);

        if (avisoInteracao != null)
            avisoInteracao.SetActive(false);

        Debug.Log("Mensagem de interação aberta.");
    }

    private void FecharDialogo()
    {
        dialogoAberto = false;

        if (painelDialogo != null)
            painelDialogo.SetActive(false);

        if (avisoInteracao != null && jogadorPerto)
            avisoInteracao.SetActive(true);
    }
}