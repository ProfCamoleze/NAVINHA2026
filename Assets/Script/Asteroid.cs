using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    //variaveis de velocidade
    public float velocMin;
    public float velocMax;
    public float velocAtual;
    //variaveis dos componentes dos objetos;
    Rigidbody2D rig; //anexa a fisica do objeto ao script
    Transform acharPlayer;
    //criar uma "ligação" entre Asteroid e GameController
    public GameController controller;

    // Start is called before the first frame update
    void Start()
    {
        rig=GetComponent<Rigidbody2D>();
        acharPlayer=FindAnyObjectByType<Player>().transform;
        velocAtual=Random.Range(velocMin,velocMax);
        //chamar uma função para achar o gameController quando o Prefab for criado
        controller=FindAnyObjectByType<GameController>();
    }
    // Update is called once per frame
    void Update()
    {
        mover();
    }
    void mover(){
        transform.position=Vector2.MoveTowards(transform.position , acharPlayer.position, velocAtual * Time.deltaTime);
    }
    //verifica se o Laser colidiu com o asteroide
    void OnCollisionEnter2D(Collision2D col){ 
        if(col.gameObject.tag=="laser"){ //cuidado a tag tem que ser extamente = ao que vc colocou no Laser
            Destroy(this.gameObject);
            controller.recebePontos(1);
        }
    }
}
