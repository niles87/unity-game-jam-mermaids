using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
  private PlayerMap _actions;
  private Rigidbody2D _rb;

  [SerializeField]
  private float force;
  [SerializeField]
  private float forceMultiplier;
  [SerializeField]
  private bool _isGrounded;
  [SerializeField]
  private Vector2 _direction;

  private void Awake() {
    _actions = new PlayerMap();
    _rb = GetComponent<Rigidbody2D>();
  }
  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  private void FixedUpdate() {
    Movement();
    Jump();
  }

  public void Direction(InputAction.CallbackContext context) {
    Debug.Log("Input action direction");
    Vector2 _inputVector = context.ReadValue<Vector2>();

    if ( _inputVector.x > 0 ) {
      _direction = Vector2.right;
    } else if ( _inputVector.x < 0 ) {
      _direction = Vector2.left;
    } else {
      _direction = Vector2.zero;
    }
  }

  private void Movement() {
    _rb.velocity = _direction * force;
  }

  private void Jump() {
    if ( _actions.Player.Jump.ReadValue<float>() > 0 && _isGrounded ) {
      _rb.AddForce(Vector2.up * ( force * forceMultiplier ), ForceMode2D.Impulse);
      _isGrounded = false;
    }
  }

  private void OnCollisionEnter2D(Collision2D collision) {
    if ( collision.gameObject.CompareTag("Ground") ) {
      _isGrounded = true;
    }
  }

  private void OnEnable() {
    _actions.Player.Enable();
  }

  private void OnDisable() {
    _actions.Player.Disable();
  }
}
