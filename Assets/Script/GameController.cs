using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameController : MonoBehaviour
{   
    [Header("Asteroide")]
    //variaveis para o astereid
    public Transform[] pontoOrigem; //o colchetes gera um conjunto de dados do mesmo tipo , inicia com 0 zero 
    public GameObject asteroid;
    //variveis que definirão o intervalo de criação
    public float timer; //armazena o tempo do ultimo objeto Criado
    public float intervaloTempo; // vamos definir o intervalo entre a criação dos objetos
    [Header("Pontuação")]
    public int pontos; //armazenar os pontos
    public TextMeshProUGUI txtPontos; // exibir a pontuação na tela, using TMPro
    // Start is called before the first frame update
    void Start()
    {
        timer=intervaloTempo;
    }

    // Update is called once per frame
    void Update()
    {
        criaAsteroides();
    }
    void criaAsteroides(){
        timer -= Time.deltaTime; // contagem regressiva do tempo
        if (timer <=0){ // verifica se acabou o tempo
            int pontoAleatorio=Random.Range(0, pontoOrigem.Length -1); // cria um numero aleatorio entre zero e posição do array
            Instantiate(asteroid, pontoOrigem[pontoAleatorio].position, pontoOrigem[pontoAleatorio].rotation); // criando nosso asteroid
            timer=intervaloTempo; // reset do valor do time , para o valor original
        }
    }
     public void recebePontos(int recebe){
        pontos +=recebe;
        txtPontos.text="Pontos : " + pontos;
     }

}
