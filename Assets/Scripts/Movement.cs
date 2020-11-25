using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassManagement;

public class Movement : MonoBehaviour
{
    public GameObject T;
    public Vector3 lookAtTarget;
    public Vector3 targetPosition;
    public bool moving = false;
    public Player A;
    // Start is called before the first frame update
    void Start()
    {
        T = GameObject.Find("Player");
        A = new Player(T);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1)){
            A.lookAtTargetPosition();
        }
        if(A.Moving){
            A.move();
        }
    }
}
