
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

    private int ultimaVidaExibida = -1;

    private void Awake()
    {
        maxHealth = Mathf.Max(1, maxHealth);
    }

    private void Start()
    {
        if (!ReferenciasValidas())
            return;

        UpdateHearts(maxHealth);
    }

    private bool ReferenciasValidas()
    {
        if (hearts == null || hearts.Length != 3)
        {
            Debug.LogError("HeartUI: configure exatamente 3 corações!");
            return false;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null)
            {
                Debug.LogError("HeartUI: falta configurar o coração " + (i + 1));
                return false;
            }
        }

        if (fullHeart == null || halfHeart == null || emptyHeart == null)
        {
            Debug.LogError("HeartUI: faltam os sprites dos corações!");
            return false;
        }

        return true;
    }

    public void UpdateHearts(int health)
    {
        if (!ReferenciasValidas())
            return;

        health = Mathf.Clamp(health, 0, maxHealth);

        ultimaVidaExibida = health;

        for (int i = 0; i < hearts.Length; i++)
        {
            int vidaDoCoracao = health - (i * 2);

            if (vidaDoCoracao >= 2)
                hearts[i].sprite = fullHeart;
            else if (vidaDoCoracao == 1)
                hearts[i].sprite = halfHeart;
            else
                hearts[i].sprite = emptyHeart;

            hearts[i].enabled = true;
            hearts[i].preserveAspect = true;
        }

        Debug.Log("HeartUI atualizado: " + health + "/" + maxHealth);
    }

    public int GetUltimaVidaExibida()
    {
        return ultimaVidaExibida;
    }
}
