using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachAndShoot : ActionBase
{
  GameObject detectedEnemy;
  AStar aStar;
  BasicPathFollowing bpf;
  PathData pd;
  float shootingCost;

  TilemapSetup tm;

  bool isGunFired;

  Vector3 bulletDir;

  float elapsedTime;
  float coolTime;

  public ApproachAndShoot(GameObject agent, GameObject detectedEnemy)
  {
    this.detectedEnemy = detectedEnemy;
    aStar = agent.GetComponent<AStar>();
    shootingCost = 5f;
    this.agent = agent;

    bpf = agent.GetComponent<BasicPathFollowing>();
    pd = agent.GetComponent<PathData>();

    tm = GameObject.Find("GameManager").GetComponent<TilemapSetup>();

    isGunFired = false;

    bulletDir = new Vector3();

    elapsedTime = 0f;
    coolTime = 2f;

    textMesh = agent.GetComponent<GOAP_base>().textMesh;
  }

  public override float ComputeCost()
  {
    cost = 0f;

    if (detectedEnemy.GetComponent<AgentCondition>().hasBaton)
      cost += 5f;

    if (detectedEnemy.GetComponent<AgentCondition>().ammoCount > 0)
      cost += 5f * detectedEnemy.GetComponent<AgentCondition>().ammoCount;

    cost += agent.GetComponent<AStar>().ComputeTotalPathCost(agent.transform.position, detectedEnemy.transform.position) / 2f;

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
    // or out of ammo
    if (detectedEnemy == null || detectedEnemy.tag == "Prisoner"
      || agent.GetComponent<AgentCondition>().ammoCount == 0)
    {
      currStatus = ActionStatus.ON_EXIT;
      return;
    }

    // Approach enemy
    if (Vector3.Distance(agent.transform.position, detectedEnemy.transform.position) > 40f)
    {
      if (pd.pathSet == false)
        bpf.SetNextDestination(detectedEnemy.transform.position);

      steeringVec += bpf.ComputePathFollowing();

      textMesh.text = "Chasing";
    }
    // Shoot enemy
    else
    {
      if (pd.pathSet == true)
        bpf.ResetPath();

      // Slow down the agent
      agent.GetComponent<Rigidbody>().velocity *= 0.92f;

      if (isGunFired == false && agent.GetComponent<AgentCondition>().ammoCount > 0)
      {
        Vector3 dir
          = ((detectedEnemy.transform.position + detectedEnemy.GetComponent<Rigidbody>().velocity * Time.deltaTime) - agent.transform.position).normalized;

        Vector3 initPos = agent.transform.position
          + dir * 1.5f * (agent.GetComponent<Renderer>().bounds.size.x / 2);

        float bulletSpeed = tm.Bullet.GetComponent<BulletStats>().speed;

        GameObject newBullet
          = GameObject.Instantiate(tm.Bullet, initPos, Quaternion.identity);
        newBullet.GetComponent<Rigidbody>().velocity = dir * bulletSpeed;

        isGunFired = true;
        bulletDir = dir;

        --agent.GetComponent<AgentCondition>().ammoCount;

        textMesh.text = "Shooting";
      }

      // Make the agent look toward the bullet direction
      agent.transform.rotation = Quaternion.LookRotation(bulletDir);
    }

    // Give cooltime for gunfire
    if (isGunFired == true)
    {
      elapsedTime += Time.deltaTime;

      if (elapsedTime >= coolTime)
      {
        isGunFired = false;
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
    return "ApproachAndShoot";
  }
}
