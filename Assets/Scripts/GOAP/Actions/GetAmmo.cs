using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAmmo : ActionBase
{
  GameObject nearestAmmo;
  AStar aStar;
  BasicMovement bm;
  AgentStats agentStats;
  AgentCondition agentCondition;
  TilemapSetup tm;

  // TODO : Decide whether to use class constructor or Start()
  public GetAmmo(GameObject agent)
  {
    this.agent = agent;
    nearestAmmo = null;
    aStar = agent.GetComponent<AStar>();
    bm = agent.GetComponent<BasicMovement>();
    agentStats = agent.GetComponent<AgentStats>();
    agentCondition = agent.GetComponent<AgentCondition>();
    GameObject.Find("GameManager").GetComponent<TilemapSetup>();

    textMesh = agent.GetComponent<GOAP_base>().textMesh;
  }

  public override float ComputeCost()
  {
    GameObject[] ammoList = GameObject.FindGameObjectsWithTag("Ammo");

    if (ammoList.Length == 0)
      return 999f;
    
    float minDist = float.MaxValue;

    // Find the nearest ammo from the agent based on A*
    foreach (GameObject ammo in ammoList)
    {
      float distToAmmo
        = aStar.ComputeTotalPathCost(agent.transform.position, ammo.transform.position);

      if (distToAmmo < minDist)
      {
        nearestAmmo = ammo;
        minDist = distToAmmo;
      }
    }

    cost = minDist;
    return cost;
  }

  // Initialize values
  public override void OnStart(ref Vector3 steeringVec)
  {
    GameObject[] ammoList = GameObject.FindGameObjectsWithTag("Ammo");
    float minDist = float.MaxValue;

    // Find the nearest ammo from the agent based on A*
    foreach(GameObject ammo in ammoList)
    {
      float distToAmmo 
        = aStar.ComputeTotalPathCost(agent.transform.position, ammo.transform.position);

      if (distToAmmo < minDist)
      {
        nearestAmmo = ammo;
        minDist = distToAmmo;
      }
    }

    cost = minDist;
    currStatus = ActionStatus.ON_UPDATE;
  }

  // Actual action
  public override void OnUpdate(ref Vector3 steeringVec)
  {
    if (nearestAmmo == null)
    {
      currStatus = ActionStatus.ON_EXIT;
      return;
    }      

    if (agent.GetComponent<PathData>().pathSet == false)
      agent.GetComponent<BasicPathFollowing>().SetNextDestination(nearestAmmo.transform.position);

    //steeringVec += bm.ComputeSeek(nearestAmmo.transform.position, agentStats.maxSpeed);
    steeringVec += agent.GetComponent<BasicPathFollowing>().ComputePathFollowing();

    textMesh.text = "Going for ammo";

    // TODO : Handle it in a script owned by Ammo object
    //if (Vector3.Distance(agent.transform.position, nearestAmmo.transform.position) < 10f)
    //{
    //  agentCondition.ammoCount += 5;

    //  //Destroy(nearestAmmo);
    //  nearestAmmo = null;
    //  currStatus = ActionStatus.ON_EXIT;
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
    return "GetAmmo";
  }
}
