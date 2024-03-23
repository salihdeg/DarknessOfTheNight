using Network;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour
{
    private Lobby lobby;
    [SerializeField] private TMPro.TextMeshProUGUI lobbyNameText;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameLobby.Instance.JoinWithId(lobby.Id);
        });
    }
    public void SetLobby(Lobby lobby)
    {
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
    }

}//class
