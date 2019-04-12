using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;


public class ExitTrigger : MonoBehaviour
{
    public GameObject carCam;
    public GameObject player;
    public GameObject exitTrigger;
    public GameObject car;
    public Transform exitPlace;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.B))
        {
            //player = GameObject.FindGameObjectWithTag("Player");
            player.SetActive(true);
            player.transform.position = exitPlace.position;
            CarController carController = car.GetComponent<CarController>();
            if (carController)
            {
                carController.enabled = false;
            }
            CarUserControl carUserController = car.GetComponent<CarUserControl>();
            if (carUserController)
            {
                carUserController.enabled = false;
            }
            exitTrigger.SetActive(false);
            carCam.SetActive(false);
        }
        
    }
}
