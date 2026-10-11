
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Fonte dos efeitos sonoros")]
    public AudioSource sfxSource;

    [Header("Fonte exclusiva dos passos")]
    public AudioSource footstepsSource;

    [Header("Sons do jogador")]
    public AudioClip jumpClip;
    public AudioClip footstepClip;
    public AudioClip attackClip;
    public AudioClip landingClip;
    public AudioClip damageClip;
    public AudioClip respawnClip;

    [Header("Sons das moedas e perigos")]
    public AudioClip coinClip;
    public AudioClip medusaWarningClip;
    public AudioClip checkpointClip;
    public AudioClip pressurePlateClip;

    [Header("Sons da Medusa")]
    public AudioClip medusaCastClip;
    public AudioClip medusaHitClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        AudioSource[] sources = GetComponents<AudioSource>();

        if (sfxSource == null)
        {
            sfxSource = sources.Length > 0
                ? sources[0]
                : gameObject.AddComponent<AudioSource>();
        }

        if (footstepsSource == null)
        {
            footstepsSource = gameObject.AddComponent<AudioSource>();
        }

        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.spatialBlend = 0f;

        footstepsSource.playOnAwake = false;
        footstepsSource.loop = true;
        footstepsSource.spatialBlend = 0f;
        footstepsSource.clip = footstepClip;
        footstepsSource.Stop();
    }

    private void Start()
    {
        if (footstepsSource != null)
            footstepsSource.clip = footstepClip;
    }

    public void TocarPulo()
    {
        TocarSom(jumpClip);
    }

    public void TocarAtaque()
    {
        TocarSom(attackClip);
    }

    public void TocarAterrissagem()
    {
        TocarSom(landingClip);
    }

    public void TocarMoeda()
    {
        TocarSom(coinClip);
    }

    public void TocarAvisoMedusa()
    {
        TocarSom(medusaWarningClip);
    }

    public void TocarCheckpoint()
    {
        TocarSom(checkpointClip);
    }

    public void TocarPlacaPressao()
    {
        TocarSom(pressurePlateClip);
    }

    public void TocarAtaqueMedusa()
    {
        TocarSom(medusaCastClip);
    }

    public void TocarImpactoMedusa()
    {
        TocarSom(medusaHitClip);
    }

    public void TocarDano()
    {
        TocarSom(damageClip);
    }

    public void TocarRetornoInicio()
    {
        TocarSom(respawnClip);
    }

    public void TocarSom(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    public void IniciarPassos()
    {
        if (footstepsSource == null || footstepClip == null)
            return;

        if (footstepsSource.clip != footstepClip)
            footstepsSource.clip = footstepClip;

        if (!footstepsSource.isPlaying)
        {
            footstepsSource.loop = true;
            footstepsSource.Play();
        }
    }

    public void PararPassos()
    {
        if (footstepsSource != null && footstepsSource.isPlaying)
            footstepsSource.Stop();
    }
}
