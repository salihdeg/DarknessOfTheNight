using Player;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private const string SPEED = "Speed";
    private const string IS_JUMP = "IsJump";
    private const string IS_GROUNDED = "IsGrounded";
    private const string FREE_FALL = "FreeFall";

    [SerializeField] private PlayerController _playerController;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_animator == null) return;
        // if (!IsOwner) return;

        _animator.SetBool(IS_JUMP, _playerController.IsJumping());
        _animator.SetBool(IS_GROUNDED, _playerController.IsGrounded());
        _animator.SetBool(FREE_FALL, _playerController.IsFreeFall());
        _animator.SetFloat(SPEED, _playerController.GetSpeed());
    }
}
