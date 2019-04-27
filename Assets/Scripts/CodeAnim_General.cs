using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CodeAnim_General : MonoBehaviour
{
    private Squishy3D _squish;
    private bool _prevJumping;
    private bool _prevFlinched;
    [SerializeField] private PhysicsController _parentPhysics;
    private PlayerScript _player;

    [SerializeField] private Transform _model;
    private Vector3 _amplitude;

    private Renderer[] _rends;
    private Shader _default;
    private Shader _white;
    private float _whiteTimer;
    private List<List<Color>> _startCol = new List<List<Color>>();

    // Start is called before the first frame update
    void Start()
    {
        _squish = GetComponent<Squishy3D>();
        _rends = _model.GetComponentsInChildren<Renderer>();
        _player = _parentPhysics.GetComponent<PlayerScript>();

        _default = _rends[0].material.shader;
        _white = Shader.Find("GUI/Text Shader");

        for (int i = 0; i < _rends.Length; i++)
            {
            _startCol.Add(new List<Color>());
            for (int j = 0; j < _rends[i].materials.Length; j++)
                {
                _startCol[i].Add(_rends[i].materials[j].color);
                }
            }
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

        if (_player._isFlinched && !_prevFlinched)
            {
            _whiteTimer = 0.05f;
            for (int i = 0; i < _rends.Length; i++)
                {
                for (int j = 0; j < _rends[i].materials.Length; j++)
                    {
                    _rends[i].materials[j].color = Color.white;
                    _rends[i].materials[j].shader = _white;
                    }
                }
            }

        _prevFlinched = _player._isFlinched;
        }

    private void FixedUpdate()
        {
        if (_whiteTimer > 0)
            {
            _whiteTimer -= Time.deltaTime;
            if (_whiteTimer <= 0)
                {
                for (int i = 0; i < _rends.Length; i++)
                    {
                    for (int j = 0; j < _rends[i].materials.Length; j++)
                        {
                        _rends[i].materials[j].color = _startCol[i][j];
                        _rends[i].materials[j].shader = _default;
                        }
                    }
                }
            }
        }
    }
