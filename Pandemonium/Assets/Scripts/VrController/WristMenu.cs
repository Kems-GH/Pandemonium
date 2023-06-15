using UnityEngine;

public class WristMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    private bool menuIsOpen = false;
    private bool startIsPressed;

    void Update()
    {
        if (startIsPressed == OVRInput.Get(OVRInput.Button.Start)) return;
        
        startIsPressed = OVRInput.Get(OVRInput.Button.Start);
        
        if (startIsPressed)
            if(menuIsOpen) CloseMenu(); else OpenMenu();
    }

    private void OpenMenu()
    {
        menuIsOpen = true;
        menu.SetActive(true);
    }

    private void CloseMenu()
    {
        menuIsOpen = false;
        menu.SetActive(false);
    }
}
