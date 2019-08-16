using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobberGOAP : GOAP_base
{
  Rigidbody rb;

  public BasicPathFollowing bpf;
  public PathData pd;
  public AStar aStar;
  public BasicMovement bm;
  public BasicCollisionAvoidance bca;

  AgentStats agentStats;
  AgentCondition agentCondition;

  GameObject detectedEnemy;

  void Awake()
  {
    base.BaseAwake();
  }

  // Use this for initialization
  void Start()
  {
    base.BaseStart();

    rb = gameObject.GetComponent<Rigidbody>();

    bpf = gameObject.AddComponent<BasicPathFollowing>();
    pd = gameObject.AddComponent<PathData>();
    aStar = gameObject.AddComponent<AStar>();
    bm = gameObject.AddComponent<BasicMovement>();
    bca = gameObject.AddComponent<BasicCollisionAvoidance>();

    steeringVec = new Vector3();

    agentStats = gameObject.GetComponent<AgentStats>();
    agentCondition = gameObject.GetComponent<AgentCondition>();

    detectedEnemy = null;
  }

  // Update is called once per frame
  void Update()
  {
    if (textMesh.text == "Imprisoned")
    {
      Destroy(gameObject.GetComponent<Rigidbody>());
      Destroy(this);
    }

    base.UpdateTextPos();

    if (agentCondition.capturer != null)
    {
      return;
    }
    else
    {
      if(gameObject.tag == "Prisoner")
      {
        gameObject.tag = "Robber";
      }
    }

    ProcessGoalAndAction();

    steeringVec = Vector3.zero;

    actionsForAchievingGoal[0].Run(ref steeringVec);

    //if (pd.pathSet == false)
    //{
    //  bpf.SetNextDestinationRandomly();
    //}

    //Vector3 pathFollowing = bpf.ComputePathFollowing();

    //resultVec += 5 * pathFollowing;

    Vector3 avoidVec = 10f * bca.ComputeCollisionAvoidance();
    steeringVec += avoidVec;

    steeringVec += separationWeight * bm.ComputeSeparation("Robber");

    // Move the agent by adding up vectors
    steeringVec *= 100 * Time.deltaTime;

    steeringVec = Vector3.ClampMagnitude(steeringVec, agentStats.maxForce);
    steeringVec /= rb.mass;
    rb.velocity = Vector3.ClampMagnitude(rb.velocity + steeringVec, agentStats.maxSpeed);

    // To change forward vector of the agent
    if (rb.velocity.magnitude >= 0.01f)
      transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);

    //Debug.Log(rb.velocity);
  }

  public override bool ChangeGoalBasedOnCondition()
  {
    if (pd.pathSet == true)
      bpf.ResetPath();

    GoalBase prevGoal = currGoal;

    //currGoal = new G_Idle(gameObject);

    //if(agentStats.health >= 30f)
    //{
    if (DetectEnemy() != null)
      //&& !(!agentCondition.hasBaton && agentCondition.ammoCount <= 0))
      currGoal = new G_DealWithPolice(gameObject, detectedEnemy);
    //else
    //  currGoal = new Patrol();
    //}
    //else
    //{
    // currGoal = new Retreat();
    //}

    else
      currGoal = new G_Idle(gameObject);

    return prevGoal.GetName() == currGoal.GetName() ? false : true;
  }

  GameObject DetectEnemy(float dist = 50f)
  {
    GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Police");
    //detectedEnemy = null;
    float minDist = float.MaxValue;

    foreach (GameObject enemy in enemyList)
    {
      // Exclude null objects and itself
      if (enemy != null && enemy != gameObject)
      {
        float distToEnemy = Vector3.Distance(gameObject.transform.position, enemy.transform.position);

        if (/*distToEnemy <= dist && */distToEnemy < minDist)
        {
          detectedEnemy = enemy;
          minDist = distToEnemy;
        }
      }
    }

    return detectedEnemy;
  }
}
