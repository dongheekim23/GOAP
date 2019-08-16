using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Escape: ActionBase
{
    GameObject goalPoint;
    AStar aStar;
    BasicMovement bm;
    AgentStats agentStats;
    AgentCondition agentCondition;
    TilemapSetup tm;

    // TODO : Decide whether to use class constructor or Start()
    public A_Escape(GameObject agent, GameObject _goal)
    {
        this.agent = agent;
        goalPoint = _goal;
        aStar = agent.GetComponent<AStar>();
        bm = agent.GetComponent<BasicMovement>();
        agentStats = agent.GetComponent<AgentStats>();
        agentCondition = agent.GetComponent<AgentCondition>();
        GameObject.Find("GameManager").GetComponent<TilemapSetup>();

        textMesh = agent.GetComponent<GOAP_base>().textMesh;
    }

    public override float ComputeCost()
    {
        if (goalPoint == null)
            return 999f;

        float distToKey = aStar.ComputeTotalPathCost(agent.transform.position, goalPoint.transform.position);
        
        cost = distToKey;
        return cost;
    }

    // Initialize values
    public override void OnStart(ref Vector3 steeringVec)
    {
        float distToKey = aStar.ComputeTotalPathCost(agent.transform.position, goalPoint.transform.position);
        cost = distToKey;
        currStatus = ActionStatus.ON_UPDATE;
    }

    // Actual action
    public override void OnUpdate(ref Vector3 steeringVec)
    {
        if (goalPoint == null)
        {
            currStatus = ActionStatus.ON_EXIT;
            return;
        }

        if(agent.tag == "Police")
            textMesh.text = "Going after fugitive";
        else if(agent.tag == "Robber")
            textMesh.text = "Escape from prison";

        if (agent.GetComponent<PathData>().pathSet == false)
        {
            if (cost < 7)
            {
                textMesh.text = "";
                GameObject.Destroy(agent);
            }

            agent.GetComponent<BasicPathFollowing>().SetNextDestination(goalPoint.transform.position);
        }

        //steeringVec += bm.ComputeSeek(nearestAmmo.transform.position, agentStats.maxSpeed);
        steeringVec += agent.GetComponent<BasicPathFollowing>().ComputePathFollowing();
        
    }

    // Reset values
    //public override void OnExit(ref Vector3 steeringVec)
    //{
    //  cost = 0f;

    //  currStatus = ActionStatus.ON_START;
    //}

    public override string GetName()
    {
        return "Escape";
    }
}
