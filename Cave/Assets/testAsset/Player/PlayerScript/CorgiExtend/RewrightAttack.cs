using UnityEngine;
using System.Collections;

public class AnimStuff : MonoBehaviour
{
    public AnimationClip walk, run, jump;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        RuntimeAnimatorController myController = animator.runtimeAnimatorController;

        AnimatorOverrideController myAnimatorOverride = new AnimatorOverrideController();
        myAnimatorOverride.runtimeAnimatorController = myController;

        animator.runtimeAnimatorController = myAnimatorOverride;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) ||
            Input.GetKeyDown(KeyCode.Keypad1))
        {
            SetCurrentAnimation(animator, walk);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) ||
            Input.GetKeyDown(KeyCode.Keypad2))
        {
            SetCurrentAnimation(animator, run);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) ||
            Input.GetKeyDown(KeyCode.Keypad3))
        {
            SetCurrentAnimation(animator, jump);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            animator.SetBool("Move", false);
        }
    }

    void OnGUI()
    {
        GUILayout.Label("1 - to override to walk");
        GUILayout.Label("2 - to override to run");
        GUILayout.Label("3 - to override to jump");
        GUILayout.Label("Esc - to go back to idle");
    }

    /// <summary>
    /// Sub in real animations for stubs
    /// </summary>
    /// <param name="animator">Reference to animator</param>
    public void SetCurrentAnimation(Animator animator, AnimationClip animClip)
    {
        AnimatorOverrideController myCurrentOverrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
        if (myCurrentOverrideController["Stub"] == animClip && animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
        {
            Debug.Log("Current state is already" + animClip.name);
            return;
        }

        RuntimeAnimatorController myOriginalController = myCurrentOverrideController.runtimeAnimatorController;

        // Know issue: Disconnect the orignal controller first otherwise when you will delete this override it will send a callback to the animator
        // to reset the SM
        myCurrentOverrideController.runtimeAnimatorController = null;

        AnimatorOverrideController myNewOverrideController = new AnimatorOverrideController();
        myNewOverrideController.runtimeAnimatorController = myOriginalController;

        myNewOverrideController["Stub"] = animClip;
        animator.runtimeAnimatorController = myNewOverrideController;

        animator.SetBool("Move", true);

        Object.Destroy(myCurrentOverrideController);
    }
}
