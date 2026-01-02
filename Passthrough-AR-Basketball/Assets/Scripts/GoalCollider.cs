using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalCollider : MonoBehaviour
{
    private Text text;

    void Start() {
        text = GameObject.Find("Debug").GetComponent<Text>();
    }

    void OnCollisionEnter(Collision other) {
        Destroy(other.gameObject);
        StartCoroutine(UpdateText());
    }
    
    IEnumerator UpdateText() {
        text.text = "GOOOOAAALL!!!!";
        TriggerListener.canSpawnBall = true;
        yield return new WaitForSeconds(2);
        text.text = "";
    }
}
