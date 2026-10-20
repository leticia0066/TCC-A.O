using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 20;

    public int currentHealth;

    public Slider healthBar;

    private Animator anim;

    private bool morreu;

    private void Start()
    {
        currentHealth = maxHealth;

        anim = GetComponent<Animator>();

        AtualizarBarra();
    }

    public void TakeDamage(int damage)
    {
        if (morreu)
            return;

        currentHealth -= damage;

        AtualizarBarra();

        StartCoroutine(Knockback());

        if (currentHealth <= 0)
        {
            Morrer();
        }
    }

    void AtualizarBarra()
    {
        healthBar.value = currentHealth;
    }

    System.Collections.IEnumerator Knockback()
    {
        Vector3 posOriginal =
            transform.position;

        transform.position +=
            Vector3.left * 0.2f;

        yield return new WaitForSeconds(0.1f);

        transform.position =
            posOriginal;
    }

    void Morrer()
    {
        morreu = true;

        anim.SetTrigger("Die");

        Destroy(gameObject, 2f);
    }
}