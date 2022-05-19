using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public GameObject player;
    public bool on;

    /// <summary>
    /// Initialize on variable to activate script
    /// </summary>
    void Start()
    {
        on = true;
    }

    /// <summary>
    /// Look at player if on from different angles of the camera
    /// </summary>
    void Update()
    {
        if (on)
            transform.LookAt(player.transform); 
    }
}
