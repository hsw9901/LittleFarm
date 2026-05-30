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

    [Header("상호작용 설정")]
    public float InteractionDistance = 1.0f;
    public float InteractionRadius = 0.5f;
    public LayerMask InteractableLayer;

    private Rigidbody2D _rigidBody;
    private Vector2 _moveInput;
    private bool _isRunning;
    private bool _lookRight = true;

    private Vector2 _lastLook = Vector2.right;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.gravityScale = 0f;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    private void Start()
    {
        if (GameManager.Inst != null)
        {
            GameManager.Inst.RegisterPlayer(this);
        }
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

        ActiveInteraction();

    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GetInput()
    {
        if (GameInputManager.Inst != null)
        {
            _moveInput = GameInputManager.Inst.MoveInput;
            _isRunning = GameInputManager.Inst.IsRunning;
        }

        if (_moveInput != Vector2.zero)
        {
            if (Mathf.Abs(_moveInput.x) > Mathf.Abs(_moveInput.y))
            {
                _lastLook = new Vector2(Mathf.Sign(_moveInput.x), 0); 
            }
            else
            {
                _lastLook = new Vector2(0, Mathf.Sign(_moveInput.y));
            }
        }
    }

    private void Move()
    {
        float speed = _isRunning ? _runSpeed : _moveSpeed;
        _rigidBody.linearVelocity = _moveInput * speed;
    }

    private void CheckFilp()
    {
        if (_lastLook.x > 0 && !_lookRight) 
        {
            Flip(); 
        }
        if (_lastLook.x < 0 && _lookRight)
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
   
    public Vector2Int GetTargetGridPosition()
    {
        Vector3 currentPos = transform.position;
        float interactDistance = 1.0f;
        Vector3 targetPos = currentPos + (Vector3)(_lastLook * interactDistance);

        return FarmManager.Inst.GetGridPosition(targetPos);
    }

    private void ActiveInteraction()
    {
        if (GameInputManager.Inst != null && GameInputManager.Inst.IsInteractDown)
        {
            Vector2 playerCenter = new Vector2(transform.position.x, transform.position.y + 0.5f);
            Vector2 checkWorldPos = playerCenter + (_lastLook * InteractionDistance);

            // ⭐️ 하드코딩 되어있던 레이어 대신, 인스펙터에서 설정한 InteractableLayer 변수를 사용합니다.
            Collider2D hit = Physics2D.OverlapCircle(checkWorldPos, InteractionRadius, InteractableLayer);

            if (hit != null)
            {
                // 부딪힌 물체가 상자라면 열기!
                if (hit.TryGetComponent(out Chest chest))
                {
                    chest.OpenChest();
                }
                return; // 상자를 열었다면 도구 사용(UseEquippedItem)은 생략합니다.
            }

            // 상호작용할 물체가 없으면 손에 들고 있는 도구 사용!
            if (InventoryManager.Inst != null)
            {
                InventoryManager.Inst.UseEquippedItem();
            }
        }

        if (GameInputManager.Inst != null && GameInputManager.Inst.IsHarvestDown)
        {
            Vector2Int targetPos = GetTargetGridPosition();
            bool harvestSuccess = FarmManager.Inst.RequestHarvest(targetPos);
        }
    }

    // 💡 추가됨: 씬 뷰(Scene View)에서 캐릭터가 상호작용하는 원의 범위가 보이게 해줍니다!
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        // 캐릭터의 y위치를 조금 올려서(0.5f) 머리~가슴쯤에서 원이 나가도록 시각화
        Vector2 playerCenter = new Vector2(transform.position.x, transform.position.y + 0.5f);

        // 마지막으로 바라본 방향이 (0,0)이라면 기본값으로 아래쪽이나 오른쪽을 그려줍니다.
        Vector2 lookDir = _lastLook == Vector2.zero ? Vector2.right : _lastLook;

        Vector2 checkWorldPos = playerCenter + (lookDir * InteractionDistance);

        // 유니티 씬 화면에 노란색 테두리 원을 그려줍니다.
        Gizmos.DrawWireSphere(checkWorldPos, InteractionRadius);
    }
}
