using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA;

public class IA_Manager : MonoBehaviour {

    List<GameObject> nodes;
    List<GameObject> agentesIA;
    List<GameObject> alvosIA;
    SteeringBehaviours sb = new SteeringBehaviours();


    // Use this for initialization
    void Start()
    {

        //Carrega os agentes e alvos nas listas
        agentesIA = CarregarAgentes(tiposDeAgentes.Agente);
        alvosIA = CarregarAgentes(tiposDeAgentes.Alvo);
        nodes = CarregarNodes();

    }

    // Update is called once per frame
    void Update()
    {

        BehaviourFlow(agentesIA, alvosIA, nodes);
        BehaviourFlow(alvosIA, agentesIA, nodes);

    }




    public void BehaviourFlow(List<GameObject> listAgente, List<GameObject> listAlvo, List<GameObject> nodes)
    {

        foreach (GameObject agente in listAgente)
        {

            Steering steering = new Steering();
            for (int i = 0; i < agente.GetComponent<IA_Client>().acao.Length; i++)
            {
                GameObject alvo = listAlvo[0];
                if (listAlvo.Count > 1)
                    for (int j = 0; j < listAlvo.Count - 1; j++)
                    {
                        if (Vector3.Distance(agente.transform.position, listAlvo[j].transform.position) < Vector3.Distance(agente.transform.position, alvo.transform.position))
                            alvo = listAlvo[j];
                    }

                switch (agente.GetComponent<IA_Client>().acao[i])
                {
                    case Acoes.Flee:
                        steering = sb.Flee(agente, alvo);
                        break;
                    case Acoes.Arrive:
                        steering = sb.Arrival(agente, alvo);
                        break;
                    case Acoes.Pursue:
                        steering = sb.Pursue(agente, alvo);
                        break;
                    case Acoes.Avoid:
                        steering = sb.Avoid(agente, alvo);
                        break;
                    case Acoes.AvoidAgent:
                        steering = sb.AvoidAgent(agente, listAgente);
                        break;
                    case Acoes.PathFollower:
                        steering = sb.PathFollower(agente, nodes);
                        break;

                    case Acoes.Seek:
                    default:
                        steering = sb.Seek(agente, alvo);
                        break;

                }

                agente.GetComponent<IA_Client>().steering = steering;
                Debug.DrawRay(agente.transform.position, steering.direcao, Color.red);
                agente.transform.rotation = Quaternion.Slerp(agente.transform.rotation, Quaternion.LookRotation(steering.direcao), Time.deltaTime * agente.GetComponent<IA_Client>().velocidadeRotacao);
                agente.transform.Translate(0, 0, steering.velocidade * Time.deltaTime);

            }
        }

    }


    //metodo que carrega os agentes em listas caso eles tenham o componente InteligenciaArtificial_cliente e
    //conforme o tipo de agente
    public List<GameObject> CarregarAgentes(tiposDeAgentes tda)
    {

        GameObject[] tempAgentes; //declaracao de array temporario de agentes 
        List<GameObject> returnAgentes = new List<GameObject>(); //declaração da lista que sera retornada
        tempAgentes = FindObjectsOfType(typeof(GameObject)) as GameObject[]; //carrega todos os gameobjects num array
        foreach (GameObject agt in tempAgentes)
        { //para cada gameobject na lista 
            if (!(agt.GetComponent(typeof(IA_Client)) == null))
            { // testamos se ele tem o script client
                if (agt.GetComponent<IA_Client>().tipoDeAgente == tda)
                { //se tiver testamos se é igual ao tipo informado como parametro
                    returnAgentes.Add(agt); //se for igual adicionamo-lo à lista temporaria
                }
            }
        }
        return returnAgentes; // retornamos a lista temporaria
    }

    public List<GameObject> CarregarNodes()
    {
        GameObject[] tempNodes = GameObject.FindGameObjectsWithTag("Node");
        List<GameObject> nodes = new List<GameObject>(); //declaração da lista que sera retornada
        foreach (GameObject go in tempNodes)
        {

            nodes.Add(go);


        }
        Debug.Log(nodes.Count);
        return nodes; // retornamos a lista 
    }
}
