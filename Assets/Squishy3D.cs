using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squishy3D : MonoBehaviour {

    //SCRIPT BY PAPERCOOKIES.ITCH.IO

    //Attach script to your own script and use Squish() to squish :-)

    private Vector3 _startScale;

    private float _squishyScaleX;
    private float _squishyScaleY;
    private float _squishyScaleZ;

    private float _squish = 0;

    public bool Reversed { get; set; }

    [SerializeField, Range(0, 1)] private float _squishSlowdownSpeed = 0.1f;
    [SerializeField] private float _squichinessLength = 4;
    [SerializeField] private float _squishSpeed = 3;
    [SerializeField, Range(0, 1)] private float _lerpSpeed = 1f;
    [SerializeField] private float _squishSize = 1f;
    [SerializeField] private bool _worldSpace = false;

    private float[] originalParameters = new float[5];

    private void Awake()
        {
        _startScale = transform.localScale;
        _squishyScaleX = transform.localScale.x;
        _squishyScaleY = transform.localScale.y;
        _squishyScaleZ = transform.localScale.z;

        //save original parameters
        originalParameters[0] = _squishSlowdownSpeed;
        originalParameters[1] = _squichinessLength;
        originalParameters[2] = _squishSpeed;
        originalParameters[3] = _lerpSpeed;
        originalParameters[4] = _squishSize;
        }

    private void Update()
        {

        //lerp squish
        _squish = Mathf.Lerp(_squish, 0, _squishSlowdownSpeed);
        //edit scale with squish
        if (Reversed)
            {
            _squishyScaleX = Mathf.Lerp(_squishyScaleX, _startScale.x - SinWave(_squichinessLength - _squish, _squish * _squishSpeed, _squish), _lerpSpeed);
            _squishyScaleZ = Mathf.Lerp(_squishyScaleZ, _startScale.y - SinWave(_squichinessLength - _squish, _squish * _squishSpeed, _squish), _lerpSpeed);
            _squishyScaleY = Mathf.Lerp(_squishyScaleX, _startScale.z + SinWave(_squichinessLength - _squish, _squish * _squishSpeed, _squish), _lerpSpeed);
            }
        else
            {
            _squishyScaleX = Mathf.Lerp(_squishyScaleX, _startScale.x + SinWave(_squichinessLength - _squish, _squish * _squishSpeed, _squish), _lerpSpeed);
            _squishyScaleZ = Mathf.Lerp(_squishyScaleZ, _startScale.y + SinWave(_squichinessLength - _squish, _squish * _squishSpeed, _squish), _lerpSpeed);
            _squishyScaleY = Mathf.Lerp(_squishyScaleX, _startScale.z - SinWave(_squichinessLength - _squish, _squish * _squishSpeed, _squish), _lerpSpeed);
            }

        if (_worldSpace)
            {
            SetGlobalScale(transform, new Vector3(_squishyScaleX, _squishyScaleY, _squishyScaleZ));
            }
        else
            {
            transform.localScale = new Vector3(_squishyScaleX, _squishyScaleY, _squishyScaleZ);
            }
        }

    private float SinWave(float t, float speed, float amplitude)
        {
        return Mathf.Sin(t * speed) * amplitude;
        }

    /// <summary>
    /// Returns true if the object is being is squished.
    /// </summary>
    /// <returns></returns>
    public bool IsSquishing()
        {
        if (_squish > 0.1f)
            {
            return true;
            }
        return false;
        }

    /// <summary> 
    /// Squishes the object with the parameters defined in the inspector.
    /// </summary>
    public void Squish()
        {
        ResetParameters();
        //start squish
        _squish = _squishSize;
        }

    /// <summary>
    /// Call this every frame to freeze the squish.
    /// </summary>
    public void FreezeSquish()
        {
        _squish = _squishSize;
        }

    /// <summary> 
    /// Squishes the object and overrides the parameters defined in the inspector.
    /// </summary>
    public void Squish(float squichinessLength, float squishSpeed, float lerpSpeed = 1f, float squishSize = 1f, float squishSlowdownSpeed = 0.1f)
        {
        //override vars
        _squichinessLength = squichinessLength;
        _squishSpeed = squishSpeed;
        _lerpSpeed = lerpSpeed;
        _squishSlowdownSpeed = squishSlowdownSpeed;
        _squishSize = squishSize;
        //start squish
        _squish = _squishSize;
        }

    private void ResetParameters()
        {
        _squishSlowdownSpeed = originalParameters[0];
        _squichinessLength = originalParameters[1];
        _squishSpeed = originalParameters[2];
        _lerpSpeed = originalParameters[3];
        _squishSize = originalParameters[4];
        }

    private void SetGlobalScale(Transform transform, Vector3 globalScale)
        {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
        }
    }