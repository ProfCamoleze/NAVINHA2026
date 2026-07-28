using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float velocidade; //define a velocidade do tiro
    public float tempoDeVida; //define quanto tempo vai durar o tiro
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject,tempoDeVida);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * velocidade * Time.deltaTime);
    }
}
