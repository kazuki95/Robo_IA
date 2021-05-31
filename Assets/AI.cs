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
    //brra de vida
    public Slider healthBar;   
    //prefab da bala
    public GameObject bulletPrefab;

    NavMeshAgent agent;
    // o destino da movimentação
    public Vector3 destination; 
    // a posição do alvo
    public Vector3 target;   
    //valor da vida
    float health = 100.0f;
    //velocidade da rotação
    float rotSpeed = 5.0f;


    //limite do alcance
    float visibleRange = 80.0f;
    //alcance do tiro
    float shotRange = 40.0f;

    void Start()
    {
        // utilizando o component navmesh do objeto
        agent = this.GetComponent<NavMeshAgent>(); 
        agent.stoppingDistance = shotRange - 5; 
        InvokeRepeating("UpdateHealth", 5, 0.5f);
    }

    void Update()
    {
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        //Barra da vida ser equivalente a vida dentro dela
        healthBar.value = (int)health;
        healthBar.transform.position = healthBarPos + new Vector3(0, 60, 0);
    }

    void UpdateHealth()
    {
    //adicionar vida se for menor que 100
        if (health < 100)
            health++;
    }

    void OnCollisionEnter(Collision col)
    {
        //Tira 10 de vida  ao colidir com o objeto que tenha a tag bullet 
        if (col.gameObject.tag == "bullet")
        {
            health -= 10;
        }
    }
    [Task]
    public void PickRandomDestination()
    {
        //Randomizar destino 
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        // dando o destino do objeto, com o destino randomico
        agent.SetDestination(dest); 
        Task.current.Succeed();
    }
    [Task]
    public void MoveToDestination()
    {
        
        if (Task.isInspected)
            //Mostrar quanto tempo levou para executar
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }
    [Task]
    public void PickDestination(int x, int z)
    {
        //Checando posição do destino
        Vector3 dest = new Vector3(x, 0, z);
        //Indo para a posição de destino
        agent.SetDestination(dest);
        Task.current.Succeed();
    }
    [Task]
    public void TargetPlayer()
    {
        //Pegando a posição do player 
        target = player.transform.position;
        Task.current.Succeed();
    }
    [Task]
    public bool Fire()
    {
        //Instanciando as balas com o prefab
        GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        //A velocidade da bala
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);
        return true;
    }
    [Task]
    public void LookAtTarget()
    {
        //Calculando direção do alvo
        Vector3 direction = target - this.transform.position;
        //Direcionando a posição para o alvo
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
        if (Task.isInspected)
        //Mover até o alvo
            Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(this.transform.forward, direction));
        if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
        {
            Task.current.Succeed();
        }
    }
    [Task]
    bool SeePlayer()
    {
        //Calculando ditancia entre ele e o alvo
        Vector3 distance = player.transform.position - this.transform.position;
        //Raycast para identificar colisao
        RaycastHit hit;
        bool seeWall = false;
        Debug.DrawRay(this.transform.position, distance, Color.red);
        //Checar se está colidindo com a parede 
        if (Physics.Raycast(this.transform.position, distance, out hit))
        {
            
            if (hit.collider.gameObject.tag == "wall")
            {
                seeWall = true;
            }
        }
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("wall={0}", seeWall);
        
        if (distance.magnitude < visibleRange && !seeWall)
            return true;

        else return false;
    }
    [Task]
    //Rotação do objeto 
    bool Turn(float angle) 
    {
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;
        target = p;
        return true;
    }
}