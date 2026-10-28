using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [Header("Imagens dos 3 corações")]
    public Image[] hearts = new Image[3];

    [Header("Sprites dos corações")]
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    [Header("Configuração")]
    public int maxHealth = 6;

    private void Awake()
    {
        maxHealth = Mathf.Max(1, maxHealth);
    }

    private void Start()
    {
        if (hearts == null || hearts.Length != 3)
        {
            Debug.LogError(
                "HeartUI: configure exatamente 3 imagens de coração!"
            );
            return;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null)
            {
                Debug.LogError(
                    "HeartUI: falta configurar a imagem do coração " +
                    (i + 1) + " no Inspector!"
                );

                return;
            }
        }

        if (fullHeart == null ||
            halfHeart == null ||
            emptyHeart == null)
        {
            Debug.LogError(
                "HeartUI: configure os sprites cheio, meio e vazio!"
            );
            return;
        }

        UpdateHearts(maxHealth);
    }

    public void UpdateHearts(int health)
    {
        if (hearts == null || hearts.Length != 3)
            return;

        health = Mathf.Clamp(health, 0, maxHealth);

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null)
                continue;

            // Cada coração representa 2 pontos de vida.
            int vidaDoCoracao = health - (i * 2);

            if (vidaDoCoracao >= 2)
            {
                hearts[i].sprite = fullHeart;
            }
            else if (vidaDoCoracao == 1)
            {
                hearts[i].sprite = halfHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            hearts[i].enabled = true;
            hearts[i].preserveAspect = true;
        }
    }
}