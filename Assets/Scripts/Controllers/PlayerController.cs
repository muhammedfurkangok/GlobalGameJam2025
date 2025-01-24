using System;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour, IPlayerController
{
    #region Stats

    [Header("LAYERS")] [Tooltip("Set this to the layer your player is on")]
    public LayerMask PlayerLayer;

    [Header("INPUT")]
    [Tooltip(
        "Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
    public bool SnapInput = true;

    [Tooltip(
         "Minimum input required before you mount a ladder or climb a ledge. Avoids unwanted climbing using controllers"),
     Range(0.01f, 0.99f)]
    public float VerticalDeadZoneThreshold = 0.3f;

    [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"),
     Range(0.01f, 0.99f)]
    public float HorizontalDeadZoneThreshold = 0.1f;

    [Header("MOVEMENT")] [Tooltip("The top horizontal movement speed")]
    public float MaxSpeed = 14;

    [Tooltip("The player's capacity to gain horizontal speed")]
    public float Acceleration = 120;

    [Tooltip("The pace at which the player comes to a stop")]
    public float GroundDeceleration = 60;

    [Tooltip("Deceleration in air only after stopping input mid-air")]
    public float AirDeceleration = 30;

    [Tooltip("A constant downward force applied while grounded. Helps on slopes"), Range(0f, -10f)]
    public float GroundingForce = -1.5f;

    [Tooltip("The detection distance for grounding and roof detection"), Range(0f, 0.5f)]
    public float GrounderDistance = 0.05f;

    [Header("JUMP")] [Tooltip("The immediate velocity applied when jumping")]
    public float JumpPower = 36;

    [Tooltip("The maximum vertical movement speed")]
    public float MaxFallSpeed = 40;

    [Tooltip("The player's capacity to gain fall speed. a.k.a. In Air Gravity")]
    public float FallAcceleration = 110;

    [Tooltip("The gravity multiplier added when jump is released early")]
    public float JumpEndEarlyGravityModifier = 3;

    [Tooltip(
        "The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
    public float CoyoteTime = .15f;

    [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
    public float JumpBuffer = .2f;

    [Tooltip(
        "The amount of times the player can jump on the air before touching the ground again. 0 for normal, 1 for double, 2 for triple jump.")]
    public int AirJumps = 1;

    [Tooltip("The strenght of the player's dash")]
    public float DashForce = 15.0f;

    [Tooltip("Seconds between a player can dash consecutively. Jumping or landing resets dash timing.")]
    public float DashCooldown = 0.75f;

    #endregion

    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;

    public enum PlayerControlScheme
    {
        WASD,
        Arrows,
        Joystick1,
        Joystick2,
        JoystickAlt1,
        JoystickAlt2
    }

    public bool isPlayer1 = true;
    public PlayerControlScheme controlScheme = PlayerControlScheme.WASD;
    [SerializeField] private ParticleSystem DashParticles;

    #region Interface

    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action<bool> Jumped;

    public event Action OnShoot;

    public void Reset()
    {
        _rb.linearVelocity = new Vector2(0, 0);
        _airDash = true;
        _nextDashTime = _time;
        _airJumps = AirJumps;
    }

    #endregion

    private float _time;
    private float _dir = -1;
    private bool _dashBuffer = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void OnChangeInputForPlayer1(PlayerControlScheme obj)
    {
        if (isPlayer1) controlScheme = obj;
    }

    private void OnChangeInputForPlayer2(PlayerControlScheme obj)
    {
        if (!isPlayer1) controlScheme = obj;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        GatherInput();
        if (_frameInput.ShootHeld) HandleShootingEvent();
    }

    void HandleShootingEvent()
    {
        OnShoot?.Invoke();
    }

    private void GatherInput()
    {
        if (controlScheme == PlayerControlScheme.WASD)
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.W),
                DropHeld = Input.GetKey(KeyCode.S),
                DashDown = Input.GetKeyDown(KeyCode.Y),
                ShootHeld = Input.GetKey(KeyCode.T),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };
        }
        else if (controlScheme == PlayerControlScheme.Arrows)
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetKeyDown(KeyCode.UpArrow),
                JumpHeld = Input.GetKey(KeyCode.UpArrow),
                DropHeld = Input.GetKey(KeyCode.DownArrow),
                DashDown = Input.GetKeyDown(KeyCode.Keypad2),
                ShootHeld = Input.GetKey(KeyCode.Keypad0),
                Move = new Vector2(Input.GetAxisRaw("HorizontalArrow"), Input.GetAxisRaw("VerticalArrow"))
            };
        }

        else if (controlScheme == PlayerControlScheme.Joystick1)
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetKeyDown(KeyCode.Joystick1Button3),
                JumpHeld = Input.GetKey(KeyCode.Joystick1Button3),
                //GetAxis'in sadece 0 ve -1 arası değerleri bize lazım, pozitif değerleri filtreliyoruz
                DropHeld = 1 % (Mathf.Abs(Input.GetAxis("VerticalJ1") < 0 ? Input.GetAxis("VerticalJ1") : 0) < .5f
                    ? 0
                    : Mathf.Sign(_frameInput.Move.x)) == 0,
                DashDown = Input.GetKeyDown(KeyCode.Joystick1Button0),
                ShootHeld = Input.GetKey(KeyCode.Joystick1Button2),
                Move = new Vector2(Input.GetAxisRaw("HorizontalJ1"), Input.GetAxisRaw("VerticalJ1"))
            };
        }
        else if (controlScheme == PlayerControlScheme.Joystick2)
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetKeyDown(KeyCode.Joystick2Button3),
                JumpHeld = Input.GetKey(KeyCode.Joystick2Button3),
                //GetAxis'in sadece 0 ve -1 arası değerleri bize lazım, pozitif değerleri filtreliyoruz
                DropHeld = 1 % (Mathf.Abs(Input.GetAxis("VerticalJ2") < 0 ? Input.GetAxis("VerticalJ2") : 0) < .5f
                    ? 0
                    : Mathf.Sign(_frameInput.Move.x)) == 0,
                DashDown = Input.GetKeyDown(KeyCode.Joystick2Button0),
                ShootHeld = Input.GetKey(KeyCode.Joystick2Button2),
                Move = new Vector2(Input.GetAxisRaw("HorizontalJ2"), Input.GetAxisRaw("VerticalJ2"))
            };
        }
        //Joystick Alt Kontrolleri
        else if (controlScheme == PlayerControlScheme.JoystickAlt1) //button3 zıplama, button2 ateş, button1 dash
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetKeyDown(KeyCode.Joystick1Button0),
                JumpHeld = Input.GetKey(KeyCode.Joystick1Button0),
                //GetAxis'in sadece 0 ve -1 arası değerleri bize lazım, pozitif değerleri filtreliyoruz
                DropHeld = 1 % (Mathf.Abs(Input.GetAxis("VerticalJ1") < 0 ? Input.GetAxis("VerticalJ1") : 0) < .5f
                    ? 0
                    : Mathf.Sign(_frameInput.Move.x)) == 0,
                DashDown = Input.GetKeyDown(KeyCode.Joystick1Button1),
                ShootHeld = Input.GetKey(KeyCode.Joystick1Button2),
                Move = new Vector2(Input.GetAxisRaw("HorizontalJ1"), Input.GetAxisRaw("VerticalJ1"))
            };
        }
        else if (controlScheme == PlayerControlScheme.JoystickAlt2) //button3 zıplama, button2 ateş, button1 dash
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetKeyDown(KeyCode.Joystick2Button0),
                JumpHeld = Input.GetKey(KeyCode.Joystick2Button0),
                //GetAxis'in sadece 0 ve -1 arası değerleri bize lazım, pozitif değerleri filtreliyoruz
                DropHeld = 1 % (Mathf.Abs(Input.GetAxis("VerticalJ2") < 0 ? Input.GetAxis("VerticalJ2") : 0) < .5f
                    ? 0
                    : Mathf.Sign(_frameInput.Move.x)) == 0,
                DashDown = Input.GetKeyDown(KeyCode.Joystick2Button1),
                ShootHeld = Input.GetKey(KeyCode.Joystick2Button2),
                Move = new Vector2(Input.GetAxisRaw("HorizontalJ2"), Input.GetAxisRaw("VerticalJ2"))
            };
        }


        if (SnapInput)
        {
            _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < HorizontalDeadZoneThreshold
                ? 0
                : Mathf.Sign(_frameInput.Move.x);
            _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < VerticalDeadZoneThreshold
                ? 0
                : Mathf.Sign(_frameInput.Move.y);
        }

        _dir = _frameInput.Move.x != 0f ? Mathf.Sign(_frameInput.Move.x) : _dir;
        _dashBuffer = _dashBuffer ? _dashBuffer : _frameInput.DashDown;

        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }
    }

    private void FixedUpdate()
    {
        _frameVelocity = _rb.linearVelocity;

        CheckCollisions();

        HandleJump();
        HandleDirection();
        HandleGravity();
        HandleDash();
        HandleDropDown();

        ApplyMovement();
    }

    #region Collisions

    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down,
            GrounderDistance, ~PlayerLayer);
        bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up,
            GrounderDistance, ~PlayerLayer);

        // Hit a Ceiling
        if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        // Landed on the Ground
        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            _airJumps = AirJumps;
            _airDash = true;
            _nextDashTime = _time;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Left the Ground
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
            _airDash = true;
            _nextDashTime = _time;
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    #endregion

    #region Jumping

    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;

    private bool _coyoteUsable;

    // Must be less than -1 * JumpBuffer, otherwise can trigger a jump at the start. Set to -100 for good measure.
    private float _timeJumpWasPressed = -100;
    private int _airJumps;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + CoyoteTime;

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0) _endedJumpEarly = true;

        if (!_jumpToConsume && !HasBufferedJump) return;

        if (_grounded || CanUseCoyote)
        {
            ExecuteJump();
        }
        else if (_airJumps >= 1)
        {
            ExecuteAirJump();
        }

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = JumpPower;
        Jumped?.Invoke(false);
    }

    private void ExecuteAirJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = JumpPower;
        _airJumps -= 1;
        Jumped?.Invoke(true);
    }

    #endregion

    #region Horizontal

    private void HandleDirection()
    {
        Func<bool> isMax = () =>
            Mathf.Abs(_frameVelocity.x) > MaxSpeed && Mathf.Sign(_frameInput.Move.x) == Mathf.Sign(_frameVelocity.x);

        if (_frameInput.Move.x == 0 || isMax())
        {
            var deceleration = _grounded ? GroundDeceleration : AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * MaxSpeed,
                Acceleration * Time.fixedDeltaTime);
        }
    }

    #endregion

    #region Dash

    private float _nextDashTime = 0;
    private bool _airDash = true;

    private void HandleDash()
    {
        if (_dashBuffer)
        {
            _dashBuffer = false;
            if (_nextDashTime <= _time && _airDash)
            {
                if (!_grounded)
                {
                    _airDash = false;
                }

                _nextDashTime = _time + DashCooldown;
                _dashBuffer = false;
                _frameVelocity += Vector2.right * _dir * DashForce;
                DashParticles.Play();
            }
        }
    }

    #endregion

    #region Drop

    private float _dropTime = 0;

    private void HandleDropDown()
    {
        if (_frameInput.DropHeld)
        {
            _dropTime = _time + 0.035f;
        }

        if (_dropTime > _time)
        {
            gameObject.layer = 6;
        }
        else
        {
            gameObject.layer = 3;
        }
    }

    #endregion

    #region Gravity

    private void HandleGravity()
    {
        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = GroundingForce;
        }
        else
        {
            var inAirGravity = FallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= JumpEndEarlyGravityModifier;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    #endregion

    private void ApplyMovement() => _rb.linearVelocity = _frameVelocity;
}

public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public bool DashDown;
    public bool DropHeld;
    public bool ShootHeld;
    public Vector2 Move;
}

public interface IPlayerController
{
    public event Action<bool, float> GroundedChanged;

    public event Action<bool> Jumped;
    public Vector2 FrameInput { get; }
}