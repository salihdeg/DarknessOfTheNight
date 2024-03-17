using UnityEngine;

public class Role<T> : MonoBehaviour where T : IFaction
{
    public Roles myRole;

    public void DebugRoleName(string roleName)
    {
        Debug.Log("RoleName: " + roleName);
    }
}