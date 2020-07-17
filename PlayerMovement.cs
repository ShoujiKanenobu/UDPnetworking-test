using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movespeed;

    //VERY simple code to move around. 
    void Update()
    {
        float horizonal = Input.GetAxis("Horizontal") * movespeed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * movespeed * Time.deltaTime;
        Vector3 temp = new Vector3(horizonal, 0, vertical);
        this.transform.Translate(temp);
    }
}
