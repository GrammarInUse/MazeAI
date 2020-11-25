﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsForWall : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        player.transform.position = Vector3.MoveTowards(player.transform.position, player.transform.position, 10 * Time.deltaTime);
    }
}