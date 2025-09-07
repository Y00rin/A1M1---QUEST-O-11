using UnityEngine;
using UnityEngine.UI;

public class UIBarraVida : MonoBehaviour
{
    [Header("Configurações")]
    public Image barra;
    public GameObject objeto;

    private PlayerVida playerVida;
    private ChefeIA chefeIA;

    void Start()
    {
        if (barra == null || objeto == null)
        {
            return;
        }

        playerVida = objeto.GetComponent<PlayerVida>();
        chefeIA = objeto.GetComponent<ChefeIA>();

        if (playerVida == null && chefeIA == null)
        {
            return;
        }

        AtualizarBarra();
    }

    void Update()
    {
        AtualizarBarra();
    }

    void AtualizarBarra()
    {
        float vidaAtual = 0;
        float vidaMaxima = 1;

        if (playerVida != null)
        {
            vidaAtual = playerVida.GetVidaAtual();
            vidaMaxima = playerVida.GetVidaMaxima();
        }
        else if (chefeIA != null)
        {
            vidaAtual = chefeIA.vidaAtual;
            vidaMaxima = chefeIA.vidaMaxima;
        }

        if (barra != null && vidaMaxima > 0)
        {
            barra.fillAmount = Mathf.Clamp01(vidaAtual / vidaMaxima);
        }
    }
}