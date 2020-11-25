using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    public Vector3 camPosition = Vector3.zero;
    public Vector3 testPosition = Vector3.zero;

    public Quaternion testAngle;
    public GameObject player;

    public bool cameraSet1 = true;
    public bool cameraSet2 = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(cameraSet1){
            camPosition = new Vector3(player.transform.position.x, 8f, player.transform.position.z - 2f);
            transform.position = camPosition;
            transform.rotation = Quaternion.Euler(70f, 0f, 0f);
        }
        
        if(cameraSet2){
            camPosition = new Vector3(player.transform.position.x - 4f, 8f, player.transform.position.z - 1f);
            transform.position = camPosition;
            transform.rotation = Quaternion.Euler(70f, 90f, 0f);
        }
        

        if(Input.GetKeyDown("space")){
            cameraSet1 = !cameraSet1;
            cameraSet2 = !cameraSet2;
        }
    }
}
