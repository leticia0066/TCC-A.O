
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 8;
    private int currentHealth;

    [Header("Petrificacao da Medusa")]
    [Range(0f, 1f)]
    public float porcentagemParaPetrificar = 0.70f;

    public MedusaController medusaController;

    [Header("Porta do chefe")]
    public GameObject bossWall;

    [Header("Barra de vida")]
    public Slider barraVida;
    public Image fillImage;

    [Header("Tremor ao receber dano")]
    public float duracaoTremor = 0.15f;
    public float intensidadeTremor = 0.07f;

    private Coroutine rotinaTremor;
    private Vector3 posicaoInicial;
    private bool morto;

    private void Awake()
    {
        posicaoInicial = transform.localPosition;

        if (medusaController == null)
            medusaController = GetComponent<MedusaController>();

        if (medusaController == null)
            medusaController = GetComponentInParent<MedusaController>();

        if (medusaController == null)
            medusaController = GetComponentInChildren<MedusaController>(true);
    }

    private void Start()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = maxHealth;

        AtualizarBarra();

        if (medusaController == null)
            Debug.LogError("EnemyHealth: MedusaController nao encontrado!");
    }

    public void TakeDamage(int damage)
    {
        if (morto || damage <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        Debug.Log("Medusa tomou dano! Vida: " + currentHealth);

        AtualizarBarra();

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        float porcentagemVida =
            (float)currentHealth / maxHealth;

        if (porcentagemVida <= porcentagemParaPetrificar &&
            medusaController != null)
        {
            medusaController.AtivarPetrificacao();
        }

        Tremer();
    }

    private void AtualizarBarra()
    {
        if (barraVida != null)
        {
            barraVida.maxValue = maxHealth;
            barraVida.minValue = 0;
            barraVida.value = currentHealth;
        }

        if (fillImage != null && maxHealth > 0)
        {
            float porcentagem = (float)currentHealth / maxHealth;

            if (porcentagem > 0.5f)
                fillImage.color = Color.green;
            else if (porcentagem > 0.25f)
                fillImage.color = Color.yellow;
            else
                fillImage.color = Color.red;
        }
    }

    private void Tremer()
    {
        if (rotinaTremor != null)
        {
            StopCoroutine(rotinaTremor);
            transform.localPosition = posicaoInicial;
        }

        rotinaTremor = StartCoroutine(ExecutarTremor());
    }

    private IEnumerator ExecutarTremor()
    {
        float tempo = 0f;

        while (tempo < duracaoTremor)
        {
            tempo += Time.deltaTime;

            Vector2 deslocamento = Random.insideUnitCircle *
                intensidadeTremor;

            transform.localPosition = posicaoInicial +
                new Vector3(deslocamento.x, deslocamento.y, 0f);

            yield return null;
        }

        transform.localPosition = posicaoInicial;
        rotinaTremor = null;
    }

    private void Die()
    {
        if (morto)
            return;

        morto = true;

        Debug.Log("Medusa morreu!");

        if (rotinaTremor != null)
        {
            StopCoroutine(rotinaTremor);
            transform.localPosition = posicaoInicial;
        }

        if (barraVida != null)
            barraVida.gameObject.SetActive(false);

        if (bossWall != null)
            bossWall.SetActive(false);

        Destroy(gameObject);
    }
}
