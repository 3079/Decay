using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{
    static int PENTAGRAMS = 0;

    public static void Pentagram()
    {
        PENTAGRAMS++;

        if(PENTAGRAMS == 3)
            SceneManager.LoadScene("EndScreen");
    }

    public static void Lose()
    {
        SceneManager.LoadScene("EndScreen");
    }
}
