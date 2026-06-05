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
            GameManager.Inst.MovePlayerToSavedPosition();
        }
        
    }

    private void Update()
    {
        if (GameStateManager.Inst != null && !GameStateManager.Inst.IsPlaying)
        {
            _moveInput = Vector2.zero;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            ChangePlayerState(Player_AnimState.Idle);
            return;
        }

        GetInput();
        CheckFilp();
        UpdatePlayerState();

        ActiveInteraction();

    }

    private void FixedUpdate()
    {
        if (GameStateManager.Inst != null && !GameStateManager.Inst.IsPlaying) return;
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

            Collider2D hit = Physics2D.OverlapCircle(checkWorldPos, InteractionRadius, InteractableLayer);

            if (hit != null)
            {
                if (hit.TryGetComponent(out Chest chest))
                {
                    chest.OpenChest();
                    return;
                }
                if (hit.TryGetComponent(out ShopNPC shop))
                {
                    shop.OpenShop();
                    return;
                }
            }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector2 playerCenter = new Vector2(transform.position.x, transform.position.y + 0.5f);

        Vector2 lookDir = _lastLook == Vector2.zero ? Vector2.right : _lastLook;

        Vector2 checkWorldPos = playerCenter + (lookDir * InteractionDistance);

        Gizmos.DrawWireSphere(checkWorldPos, InteractionRadius);
    }

    public void HitFieldResource(ToolCategory tool, int power)
    {
        Vector2 playerCenter = new Vector2(transform.position.x, transform.position.y + 0.5f);
        Vector2 checkWorldPos = playerCenter + (_lastLook * InteractionDistance);

        Collider2D hit = Physics2D.OverlapCircle(checkWorldPos, InteractionRadius, InteractableLayer);

        if (hit != null)
        {
            if (hit.TryGetComponent(out FieldResource resource))
            {
              
                resource.TakeHit(tool, power);
                return;
            }
        }
    }
}
