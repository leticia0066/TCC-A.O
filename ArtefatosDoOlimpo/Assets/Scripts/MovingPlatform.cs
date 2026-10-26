using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Pontos de movimento")]
    public Transform pointA;
    public Transform pointB;

    [Header("Configurações")]
    public float speed = 2f;

    private Transform target;

    void Start()
    {
        target = pointB;
    }

    void FixedUpdate()
    {
        if (pointA == null || pointB == null)
            return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.fixedDeltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            target = target == pointA ? pointB : pointA;
        }
    }
}