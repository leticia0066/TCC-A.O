
using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [Header("Barra de Vida")]
    public Slider healthSlider;

    [Header("Coração")]
    public Image heartIcon;

    [Header("Vida")]
    public int maxHealth = 6;

    [Header("Cores")]
    public Color highHealthColor = new Color(0.2f, 0.85f, 0.2f);
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;

    [Header("Transição")]
    public bool smoothTransition = true;
    public float transitionSpeed = 5f;

    private float vidaDesejada;
    private Color corDesejada;

    private Image imagemPreenchimento;

    private void Awake()
    {
        LocalizarPreenchimento();
    }

    private void Start()
    {
        maxHealth = Mathf.Max(1, maxHealth);

        if (healthSlider == null)
        {
            Debug.LogError(
                "HeartUI: arraste BarraVidaSlider para Health Slider!"
            );
            return;
        }

        healthSlider.minValue = 0;
        healthSlider.maxValue = maxHealth;
        healthSlider.wholeNumbers = true;
        healthSlider.interactable = false;

        LocalizarPreenchimento();

        if (imagemPreenchimento == null)
        {
            Debug.LogError(
                "HeartUI: o Fill Rect precisa ter um componente Image!"
            );
            return;
        }

        // Começa com a barra cheia.
        AtualizarHearts(maxHealth);

        healthSlider.SetValueWithoutNotify(maxHealth);

        imagemPreenchimento.color = corDesejada;

        if (heartIcon != null)
            heartIcon.color = corDesejada;
    }

    private void Update()
    {
        if (healthSlider == null)
            return;

        // Atualiza o preenchimento da barra.
        if (smoothTransition)
        {
            healthSlider.value = Mathf.MoveTowards(
                healthSlider.value,
                vidaDesejada,
                transitionSpeed * Time.deltaTime
            );

            // Atualiza a cor suavemente.
            if (imagemPreenchimento != null)
            {
                imagemPreenchimento.color = Color.Lerp(
                    imagemPreenchimento.color,
                    corDesejada,
                    transitionSpeed * Time.deltaTime
                );
            }
        }
        else
        {
            healthSlider.value = vidaDesejada;

            if (imagemPreenchimento != null)
                imagemPreenchimento.color = corDesejada;
        }

        if (heartIcon != null)
            heartIcon.color = corDesejada;
    }

    // Método chamado pelo PlayerHealth.
    public void UpdateHearts(int health)
    {
        AtualizarHearts(health);
    }

    private void AtualizarHearts(int health)
    {
        if (healthSlider == null)
            return;

        health = Mathf.Clamp(health, 0, maxHealth);

        // Define quanto a barra deve ficar preenchida.
        vidaDesejada = health;

        float porcentagemVida = (float)health / maxHealth;

        // Mais de 50%: verde.
        // Mais de 20% até 50%: amarelo.
        // De 1% até 20%: vermelho.
        // 0%: vermelho e barra vazia.
        if (health == 0)
        {
            corDesejada = lowHealthColor;
        }
        else if (porcentagemVida > 0.5f)
        {
            corDesejada = highHealthColor;
        }
        else if (porcentagemVida > 0.2f)
        {
            corDesejada = mediumHealthColor;
        }
        else
        {
            corDesejada = lowHealthColor;
        }
    }

    private void LocalizarPreenchimento()
    {
        if (healthSlider == null)
            return;

        if (healthSlider.fillRect == null)
            return;

        imagemPreenchimento =
            healthSlider.fillRect.GetComponent<Image>();
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        if (newMaxHealth <= 0)
            return;

        maxHealth = newMaxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
        }
    }
}
