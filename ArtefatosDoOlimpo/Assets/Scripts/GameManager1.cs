
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Spawn inicial")]
    public Transform startPoint;

    private Vector3 currentSpawnPosition;
    private bool checkpointDefinido = false;

    [Header("Artefatos do Olimpo")]
    public bool espadaColetada;
    public bool escudoColetado;
    public bool raioColetado;

    private SaveData dadosCarregados;
    private bool continuacaoPendente = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("GameManager iniciado!");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AoCarregarCena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AoCarregarCena;
    }

    private void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        // Se estamos continuando uma partida salva.
        if (dadosCarregados != null &&
            cena.name == dadosCarregados.cenaCheckpoint)
        {
            currentSpawnPosition = new Vector3(
                dadosCarregados.posicaoX,
                dadosCarregados.posicaoY,
                dadosCarregados.posicaoZ
            );

            checkpointDefinido = true;
            GameObject jogadorObjeto = GameObject.FindGameObjectWithTag("Player");

if (jogadorObjeto != null)
{
    jogadorObjeto.transform.position = currentSpawnPosition;

    Rigidbody2D rbJogador = jogadorObjeto.GetComponent<Rigidbody2D>();

    if (rbJogador != null)
    {
        rbJogador.linearVelocity = Vector2.zero;
        rbJogador.angularVelocity = 0f;
    }
}
else
{
    Debug.LogError("Não foi encontrado um jogador com a Tag Player!");
}

            espadaColetada = dadosCarregados.espadaColetada;
            escudoColetado = dadosCarregados.escudoColetado;
            raioColetado = dadosCarregados.raioColetado;

            if (ContadorMoedas.Instance != null)
            {
                ContadorMoedas.Instance.RestaurarMoedas(
                    dadosCarregados.moedasColetadas
                );
            }

            continuacaoPendente = true;

            Debug.Log("Progresso restaurado na cena: " + cena.name);
            return;
        }

        // Em uma fase normal, procura o ponto inicial daquela cena.
        GameObject objetoStart = GameObject.Find("Start Point");

        if (objetoStart != null)
        {
            currentSpawnPosition = objetoStart.transform.position;
            checkpointDefinido = true;

            Debug.Log("Start Point encontrado na cena: " + cena.name);
        }
        else
        {
            Debug.LogWarning(
                "Não foi encontrado um objeto chamado Start Point na cena " +
                cena.name
            );
        }
    }

    // CHECKPOINT
    public void SetCheckpoint(Vector3 posicao, PlayerHealth jogador)
    {
        currentSpawnPosition = posicao;
        checkpointDefinido = true;

        SaveData dados = new SaveData();

        dados.cenaCheckpoint = SceneManager.GetActiveScene().name;

        dados.posicaoX = posicao.x;
        dados.posicaoY = posicao.y;
        dados.posicaoZ = posicao.z;

        dados.vidas = jogador != null ? jogador.lives : 3;

        dados.espadaColetada = espadaColetada;
        dados.escudoColetado = escudoColetado;
        dados.raioColetado = raioColetado;

        if (ContadorMoedas.Instance != null)
        {
            dados.moedasColetadas =
                ContadorMoedas.Instance.ObterMoedasColetadas();
        }

        SaveSystem.Salvar(dados);

        Debug.Log("CHECKPOINT SALVO!");
        Debug.Log("Cena: " + dados.cenaCheckpoint);
        Debug.Log("Vidas salvas: " + dados.vidas);
    }

    public Vector3 GetSpawnPosition()
    {
        if (!checkpointDefinido)
        {
            GameObject objetoStart = GameObject.Find("Start Point");

            if (objetoStart != null)
            {
                currentSpawnPosition = objetoStart.transform.position;
                checkpointDefinido = true;
            }
        }

        return currentSpawnPosition;
    }

    public bool TentarConsumirVidasSalvas(out int vidasSalvas)
    {
        vidasSalvas = 0;

        if (!continuacaoPendente || dadosCarregados == null)
            return false;

        vidasSalvas = dadosCarregados.vidas;

        continuacaoPendente = false;
        dadosCarregados = null;

        return true;
    }

    // CONTINUAR JOGO
    public void ContinuarJogo()
    {
        SaveData dados = SaveSystem.Carregar();

        if (dados == null)
        {
            Debug.LogWarning("Não existe uma partida para continuar.");
            return;
        }

        Time.timeScale = 1f;

        dadosCarregados = dados;

        if (TransicaoCenas.Instance != null)
        {
            TransicaoCenas.Instance.TrocarCena(dados.cenaCheckpoint);
        }
        else
        {
            SceneManager.LoadScene(dados.cenaCheckpoint);
        }
    }

    // NOVO JOGO
    public void NovoJogo()
    {
        SaveSystem.ApagarSave();

        espadaColetada = false;
        escudoColetado = false;
        raioColetado = false;

        dadosCarregados = null;
        continuacaoPendente = false;

        checkpointDefinido = false;

        if (ContadorMoedas.Instance != null)
        {
            ContadorMoedas.Instance.ResetarMoedas();
        }

        // Apaga dados antigos dos sistemas anteriores.
        PlayerPrefs.DeleteKey("Artefato_Espada");
        PlayerPrefs.DeleteKey("Artefato_Escudo");
        PlayerPrefs.DeleteKey("Artefato_Raio");
        PlayerPrefs.DeleteKey("TotalMoedas");
        PlayerPrefs.DeleteKey("MoedasColetadas");

        PlayerPrefs.Save();

        Debug.Log("NOVO JOGO INICIADO!");
    }

    // ARTEFATOS
    public void ColetarEspada()
    {
        espadaColetada = true;
        Debug.Log("ESPADA DO OLIMPO COLETADA!");
    }

    public void ColetarEscudo()
    {
        escudoColetado = true;
        Debug.Log("ESCUDO DO OLIMPO COLETADO!");
    }

    public void ColetarRaio()
    {
        raioColetado = true;
        Debug.Log("RAIO DO OLIMPO COLETADO!");
    }

    public bool TodosArtefatosColetados()
    {
        return espadaColetada && escudoColetado && raioColetado;
    }

    public void ResetarArtefatos()
    {
        espadaColetada = false;
        escudoColetado = false;
        raioColetado = false;

        PlayerPrefs.DeleteKey("Artefato_Espada");
        PlayerPrefs.DeleteKey("Artefato_Escudo");
        PlayerPrefs.DeleteKey("Artefato_Raio");
        PlayerPrefs.Save();
    }
}
