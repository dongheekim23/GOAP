using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Idle : ActionBase
{
  float elapsedTime;
  float goalTime;

  AgentCondition ac;

  public A_Idle(GameObject agent, float timer = 5f)
  {
    base.BaseStart();

    this.agent = agent;
    ac = agent.GetComponent<AgentCondition>();

    elapsedTime = 0f;
    goalTime = timer;

    textMesh = agent.GetComponent<GOAP_base>().textMesh;
  }

  public override float ComputeCost()
  {
    cost = 0f;

    if (ac.hasBaton == true)
      cost += 20f;

    if (ac.ammoCount > 0)
      cost += 20f * ac.ammoCount;

    return cost;
  }

  public override void OnStart(ref Vector3 steeringVec)
  {
    currStatus = ActionStatus.ON_UPDATE;
  }

  public override void OnUpdate(ref Vector3 steeringVec)
  {
    agent.GetComponent<Rigidbody>().velocity *= 0.8f;

    elapsedTime += Time.deltaTime;

    if(elapsedTime >= goalTime)
      currStatus = ActionStatus.ON_EXIT;

    textMesh.text = "On idle";
  }

  //public override void OnExit(ref Vector3 steeringVec)
  //{
  //  currStatus = ActionStatus.ON_START;
  //}

  public override string GetName()
  {
    return "A_Idle";
  }
}
