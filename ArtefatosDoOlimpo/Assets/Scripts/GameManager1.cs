using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Spawn")]
    public Transform startPoint;

    private Vector3 currentSpawnPosition;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (startPoint != null)
        {
            currentSpawnPosition =
                startPoint.position;
        }
    }

    public void SetCheckpoint(Vector3 pos)
    {
        currentSpawnPosition = pos;
    }

    public Vector3 GetSpawnPosition()
    {
        return currentSpawnPosition;
    }

    public void ResetToStart()
    {
        if (startPoint != null)
        {
            currentSpawnPosition =
                startPoint.position;
        }
    }
}