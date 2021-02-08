using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public enum AgentControllers
{
    human = 0,
    navmesh = 1,
    mlagent = 2
}

public class CharacterController : MonoBehaviour
{
    public bool playerControlled;

    NavMeshMovementController navMeshMovementController;
    UserMovementController userMovementController;
    NavMeshAgent agent;
    private FieldOfView fov;
    
    
    public float speed;

    public void OnEnable()
    {
        if (gameObject.CompareTag("Seeker"))
        {
            fov = GetComponent<FieldOfView>();
            fov.viewRadius = 0;
        }

        
    }


    public void turnMovementOff()
    {
        Debug.Log("Disabling movement for " + gameObject.name);
        if (agent != null)
        {
            agent.ResetPath();
            Destroy(agent);
        }

        if (userMovementController != null)
        {
            Destroy(userMovementController);
        }

        if (navMeshMovementController != null)
        {
            Destroy(navMeshMovementController);
        }


    }


    public void SetPlayerCotrollerStatus(bool userControlled)
    {
        if (userControlled)
        {
            userMovementController = gameObject.AddComponent<UserMovementController>();
            userMovementController.moveSpeed = speed;
            userMovementController.movementEnabled = false;

        }
        else
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            agent.speed = speed;
            navMeshMovementController =
                gameObject.AddComponent<NavMeshMovementController>();
            navMeshMovementController.movementEnabled = false;

        }

        playerControlled = userControlled;
    }

    public void turnMovementOn()
    {
        if (playerControlled)
        {
            Debug.Log("Enabling player movement for " + gameObject.name);
            userMovementController.movementEnabled = true;
        }
        else
        {
            Debug.Log("Enabling NavMesh movement for " + gameObject.name);
            navMeshMovementController.movementEnabled = true;

        }

        if (fov != null)
        {
            fov.viewRadius = 10;
        }
    }
}