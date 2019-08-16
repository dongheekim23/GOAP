using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetKey: ActionBase
{
    GameObject nearestKey;
    AStar aStar;
    BasicMovement bm;
    AgentStats agentStats;
    AgentCondition agentCondition;
    TilemapSetup tm;

    // TODO : Decide whether to use class constructor or Start()
    public GetKey(GameObject agent, GameObject _key)
    {
        this.agent = agent;
        nearestKey = _key;
        aStar = agent.GetComponent<AStar>();
        bm = agent.GetComponent<BasicMovement>();
        agentStats = agent.GetComponent<AgentStats>();
        agentCondition = agent.GetComponent<AgentCondition>();
        GameObject.Find("GameManager").GetComponent<TilemapSetup>();

        textMesh = agent.GetComponent<GOAP_base>().textMesh;
    }

    public override float ComputeCost()
    {
        if (nearestKey == null)
            return 999f;

        float distToKey = aStar.ComputeTotalPathCost(agent.transform.position, nearestKey.transform.position);
        
        cost = distToKey;
        return cost;
    }

    // Initialize values
    public override void OnStart(ref Vector3 steeringVec)
    {
        float distToKey = aStar.ComputeTotalPathCost(agent.transform.position, nearestKey.transform.position);
        cost = distToKey;
        currStatus = ActionStatus.ON_UPDATE;
    }

    // Actual action
    public override void OnUpdate(ref Vector3 steeringVec)
    {
        if(nearestKey == null)
        {
            currStatus = ActionStatus.ON_EXIT;
            return;
        }

        textMesh.text = "Find lockpick";
        steeringVec += bm.ComputeSeek(nearestKey.transform.position, agentStats.maxSpeed);

        // TODO : Handle it in a script owned by Key object
        //if (Vector3.Distance(agent.transform.position, nearestKey.transform.position) < 10f)
        //{
        //    // TODO : Set Key as True
        //    agentCondition.hasKey = true;

        //    //Destroy(nearestKey);
        //    //nearestKey = null;
        //    GameObject.Destroy(nearestKey);
        //    currStatus = ActionStatus.ON_EXIT;
        //}
    }

    // Reset values
    //public override void OnExit(ref Vector3 steeringVec)
    //{
    //  cost = 0f;

    //  currStatus = ActionStatus.ON_START;
    //}

    public override string GetName()
    {
        return "GetKey";
    }
}
