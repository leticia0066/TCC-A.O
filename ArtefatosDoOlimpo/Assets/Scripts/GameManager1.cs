
using UnityEngine;

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

    private const string CHAVE_ESPADA = "Artefato_Espada";
    private const string CHAVE_ESCUDO = "Artefato_Escudo";
    private const string CHAVE_RAIO = "Artefato_Raio";

    private void Awake()
    {
        Debug.Log("AWAKE DO GAMEMANAGER EXECUTOU!");

        // Impede a existência de dois GameManagers.
        if (instance != null && instance != this)
        {
            Debug.LogWarning("GameManager duplicado encontrado!");
            Destroy(gameObject);
            return;
        }

        // Registra esta instância.
        instance = this;

        // Mantém o objeto durante as trocas de cena.
        DontDestroyOnLoad(gameObject);

        Debug.Log("INSTANCIA DO GAMEMANAGER CRIADA!");

        // Define a posição inicial.
        if (startPoint != null)
        {
            currentSpawnPosition = startPoint.position;
            checkpointDefinido = true;
        }
        else
        {
            currentSpawnPosition = transform.position;

            Debug.LogWarning(
                "GameManager: Start Point não configurado. " +
                "Usando a posição do próprio GameManager."
            );
        }

        // Recupera os artefatos salvos.
        CarregarArtefatos();

        Debug.Log("GAMEMANAGER INICIADO CORRETAMENTE!");
    }

    // CHECKPOINTS

    public void SetCheckpoint(Vector3 pos)
    {
        currentSpawnPosition = pos;
        checkpointDefinido = true;

        Debug.Log("CHECKPOINT SALVO: " + pos);
    }

    public Vector3 GetSpawnPosition()
    {
        if (!checkpointDefinido && startPoint != null)
        {
            currentSpawnPosition = startPoint.position;
            checkpointDefinido = true;
        }

        return currentSpawnPosition;
    }

    public void ResetToStart()
    {
        if (startPoint != null)
        {
            currentSpawnPosition = startPoint.position;
            checkpointDefinido = true;

            Debug.Log("POSIÇÃO INICIAL RESTAURADA!");
        }
    }

    // COLETA DE ARTEFATOS

    public void ColetarEspada()
    {
        espadaColetada = true;

        PlayerPrefs.SetInt(CHAVE_ESPADA, 1);
        PlayerPrefs.Save();

        Debug.Log("ESPADA DO OLIMPO COLETADA!");
    }

    public void ColetarEscudo()
    {
        escudoColetado = true;

        PlayerPrefs.SetInt(CHAVE_ESCUDO, 1);
        PlayerPrefs.Save();

        Debug.Log("ESCUDO DO OLIMPO COLETADO!");
    }

    public void ColetarRaio()
    {
        raioColetado = true;

        PlayerPrefs.SetInt(CHAVE_RAIO, 1);
        PlayerPrefs.Save();

        Debug.Log("RAIO DO OLIMPO COLETADO!");
    }

    // CARREGAR ARTEFATOS SALVOS

    public void CarregarArtefatos()
    {
        espadaColetada =
            PlayerPrefs.GetInt(CHAVE_ESPADA, 0) == 1;

        escudoColetado =
            PlayerPrefs.GetInt(CHAVE_ESCUDO, 0) == 1;

        raioColetado =
            PlayerPrefs.GetInt(CHAVE_RAIO, 0) == 1;

        Debug.Log(
            "ARTEFATOS CARREGADOS: " +
            "Espada: " + espadaColetada +
            " | Escudo: " + escudoColetado +
            " | Raio: " + raioColetado
        );
    }

    // VERIFICAR FINAL DO JOGO

    public bool TodosArtefatosColetados()
    {
        return espadaColetada &&
               escudoColetado &&
               raioColetado;
    }

    // REINICIAR PROGRESSO DOS ARTEFATOS

    public void ResetarArtefatos()
    {
        espadaColetada = false;
        escudoColetado = false;
        raioColetado = false;

        PlayerPrefs.DeleteKey(CHAVE_ESPADA);
        PlayerPrefs.DeleteKey(CHAVE_ESCUDO);
        PlayerPrefs.DeleteKey(CHAVE_RAIO);

        PlayerPrefs.Save();

        Debug.Log("PROGRESSO DOS ARTEFATOS REINICIADO!");
    }

    private void OnDestroy()
    {
        // Evita limpar uma instância que não seja a atual.
        if (instance == this)
        {
            instance = null;
        }
    }
}
