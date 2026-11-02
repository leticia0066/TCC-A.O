
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
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        if (startPoint != null)
        {
            currentSpawnPosition = startPoint.position;
            checkpointDefinido = true;
        }
        else
        {
            currentSpawnPosition = transform.position;

            Debug.LogWarning(
                "GameManager: Start Point não foi atribuído. " +
                "Usando a posição do próprio GameManager."
            );
        }

        CarregarArtefatos();
    }

    // CHECKPOINTS

    public void SetCheckpoint(Vector3 pos)
    {
        currentSpawnPosition = pos;
        checkpointDefinido = true;

        Debug.Log("Checkpoint ativado: " + pos);
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

    public void CarregarArtefatos()
    {
        espadaColetada = PlayerPrefs.GetInt(CHAVE_ESPADA, 0) == 1;
        escudoColetado = PlayerPrefs.GetInt(CHAVE_ESCUDO, 0) == 1;
        raioColetado = PlayerPrefs.GetInt(CHAVE_RAIO, 0) == 1;
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

        PlayerPrefs.DeleteKey(CHAVE_ESPADA);
        PlayerPrefs.DeleteKey(CHAVE_ESCUDO);
        PlayerPrefs.DeleteKey(CHAVE_RAIO);

        PlayerPrefs.Save();

        Debug.Log("Progresso dos artefatos reiniciado.");
    }
}
