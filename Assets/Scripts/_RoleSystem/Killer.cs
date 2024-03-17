using UnityEngine;

public class Killer : Role<ITratior>
{
    private void Start()
    {
        DebugRoleName(myRole.ToString());
    }
}