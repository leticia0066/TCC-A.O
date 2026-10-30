using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager Instance { get; private set; }

    [Header("Som dos botões")]
    public AudioSource audioSource;
    public AudioClip somBotao;

    [Range(0f, 1f)]
    public float volume = 0.6f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    public void TocarSomBotao()
    {
        if (audioSource == null || somBotao == null)
        {
            Debug.LogWarning(
                "UISoundManager: configure o AudioSource e o Som Botao."
            );

            return;
        }

        audioSource.PlayOneShot(somBotao, volume);
    }
}