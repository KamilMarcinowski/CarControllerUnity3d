using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [Header("Parametry Pojazdu")]
    [Space(3)]

    [SerializeField] private int _power = 150;
    [SerializeField] private int _maxSpeed = 250;
    [SerializeField] private int _brakingPower = 300;
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
    [SerializeField] private int _maxRpm;
    [SerializeField] private gear _gear;
    [SerializeField] private enum gear
    {
        _gear1,
        _gear2, 
        _gear3,
        _gear4,
        _gear5,
    }
    [SerializeField] private bool _engineOn;
    [SerializeField] private float _actualSpeed;


    [Header("Dzwiêki Pojazdu")]
    [Space(3)]

    [SerializeField] private AudioSource _startSound;
    [SerializeField] private AudioSource _idleSound;
    [SerializeField] private AudioSource _lowPower;
    [SerializeField] private AudioSource _mediumPower;
    [SerializeField] private AudioSource _highPower;

    [Header("WheelsCollider")]
    [Space(3)]

    [SerializeField] private WheelCollider FrontLeftCollider;
    [SerializeField] private WheelCollider FrontRightCollider;
    [SerializeField] private WheelCollider BackLeftCollider;
    [SerializeField] private WheelCollider BackRightCollider;

    [Header("Skrzynia Biegów")]
    [Space(3)]
    [SerializeField][Range(0, 250)] private int _gear1Range;
    [SerializeField][Range(0, 250)] private int _gear2Range;
    [SerializeField][Range(0, 250)] private int _gear3Range;
    [SerializeField][Range(0, 250)] private int _gear4Range;
    [SerializeField][Range(0, 250)] private int _gear5Range;

    [SerializeField] private TextMeshProUGUI _gearText;
    [SerializeField] private TextMeshProUGUI _rpmText;

    [Header("Oœwietlenie Pojazdu")]
    [Space(3)]

    [SerializeField] private bool _driveLighStatus;
    [SerializeField] private Light _driveLightsFL;
    [SerializeField] private Light _driveLightsFR;
    [SerializeField] private Light _driveLightsBL;
    [SerializeField] private Light _driveLightsBR;
    [SerializeField] private int _backBrakeLightIntensity = 8;
    [SerializeField] private int _backNormalLightIntensity = 8;

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

        if (Input.GetKeyDown(KeyCode.L))
        {
            _driveLighStatus = !_driveLighStatus;
        }

        if (_driveLighStatus == true)
        {
            _driveLightsFL.enabled = true;
            _driveLightsFR.enabled = true;
            _driveLightsBL.enabled = true;
            _driveLightsBR.enabled = true;
        }
        else
        {
            _driveLightsFL.enabled = false;
            _driveLightsFR.enabled = false;
            _driveLightsBL.enabled = false;
            _driveLightsBR.enabled = false;
        }

        UpdateGears();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateWheelsSpeed();
        UpdateWheelsAngle();
        BrakeUpdate();
        UpdateEngineSound();    
    }

    private void UpdateWheelsSpeed()
    {
        //napêd
        if (_rpm < _maxRpm)
        {
            if (_drive == Drive.FWD)
            {
                FrontLeftCollider.motorTorque = _power * Vertical * _rpm / 100;
                FrontRightCollider.motorTorque = _power * Vertical * _rpm / 100;
            }
            if (_drive == Drive.RWD)
            {
                BackLeftCollider.motorTorque = _power * Vertical * _rpm / 100;
                BackRightCollider.motorTorque = _power * Vertical * _rpm / 100;
            }
            if (_drive == Drive.AWD)
            {
                FrontLeftCollider.motorTorque = _power * Vertical * _rpm / 100;
                FrontRightCollider.motorTorque = _power * Vertical * _rpm / 100;
                BackLeftCollider.motorTorque = _power * Vertical * _rpm / 100;
                BackRightCollider.motorTorque = _power * Vertical * _rpm / 100;
            }
        }
    }

    private void UpdateWheelsAngle()
    {
        FrontLeftCollider.steerAngle = _angle * horizontal;
        FrontRightCollider.steerAngle = _angle * horizontal;
    }

    private void BrakeUpdate()
    {
        if (Input.GetKey(KeyCode.S))
        {
            _driveLightsBL.intensity = _backBrakeLightIntensity;
            _driveLightsBR.intensity = _backBrakeLightIntensity;

            FrontLeftCollider.brakeTorque = _brakingPower;
            FrontRightCollider.brakeTorque = _brakingPower;
        }
        else
        {
            _driveLightsBL.intensity = _backNormalLightIntensity;
            _driveLightsBR.intensity = _backNormalLightIntensity;

            FrontLeftCollider.brakeTorque = 0;
            FrontRightCollider.brakeTorque = 0;
        }
    }

    private void StartEngine()
    {
        _startSound.Play();
        Thread.Sleep(_startTime);
        _engineOn = true;
        _rpm = 1000;
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
    }

    private void UpdateGears()
    {
        int oldRpm = (int)Vertical;

        if (_rpm < _maxRpm)
        {
            _rpm += oldRpm * 10;
        }

        if (Vertical == 0 && _rpm > 1000)
        {
            _rpm -= 10;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            switch (_gear)
            {
                case gear._gear1:
                    _gear = gear._gear1;
                    break;
                case gear._gear2:
                    _gear = gear._gear3;
                    break;
                case gear._gear3:
                    _gear = gear._gear4;
                    break;
                case gear._gear4:
                    _gear = gear._gear5;
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            switch (_gear)
            {
                case gear._gear5:
                    _gear = gear._gear4;
                    break;
                case gear._gear4:
                    _gear = gear._gear3;
                    break;
                case gear._gear3:
                    _gear = gear._gear2;
                    break;
                case gear._gear2:
                    _gear = gear._gear1;
                    break;
            }
        }

        _gearText.text = _gear.ToString();
        _rpmText.text = _rpm.ToString();
    }

    private void UpdateEngineSound()
    {
        if (_rpm > 1000 && _rpm < 3500)
        {
            for (int i = 0; i < _lowPower.clip.length; i++)
            {
                _lowPower.Play();
            }
        }

        if (_rpm > 3500 && _rpm < 6000)
        {
            for (int i = 0; i < _mediumPower.clip.length; i++)
            {
                _mediumPower.Play();
            }
        }

        if (_rpm > 6000 && _rpm < _maxRpm)
        {
            for (int i = 0; i < _lowPower.clip.length; i++)
            {
                _highPower.Play();
            }
        }
    }
}
