using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_GoBackToStation : ActionBase
{
  BasicPathFollowing bpf;
  PathData pd;
  Vector3 originPos;

  public A_GoBackToStation(GameObject agent)
  {
    base.BaseStart();

    this.agent = agent;

    bpf = agent.GetComponent<BasicPathFollowing>();
    pd = agent.GetComponent<PathData>();

    originPos = agent.GetComponent<CopGOAP>().originPos;

    textMesh = agent.GetComponent<GOAP_base>().textMesh;
  }

  public override float ComputeCost()
  {
    float distToGoal = Vector3.Distance(agent.transform.position, originPos);

    cost = distToGoal > 5 ? -distToGoal : 999f;

    return cost;
  }

  public override void OnStart(ref Vector3 steeringVec)
  {
    bpf.SetNextDestination(originPos);

    currStatus = ActionStatus.ON_UPDATE;
  }

  public override void OnUpdate(ref Vector3 steeringVec)
  {
    steeringVec += bpf.ComputePathFollowing();

    textMesh.text = "Going back to station";

    if (pd.pathSet == false)
      currStatus = ActionStatus.ON_EXIT;
  }

  public override string GetName()
  {
    return "A_GoBackToStation";
  }
}
