
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Spawn inicial")]
    public Transform startPoint;

    private Vector3 currentSpawnPosition;
    private bool checkpointDefinido = false;

    void Awake()
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
    }

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
}
