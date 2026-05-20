using UnityEngine;
public enum Player_AnimState
{
    None = 0,
    Idle,
    Walk,
    Run
}
public class AnimatorController : MonoBehaviour
{
    [SerializeField] private Animator Animator_Player;

    private Player_AnimState _currentAnimState;

    public void SetState(Player_AnimState newState)
    {
        if (newState == _currentAnimState) return;

        _currentAnimState = newState;

        switch (_currentAnimState)
        {
            case Player_AnimState.Idle:
                ResetAllAnimParameters();
                break;
            case Player_AnimState.Walk:
                ResetAllAnimParameters();
                Animator_Player.SetBool("IsWalk", true);
                break;
            case Player_AnimState.Run:
                ResetAllAnimParameters();
                Animator_Player.SetBool("IsRun", true);
                break;
            default:
                ResetAllAnimParameters();
                break;
        }
    }

    private void ResetAllAnimParameters()
    {
        Animator_Player.SetBool("IsWalk", false);
        Animator_Player.SetBool("IsRun", false);
    }
}