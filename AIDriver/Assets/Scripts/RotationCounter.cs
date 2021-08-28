using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotationCounter : MonoBehaviour
{
    
    void FixedUpdate()
    {
       
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0); ;
       
    }

    void LateUpdate()
    {

        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0); ;

    }
}
