
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private bool continuacaoPendente;
    private bool vidasSalvasConsumidas;

    // Estado temporário do jogador entre as fases.
    private int vidaJogadorSalva;
    private int tentativasJogadorSalvas;
    private bool estadoJogadorDisponivel;

    public bool TemJogoSalvo
    {
        get { return SaveSystem.TemSave(); }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += AoCarregarCena;
    }

    private void Start()
    {
        EncontrarSpawnInicial();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= AoCarregarCena;
            instance = null;
        }
    }

    private void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        if (continuacaoPendente &&
            dadosCarregados != null &&
            cena.name == dadosCarregados.cenaCheckpoint)
        {
            currentSpawnPosition = new Vector3(
                dadosCarregados.posicaoX,
                dadosCarregados.posicaoY,
                dadosCarregados.posicaoZ
            );

            checkpointDefinido = true;

            espadaColetada = dadosCarregados.espadaColetada;
            escudoColetado = dadosCarregados.escudoColetado;
            raioColetado = dadosCarregados.raioColetado;

            if (ContadorMoedas.Instance != null)
            {
                ContadorMoedas.Instance.RestaurarMoedas(
                    dadosCarregados.moedasColetadas
                );
            }

            PosicionarJogador(currentSpawnPosition);

            Debug.Log("PROGRESSO CARREGADO: " + cena.name);
            return;
        }

        EncontrarSpawnInicial();
    }

    private void EncontrarSpawnInicial()
    {
        GameObject objetoSpawn = GameObject.Find("Start Point");

        if (objetoSpawn != null)
        {
            startPoint = objetoSpawn.transform;
            currentSpawnPosition = startPoint.position;
            checkpointDefinido = true;

            Debug.Log("Spawn inicial encontrado: " + currentSpawnPosition);
        }
        else
        {
            Debug.LogWarning(
                "Não encontrei um objeto chamado Start Point nesta cena."
            );
        }
    }

    private void PosicionarJogador(Vector3 posicao)
    {
        Debug.LogWarning(
            "GAMEMANAGER POSICIONANDO O PLAYER: " + posicao
        );

        PlayerHealth jogador =
            FindFirstObjectByType<PlayerHealth>();

        if (jogador == null)
        {
            Debug.LogError("Não encontrei o PlayerHealth na cena.");
            return;
        }

        BloquearTeleportePlayer bloqueador =
            jogador.GetComponent<BloquearTeleportePlayer>();

        if (bloqueador != null)
        {
            bloqueador.AutorizarTeleporte(posicao);
        }
        else
        {
            Rigidbody2D rb = jogador.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.position = new Vector2(posicao.x, posicao.y);
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            jogador.transform.position = posicao;
        }

        Physics2D.SyncTransforms();
    }

    // Guarda vida e tentativas enquanto o jogador troca de fase.
    public void SalvarEstadoJogador(int vida, int tentativas)
    {
        vidaJogadorSalva = vida;
        tentativasJogadorSalvas = tentativas;
        estadoJogadorDisponivel = true;
    }

    // Recupera vida e tentativas na próxima fase.
    public bool TentarCarregarEstadoJogador(
        out int vida,
        out int tentativas)
    {
        vida = 0;
        tentativas = 0;

        if (!estadoJogadorDisponivel)
            return false;

        vida = vidaJogadorSalva;
        tentativas = tentativasJogadorSalvas;

        return true;
    }

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

        dadosCarregados = dados;
        continuacaoPendente = false;
        vidasSalvasConsumidas = false;

        SaveSystem.Salvar(dados);

        Debug.Log("CHECKPOINT SALVO: " + dados.cenaCheckpoint);
    }

    public Vector3 GetSpawnPosition()
    {
        if (!checkpointDefinido)
        {
            EncontrarSpawnInicial();
        }

        return currentSpawnPosition;
    }

    public bool TentarConsumirVidasSalvas(out int vidasSalvas)
    {
        vidasSalvas = 0;

        if (!continuacaoPendente ||
            dadosCarregados == null ||
            vidasSalvasConsumidas)
        {
            return false;
        }

        vidasSalvas = dadosCarregados.vidas;
        vidasSalvasConsumidas = true;

        return true;
    }

    public void ContinuarJogo()
    {
        SaveData dados = SaveSystem.Carregar();

        if (dados == null)
        {
            Debug.LogWarning("Não existe um jogo salvo para continuar.");
            return;
        }

        dadosCarregados = dados;
        continuacaoPendente = true;
        vidasSalvasConsumidas = false;

        // O progresso carregado será inicializado pelo PlayerHealth.
        estadoJogadorDisponivel = false;

        Debug.Log("Continuando jogo na cena: " + dados.cenaCheckpoint);

        SceneManager.LoadScene(dados.cenaCheckpoint);
    }

    public void NovoJogo()
    {
        SaveSystem.ApagarSave();

        dadosCarregados = null;
        continuacaoPendente = false;
        vidasSalvasConsumidas = false;

        checkpointDefinido = false;

        // Limpa o estado temporário do jogador.
        estadoJogadorDisponivel = false;
        vidaJogadorSalva = 0;
        tentativasJogadorSalvas = 0;

        espadaColetada = false;
        escudoColetado = false;
        raioColetado = false;

        if (ContadorMoedas.Instance != null)
        {
            ContadorMoedas.Instance.ResetarMoedas();
        }

        EncontrarSpawnInicial();

        Debug.Log("NOVO JOGO INICIADO!");
    }

    public void ColetarEspada()
    {
        espadaColetada = true;
        SalvarProgressoAtual();

        Debug.Log("ESPADA COLETADA!");
    }

    public void ColetarEscudo()
    {
        escudoColetado = true;
        SalvarProgressoAtual();

        Debug.Log("ESCUDO COLETADO!");
    }

    public void ColetarRaio()
    {
        raioColetado = true;
        SalvarProgressoAtual();

        Debug.Log("RAIO COLETADO!");
    }

    public bool TodosArtefatosColetados()
    {
        return espadaColetada &&
               escudoColetado &&
               raioColetado;
    }

    private void SalvarProgressoAtual()
    {
        if (!checkpointDefinido)
            return;

        SaveData dados = new SaveData();

        dados.cenaCheckpoint = SceneManager.GetActiveScene().name;

        dados.posicaoX = currentSpawnPosition.x;
        dados.posicaoY = currentSpawnPosition.y;
        dados.posicaoZ = currentSpawnPosition.z;

        dados.vidas = dadosCarregados != null
            ? dadosCarregados.vidas
            : 3;

        dados.espadaColetada = espadaColetada;
        dados.escudoColetado = escudoColetado;
        dados.raioColetado = raioColetado;

        if (ContadorMoedas.Instance != null)
        {
            dados.moedasColetadas =
                ContadorMoedas.Instance.ObterMoedasColetadas();
        }

        dadosCarregados = dados;

        SaveSystem.Salvar(dados);
    }
}
