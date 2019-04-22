using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeAnim_ButternutModel : MonoBehaviour
{
    [SerializeField] private float _timeForSpecial = 2.5f;
    [SerializeField] private PlayerScript _player;
    [SerializeField] private Transform[] _scaleDown;
    [SerializeField] private Vector3[] _prevScale;
    [SerializeField] private Transform _scaleUp;

    private float _lerp = 13;
    private bool _special;

    private void Start()
        {
        _prevScale = new Vector3[_scaleDown.Length];
        for (int i = 0; i < _scaleDown.Length; i++)
            {
            _prevScale[i] = _scaleDown[i].localScale;
            }
        }


    // Update is called once per frame
    void LateUpdate()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, _player.SpecialAttacking ? Quaternion.Euler(0, 90, 0) : Quaternion.identity, _lerp * Time.deltaTime);

        for (int i = 0; i < _scaleDown.Length; i++)
            {
            _scaleDown[i].localScale = Vector3.Lerp(_prevScale[i], Vector3.one * (_player.SpecialAttacking ? 0 : 1), _lerp * Time.deltaTime);
            _prevScale[i] = _scaleDown[i].localScale;
            }
        _scaleUp.localScale = Vector3.Lerp(_scaleUp.localScale, Vector3.one * (_player.SpecialAttacking ? 1 : 0), _lerp * Time.deltaTime);
        }
}
