using Player;
using UnityEngine;

public class Role<T> : MonoBehaviour where T : IFaction
{
    public Roles role;
    private PlayerController _playerController;

    internal virtual void Start()
    {
        _playerController = GetComponent<PlayerController>();
        DebugRoleName(role.ToString());
        if (_playerController.OwnerClientId == MultiplayerManager.Instance.GetPlayerData().clientId)
            GameInformationUI.Instance.SetRoleText(role);
    }

    public void DebugRoleName(string roleName)
    {
        Debug.Log(Player.Player.LocalPlayer.GetPlayerName() + " RoleName: " + roleName);
    }
}