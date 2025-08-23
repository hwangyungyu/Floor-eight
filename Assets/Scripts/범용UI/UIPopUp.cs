using UnityEngine;

public class UIPopUp : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private static UIPopUp currentlyOpen;

    public void TogglePopupState()
    {
        if (currentlyOpen == this)
        {
            CloseThisPopup();
        }
        else if (currentlyOpen != null)
        {
            currentlyOpen.CloseThisPopup();
            OpenThisPopup();
        }
        else
        {
            OpenThisPopup();
        }
    }

    private void OpenThisPopup()
    {
        animator.SetBool("isShown", true);
        currentlyOpen = this;
    }

    private void CloseThisPopup()
    {
        animator.SetBool("isShown", false);
        currentlyOpen = null;
    }
}

