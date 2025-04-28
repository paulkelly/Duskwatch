using UnityEngine;

public class TestAction : IAgentAction
{
    public bool Complete => true;

    public void Start() => Debug.Log("Perform Test Action");
}
