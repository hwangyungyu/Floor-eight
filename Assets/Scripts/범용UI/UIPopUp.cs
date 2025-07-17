using UnityEngine;

public class UIPopUp : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    public void TogglePopUP()
    {
        bool enabled = animator.GetBool("isShown");
        animator.SetBool("isShown", !enabled);
    }
}

