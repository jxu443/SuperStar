using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    public Transform respawnPoint;
    public GameObject player;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Respawn");
            player.transform.position = respawnPoint.transform.position;
        }
    }
}
