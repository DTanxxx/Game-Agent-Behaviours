using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavGraphNode : MonoBehaviour
{
    private int index = -1;
    private int extraInfo;
    private Vector3 position;

    public void InitialiseNode(int index, Vector3 position, int extraInfo = -1)
    {
        this.index = index;
        this.extraInfo = extraInfo;
        this.position = position;
    }

    public int GetIndex()
    {
        return index;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public Vector3 GetPosition()
    {
        return position;
    }
}
