#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
[NodeInfo("Group", "General", "Creates a group to organize nodes into")]
public class NodeGroup : Group
{
    public NodeGroup()
    {
        title = "New Group";
    }
}
#endif