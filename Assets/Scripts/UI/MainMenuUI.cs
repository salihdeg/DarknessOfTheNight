using Network;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public Button quickJoin, createLobby, joinLobby, options, exit;
        private void Awake()
        {
            quickJoin.onClick.AddListener(() =>
            {
                GameLobby.Instance.QuickJoin();
            });
            createLobby.onClick.AddListener(() =>
            {

            });
            joinLobby.onClick.AddListener(() =>
            {

            });
            options.onClick.AddListener(() =>
            {

            });
            exit.onClick.AddListener(() =>
            {
                Application.Quit();
            });


        }

    }//class

}

