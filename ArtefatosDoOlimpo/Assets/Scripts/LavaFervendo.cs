
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class LavaFervendo : MonoBehaviour
{
    [Header("Tilemap da Lava")]
    public Collider2D colisorLava;

    [Header("Quantidade de Bolhas")]
    public float bolhasPorSegundo = 8f;

    [Header("Tamanho das Bolhas")]
    public float tamanhoMinimo = 0.07f;
    public float tamanhoMaximo = 0.16f;

    [Header("Pulo das Bolhas")]
    public float velocidadeMinima = 1.5f;
    public float velocidadeMaxima = 3.5f;
    public float gravidade = 2f;

    [Header("Tempo de Vida")]
    public float tempoMinimo = 0.4f;
    public float tempoMaximo = 0.9f;

    [Header("Aparência")]
    public Color corInicial = new Color(1f, 0.25f, 0f);
    public Color corFinal = new Color(1f, 0.85f, 0.1f);

    private ParticleSystem particulas;

    void Start()
    {
        particulas = GetComponent<ParticleSystem>();

        if (colisorLava == null)
        {
            Debug.LogError("Arraste o Collider da lava no Inspector!");
            return;
        }

        ConfigurarParticulas();
    }

    void ConfigurarParticulas()
    {
        Bounds limites = colisorLava.bounds;

        // Posiciona o efeito na superfície da lava
        transform.position = new Vector3(
            limites.center.x,
            limites.max.y,
            0f
        );

        var main = particulas.main;
        main.loop = true;
        main.playOnAwake = true;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = new ParticleSystem.MinMaxCurve(
            tempoMinimo, tempoMaximo
        );
        main.startSpeed = new ParticleSystem.MinMaxCurve(
            velocidadeMinima, velocidadeMaxima
        );
        main.startSize = new ParticleSystem.MinMaxCurve(
            tamanhoMinimo, tamanhoMaximo
        );
        main.startColor = corInicial;
        main.gravityModifier = gravidade;
        main.maxParticles = 300;

        var emission = particulas.emission;
        emission.enabled = true;
        emission.rateOverTime = bolhasPorSegundo;

        var shape = particulas.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(limites.size.x, 0.03f, 0.01f);

        var color = particulas.colorOverLifetime;
        color.enabled = true;

        Gradient gradiente = new Gradient();
        gradiente.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(corInicial, 0f),
                new GradientColorKey(corFinal, 0.5f),
                new GradientColorKey(corInicial, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            }
        );

        color.color = new ParticleSystem.MinMaxGradient(gradiente);

        var renderer = particulas.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.sortingLayerName = "Default";
        renderer.sortingOrder = 10;

        particulas.Play();
    }
}
