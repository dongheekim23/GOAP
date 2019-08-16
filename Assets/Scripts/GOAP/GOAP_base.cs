using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GOAP_base : MonoBehaviour
{
  // List of actions for the agent to perform to achieve current goal
  protected List<ActionBase> actionsForAchievingGoal;
  // Current action the GOAP-using agent is performing

  //protected ActionBase currAction;

  protected GoalBase currGoal;

  protected float checkStatusInterval;
  protected float elapsedTime;
  //protected float nextTime;

  public Vector3 steeringVec;

  public GameObject textObject;
  public TextMesh textMesh;
  public Vector3 relativeTextPos;

  public float separationWeight;

  List<string> actionsNotToBeDisturbed;

  protected void BaseAwake()
  {
    // For information text
    Vector3 objectSize = gameObject.GetComponent<Renderer>().bounds.size;

    textObject = new GameObject("FloatingText");
    //textObject.transform.parent = gameObject.transform;
    relativeTextPos = new Vector3(-1 * objectSize.x, 1, -0.7f * objectSize.z);
    textObject.transform.position = gameObject.transform.position + relativeTextPos;
    textObject.transform.rotation = Quaternion.Euler(90, 0, 0);
    textMesh = textObject.AddComponent<TextMesh>();

    textMesh.color = Color.blue;//new Color(11, 114, 217, 255);
    textMesh.characterSize = 5;
    textMesh.text = "On idle";
  }

  protected void BaseStart()
  {
    currGoal = new G_Idle(gameObject, 1.5f);
    actionsForAchievingGoal = currGoal.GetActionListWithLowestCost();
    //currAction = actionsForAchievingGoal[0];

    checkStatusInterval = 3f;
    elapsedTime = 0f;
    //nextTime = 0f;

    separationWeight = 10f;

    // Add action names that are not to be affected by
    // change of goal based on timer
    actionsNotToBeDisturbed = new List<string>();
    actionsNotToBeDisturbed.Add("Taking robber to prison");
    actionsNotToBeDisturbed.Add("Running away");
    actionsNotToBeDisturbed.Add("Patrol");
  }

  protected void UpdateTextPos()
  {
    textObject.transform.position = gameObject.transform.position + relativeTextPos;
  }

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public ActionBase GetCurrAction()
  {
    return actionsForAchievingGoal[0];//currAction;
  }
  public GoalBase GetCurrGoal()
  {
    return currGoal;
  }

  public void ProcessGoalAndAction()
  {
    // TODO : Get rid of currAction variable
    //        and use the first element of actionsForAchievingGoal list


    // If the current action is done, then pop it from the list
    // and assign the action right next to it
    if (actionsForAchievingGoal[0].GetStatus() == ActionBase.ActionStatus.ON_EXIT)
    {
      actionsForAchievingGoal.RemoveAt(0);

      // Assign the next action only when it is available
      //if (actionsForAchievingGoal.Count > 0)
      //  currAction = actionsForAchievingGoal[0];
    }

    // Change the goal every "checkStatusInterval" seconds
    // or all the actions for the current goal have been already executed
    if (actionsForAchievingGoal.Count == 0)
    {
      bool goalChanged = ChangeGoalBasedOnCondition();

      actionsForAchievingGoal
        = currGoal.GetActionListWithLowestCost();

      elapsedTime = 0f;
    }
    else if (elapsedTime >= checkStatusInterval)
    {
      if (IsActionNotToBeDisturbed() == false)
      {
        bool goalChanged = ChangeGoalBasedOnCondition();

        actionsForAchievingGoal
          = currGoal.GetActionListWithLowestCost();
      }

      elapsedTime = 0f;
    }

    elapsedTime += Time.deltaTime;
  }

  // Check if the current goal has to be changed based on conditions
  // Return the newly set goal
  // Ideas : Check conditions every 'N' seconds instead of every frame
  public abstract bool ChangeGoalBasedOnCondition();

  bool IsActionNotToBeDisturbed()
  {
    foreach(string actionName in actionsNotToBeDisturbed)
    {
      if(actionName == textMesh.text)
      {
        return true;
      }
    }

    return false;
  }
}
