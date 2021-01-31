using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBoundsGizmo : MonoBehaviour
{
    public float overlapingSphereRadius;
    public bool overlapping;
    private Bounds myBounds;

    
    public Collider[] hitColliders;
    
    private void Start()
    {
        myBounds = GetComponent<Collider>().bounds;
    }

    private void Update()
    {
        overlapping = IAmOverlapping(transform.position, myBounds.size);
    }

    private void OnDrawGizmos()
    {

        
        if (overlapping)
        {
            Gizmos.color = new Color(r:20, g:0,b:0,a:0.2f);
        }
        else
        {
            Gizmos.color = new Color(r:0, g:20,b:0,a:0.2f);   
        }
        
        Gizmos.DrawSphere(transform.position, overlapingSphereRadius);
        
    }
    
    
    
    
    public bool IAmOverlapping(Vector3 position, Vector3 boundsSize)
    {
        Bounds boxBounds = new Bounds(position, boundsSize);
        float sqrHalfBoxSize = boxBounds.extents.sqrMagnitude;
        overlapingSphereRadius = Mathf.Sqrt(sqrHalfBoxSize + sqrHalfBoxSize);
        
        
        /* Hoping I have the previous calculation right, move on to finding the nearby colliders */
        hitColliders = Physics.OverlapSphere(position, overlapingSphereRadius, 1<< (int) MyLayers.Obstacles );
        foreach (Collider otherCollider in hitColliders)
        {
            
            //now we ask each of those gentle colliders if they sens something is within their bounds
            if (otherCollider.bounds.Intersects(boxBounds))
            {
                if (otherCollider!=this.GetComponent<Collider>())
                    return (true);
            }

        }

        return false;
    }
    
}
