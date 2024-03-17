using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RoleListSO : ScriptableObject
{
    public List<RolesAndStates> rolesAndStates = new List<RolesAndStates>();
}

[Serializable]
public struct RolesAndStates
{
    public Faction faction;
    public Roles role;
}
