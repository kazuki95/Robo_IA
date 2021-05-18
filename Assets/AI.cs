using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    public Transform player;
    public Transform bulletSpawn;
    public Slider healthBar;
    public GameObject bulletPrefab;

    NavMeshAgent agent;
    public Vector3 destination; // mover até o destino
    public Vector3 target;      // alvo do destino
    float health = 100.0f;
    float rotSpeed = 5.0f;

    float visibleRange = 80.0f;
    float shotRange = 40.0f;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = shotRange - 5; //tiro
        InvokeRepeating("UpdateHealth", 5, 0.5f);
    }

    void Update()
    {
        //andar com clicks
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        //barra de vida
        healthBar.value = (int)health;
        healthBar.transform.position = healthBarPos + new Vector3(0, 60, 0);
    }

    void UpdateHealth()
    {
        // se vida for menor que 100, health +1
        if (health < 100)
            health++;
    }

    void OnCollisionEnter(Collision col)               
    {
        //caso o alvo for atingido pela bala retira 10 vidas
        if (col.gameObject.tag == "bullet")
        {
            health -= 10;
        }
    }

    [Task]
    public void PickRandomDestination() 
    { 
        //andar randomicamente pela area
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        agent.SetDestination(dest);
        Task.current.Succeed(); 
    }
    [Task] 
    public void MoveToDestination() 
    { 
        //mover até o destino
        if (Task.isInspected) 
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time); 
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending) 
        { 
            Task.current.Succeed();
        } 
    }

}

