using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NavMeshMovementController : MonoBehaviour
{
    public float maxWanderRadius = 30.0f;
    public float minWanderRadius = 15.0f;
    private float wanderRadius = 0.0f;
    public float minWanderTimer = 0.5f;
    public float maxWanderTimer = 2.0f;
    private float wanderTimer = 0.0f;

    public bool movementEnabled = false;

    // Navmesh path
    private LineRenderer lr;
    public static Vector3[] path = new Vector3[0];

    private Transform target;
    private NavMeshAgent agent;
    private float timer;

    private string hiderTag = "Hider";
    private string seekerTag = "Seeker";
    private string enemyTag;

    public float safeDistance = 15.0f;


    [SerializeField] private GameObject closestEnemy;

    private void OnDrawGizmos()
    {
        
       
        if (gameObject.CompareTag(seekerTag))
        {
            Gizmos.color = new Color(r: 20, g: 00, b: 0, a: 0.2f);
            Gizmos.DrawSphere(transform.position, wanderRadius);

        }
        else
        {
            Gizmos.color = new Color(r: 0, g: 20, b: 0, a: 0.2f);
            Gizmos.DrawSphere(transform.position, safeDistance);
        }
    }


    // Use this for initialization
    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        if (gameObject.CompareTag(seekerTag))
            enemyTag = hiderTag;
        else
        {
            enemyTag = seekerTag;
            minWanderTimer = 0.01f;
            maxWanderTimer = 0.02f;
        }
            
        closestEnemy = FindClosestEnemy();

        movementEnabled = true;
        AdjustPositionOnNavmesh();
    }


//    void DrawPath()
//    {
//        if (path != null && path.Length > 1)
//        {
//            lr.positionCount = path.Length;
//            for (int i = 0; i < path.Length; i++)
//            {
//                lr.SetPosition(i, path[i]);
//            }
//        }
//    }


    void AdjustPositionOnNavmesh()
    {
        if (!OnNavmesh())
        {
            Vector3 navMeshPos = RandomNavSphere(transform.position, 5, -1);
            if (navMeshPos.x < Mathf.Infinity)
            {
                this.transform.position = navMeshPos;
            }
        }
    }

    public GameObject FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }

        return closest;
    }





    Vector3 GetRandomPointNear(Vector3 origin, float minRadius, float maxRadius)
    {
        wanderRadius = Random.Range(minRadius, maxRadius);
        Vector3 newPos = RandomNavSphere(origin, wanderRadius, -1);
        return newPos;
    }

    void DriftTowardsEnemy()
    {
        Vector3 newPos = GetRandomPointNear(closestEnemy.transform.position,
            minWanderRadius, maxWanderRadius);
        MoveToPoint(newPos);
    }

    void FleeEnemy()
    {
        float OrigDistance =
            Vector3.Distance(transform.position, closestEnemy.transform.position);

        Vector3 dirtoPlayer = transform.position - closestEnemy.transform.position;
        Vector3 moveaway = transform.position + dirtoPlayer;

        float maxDist = OrigDistance;
        Vector3 furthestPt = new Vector3();

        for (int i = 0; i < 10; i++)
        {
            Vector3 newPos = GetRandomPointNear(transform.position, 3, maxWanderRadius);
            float distance =
                Vector3.Distance(newPos, closestEnemy.transform.position);
            if (distance > maxDist)
            {
                maxDist = distance;
                furthestPt = newPos;
            }
        }

        MoveToPoint(furthestPt);     
        
//        if (distance < 10.0f)
//        {
//            MoveToPoint(moveaway);
//        }
//        else
//        {
//            Vector3 newPos = GetRandomPointNear(moveaway, 5, 10);
//            MoveToPoint(newPos);
//        }
    }


    void MoveToPoint(Vector3 newPos)
    {
        agent.SetDestination(newPos);
        Debug.DrawRay(newPos, Vector3.up, Color.blue, 1.0f);
        timer = 0;
        wanderTimer = Random.Range(minWanderTimer, maxWanderTimer);
    }

    void Wander()
    {
        Vector3 newpos = GetRandomPointNear(transform.position, minWanderRadius,
            maxWanderRadius);
        MoveToPoint(newpos);
    }

    void Move()
    {
        timer += Time.deltaTime;
        if (timer >= wanderTimer)
        {
            if (closestEnemy != null)
            {
                if (gameObject.CompareTag(seekerTag))
                {
                    DriftTowardsEnemy();
                }
                else
                {
                    FleeEnemy();
                }
            }
            else
            {
                Wander();
            }

            FindClosestEnemy();
        }

//        DrawPath();
    }

    void Update()
    {
        if (movementEnabled)
            Move();
    }


    private bool OnNavmesh()
    {
        NavMeshHit navHit;
        return NavMesh.SamplePosition(transform.position, out navHit, 0.5f, -1);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}