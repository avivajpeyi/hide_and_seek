using System.Collections;
using UnityEngine;
using Unity.MLAgents;

public class MlAgent_Hider: Agent
{
    
    // observation --> data
    // data --> decision
    // decision --> action
    // action --> reward


    public override void OnActionReceived(float[] vectorAction)
    {
        base.OnActionReceived(vectorAction);
    }
}