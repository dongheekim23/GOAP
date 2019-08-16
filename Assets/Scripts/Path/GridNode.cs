using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PathResult
{
  PROCESSING,
  COMPLETE,
  IMPOSSIBLE
};

public class GridPos
{
  public GridPos()
  {
    x = 0;
    y = 0;
  }

  public GridPos(int x, int y)
  {
    this.x = x;
    this.y = y;
  }

  //public class EqualityComparer : IEqualityComparer<GridPos>
  //{
  //public int GridPosID { get; set; }
  //public override int GetHashCode()
  //{
  //  return GridPosID;
  //}
  public override int GetHashCode()
  {
    return 100 * x + y;
  }
  public override bool Equals(object obj)
    {
      return Equals(obj as GridPos);
    }
    public bool Equals(GridPos obj)
    {
      return (this.x == obj.x && this.y == obj.y);
    }
  //}

  public int x;
  public int y;
}

public class GridNode
{
  public int xPos, yPos;
  public int xParent, yParent;
  public float given; // h(x)
  public float total; // f(x)

  public GridNode()
  {
    this.xPos = 0;
    this.yPos = 0;
    this.xParent = 0;
    this.yParent = 0;
    this.given = 0;
    this.total = 0;
  }

  public GridNode(int xPos, int yPos, int xParent, int yParent, float given, float total)
  {
    this.xPos = xPos;
    this.yPos = yPos;
    this.xParent = xParent;
    this.yParent = yParent;
    this.given = given;
    this.total = total;
  }

  public GridNode(GridNode rhs)
  {
    this.xPos = rhs.xPos;
    this.yPos = rhs.yPos;
    this.xParent = rhs.xParent;
    this.yParent = rhs.yParent;
    this.given = rhs.given;
    this.total = rhs.total;
  }
}