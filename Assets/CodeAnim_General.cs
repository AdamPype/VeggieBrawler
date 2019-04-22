using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeAnim_General : MonoBehaviour
{
    private Squishy3D _squish;
    private bool _prevJumping;
    [SerializeField] private PhysicsController _parentPhysics;

    [SerializeField] private Transform _model;
    private Vector3 _amplitude;

    // Start is called before the first frame update
    void Start()
    {
        _squish = GetComponent<Squishy3D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_prevJumping != _parentPhysics.IsJumping)
            {
            _squish.Squish();
            _squish.Reversed = _parentPhysics.IsJumping;
            }

        _prevJumping = _parentPhysics.IsJumping;
    }
}
