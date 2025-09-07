using UnityEngine;

public class ProjetilMagico : MonoBehaviour
{
    public float velocidade = 10f;
    public float dano = 25f;
    public float tempoVida = 5f;

    private void Start()
    {
        Destroy(gameObject, tempoVida);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * velocidade * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerVida player = other.GetComponent<PlayerVida>();
        if (player != null)
        {
            player.ReceberDano(dano);
        }

        Destroy(gameObject);
    }
}