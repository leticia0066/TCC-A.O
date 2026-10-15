using UnityEngine;

public class Machado : MonoBehaviour
{
    public float speed = 8f;
    public float rotationSpeed = 500f;

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.position +=
            (Vector3)(direction * speed * Time.deltaTime);

        transform.Rotate(
            0,
            0,
            rotationSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.SendMessage(
                "TakeDamage",
                1,
                SendMessageOptions.DontRequireReceiver
            );

            Destroy(gameObject);
        }

        if (!collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}