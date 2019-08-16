using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_ArrestAndBringToPrison : ActionBase
{
  bool robberArrested;
  bool robberBroughtToPrison;
  GameObject robber;

  BasicMovement bm;
  BasicPathFollowing bpf;
  PathData pd;
  AStar aStar;

  TilemapSetup tm;

  GridPos setCellDest;

  bool departed;

  float maxSpeed;

  public A_ArrestAndBringToPrison(GameObject agent, GameObject robber)
  {
    base.BaseStart();

    robberArrested = false;
    robberBroughtToPrison = false;

    this.agent = agent;
    this.robber = robber;

    bm = agent.GetComponent<BasicMovement>();
    bpf = agent.GetComponent<BasicPathFollowing>();
    pd = agent.GetComponent<PathData>();
    aStar = agent.GetComponent<AStar>();

    tm = GameObject.Find("GameManager").GetComponent<TilemapSetup>();

    setCellDest = null;

    departed = false;

    textMesh = agent.GetComponent<GOAP_base>().textMesh;

    maxSpeed = agent.GetComponent<AgentStats>().maxSpeed;
  }

  public override float ComputeCost()
  {
    // If the robber is unarmed, then the cop MUST arrest him
    if(robber.GetComponent<AgentCondition>().ammoCount <= 0
      && robber.GetComponent<AgentCondition>().hasBaton == false)
    {
      return 0f;
    }

    cost = 0f;

    // If the robber has a weapon (either a knife or a gun)
    // the cost should be high. Otherwise, cost should be low.
    if (robber.GetComponent<AgentCondition>().ammoCount > 0)
      cost += robber.GetComponent<AgentCondition>().ammoCount * 20f;

    if (robber.GetComponent<AgentCondition>().hasBaton == true)
      cost += 40f;

    cost += (aStar.ComputeTotalPathCost(agent.transform.position, robber.transform.position)
         + aStar.ComputeTotalPathCost(robber.transform.position, tm.GridToWorld(tm.prisonTileList[0]))) / 2f;

    return cost;
  }

  public override void OnStart(ref Vector3 steeringVec)
  {
    currStatus = ActionStatus.ON_UPDATE;
  }

  public override void OnUpdate(ref Vector3 steeringVec)
  {
    if(robber == null || 
      (robber.tag == "Prisoner" 
      && robber.GetComponent<AgentCondition>().capturer != agent))
    {
      currStatus = ActionStatus.ON_EXIT;
      return;
    }

    // Approach and arrest robber
    if (!robberArrested && !robberBroughtToPrison)
    {
      float distToRobber 
        = Vector3.Distance(agent.transform.position, robber.transform.position);

      //if (departed && !pd.pathSet)//tm.WorldToGrid(agent.transform.position) == tm.WorldToGrid(robber.transform.position))
      if (distToRobber <= 5f)
      {
        textMesh.text = "Captured robber";

        bpf.ResetPath();

        robberArrested = true;
        //pd.pathSet = false;
        departed = false;

        robber.GetComponent<BasicPathFollowing>().ResetPath();

        //MonoBehaviour.Destroy(robber.GetComponent<RobberGOAP>().textObject);
        //MonoBehaviour.Destroy(robber.GetComponent<RobberGOAP>());
        robber.tag = "Prisoner";
        robber.GetComponent<RobberGOAP>().textMesh.text = "Arrested";
        robber.GetComponent<AgentCondition>().capturer = agent;

        return;
      }

      if (/*departed == false && */distToRobber > 60f)
      {
        if(pd.pathSet == false)
          bpf.SetNextDestination(robber.transform.position);
        //departed = true;

        steeringVec += bpf.ComputePathFollowing();
      }
      else
      {
        if (pd.pathSet == true)
          bpf.ResetPath();

        steeringVec += bm.ComputeSeek(robber.transform.position, maxSpeed);
      }

      textMesh.text = "Going after robber";
    }
    // Bringing robber to prison
    else if (robberArrested && !robberBroughtToPrison)
    {
      if (setCellDest != null
        && !pd.pathSet && departed)//tm.WorldToGrid(agent.transform.position) == setCellDest)
      {
        robberBroughtToPrison = true;
        //pd.pathSet = false;
        departed = false;
        return;
      }

      if (departed == false && pd.pathSet == false)
      {
        int cellNum = Random.Range(0, tm.prisonTileList.Count - 1);
        setCellDest = tm.prisonTileList[cellNum];
        bpf.SetNextDestination(tm.GridToWorld(setCellDest));
        departed = true;
      }

      textMesh.text = "Taking robber to prison";

      steeringVec += bpf.ComputePathFollowing();

      // Take the arrest robber as well
      robber.transform.position = agent.transform.position + agent.transform.right * 5f;
    }
    // Robber successfully brought to prison
    else if (robberArrested && robberBroughtToPrison)
    {
      robber.transform.position = agent.transform.position;
      robber.GetComponent<RobberGOAP>().textMesh.text = "Imprisoned";

      currStatus = ActionStatus.ON_EXIT;
    }
  }

  //public override void OnExit(ref Vector3 steeringVec)
  //{
  //  currStatus = ActionStatus.ON_START;
  //}

  public override string GetName()
  {
    return "A_ArrestAndBringToPrison";
  }
}
