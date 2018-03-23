using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA;

public class IA_Client : MonoBehaviour {

    public tiposDeAgentes tipoDeAgente;
    public Acoes[] acao;

    public float velocidade = 2.0f;
    public float velocidadeRotacao = 0.5f;
    public float distanciaMaxima = 1.0f;
    public float distanciaFrenagem = 3.0f;
    public float previsaoMax = 5.0f;
    public float olharAFrente = 3.0f;
    public float distanciaDeDesvio = 1.0f;
    public float raioDeColisao = 0.4f;
    public int noAtual = 0;


    public Steering steering = new Steering();

    private void Start()
    {
        steering.direcao = transform.forward;
        steering.velocidade = 0.0f;

    }


}
