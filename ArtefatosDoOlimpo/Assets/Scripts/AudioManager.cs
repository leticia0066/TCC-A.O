
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

    [Tooltip("Som da subida ou descida do pilar")]
    public AudioClip pressurePlateClip;

    [Tooltip("Som emitido quando o jogador pisa na placa")]
    public AudioClip plateStepClip;

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
        {
            footstepsSource.clip = footstepClip;
        }
    }

    // SONS DO JOGADOR

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

    public void TocarDano()
    {
        TocarSom(damageClip);
    }

    public void TocarRetornoInicio()
    {
        TocarSom(respawnClip);
    }

    // MOEDAS, AVISOS E CHECKPOINTS

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

    // PLACA E PILAR

    // Mantém o método antigo para compatibilidade
    // com scripts que já chamam TocarPlacaPressao().
    public void TocarPlacaPressao()
    {
        TocarSom(pressurePlateClip);
    }

    // Novo método: som de quando o jogador pisa na placa.
    public void TocarSomPlaca()
    {
        TocarSom(plateStepClip);
    }

    // Método adicional para deixar explícito
    // que este áudio corresponde ao movimento do pilar.
    public void TocarSubidaPilar()
    {
        TocarSom(pressurePlateClip);
    }

    // SONS DA MEDUSA

    public void TocarAtaqueMedusa()
    {
        TocarSom(medusaCastClip);
    }

    public void TocarImpactoMedusa()
    {
        TocarSom(medusaHitClip);
    }

    // REPRODUÇÃO GERAL

    public void TocarSom(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    // PASSOS DO JOGADOR

    public void IniciarPassos()
    {
        if (footstepsSource == null || footstepClip == null)
        {
            return;
        }

        if (footstepsSource.clip != footstepClip)
        {
            footstepsSource.clip = footstepClip;
        }

        if (!footstepsSource.isPlaying)
        {
            footstepsSource.loop = true;
            footstepsSource.Play();
        }
    }

    public void PararPassos()
    {
        if (footstepsSource != null &&
            footstepsSource.isPlaying)
        {
            footstepsSource.Stop();
        }
    }
}
