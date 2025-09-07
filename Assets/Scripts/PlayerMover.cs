using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public float velocidade = 10f;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        transform.position += move * velocidade * Time.deltaTime;
    }
}