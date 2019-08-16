using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopGOAP : GOAP_base
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

  public Vector3 originPos;


  // TODO : 
  // Make bool type variables to determine agent status
  // (e.g. has a gun? has ammo?)

  // TODO : Make enums for agent goals and actions

  // TODO : Use Queue to store agent actions in order

  /*
   
  Possible actions : 
    1. Get ammo
    2. Attack enemy
    3. Melee
    4. Get medicine
    5. Use special weapon

  */

  /*
  Possible goals : 
    1. Kill enemy
    2. Retreat
    3. 
        
  */

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

    originPos = gameObject.transform.position;
  }

  // Update is called once per frame
  void Update()
  {
    base.UpdateTextPos();

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

    steeringVec += separationWeight * bm.ComputeSeparation("Police");

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

    bool isEnemyDetected = DetectEnemy();

    //currGoal = new G_Idle(gameObject);

    //if(agentStats.health >= 30f)
    //{
    if (isEnemyDetected == true)
      currGoal = new G_DealWithRobber(gameObject, detectedEnemy);
    //else
    //  currGoal = new Patrol();
    //}
    //else
    //{
    // currGoal = new Retreat();
    //}

    // TODO : Add Idle action state and set timer
    else
      currGoal = new G_Return(gameObject);//new G_Idle(gameObject);

    return prevGoal.GetName() == currGoal.GetName() ? false : true;
  }

  bool DetectEnemy(float dist = 50f)
  {
    GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Robber");
    //detectedEnemy = null;
    float minDist = float.MaxValue;
    detectedEnemy = null;

    bool isEnemyDetected = false;

    foreach(GameObject enemy in enemyList)
    {
      // Exclude null objects and itself
      if(enemy != null && enemy != gameObject)
      {
        float distToEnemy = Vector3.Distance(gameObject.transform.position, enemy.transform.position);

        if (/*distToEnemy <= dist && */distToEnemy < minDist)
        {
          detectedEnemy = enemy;
          minDist = distToEnemy;
          isEnemyDetected = true;
        }
      }
    }

    return isEnemyDetected;
  }
}
