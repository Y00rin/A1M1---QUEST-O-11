using UnityEngine;

public class ChefeIA : MonoBehaviour
{
    [Header("Configurações")]
    public Transform jogador;
    public Transform pontoFuga;
    public float raioPerseguicao = 15f;
    public float raioMelee = 3f;
    public float raioRanged = 10f;
    public float velocidade = 5f;
    public float taxaRegeneracao = 5f;
    public float intervaloAtaqueEspecial = 4f;
    public Transform pontoDeLancamento;
    public float cooldownAtaque = 1.5f;
    public float cooldownRecuperacao = 5f;

    [Header("Status")]
    public float vidaAtual = 100f;
    public float vidaMaxima = 100f;
    private EstadoChefe estadoAtual;
    private float ultimoAtaqueEspecial;
    private bool podeAtacar = true;

    [Header("Referências")]
    public GameObject painelMorteChefao;
    private CharacterController controller;
    public GameObject prefabProjetil;
    private float tempoUltimaRecuperacao;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            enabled = false;
            return;
        }

        vidaAtual = vidaMaxima;
        estadoAtual = EstadoChefe.Idle;
        ultimoAtaqueEspecial = Time.time;
    }

    void Update()
    {
        if (vidaAtual <= 0 && estadoAtual != EstadoChefe.Dead)
        {
            MudarEstado(EstadoChefe.Dead);
            return;
        }

        switch (estadoAtual)
        {
            case EstadoChefe.Idle:
                AtualizarIdle();
                break;

            case EstadoChefe.Chase:
                MoverEmDirecao(jogador.position);
                AtualizarCombate();
                break;

            case EstadoChefe.AttackRanged:
                if (podeAtacar)
                {
                    podeAtacar = false;
                    Invoke(nameof(AtacarADistancia), 0.5f);
                    Invoke(nameof(ReativarAtaque), cooldownAtaque);
                }
                MudarEstado(EstadoChefe.Chase);
                break;

            case EstadoChefe.AttackMelee:
                Invoke(nameof(DarDanoNoJogador), 0.5f);
                MudarEstado(EstadoChefe.Chase);
                break;

            case EstadoChefe.SpecialAttack:
                AtualizarSpecialAttack();
                break;

            case EstadoChefe.FleeRecover:
                AtualizarFleeRecover();
                break;

            case EstadoChefe.Dead:
                AtualizarDead();
                break;
        }
    }

    void MoverEmDirecao(Vector3 alvo)
    {
        Vector3 direcao = (alvo - transform.position).normalized;
        Vector3 velocidadeMovimento = direcao * velocidade * Time.deltaTime;

        controller.Move(velocidadeMovimento);
        transform.LookAt(alvo);
    }

    void AtualizarIdle()
    {
        if (jogador != null && Vector3.Distance(transform.position, jogador.position) <= raioPerseguicao)
        {
            MudarEstado(EstadoChefe.Chase);
        }
    }

    void AtualizarCombate()
    {
        if (vidaAtual < vidaMaxima * 0.2f)
        {
            MudarEstado(EstadoChefe.FleeRecover);
            return;
        }

        if (vidaAtual < vidaMaxima * 0.5f &&
            Time.time - tempoUltimaRecuperacao > cooldownRecuperacao)
        {
            MudarEstado(EstadoChefe.SpecialAttack);
            return;
        }

        float distancia = Vector3.Distance(transform.position, jogador.position);

        if (distancia <= raioMelee)
        {
            MudarEstado(EstadoChefe.AttackMelee);
        }
        else if (distancia <= raioRanged)
        {
            MudarEstado(EstadoChefe.AttackRanged);
        }
        else if (distancia <= raioPerseguicao)
        {
            MudarEstado(EstadoChefe.Chase);
        }
    }

    void AtacarADistancia()
    {
        if (jogador == null || prefabProjetil == null || pontoDeLancamento == null)
            return;

        float distancia = Vector3.Distance(transform.position, jogador.position);

        if (distancia <= raioRanged && distancia > raioMelee)
        {
            GameObject projetil = Instantiate(prefabProjetil, pontoDeLancamento.position, pontoDeLancamento.rotation);
            projetil.transform.LookAt(jogador);

            Rigidbody rb = projetil.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = projetil.transform.forward * 10f;
            }

            DarDanoNoJogador();
        }

        MudarEstado(EstadoChefe.Chase);
    }

    void ReativarAtaque()
    {
        podeAtacar = true;
    }

    void DarDanoNoJogador()
    {
        if (jogador != null)
        {
            PlayerVida playerVida = jogador.GetComponent<PlayerVida>();
            if (playerVida != null)
            {
                playerVida.ReceberDano(15f);
            }
        }
    }

    void AtualizarSpecialAttack()
    {
        if (Time.time - ultimoAtaqueEspecial >= intervaloAtaqueEspecial)
        {
            Debug.Log("CHEFE USA ATAQUE ESPECIAL EM ÁREA!");
            DarDanoNoJogador();
            ultimoAtaqueEspecial = Time.time;
        }

        if (vidaAtual >= vidaMaxima * 0.5f)
        {
            MudarEstado(EstadoChefe.Chase);
        }
        else if (vidaAtual < vidaMaxima * 0.2f)
        {
            MudarEstado(EstadoChefe.FleeRecover);
        }
    }

    void AtualizarFleeRecover()
    {
        if (pontoFuga == null)
        {
            MudarEstado(EstadoChefe.Chase);
            return;
        }

        Vector3 direcao = (pontoFuga.position - transform.position).normalized;
        Vector3 velocidadeMovimento = direcao * velocidade * Time.deltaTime;
        controller.Move(velocidadeMovimento);
        transform.LookAt(pontoFuga.position);

        float distanciaParaPonto = Vector3.Distance(transform.position, pontoFuga.position);

        if (distanciaParaPonto <= 1.5f)
        {
            vidaAtual += taxaRegeneracao * Time.deltaTime;
            vidaAtual = Mathf.Min(vidaAtual, vidaMaxima);
        }
        else
        {
            Debug.Log($"🏃‍♂️ Fugindo... Distância: {distanciaParaPonto:F1}");
        }

        if (jogador != null && Vector3.Distance(transform.position, jogador.position) < 3f)
        {
            MudarEstado(EstadoChefe.Chase);
            return;
        }

        if (distanciaParaPonto <= 1.5f && vidaAtual >= vidaMaxima)
        {
            MudarEstado(EstadoChefe.Chase);
        }
    }

    void AtualizarDead()
    {
        GetComponent<Collider>().enabled = false;
        if (painelMorteChefao != null)
        {
            painelMorteChefao.SetActive(true);
        }
        enabled = false;
    }

    public void ReceberDano(float dano)
    {
        if (estadoAtual == EstadoChefe.Dead) return;

        vidaAtual -= dano;
        vidaAtual = Mathf.Clamp(vidaAtual, 0, vidaMaxima);
    }

    void MudarEstado(EstadoChefe novoEstado)
    {
        Debug.Log($"[Estado] {estadoAtual} → {novoEstado}");
        estadoAtual = novoEstado;
    }
}