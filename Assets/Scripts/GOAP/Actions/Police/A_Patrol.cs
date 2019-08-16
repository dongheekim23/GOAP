using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Patrol : ActionBase
{
  BasicPathFollowing bpf;
  PathData pd;
  TilemapSetup tm;

  List<Vector3> patrolPoints;

  GameManager gm;

  int patrolIt;

  public A_Patrol(GameObject agent)
  {
    base.BaseStart();

    this.agent = agent;

    bpf = agent.GetComponent<BasicPathFollowing>();
    pd = agent.GetComponent<PathData>();

    textMesh = agent.GetComponent<GOAP_base>().textMesh;

    tm = GameObject.Find("GameManager").GetComponent<TilemapSetup>();

    float agentHeight = agent.GetComponent<Renderer>().bounds.size.y / 2;

    patrolPoints = new List<Vector3>();
    patrolPoints.Add(tm.GridToWorld(new GridPos(14, 28), agentHeight));
    patrolPoints.Add(tm.GridToWorld(new GridPos(28, 18), agentHeight));
    patrolPoints.Add(tm.GridToWorld(new GridPos(14, 2), agentHeight));
    patrolPoints.Add(tm.GridToWorld(new GridPos(3, 10), agentHeight));

    gm = GameObject.Find("GameManager").GetComponent<GameManager>();

    patrolIt = Random.Range(0, 3);
  }

  public override float ComputeCost()
  {
    return cost;
  }

  public override void OnStart(ref Vector3 steeringVec)
  {
    //bpf.SetNextDestination(patrolPoints[Random.Range(0, patrolPoints.Count - 1)]);
    bpf.SetNextDestination(patrolPoints[patrolIt]);

    if (patrolIt == 3)
      patrolIt = 0;
    else
      ++patrolIt;

    currStatus = ActionStatus.ON_UPDATE;
  }

  public override void OnUpdate(ref Vector3 steeringVec)
  {
    steeringVec += bpf.ComputePathFollowing();

    textMesh.text = "Patrol";

    if (pd.pathSet == false)
      currStatus = ActionStatus.ON_EXIT;
    else if(gm.convictEscaped == true)
    {
      bpf.ResetPath();
      currStatus = ActionStatus.ON_EXIT;
    }
  }

  public override string GetName()
  {
    return "A_Patrol";
  }
}
