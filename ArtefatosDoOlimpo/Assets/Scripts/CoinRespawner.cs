
using UnityEngine;

public class CoinRespawner : MonoBehaviour
{
    private Coin[] coins;

    private void Awake()
    {
        coins = FindObjectsOfType<Coin>(true);
    }

    public void RespawnAllCoins()
    {
        foreach (Coin coin in coins)
        {
            if (coin != null)
            {
                coin.Respawn();
            }
        }

        Debug.Log("Moedas não coletadas foram restauradas!");
    }
}
