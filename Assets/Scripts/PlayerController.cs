using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerMap.IPlayerActions {
  private PlayerMap _actions;
  private Rigidbody2D _rb;
  private GameManager _gm;
  private Animator _anim;
  private SpriteRenderer _sr;

  [SerializeField]
  private float force;
  [SerializeField]
  private float forceMultiplier;
  private readonly float _jumpForce = 5f;
  [SerializeField]
  private bool _isGrounded;
  private bool _isJumping;
  private bool _canClimb;
  private bool _running;
  private bool _climbing;

  [SerializeField]
  private Vector2 _direction;
  [SerializeField]
  private Vector2 _climbDirection;
  private readonly string _jumpAnim = "Jump";
  private readonly string _runAnim = "IsRunning";
  private readonly string _climbAnim = "IsClimbing";

  private void Awake() {
    _actions = new PlayerMap();
    _rb = GetComponent<Rigidbody2D>();
    _anim = GetComponent<Animator>();
    _sr = GetComponent<SpriteRenderer>();
    _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    _actions.Player.SetCallbacks(this);
  }
  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    if (_running && _isGrounded) {
       _anim.SetBool(_runAnim, true);
    } else {
       _anim.SetBool(_runAnim, false);
    }
    if (_climbing && _canClimb) {
       _anim.SetBool(_climbAnim, true);
    } else {
       _anim.SetBool(_climbAnim, false);
    }
  }

  private void FixedUpdate() {

  }

  private void Movement() {
    _rb.AddForce(_direction * force, ForceMode2D.Force);
  }
  private void Jump() {
    if ( _isJumping && _isGrounded ) {
      _rb.AddForce(Vector2.up * ( _jumpForce * forceMultiplier ), ForceMode2D.Impulse);
      _isGrounded = false;
      _anim.SetTrigger(_jumpAnim);
    }
  }

  private void Climb() {
    if ( _canClimb ) {
      _rb.velocity = (_climbDirection * force);
      _climbing = true;
    }
  }

  private void OnCollisionEnter2D(Collision2D collision) {
    if ( collision.gameObject.CompareTag("Ground") ) {
      _isGrounded = true;
      _isJumping = false;
    }
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if ( collision.CompareTag("Water") || collision.CompareTag("LevelComp") ) {
      _gm.RestartGame();
    }
    if ( collision.CompareTag("Ladder") ) {
      _canClimb = true;
    }
  }

  private void OnTriggerExit2D(Collider2D collision) {
    if (collision.CompareTag("Ladder")) {
      _canClimb = false;
    }
  }

  private void OnEnable() {
    _actions.Player.Enable();
  }

  private void OnDisable() {
    _actions.Player.Disable();
  }

  public void OnMovement(InputAction.CallbackContext context) {
    Vector2 _inputVector = context.ReadValue<Vector2>();
    if ( _inputVector.x > 0 ) {
      _direction = Vector2.right;
      _running = true;
      _sr.flipX = false;
    } else if ( _inputVector.x < 0 ) {
      _direction = Vector2.left;
      _running = true;
      _sr.flipX = true;
    } else {
      _direction = Vector2.zero;
      _running = false;
    }

    this.Movement();
  }

  public void OnJump(InputAction.CallbackContext context) {
    float _jump = context.ReadValue<float>();

    if ( _jump > 0 ) {
      _isJumping = true;
    }

    this.Jump();
  }

  public void OnClimb(InputAction.CallbackContext context) {
    Vector2 _inputVector = context.ReadValue<Vector2>();

    if ( _inputVector.y > 0 ) {
      _climbDirection = Vector2.up;
    } else if ( _inputVector.y < 0 ) {
      _climbDirection = Vector2.down;
    } else {
      _climbDirection = Vector2.zero;
      _climbing = false;
    }

    this.Climb();
  }
}
