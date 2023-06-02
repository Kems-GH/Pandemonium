using UnityEngine;
public class WaveChecker
{
    public static bool CheckWaveFinished()
    {

        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            return true;
        }
        return false;
    }
}