using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    public float radius = 3f;               // How close do we need to be to interact?
    public Transform interactionTransform;  // The transform from where we interact in case you want to turnSmoothOffset it

    public bool interactionTrigger = false;

    public bool hasInteracted = false; // Have we already interacted with the object?

    public bool isFocus = false;

    public string interactUIText;
    public virtual void Interact()
    {
        GameManager.instance.nearestInteractable = null;
        GameManager.instance.nearestInteractableDistance = -1;
        PlayerUI.instance.EndDisplayInteractImage();
        Debug.Log("Interacting with " + transform.name);
        hasInteracted = true;
        GameManager.instance.isInteracting = true;
        StartCoroutine(ResetInteraction());
    }
    public void Update()
    {

        GameManager.instance.controls.Player.UniversalButton.started += ctx => interactionTrigger = ctx.ReadValueAsButton();
        float distance = Vector3.Distance(PlayerController.instance.gameObject.transform.position, interactionTransform.position);
        // If we are close enough
        if (distance <= radius)
        {

            isFocus = true;
            // and we haven't already interacted with the object
            if (!hasInteracted)
            {
                if (GameManager.instance.isInteracting == false)
                    if (GameManager.instance.nearestInteractable != null)
                    {
                        //if this object is the closest to the player 
                        if (distance < GameManager.instance.nearestInteractableDistance)
                        {
                            //set this object as the closest
                            GameManager.instance.nearestInteractable = this;
                            GameManager.instance.nearestInteractableDistance = distance;
                        }
                        else
                        {
                            if (GameManager.instance.nearestInteractable == this)
                            {
                                GameManager.instance.nearestInteractableDistance = distance;
                            }
                        }

                    }
                    else
                    {

                        GameManager.instance.nearestInteractable = this;
                        GameManager.instance.nearestInteractableDistance = distance;

                    }
                if (interactionTrigger && GameManager.instance.nearestInteractable == this)
                {
                    // Interact with the object

                    
                    Interact();
                    
                }
            }

        }
        else
        {
            if (GameManager.instance.nearestInteractable == this)
            {
                GameManager.instance.nearestInteractable = null;
                GameManager.instance.nearestInteractableDistance = -1;
                PlayerUI.instance.EndDisplayInteractImage();
            }
            isFocus = false;

        }

        if (GameManager.instance.nearestInteractable == this)
        {
            PlayerUI.instance.DisplayInteractImage(interactUIText);
        }
        if (interactionTrigger) interactionTrigger = false;
    }
    IEnumerator ResetInteraction()
    {
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.isInteracting = false;
        yield return null;
    }
    // Draw our radius in the editor
    void OnDrawGizmosSelected()
    {
        if (interactionTransform == null)
            interactionTransform = transform;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactionTransform.position, radius);
    }
    public IEnumerator DestroyWhenInteractionResetted()
    {
        yield return new WaitWhile(() => GameManager.instance.isInteracting);
        Destroy(gameObject);
    }

}

