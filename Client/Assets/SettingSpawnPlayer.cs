using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSpawnPlayer : MonoBehaviour
{
    public GameObject PlayerPrefab;

    PlayerDetails player;

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = Instantiate(PlayerPrefab);
        go.transform.position = transform.position;
        BoxCollider collider = go.GetComponent<BoxCollider>();

        player.Position = transform.position;
        player.ColliderWidth = collider.size.x;
        player.ColliderHeight = collider.size.y;
    }

    public void SetPlayer(PlayerDetails p)
    {
        player = p;
    }
}
