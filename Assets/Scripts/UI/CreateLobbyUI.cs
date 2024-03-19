using Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyUI : MonoBehaviour
{
    public TMP_InputField lobbyName;
    public Button createPrivate, createPublic;
    private void Awake()
    {
        createPrivate.onClick.AddListener(() =>
        {
            GameLobby.Instance.CreateLobby(lobbyName.text, true);
        });
        createPublic.onClick.AddListener(() =>
        {
            GameLobby.Instance.CreateLobby(lobbyName.text, false);
        });
    }

}//class
