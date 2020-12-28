﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void LoadScene(string scene) {
        SceneManager.LoadScene( scene );
    }

    public void CloseGame() {
        Application.Quit( 0 );
    }
}