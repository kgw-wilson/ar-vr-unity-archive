using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTriggers : MonoBehaviour
{
    void OnCollisionEnter(Collision collidingObject) {
        if (collidingObject.gameObject.tag == "Ball") { // If a ball has entered this object
            Destroy(collidingObject.gameObject); //Destroy it
            TriggerListener.canSpawnBall = true;
        }
    }
}
