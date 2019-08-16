using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachAndSmashdoor : ActionBase
{
  GameObject door;
  float meleeCost;

  float batonDamage;

  BasicPathFollowing bpf;
  PathData pd;

  TilemapSetup tm;

  bool isBatonSwung;

  Vector3 batonDir;

  float elapsedTime;
  float coolTime;

  GameObject hitEffect;
  float maxSpeed;
  public ApproachAndSmashdoor(GameObject agent, GameObject door)
  {
    this.door = door;
    meleeCost = 10f;
    this.agent = agent;

    batonDamage = 10f;

    bpf = agent.GetComponent<BasicPathFollowing>();
    pd = agent.GetComponent<PathData>();

    tm = GameObject.Find("GameManager").GetComponent<TilemapSetup>();

    isBatonSwung = false;

    batonDir = new Vector3();

    elapsedTime = 0f;
    coolTime = 1f;

    maxSpeed = agent.GetComponent<AgentStats>().maxSpeed;
    textMesh = agent.GetComponent<GOAP_base>().textMesh;
  }

  public override float ComputeCost()
  {
    cost = 10f;

    cost += agent.GetComponent<AStar>().ComputeTotalPathCost(agent.transform.position, door.transform.position);

    return cost;
  }

  public override void OnStart(ref Vector3 steeringVec)
  {
    bpf.SetNextDestination(door.transform.position);

    currStatus = ActionStatus.ON_UPDATE;
  }

  // Actual action
  public override void OnUpdate(ref Vector3 steeringVec)
  {
    // Action ends when the enemy is dead
    if (door == null || door.tag == "Door")
    {
      if (hitEffect != null)
        MonoBehaviour.Destroy(hitEffect);

      currStatus = ActionStatus.ON_EXIT;
      return;
    }
    textMesh.text = "Break Door";
    float distToDoor = Vector3.Distance(agent.transform.position, door.transform.position);

    // Approach door
    if (distToDoor > 30f)
    {
      if (pd.pathSet == false)
        bpf.SetNextDestination(door.transform.position);

      steeringVec += bpf.ComputePathFollowing();
    }
    else
    {
      if (pd.pathSet == true)
        bpf.ResetPath();

      // If the door is really close
      if (distToDoor <= 8f)
      {
        // Slow down the agent
        agent.GetComponent<Rigidbody>().velocity *= 0.8f;

        if (isBatonSwung == false)
        {
          Vector3 dir = ((door.transform.position) - agent.transform.position).normalized;

          Vector3 initPos = agent.transform.position
            + dir * 1.3f * (agent.GetComponent<Renderer>().bounds.size.x / 2);

          initPos.y = 10f;

          hitEffect = GameObject.Instantiate(tm.HitEffect, initPos, Quaternion.identity);

          //GameObject newBaton
          //  = GameObject.Instantiate(tm.Baton, initPos, Quaternion.identity);

          //newBaton.transform.rotation = Quaternion.LookRotation(-agent.transform.up);

          door.GetComponent<AgentStats>().health -= batonDamage;
          if (door.GetComponent<AgentStats>().health <= 0f)
          {
            agent.GetComponent<SecondRobberGOAP>().isBrokeDoor = true;
            GameObject.Destroy(door);
            GameObject.Find("GameManager").GetComponent<GameManager>().convictEscaped = true;
          }

          isBatonSwung = true;
          batonDir = dir;
        }

        // Make the agent look toward the bullet direction
        agent.transform.rotation = Quaternion.LookRotation(batonDir);
      }
      // When enemy is relatively close, then switch from A* to steering
      // to approach
      else
      {
        steeringVec
          += agent.GetComponent<BasicMovement>().ComputeSeek(door.transform.position, maxSpeed);
      }
    }

    // Give cooltime for smash
    if (isBatonSwung == true)
    {
      elapsedTime += Time.deltaTime;

      if (elapsedTime >= 0.3f && hitEffect != null)
      {
        MonoBehaviour.Destroy(hitEffect);
        hitEffect = null;
      }
      else if (elapsedTime >= coolTime)
      {
        isBatonSwung = false;
        elapsedTime = 0f;
      }
    }
  }

  // Reset values
  //public override void OnExit(ref Vector3 steeringVec)
  //{
  //  cost = 0f;

  //  currStatus = ActionStatus.ON_START;
  //}

  public override string GetName()
  {
    return "ApproachAndSmashdoor";
  }
}
