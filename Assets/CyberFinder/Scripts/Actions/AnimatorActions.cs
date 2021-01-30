using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorActions : MonoBehaviour
{
    private Animator _animator;

    // Start is called before the first frame update
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetBool(string parameter)
    {
        _animator.SetBool(parameter, true);
    }

    public void ClearBool(string parameter)
    {
        _animator.SetBool(parameter, false);
    }


    // Update is called once per frame
    void Update()
    {
    }
}
