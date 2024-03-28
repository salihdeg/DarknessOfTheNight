using UnityEngine;

namespace Player
{
    public class ChangePlayerLayer : MonoBehaviour
    {
        private PlayerController _playerController;
        [SerializeField] private Transform _playerModel;
        private int PLAYER_SELF_LAYER = 0; 

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            PLAYER_SELF_LAYER = LayerMask.NameToLayer("PlayerSelf");
        }
        private void Start()
        {
            if (!_playerController.IsOwner) return;

            _playerModel.gameObject.layer = PLAYER_SELF_LAYER;
            foreach (Transform child in _playerModel)
            {
                child.gameObject.layer = PLAYER_SELF_LAYER;
            }
        }
    }
}

