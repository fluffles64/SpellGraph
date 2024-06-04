using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;

[NodeInfo("Root")]
public class RootNode : ExtendedNode
{
    [OutputPort(Port.Capacity.Single, "Active")]
    public Trigger active;

    public RootNode()
    {
        capabilities &= ~Capabilities.Deletable;
        capabilities &= ~Capabilities.Movable;
        capabilities &= ~Capabilities.Selectable;
        capabilities &= ~Capabilities.Collapsible;

        SetPosition(new Rect(300, 250, 100, 150));
        RefreshNode();
    }
}
#endif

public class RuntimeRootNode : RuntimeNode
{

}