using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/// <summary>
/// Random spawn.
/// Use a collider to define the spawn area
/// Set the layer to Ignore Raycast
/// </summary>
[RequireComponent(typeof(Collider))]
public class ArenaComponentsManager : MonoBehaviour
{
    public List<GameObject> obstaclePrefabs;

    public List<GameObject> genearatedObstacles;
    public List<GameObject> generatedHiders;
    public List<GameObject> generatedSeekers;
    private Bounds spawnArea;
    public int maxIterations = 30;
    public int maxObstacles = 6;
    public int maxSeekers = 1;
    public int maxHiders = 1;
    public int iterations;
    private MaterialController myMaterialController;
    public NavMeshSurface mySurface;
    public List<GameObject> HiderPrefabs;
    public List<GameObject> SeekerPrefabs;


    void Cleanup(List<GameObject> generatedGo)
    {
        if (generatedGo != null)
        {
            if (generatedGo.Count > 0)
            {
                foreach (var go in generatedGo)
                {
                    Destroy(go);
                }
            }
        }
    }

    public void ResetArenaComponents()
    {
        Cleanup(genearatedObstacles);
        Cleanup(generatedHiders);
        Cleanup(generatedSeekers);
        SetInitReferences();
        GenerateObstacles();
        mySurface.BuildNavMesh();
        GeneratePlayableCharacters();
        if (myMaterialController != null)
        {
            myMaterialController.GetGameobjects();
        }
        Debug.Log("Arena Components placed");
    }

    void SetInitReferences()
    {
        myMaterialController = FindObjectOfType<MaterialController>();
        genearatedObstacles = new List<GameObject>();
        generatedHiders = new List<GameObject>();
        generatedSeekers = new List<GameObject>();
        spawnArea = GetComponent<Collider>().bounds;
    }
    

    void PlaceSingleObject(List<GameObject> prefabList, List<GameObject> generatedList,
        bool navMeshObj = false)
    {
        int prefabIdx = Random.Range(0, prefabList.Count);
        Vector3 randomPosition = new Vector3(
            x: Random.Range(spawnArea.min.x, spawnArea.max.x),
            y: spawnArea.min.y,
            z: Random.Range(spawnArea.min.z, spawnArea.max.z)
        );

        if (navMeshObj)
        {
            NavMeshHit navHit;
            bool pointFound = NavMesh.SamplePosition(randomPosition, out navHit, 5, -1);
            randomPosition = navHit.position;
            randomPosition.y = spawnArea.min.y;
        }


        Vector3 tempPos = randomPosition;
        tempPos.y += 100;
        GameObject tempGo =
            Instantiate(prefabList[prefabIdx], tempPos, Quaternion.identity);
        tempGo.name += " [temp]";
        Bounds goBounds = tempGo.GetComponent<Collider>().bounds;


        if (!IAmOverlapping(position: randomPosition, boundsSize: goBounds.size))
        {
            float angle = 90 * Random.Range(0, 4);

            GameObject go = Instantiate(
                prefabList[prefabIdx],
                position: randomPosition,
                rotation: Quaternion.Euler(0, angle, 0),
                parent: transform.root
            );

            generatedList.Add(go);
        }

        Destroy(tempGo);
    }

    void GenerateObstacles()
    {
        iterations = 0;
        genearatedObstacles = new List<GameObject>();
        while (iterations < maxIterations && genearatedObstacles.Count < maxObstacles)
        {
            PlaceSingleObject(obstaclePrefabs, genearatedObstacles);
            iterations += 1;
        }

        if (iterations == maxIterations)
        {
            Debug.Log("Reached maxItr for ObstacleGeneration");
        }
    }


    void GeneratePlayableCharacters()
    {
        generatedHiders = new List<GameObject>();
        generatedSeekers = new List<GameObject>();
        iterations = 0;
        while (iterations < 100 && generatedHiders.Count < maxHiders)
        {
            PlaceSingleObject(HiderPrefabs, generatedHiders, true);
            iterations += 1;
        }

        iterations = 0;
        while (iterations < 100 && generatedSeekers.Count < maxSeekers)
        {
            PlaceSingleObject(SeekerPrefabs, generatedSeekers, true);
            iterations += 1;
        }

        if (iterations == maxIterations)
        {
            Debug.Log("Reached maxItr for PlayerGeneration");
        }
    }

    public bool IAmOverlapping(Vector3 position, Vector3 boundsSize)
    {
        Bounds boxBounds = new Bounds(position, boundsSize);
        float sqrHalfBoxSize = boxBounds.extents.sqrMagnitude;
        float overlapingSphereRadius = Mathf.Sqrt(sqrHalfBoxSize + sqrHalfBoxSize);


        /* Hoping I have the previous calculation right, move on to finding the nearby colliders */
        Collider[] hitColliders = Physics.OverlapSphere(position, overlapingSphereRadius,
            1 <<
            (int) MyLayers.Obstacles);
        foreach (Collider otherCollider in hitColliders)
        {
            //now we ask each of those gentle colliders if they sens something is within their bounds
            if (otherCollider.bounds.Intersects(boxBounds))
            {
                if (otherCollider != this.GetComponent<Collider>())
                    return (true);
            }
        }

        return false;
    }
}