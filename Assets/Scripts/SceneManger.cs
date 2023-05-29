using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManger : MonoBehaviour
{
    public static SceneManger instance;
    GameObject settingMenu;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        settingMenu = GameObject.FindGameObjectWithTag("Settings");
    }

    public void OpenSettings()
    {
        settingMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        settingMenu.SetActive(false);
    }
}
