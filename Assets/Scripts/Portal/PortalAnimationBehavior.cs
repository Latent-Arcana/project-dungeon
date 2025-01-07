using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PortalAnimationBehavior : MonoBehaviour
{
    Animator portalAnimator;

    void Start()
    {
        portalAnimator = gameObject.GetComponentInChildren<Animator>();
    }

    public void AnimatePortal()
    {
        StartCoroutine(EnterPortal());
    }

    public IEnumerator EnterPortal()
    {
        string portalAnimationName = "portal-enter";

        portalAnimator.Play(portalAnimationName);
        // Wait for the duration of the animation
        float animationLength = portalAnimator.runtimeAnimatorController.animationClips.First(clip => clip.name == portalAnimationName).length;
        yield return new WaitForSeconds(animationLength);

        SceneManager.LoadScene("BSP");

    }
}
