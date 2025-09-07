using UnityEngine;

public class PlayerVida : MonoBehaviour
{
    [Header("Status")]
    public float vidaMaxima = 100f;
    private float vidaAtual;

    [Header("Referências")]
    public GameObject painelMorte;

    void Start()
    {
        vidaAtual = vidaMaxima;
        AtualizarBarraDeVida();
    }

    public void ReceberDano(float dano)
    {
        vidaAtual -= dano;
        vidaAtual = Mathf.Clamp(vidaAtual, 0, vidaMaxima);

        AtualizarBarraDeVida();

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    void Morrer()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<PlayerMover>().enabled = false;

        if (painelMorte != null)
        {
            painelMorte.SetActive(true);
        }

        enabled = false;
    }

    public float GetVidaAtual() => vidaAtual;
    public float GetVidaMaxima() => vidaMaxima;


    void AtualizarBarraDeVida()
    {
    }
}