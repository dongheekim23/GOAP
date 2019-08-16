using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachAndMelee : ActionBase
{
  GameObject detectedEnemy;
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

  public ApproachAndMelee(GameObject agent, GameObject detectedEnemy)
  {
    this.detectedEnemy = detectedEnemy;
    meleeCost = 10f;
    this.agent = agent;

    batonDamage = 15f;

    bpf = agent.GetComponent<BasicPathFollowing>();
    pd = agent.GetComponent<PathData>();

    tm = GameObject.Find("GameManager").GetComponent<TilemapSetup>();

    isBatonSwung = false;

    batonDir = new Vector3();

    elapsedTime = 0f;
    coolTime = 1f;

    textMesh = agent.GetComponent<GOAP_base>().textMesh;

    maxSpeed = agent.GetComponent<AgentStats>().maxSpeed;
  }

  public override float ComputeCost()
  {
    cost = 20f;

    if (detectedEnemy.GetComponent<AgentCondition>().hasBaton)
      cost += 10f;

    if (detectedEnemy.GetComponent<AgentCondition>().ammoCount > 0)
      cost += 10f * detectedEnemy.GetComponent<AgentCondition>().ammoCount;

    cost += agent.GetComponent<AStar>().ComputeTotalPathCost(agent.transform.position, detectedEnemy.transform.position);
    
    return cost;
  }

  public override void OnStart(ref Vector3 steeringVec)
  {
    elapsedTime = 0f;

    if (detectedEnemy == null)
    {
      currStatus = ActionStatus.ON_EXIT;
      return;
    }

    bpf.SetNextDestination(detectedEnemy.transform.position);

    currStatus = ActionStatus.ON_UPDATE;
  }

  // Actual action
  public override void OnUpdate(ref Vector3 steeringVec)
  {
    // Action ends when the enemy is dead
    if (detectedEnemy == null || detectedEnemy.tag == "Prisoner")
    {
      if (hitEffect != null)
        MonoBehaviour.Destroy(hitEffect);

      currStatus = ActionStatus.ON_EXIT;
      return;
    }

    float distToEnemy 
      = Vector3.Distance(agent.transform.position, detectedEnemy.transform.position);

    // Approach enemy
    if (distToEnemy > 15f)
    {
      if (pd.pathSet == false)
        bpf.SetNextDestination(detectedEnemy.transform.position);

      steeringVec += bpf.ComputePathFollowing();

      textMesh.text = "Chasing";
    }
    else
    {
      if (pd.pathSet == true)
        bpf.ResetPath();

      // If the enemy is really close
      if(distToEnemy <= 5f)
      {
        // Slow down the agent
        agent.GetComponent<Rigidbody>().velocity *= 0.8f;

        if (isBatonSwung == false)
        {
          Vector3 dir = (detectedEnemy.transform.position - agent.transform.position).normalized;
            //= ((detectedEnemy.transform.position + detectedEnemy.GetComponent<Rigidbody>().velocity * Time.deltaTime) - agent.transform.position).normalized;

          Vector3 initPos = agent.transform.position
            + dir * 1.1f * (agent.GetComponent<Renderer>().bounds.size.x / 2);

          initPos.y = 10f;

          hitEffect 
            = GameObject.Instantiate(tm.HitEffect, initPos, Quaternion.identity);

          //GameObject newBaton
          //  = GameObject.Instantiate(tm.Baton, initPos, Quaternion.identity);

          //newBaton.transform.rotation = Quaternion.LookRotation(-agent.transform.up);
          
          detectedEnemy.GetComponent<AgentStats>().health -= batonDamage;

          isBatonSwung = true;
          batonDir = dir;

          textMesh.text = "Melee";
        }

        // Make the agent look toward the bullet direction
        agent.transform.rotation = Quaternion.LookRotation(batonDir);
      }
      // When enemy is relatively close, then switch from A* to steering
      // to approach
      else
      {
        steeringVec 
          += agent.GetComponent<BasicMovement>().ComputeSeek(detectedEnemy.transform.position, maxSpeed);

        textMesh.text = "Chasing";
      }
    }

    // Give cooltime for gunfire
    if (isBatonSwung == true)
    {
      elapsedTime += Time.deltaTime;

      //if(elapsedTime >= 0.3f && hitEffect != null)
      //{
      //  MonoBehaviour.Destroy(hitEffect);
      //  hitEffect = null;
      //}
      //else 
      if (elapsedTime >= coolTime)
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
    return "ApproachAndMelee";
  }
}
