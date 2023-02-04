﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SKCell;


[RequireComponent(typeof(Animator))]
public sealed class SKAnimationRandomizer : MonoBehaviour
{
    private Animator anim;
    private void Start()
    {
        CommonUtils.InvokeAction(0.2f, () =>
        {
            anim = GetComponent<Animator>();
            AnimatorClipInfo info = anim.GetCurrentAnimatorClipInfo(0)[0];
            anim.Play(info.clip.name, 0, Random.Range(0f, 1f));
        });
    }
}
