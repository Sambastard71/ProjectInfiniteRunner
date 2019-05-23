using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSpawners : MonoBehaviour
{
    public PlayerDetails MinePlayer;
    public PlayerDetails OtherPlayer;

    SettingSpawnPlayer[] playersSpawner;
    
    void Start()
    {
        playersSpawner = new SettingSpawnPlayer[2];
        for (int i = 0; i < transform.childCount; i++)
        {
            playersSpawner[i] = transform.GetChild(i).GetComponent<SettingSpawnPlayer>();
        }

        if (MinePlayer.MyIdInRoom == 1)
        {
            playersSpawner[0].SetPlayer(MinePlayer);
        }
        else
        {
            playersSpawner[1].SetPlayer(MinePlayer);
        }
    }
}
