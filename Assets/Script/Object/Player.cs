using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _runSpeed;

    [Header("애니메이터")]
    [SerializeField]
    private AnimatorController AnimatorController_Player;

    private Rigidbody2D _rigidBody;
    private Vector2 _moveInput;
    private bool _isRunning;
    private bool _lookRight = true;
    public ToolType CurrentTool = ToolType.Hoe;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.gravityScale = 0f;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        //if (!GameStateManager.Inst.IsPlaying)
        //{
        //    return;
        //}

        GetInput();
        CheckFilp();
        UpdatePlayerState();

        if (Input.GetKeyDown(KeyCode.Alpha1)) CurrentTool = ToolType.Hoe;
        if (Input.GetKeyDown(KeyCode.Alpha2)) CurrentTool = ToolType.Wateringcan;

        if (Input.GetKeyDown(KeyCode.F))
        {
            InteractWithTile();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GetInput()
    {
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");
        _moveInput = _moveInput.normalized;
        _isRunning = Input.GetKey(KeyCode.LeftShift);
    }

    private void Move()
    {
        float speed = _isRunning ? _runSpeed : _moveSpeed;
        _rigidBody.linearVelocity = _moveInput * speed;
    }

    private void CheckFilp()
    {
        if (_moveInput.x > 0 && !_lookRight) 
        {
            Flip(); 
        }
        if (_moveInput.x < 0 && _lookRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        _lookRight = !_lookRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void UpdatePlayerState()
    {
        bool isMoving = _moveInput != Vector2.zero;

        if (!isMoving)
        {
            ChangePlayerState(Player_AnimState.Idle);
            return;
        }

        ChangePlayerState(_isRunning ? Player_AnimState.Run : Player_AnimState.Walk);
    }

    private void ChangePlayerState(Player_AnimState newState)
    {
        AnimatorController_Player.SetState(newState);
    }

   
    private void InteractWithTile()
    {
        Vector3 currentPos = transform.position;
        float interactDistance = 1.0f;
        Vector3 targetPos = currentPos;
        targetPos.x += _lookRight ? interactDistance : -interactDistance;

        Vector2Int gridPos = FarmManager.Inst.GetGridPosition(targetPos);
        
        switch (CurrentTool)
        {
            case ToolType.Hoe:
                FarmManager.Inst.RequestTillTile(gridPos);
                break;
            case ToolType.Wateringcan:
                FarmManager.Inst.RequestWaterTile(gridPos);
                break;

        }
    }
}
