using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CarTrigger : MonoBehaviour
{
    public GameObject carCam;
    private GameObject player;
    public GameObject exitTrigger;
    public GameObject car;
    public int triggerCheck;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == player.tag)
        {
            Debug.Log("vao trigger");
            triggerCheck = 1;

        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == player.tag)
        {
            Debug.Log("ra trigger");
            triggerCheck = 0;

        }
    }
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    // Update is called once per frame
    void Update()
    {
       
        if (triggerCheck ==1)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                Debug.Log("vao enter");

                carCam.SetActive(true);
                player.SetActive(false);
                CarController carController = car.GetComponent<CarController>();
                if (carController)
                {
                    carController.enabled = true;
                }
                CarUserControl carUserController = car.GetComponent<CarUserControl>();
                if (carUserController)
                {
                    carUserController.enabled = true;
                }
                exitTrigger.SetActive(true);
               
            }
        }
    }
}
