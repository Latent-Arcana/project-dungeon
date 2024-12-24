using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using UnityEngine;

public class EnemyAnimationBehavior : MonoBehaviour
{
    public Animator animator;

    void Awake()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
    }


    public void HandleDamageAnimations()
    {
        StartCoroutine(IncomingDamageAnimationHandler());
    }

    private IEnumerator IncomingDamageAnimationHandler()
    {
        string animationName = "player-damage-slash-1";

        animator.Play(animationName);

        float animationLength = animator.runtimeAnimatorController.animationClips.First(clip => clip.name == animationName).length / 2;

        yield return new WaitForSeconds(animationLength);

        animator.Play("Idle");

    }

}
