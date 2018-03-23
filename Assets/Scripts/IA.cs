using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IA
{

    //tipos de agentes determinados nos clientes               
    public enum tiposDeAgentes { Agente, Alvo, Obstaculo };


    //Ações Steering Behaviours disponiveis
    public enum Acoes { Seek, Flee, Pursue, Avade, Arrive, Leave, Wander, PathFollower, AvoidAgent, Avoid };

    public struct Steering
    {
        public Vector3 direcao;
        public float velocidade;

    };



    public class SteeringBehaviours
    {



        public Steering Seek(GameObject agente, GameObject alvo)
        {
            Steering newSteering = new Steering();

            Vector3 posicaoAlvo = new Vector3(alvo.transform.position.x,
                                            agente.transform.position.y,
                                            alvo.transform.position.z);

            Vector3 direcao = posicaoAlvo - agente.transform.position;
            newSteering.direcao = Vector3.Normalize(direcao);

            if (Vector3.Distance(agente.transform.position, posicaoAlvo) > agente.GetComponent<IA_Client>().distanciaMaxima)
                newSteering.velocidade = agente.GetComponent<IA_Client>().velocidade;

            return newSteering;

        }

        public Steering Flee(GameObject agente, GameObject alvo)
        {

            Steering newSteering = new Steering();

            Vector3 posicaoAlvo = new Vector3(alvo.transform.position.x,
                                            agente.transform.position.y,
                                            alvo.transform.position.z);

            Vector3 direcao = agente.transform.position - posicaoAlvo;
            newSteering.direcao = Vector3.Normalize(direcao);
            newSteering.velocidade = agente.GetComponent<IA_Client>().velocidade;

            return newSteering;

        }

        public Steering Arrival(GameObject agente, GameObject alvo)
        {
            Steering newSteering = new Steering();

            Vector3 posicaoAlvo = new Vector3(alvo.transform.position.x,
                                            agente.transform.position.y,
                                            alvo.transform.position.z);

            newSteering.direcao = Vector3.Normalize(posicaoAlvo - agente.transform.position);


            float distancia = Vector3.Distance(agente.transform.position, posicaoAlvo);

            if (distancia > agente.GetComponent<IA_Client>().distanciaFrenagem)
            {
                newSteering.velocidade = agente.GetComponent<IA_Client>().velocidade;
            }
            else if (distancia > agente.GetComponent<IA_Client>().distanciaMaxima)
            {

                newSteering.velocidade = agente.GetComponent<IA_Client>().velocidade * ((distancia - agente.GetComponent<IA_Client>().distanciaMaxima) / (agente.GetComponent<IA_Client>().distanciaFrenagem - agente.GetComponent<IA_Client>().distanciaMaxima));

            }

            return newSteering;
        }

        public Steering Pursue(GameObject agente, GameObject alvo)
        {

            Steering steering = new Steering();

            steering = alvo.GetComponent<IA_Client>().steering;


            Vector3 direction = alvo.transform.position - agente.transform.position;
            float distance = direction.magnitude;
            float speed = agente.GetComponent<IA_Client>().velocidade;
            float prediction;

            if (speed <= distance / agente.GetComponent<IA_Client>().previsaoMax)
                prediction = agente.GetComponent<IA_Client>().previsaoMax;
            else
                prediction = distance / speed;

            steering.direcao = alvo.transform.position + (steering.direcao * prediction);
            Vector3 posicaoAlvo = new Vector3(steering.direcao.x,
                                            agente.transform.position.y,
                                            steering.direcao.z);

            Vector3 direcao = Vector3.Normalize(posicaoAlvo - agente.transform.position);
            steering.direcao = direcao;
            steering.velocidade = agente.GetComponent<IA_Client>().velocidade;


            return steering;

        }

        public Steering Avoid(GameObject agente, GameObject alvo)
        {

            Steering steering = new Steering();
            Vector3 posicaoAlvo = new Vector3();
            Vector3 position = agente.transform.position;
            float lookAhead = agente.GetComponent<IA_Client>().olharAFrente;
            Vector3 rayVector = agente.transform.forward * lookAhead;
            Vector3 direction = rayVector;
            RaycastHit hit;

            if (Physics.Raycast(position, direction, out hit, lookAhead))
            {
                Debug.Log(hit.transform.gameObject.GetComponent<IA_Client>().tipoDeAgente);
                if (!(hit.transform.gameObject.GetComponent(typeof(IA_Client)) == null))
                {

                    if (hit.transform.gameObject.GetComponent<IA_Client>().tipoDeAgente == tiposDeAgentes.Obstaculo)
                    {
                        Debug.Log("Hit!");
                        position = hit.point + hit.normal * agente.GetComponent<IA_Client>().distanciaDeDesvio;
                        posicaoAlvo = new Vector3(position.x,
                                            agente.transform.position.y,
                                            position.z);

                        Vector3 direcao = Vector3.Normalize(posicaoAlvo - agente.transform.position);
                        steering.direcao = direcao;

                    }
                    else
                    {
                        posicaoAlvo = new Vector3(alvo.transform.position.x,
                                 agente.transform.position.y,
                                 alvo.transform.position.z);
                    }

                    steering.direcao = Vector3.Normalize(posicaoAlvo - agente.transform.position);

                }


            }

            if (steering.direcao == Vector3.zero)
                steering.direcao = agente.transform.forward;

            if (Vector3.Distance(agente.transform.position, posicaoAlvo) > agente.GetComponent<IA_Client>().distanciaMaxima)
                steering.velocidade = agente.GetComponent<IA_Client>().velocidade;


            return steering;



        }

        public Steering AvoidAgent(GameObject agente, List<GameObject> agentes)
        {

            Steering steering = new Steering();
            float shortestTime = Mathf.Infinity;
            GameObject firstTarget = null;
            float firstMinSeparation = 0.0f;
            float firstDistance = 0.0f;
            Vector3 firstRelativePos = Vector3.zero;
            Vector3 firstRelativeVel = Vector3.zero;

            steering = agente.GetComponent<IA_Client>().steering;

            foreach (GameObject a in agentes)
            {

                Vector3 relativePos;
                relativePos = a.transform.position - agente.transform.position;
                Vector3 relativeVel = a.GetComponent<IA_Client>().steering.direcao * a.GetComponent<IA_Client>().steering.velocidade - agente.GetComponent<IA_Client>().steering.direcao * agente.GetComponent<IA_Client>().steering.velocidade;
                float relativeSpeed = relativeVel.magnitude;
                float timeToCollision = Vector3.Dot(relativePos, relativeVel);
                timeToCollision /= relativeSpeed * relativeSpeed * -1;
                float distance = relativePos.magnitude;
                float minSeparation = distance - relativeSpeed * timeToCollision;

                if (minSeparation > 2 * agente.GetComponent<IA_Client>().raioDeColisao)
                    continue;

                if (timeToCollision > 0.0f && timeToCollision < shortestTime)
                {

                    shortestTime = timeToCollision;
                    firstTarget = a;
                    firstMinSeparation = minSeparation;
                    firstRelativePos = relativePos;
                    firstRelativeVel = relativeVel;

                }

            }

            if (firstTarget == null)
                return steering;
            if (firstMinSeparation <= 0.0f || firstDistance < 2 * agente.GetComponent<IA_Client>().raioDeColisao)
                firstRelativePos = firstTarget.transform.position;
            else
                firstRelativePos += firstRelativeVel * shortestTime;
            firstRelativePos.Normalize();
            steering.direcao = -firstRelativePos;
            return steering;

        }

        public Steering PathFollower(GameObject agente, List<GameObject> nodes)
        {
            if (nodes.Count == 0) return agente.GetComponent<IA_Client>().steering;

            Steering steering = new Steering();

            int noAtual = agente.GetComponent<IA_Client>().noAtual;
            
            Vector3 olheParaAlvo = new Vector3(nodes[noAtual].transform.position.x, agente.transform.position.y, nodes[noAtual].transform.position.z);

            Vector3 direcao = olheParaAlvo - agente.transform.position;
   
            if(direcao.magnitude < agente.GetComponent<IA_Client>().distanciaMaxima)
            {
                noAtual++;

                if (noAtual >= nodes.Count)
                    noAtual = 0;

            }


            agente.GetComponent<IA_Client>().noAtual = noAtual;

            steering.direcao = Vector3.Normalize(direcao);

            steering.velocidade = agente.GetComponent<IA_Client>().velocidade;

            return steering;
        }


    }


}
