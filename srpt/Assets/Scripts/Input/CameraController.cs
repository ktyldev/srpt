using Ktyl.Util;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _sensHorizontal;
    [SerializeField] private float _sensVertical;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration = 0.1f;
    [SerializeField] private SerialDouble _relativisticVelocity;

    [SerializeField] private SerialBool _moving;
    [SerializeField] private UnityEvent _interacted;

    // TODO: implement poincaré transformations
    [SerializeField] private Transform _lattice;

    // first person camera, spin around up axis and look up and down from a horizon
    private Vector2 _mousePos;
    private float _speed;
    private bool _dirty;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private float _moveInput;

    private void Update()
    {
        _moveInput = Input.GetAxis("Vertical");
        
        ToggleMoving();

        Look();
    }

    private void ToggleMoving()
    {
        // toggle value on tab
        _moving.Value = Input.GetKeyDown(KeyCode.Tab)
            ? !_moving
            : _moving;

        if (_moving)
        {
            _camPose = default;
        }
    }

    private Vector3 _pos;

    private void FixedUpdate()
    {
        if (_moving)
        {
            Move(Time.fixedDeltaTime);
        }
    }

    private void LateUpdate()
    {
        if (_dirty)
        {
            _interacted.Invoke();
            _dirty = false;
        }
        
        _relativisticVelocity.Value = _speed / _maxSpeed;
    }

    private void Move(float deltaTime)
    {
        var trans = transform;
        var input = _moveInput;

        if (Mathf.Abs(input) > Mathf.Epsilon)
        {
            _speed = Mathf.Lerp(_speed, input * _maxSpeed, _acceleration);
            _dirty = true;
        }
        else
        {
            _speed = Mathf.Lerp(_speed, 0, 0.1f);
        }

        trans.Translate(Vector3.forward * (input * deltaTime));
        var pos = trans.position;
        pos = _lattice.InverseTransformPoint(pos);

        // // map to 0..1
        pos += Vector3.one * 0.5f;
        pos.x = Mathf.Repeat(pos.x, 1);
        pos.y = Mathf.Repeat(pos.y, 1);
        pos.z = Mathf.Repeat(pos.z, 1);
        // map back to -0.5..0.5
        pos -= Vector3.one * 0.5f;

        pos = _lattice.TransformPoint(pos);
        transform.position = pos;
    }

    // lattice up and direction rotations
    private Vector2 _latticePose;
    private Vector2 _camPose;

    private void Look()
    {
        var dt = Time.deltaTime;
        var lookDir = Vector3.forward;

        var input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (input.magnitude > 0.01f)
        {
            _dirty = true;
        }

        var pose = _moving ? _latticePose : _camPose;
        var last = Mathf.Abs(pose.x + pose.y);

        // up/down
        pose.x += input.y * _sensVertical * dt;
        pose.x = Mathf.Clamp(pose.x, -90, 90);
        lookDir = Quaternion.Euler(-pose.x, 0, 0) * lookDir;

        // rotation around y
        pose.y += input.x * _sensHorizontal * dt;
        // 0..2PI
        pose.y = Mathf.Repeat(pose.y, 360f);
        lookDir = Quaternion.Euler(0, pose.y, 0) * lookDir;

        if (!_moving)
        {
            transform.forward = lookDir;

            _camPose = pose;
        }
        else
        {
            transform.forward = Vector3.forward;

            var apparentOldPos = _lattice.InverseTransformPoint(transform.position);

            _lattice.forward = Vector3.forward;
            _lattice.Rotate(_lattice.right * _latticePose.x);
            _lattice.Rotate(0, -_latticePose.y, 0);

            transform.position = _lattice.TransformPoint(apparentOldPos);

            _latticePose = pose;
        }
    }

}