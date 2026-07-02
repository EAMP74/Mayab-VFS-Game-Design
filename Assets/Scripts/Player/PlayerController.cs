using JetBrains.Annotations;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private Camera _mainCam;
    private Vector3 _aimPoint;
    private float _currentSpeed;


    [SerializeField] private InputReader _input;
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _runSpeed = 8f;
    [SerializeField] private float _groundPlaneHeight;
    [SerializeField] private Transform _aimPivot;
    [SerializeField] private float _aimSmoothing = 10f;
    [SerializeField] private PlayerDash _dash;

    [SerializeField] private Animator _animator;

    public Vector3 AimPoint => _aimPoint;

    public Vector3 AimDirection => (_aimPoint - transform.position).normalized;

    public Vector3 MoveDirection { get; private set; }

    public float CurrentSpeed => _currentSpeed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _mainCam = Camera.main;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {

        UpdateAiming();


    }

    private void UpdateAiming()
    {
        Vector2 mousePos = _input.MousePosition;
        Ray ray = _mainCam.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0f));

        Plane ground = new Plane(Vector3.up, new Vector3(0f, _groundPlaneHeight, 0f));
        if (ground.Raycast(ray, out float distance))
        {
            _aimPoint = ray.GetPoint(distance);
        }

        Vector3 lookDir = _aimPoint - _aimPivot.position;
        lookDir.y = 0f;


        if (lookDir.magnitude > 0.01f)
        {

            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            _aimPivot.rotation = Quaternion.Slerp(_aimPivot.rotation, targetRotation, _aimSmoothing * Time.deltaTime);

        }

}
    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (_dash != null && _dash.IsDashing) return;

        Vector2 rawInput = _input.Move;
        Vector3 inputDir = new Vector3(rawInput.x, 0f, rawInput.y);

        

        if (_mainCam != null)
        {
            Vector3 camFoward = _mainCam.transform.forward;
            Vector3 camRight = _mainCam.transform.right;
            camFoward.y = 0f;
            camRight.y = 0f;

            camFoward.Normalize();
            camRight.Normalize();

            inputDir = camRight * rawInput.x + camFoward * rawInput.y;
        }

        MoveDirection = inputDir;

        float currentSpeed = _moveSpeed;
        if (_input.Sprint) currentSpeed = _runSpeed;
        _animator.SetBool("isRunning", _input.Sprint);

        inputDir.Normalize();

        _rb.linearVelocity = inputDir * currentSpeed;

        _animator.SetFloat("Blend", _rb.linearVelocity.magnitude / _moveSpeed); 
    }
}
