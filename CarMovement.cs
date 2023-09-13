using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [Header("Parametry Pojazdu")]
    [Space(3)]

    [SerializeField] private int _power = 150;
    [SerializeField] private float _angle = 45f;
    [SerializeField] private int _startTime = 1;

    [SerializeField] private Rigidbody _rigidbody;

    [SerializeField] private Transform _centerOfMass;

    [SerializeField] private enum Drive
    {
        FWD,
        RWD,
        AWD,
    }
    [SerializeField] private Drive _drive;


    [Header("Stan Pojazdu")]
    [Space(3)]

    [SerializeField] private float horizontal;
    [SerializeField] private float Vertical;

    [SerializeField] private int _rpm;
    [SerializeField] private bool _engineOn;
    [SerializeField] private float _actualSpeed;


    [Header("Dzwiêki Pojazdu")]
    [Space(3)]

    [SerializeField] private AudioSource _startSound;
    [SerializeField] private AudioSource _idleSound;
    [SerializeField] private AudioSource _lowPower;
    [SerializeField] private AudioSource _mediumPower;

    [Header("WheelsCollider")]
    [Space(3)]
    [SerializeField] private WheelCollider FrontLeftCollider;
    [SerializeField] private WheelCollider FrontRightCollider;
    [SerializeField] private WheelCollider BackLeftCollider;
    [SerializeField] private WheelCollider BackRightCollider;


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody.centerOfMass = _centerOfMass.localPosition;
    }

    void Update()
    {
        _actualSpeed = _rigidbody.velocity.magnitude;

        if (_engineOn == false)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                StartEngine();
            }
        }

        if (_engineOn)
        {
            EngineUpdate();

            //pobranie wartoœci z osi 
            horizontal = Input.GetAxis("Horizontal");
            Vertical = Input.GetAxis("Vertical");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateWheelsSpeed();
        UpdateWheelsAngle();
    }

    private void UpdateWheelsSpeed()
    {
        //napêd
        if (_drive == Drive.FWD)
        {
            FrontLeftCollider.motorTorque = _power * Vertical;
            FrontRightCollider.motorTorque = _power * Vertical;
        }
        if (_drive == Drive.RWD)
        {
            BackLeftCollider.motorTorque = _power * Vertical;
            BackRightCollider.motorTorque = _power * Vertical;
        }
        if (_drive == Drive.AWD)
        {
            FrontLeftCollider.motorTorque = _power * Vertical;
            FrontRightCollider.motorTorque = _power * Vertical;
            BackLeftCollider.motorTorque = _power * Vertical;
            BackRightCollider.motorTorque = _power * Vertical;
        }
    }

    private void UpdateWheelsAngle()
    {
        FrontLeftCollider.steerAngle = _angle * horizontal;
        FrontRightCollider.steerAngle = _angle * horizontal;
    }

    private void StartEngine()
    {
        _startSound.Play();
        Thread.Sleep(_startTime);
        _engineOn = true;
    }

    private void EngineUpdate()
    {
        if (Vertical == 0)
        {
            if (_idleSound.isPlaying)
            {

            }
            else
            {
                _idleSound.Play();
            }
        }

        if (_actualSpeed < 30)
        {
            if (Vertical > 0)
            {
                _idleSound.Pause();

                if (_lowPower.isPlaying)
                {

                }
                else
                {
                    _lowPower.Play();
                }
            }
        }

        if (_actualSpeed > 30)
        {
            if (Vertical > 0)
            {
                _idleSound.Pause();

                if (_mediumPower.isPlaying)
                {

                }
                else
                {
                    _mediumPower.Play();
                }
            }
        }
    }
}
