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
        if (healthSlider != null && healthSlider.fillRect != null)
        {
            imagemPreenchimento =
                healthSlider.fillRect.GetComponent<Image>();
        }
    }

    private void Start()
    {
        if (maxHealth <= 0)
            maxHealth = 6;

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

        if (healthSlider.fillRect == null)
        {
            Debug.LogError(
                "HeartUI: o campo Fill Rect do Slider está vazio!"
            );
            return;
        }

        imagemPreenchimento =
            healthSlider.fillRect.GetComponent<Image>();

        if (imagemPreenchimento == null)
        {
            Debug.LogError(
                "HeartUI: o objeto Fill precisa ter um componente Image!"
            );
            return;
        }

        // Começa com a barra cheia.
        AtualizarBarra(maxHealth);

        // Inicializa a aparência imediatamente.
        healthSlider.SetValueWithoutNotify(maxHealth);
        imagemPreenchimento.color = corDesejada;

        if (heartIcon != null)
            heartIcon.color = corDesejada;
    }

    private void Update()
    {
        if (healthSlider == null)
            return;

        if (smoothTransition)
        {
            // Diminui o preenchimento suavemente.
            healthSlider.value = Mathf.MoveTowards(
                healthSlider.value,
                vidaDesejada,
                transitionSpeed * Time.deltaTime
            );

            // Faz a transição de cor.
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

    // Mantém o nome usado pelo sistema de vida atual.
    public void UpdateHearts(int health)
    {
        AtualizarBarra(health);
    }

    private void AtualizarBarra(int health)
    {
        if (healthSlider == null)
            return;

        health = Mathf.Clamp(health, 0, maxHealth);

        // Guarda a vida desejada sem redefini-la a cada frame.
        vidaDesejada = health;

        float porcentagemVida = (float)health / maxHealth;

        // Escolhe a cor conforme a vida restante.
        if (porcentagemVida > 0.6f)
        {
            corDesejada = highHealthColor;
        }
        else if (porcentagemVida > 0.3f)
        {
            corDesejada = mediumHealthColor;
        }
        else
        {
            corDesejada = lowHealthColor;
        }
    }

    // Permite atualizar a vida máxima caso o sistema precise.
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