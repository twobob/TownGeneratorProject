using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum objectRendered
{
    wall, fence, gatehouse
}

public class TypedSpline
{
    //https://github.com/benoit-dumas/SplineMesh // https://assetstore.unity.com/packages/tools/modeling/splinemesh-104989
    // FREE EXTENSION.

    public List<SplineMesh.SplineNode> nodes;
    public int chosenType;
    public bool isRendered;
    public bool tryToFloor;

    public objectRendered rendered
    {
        get { return (objectRendered)chosenType; }

        private set { }

    }

    public TypedSpline()
    {

        // defaults and not null for list.
        this.nodes = new List<SplineMesh.SplineNode>();
        this.chosenType = 0;
        this.isRendered = false;
        this.tryToFloor = false;

    }

}
