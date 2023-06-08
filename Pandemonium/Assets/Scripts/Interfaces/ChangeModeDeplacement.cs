using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ChangeModeDeplacement : MonoBehaviour
{

    private GameObject[] teleportInteractors;
    private GameObject slideInteractor;
    private TMP_Text textModeDeplacement;

    private const string TAG_TEXT_MODE_DEPLACEMENT = "TextDeplacement";
    private const string TAG_TELEPORT_INTERACTOR = "TeleportInteractor";
    private const string TAG_SLIDE_INTERACTOR = "SlideInteractor";


    private void Awake()
    {
        textModeDeplacement = GameObject.FindGameObjectWithTag(TAG_TEXT_MODE_DEPLACEMENT).GetComponent<TMP_Text>();
        teleportInteractors = GameObject.FindGameObjectsWithTag(TAG_TELEPORT_INTERACTOR);
        slideInteractor = GameObject.FindGameObjectWithTag(TAG_SLIDE_INTERACTOR);

        MenuOption menuOption = FindAnyObjectByType<MenuOption>();
        menuOption.ChangeModeHandler += ChangeModeClientRpc;
    }


    [ClientRpc]
    public void ChangeModeClientRpc(int mode)
    {
        ChangeVisualMode(mode);
        if (mode == 0)
        {
            DeactivateSlide();
            ActivateTeleport();
        }
        else if(mode == 1)
        {
            DeactivateTeleport();
            ActivateSlide();
        }
    }

    private void ChangeVisualMode(int mode)
    {
        if (mode == 0) textModeDeplacement.text = "Téléportation";
        if (mode == 1) textModeDeplacement.text = "Glisser";
    }

    private void DeactivateTeleport()
    {
        foreach (GameObject interactor in teleportInteractors)
        {
            interactor.SetActive(false);
        }
    }

    private void DeactivateSlide()
    {
        slideInteractor.GetComponent<SimpleCapsuleWithStickMovement>().enabled = false;
    }

    private void ActivateTeleport()
    {
        foreach (GameObject interactor in teleportInteractors)
        {
            interactor.SetActive(true);
        }
    }

    private void ActivateSlide()
    {
        slideInteractor.GetComponent<SimpleCapsuleWithStickMovement>().enabled = true;
    }
}
