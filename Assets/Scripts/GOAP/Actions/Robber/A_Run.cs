using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Run : ActionBase
{
  AgentCondition ac;
  BasicPathFollowing bpf;
  PathData pd;

  GameObject[] copList;

  public A_Run(GameObject agent)
  {
    base.BaseStart();

    this.agent = agent;
    ac = agent.GetComponent<AgentCondition>();
    bpf = agent.GetComponent<BasicPathFollowing>();
    pd = agent.GetComponent<PathData>();

    copList = GameObject.FindGameObjectsWithTag("Police");

    textMesh = agent.GetComponent<GOAP_base>().textMesh;
  }

  public override float ComputeCost()
  {
    // Expected distance of running
    cost = 200f;

    if (ac.hasBaton == true)
      cost += 20f;

    if (ac.ammoCount > 0)
      cost += 20f * ac.ammoCount;

    return cost;
  }

  public override void OnStart(ref Vector3 steeringVec)
  {
    bpf.SetNextDestinationRandomly();

    currStatus = ActionStatus.ON_UPDATE;
  }

  public override void OnUpdate(ref Vector3 steeringVec)
  {
    if(pd.path.Count == 0)
    {
      currStatus = ActionStatus.ON_EXIT;
      return;
    }

    bool copAround = false;

    foreach(GameObject cop in copList)
    {
      if(cop != null &&
        Vector3.Distance(cop.transform.position, agent.transform.position) <= 20f)
      {
        copAround = true;
        break;
      }
    }

    // Stay put
    if(copAround)
    {
      if (pd.pathSet == true)
        bpf.ResetPath();

      agent.GetComponent<Rigidbody>().velocity *= 0.8f;

      textMesh.text = "Yield";
    }
    else
    {
      steeringVec += bpf.ComputePathFollowing();

      textMesh.text = "Running away";
    }

    if (pd.pathSet == false)
      currStatus = ActionStatus.ON_EXIT;
  }

  public override string GetName()
  {
    return "A_Run";
  }
}
