using UnityEngine;

public class PlayerAtaque : MonoBehaviour
{
    [Header("Configurações")]
    public float dano = 20f;
    public float alcance = 2f;
    public LayerMask inimigos;

    [Header("Projétil")]
    public GameObject prefabProjetil;           
    public Transform pontoDeLancamento;        
    public float forcaProjetil = 10f;           
    public float tempoCooldown = 0.8f;          

    [Header("Referências")]
    public PlayerVida playerVida;

    private bool podeAtacar = true;

    void Start()
    {
        if (pontoDeLancamento == null)
        {
            pontoDeLancamento = transform;
        }

        if (playerVida == null)
        {
            playerVida = GetComponent<PlayerVida>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && podeAtacar)
        {
            Atacar();
        }
    }

    void Atacar()
    {
        podeAtacar = false;

        Collider[] inimigosAtingidos = Physics.OverlapSphere(transform.position, alcance, inimigos);

        bool inimigoProximo = false;
        foreach (Collider col in inimigosAtingidos)
        {
            ChefeIA chefe = col.GetComponent<ChefeIA>();
            if (chefe != null)
            {
                chefe.ReceberDano(dano);
                inimigoProximo = true;
            }
        }

        if (!inimigoProximo && prefabProjetil != null && pontoDeLancamento != null)
        {
            GameObject projetil = Instantiate(
                prefabProjetil,
                pontoDeLancamento.position,
                pontoDeLancamento.rotation
            );

            ProjetilMagico scriptProjetil = projetil.GetComponent<ProjetilMagico>();
            if (scriptProjetil != null)
            {
                scriptProjetil.dano = dano;
            }

            projetil.transform.forward = pontoDeLancamento.forward;

            Rigidbody rb = projetil.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = projetil.transform.forward * forcaProjetil;
            }
        }

        Invoke(nameof(ResetarAtaque), tempoCooldown);
    }

    void ResetarAtaque()
    {
        podeAtacar = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alcance);
    }
}