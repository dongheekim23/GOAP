using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachAndOpendoor : ActionBase
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
  float timeToUnlockDoor;

  GameObject hitEffect;
  float maxSpeed;
  public ApproachAndOpendoor(GameObject agent, GameObject door)
  {
    this.door = door;
    meleeCost = 10f;
    this.agent = agent;

    batonDamage = 20f;

    bpf = agent.GetComponent<BasicPathFollowing>();
    pd = agent.GetComponent<PathData>();

    tm = GameObject.Find("GameManager").GetComponent<TilemapSetup>();

    isBatonSwung = false;

    batonDir = new Vector3();

    elapsedTime = 0f;
    timeToUnlockDoor = 1.5f;

    maxSpeed = agent.GetComponent<AgentStats>().maxSpeed;
    textMesh = agent.GetComponent<GOAP_base>().textMesh;
  }

  public override float ComputeCost()
  {
    cost = 0f;

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

    textMesh.text = "Open door with lockpick";
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
      if (distToDoor <= 10f)
      {

        agent.GetComponent<Rigidbody>().velocity *= 0.8f;
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= timeToUnlockDoor)
        {
          elapsedTime = 0f;
          door.GetComponent<AgentStats>().health = 0;
          agent.GetComponent<SecondRobberGOAP>().isBrokeDoor = true;
          GameObject.Destroy(door);
          GameObject.Find("GameManager").GetComponent<GameManager>().convictEscaped = true;
        }
      }
      // When enemy is relatively close, then switch from A* to steering
      // to approach
      else
      {
        steeringVec
          += agent.GetComponent<BasicMovement>().ComputeSeek(door.transform.position, maxSpeed);
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
    return "ApproachAndOpendoor";
  }
}
