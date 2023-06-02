using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : PauseController
{
    public void RestartLevel()
    {
        SceneController.instance.ReloadCurrentScene();
        base.ResumeGame();
    }
}
