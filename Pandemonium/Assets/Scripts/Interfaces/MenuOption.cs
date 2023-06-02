using UnityEngine;
using TMPro;

public class MenuOption : MonoBehaviour
{
    public delegate void ChangeModeEventHandler(int mode);

    public event ChangeModeEventHandler ChangeModeHandler;

    private int mode = 0;
    

    public void ExitGame()
    {
        Application.Quit();
    }

    public void GoToLobby()
    {
        Debug.Log("Go to Lobby");
    }

    public void ChangeModeDeplacement()
    {
        if(mode == 0) mode++;
        else if(mode == 1) mode--;

        ChangeModeHandler?.Invoke(mode);
    }
}
