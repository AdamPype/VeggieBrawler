using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanupPartSystem : MonoBehaviour
{
    private ParticleSystem _part;

    private void Start()
        {
        _part = GetComponent<ParticleSystem>();
        }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent == null && _part.particleCount <= 0) Destroy(gameObject);
    }
}
