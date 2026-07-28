using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Range(0,10)]public int velocidade;
    Rigidbody2D rig;

    private Vector2 posicaoMouse; //armazena a posição do mouse
    public Camera cameraDoJogo; //conectar o script ao objeto Main Camera

    //variaveis do tiro
    public GameObject laser; //objeto que será criado
    public Transform disparo; // local onde será criado
    public AudioSource somLazer;
    public GameController controller;
  
    // Start is called before the first frame update
    void Start()
    {
        rig=GetComponent<Rigidbody2D>();
        somLazer=GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        mover();
        disparar();
    }

    void mover(){
        rig.linearVelocity=new Vector2(Input.GetAxis("Horizontal")* velocidade, Input.GetAxis("Vertical") * velocidade); //rig.velocity.y
        //rig.MovePosition(rig.position + (Input.GetAxis("Horizontal"),Input.GetAxis("Vertical")) * velocidade);
        posicaoMouse=cameraDoJogo.ScreenToWorldPoint(Input.mousePosition); //armazena a localização do Mouse na tela
        
        Vector2 distancia=posicaoMouse - rig.position;
        float anguloMira=Mathf.Atan2(distancia.y,distancia.x) * Mathf.Rad2Deg;
        rig.rotation=anguloMira;
    }

    void disparar(){
        if(Input.GetButtonDown("Fire1")){ //Fire1 Ctrl esquerdo e botão esquerdo mouse
            Instantiate(laser,disparo.position,disparo.rotation);
            somLazer.Play();
        }
    }

    void OnTriggerEnter2D(Collider2D bateu){
        if(bateu.gameObject.tag=="x"){
            transform.position= new Vector3(transform.position.x * -0.9f,transform.position.y,transform.position.z);
        }
                if(bateu.gameObject.tag=="y"){
            transform.position= new Vector3(transform.position.x,transform.position.y * -0.9f,transform.position.z);
        }
    }
}
