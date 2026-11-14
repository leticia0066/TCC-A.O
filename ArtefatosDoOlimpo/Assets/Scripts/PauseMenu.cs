
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    [Header("Transição")]
    public UITransition settingsTransition;

    [Header("Botões")]
    public Button continuarButton;
    public Button reiniciarButton;
    public Button sairButton;
    public Button botaoConfiguracao;
    public Button botaoCreditos;
    public Button botaoVoltarCreditos;

    [Header("Volume")]
    public Slider volumeSlider;
    public TMP_Text textoVolume;

    [Header("Menu principal")]
    public string nomeMenuPrincipal = "_Boot";

    private bool pausado = false;
    private bool fechandoPainel = false;

    private const string CHAVE_VOLUME = "VolumeJogo";

    private Coroutine fecharCoroutine;

    private void Start()
    {
        Time.timeScale = 1f;
        pausado = false;
        fechandoPainel = false;

        float volumeSalvo = PlayerPrefs.GetFloat(
            CHAVE_VOLUME, 1f
        );

        AudioListener.volume = volumeSalvo;

        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.wholeNumbers = false;

            volumeSlider.SetValueWithoutNotify(volumeSalvo);

            volumeSlider.onValueChanged.RemoveListener(
                AlterarVolume
            );

            volumeSlider.onValueChanged.AddListener(
                AlterarVolume
            );
        }

        if (continuarButton != null)
        {
            continuarButton.onClick.RemoveListener(Continuar);
            continuarButton.onClick.AddListener(Continuar);
        }

        if (reiniciarButton != null)
        {
            reiniciarButton.onClick.RemoveListener(ReiniciarJogo);
            reiniciarButton.onClick.AddListener(ReiniciarJogo);
        }

        if (sairButton != null)
        {
            sairButton.onClick.RemoveListener(VoltarAoMenu);
            sairButton.onClick.AddListener(VoltarAoMenu);
        }

        if (botaoConfiguracao != null)
        {
            botaoConfiguracao.onClick.RemoveListener(TogglePause);
            botaoConfiguracao.onClick.AddListener(TogglePause);
        }

        // Botão de créditos
        if (botaoCreditos != null)
        {
            botaoCreditos.onClick.RemoveListener(AbrirCreditos);
            botaoCreditos.onClick.AddListener(AbrirCreditos);
        }

        // Botão de voltar dos créditos
        if (botaoVoltarCreditos != null)
        {
            botaoVoltarCreditos.onClick.RemoveListener(FecharCreditos);
            botaoVoltarCreditos.onClick.AddListener(FecharCreditos);
        }

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        AtualizarTextoVolume(volumeSalvo);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (pausado)
            Continuar();
        else
            Pausar();
    }

    public void Pausar()
    {
        if (pausePanel == null)
        {
            Debug.LogError(
                "PauseMenu: arraste PausePanel!"
            );
            return;
        }

        if (fecharCoroutine != null)
        {
            StopCoroutine(fecharCoroutine);
            fecharCoroutine = null;
        }

        pausado = true;
        fechandoPainel = false;

        pausePanel.SetActive(true);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        if (settingsTransition != null)
        {
            settingsTransition.Show();
        }
        else if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    // ABRIR CRÉDITOS
    public void AbrirCreditos()
    {
        if (creditsPanel == null)
        {
            Debug.LogError(
                "PauseMenu: CreditsPanel não foi configurado!"
            );
            return;
        }

        creditsPanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    // VOLTAR PARA AS CONFIGURAÇÕES
    public void FecharCreditos()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void Continuar()
    {
        if (!pausado || fechandoPainel)
            return;

        pausado = false;
        fechandoPainel = true;

        Time.timeScale = 1f;

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        if (settingsTransition != null)
        {
            settingsTransition.Hide();

            fecharCoroutine = StartCoroutine(
                FecharPainelDepoisDaAnimacao()
            );
        }
        else
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(false);

            if (pausePanel != null)
                pausePanel.SetActive(false);

            fechandoPainel = false;
        }
    }

    private IEnumerator FecharPainelDepoisDaAnimacao()
    {
        float espera = 0.3f;

        if (settingsTransition != null)
            espera = settingsTransition.duration + 0.05f;

        yield return new WaitForSecondsRealtime(espera);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        fechandoPainel = false;
        fecharCoroutine = null;
    }

    public void AlterarVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        AudioListener.volume = volume;

        PlayerPrefs.SetFloat(CHAVE_VOLUME, volume);
        PlayerPrefs.Save();

        AtualizarTextoVolume(volume);
    }

    private void AtualizarTextoVolume(float volume)
    {
        if (textoVolume != null)
        {
            textoVolume.text =
                "VOLUME: " +
                Mathf.RoundToInt(volume * 100f) +
                "%";
        }
    }

    public void ReiniciarJogo()
    {
        Time.timeScale = 1f;
        pausado = false;
        fechandoPainel = false;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void VoltarAoMenu()
    {
        if (string.IsNullOrWhiteSpace(nomeMenuPrincipal))
        {
            Debug.LogError(
                "PauseMenu: informe o nome da cena do menu principal!"
            );
            return;
        }

        int indiceCena = -1;

        for (
            int i = 0;
            i < SceneManager.sceneCountInBuildSettings;
            i++)
        {
            string caminho =
                SceneUtility.GetScenePathByBuildIndex(i);

            string nomeCena =
                Path.GetFileNameWithoutExtension(caminho);

            if (nomeCena == nomeMenuPrincipal)
            {
                indiceCena = i;
                break;
            }
        }

        if (indiceCena < 0)
        {
            Debug.LogError(
                "PauseMenu: a cena '" +
                nomeMenuPrincipal +
                "' não foi encontrada nas cenas incluídas no jogo."
            );
            return;
        }

        Time.timeScale = 1f;
        pausado = false;
        fechandoPainel = false;

        SceneManager.LoadScene(indiceCena);
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(
                AlterarVolume
            );
        }
    }
}
