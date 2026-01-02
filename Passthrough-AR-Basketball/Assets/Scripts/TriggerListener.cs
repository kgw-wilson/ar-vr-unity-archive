using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class TriggerListener : MonoBehaviour
{
    /// This class listens for trigger input. To do so, it creates a list of input devices and chooses the first left and right devices respectively.
    /// These devices are stored as leftTarget/rightTarget InputDevices. The [GameObjects/Transforms] leftTarget and rightTarget are used in order to easily access
    /// the transform properties of the hands as they move. Several boolean variables are used to track whether or not the player can spawn a ball or a goal.
    /// Both the ball and the goal are prefabs that are stores in the My Prefabs folder.

    List<InputDevice> leftDevices = new List<InputDevice>();
    List<InputDevice> rightDevices = new List<InputDevice>();
    private InputDevice leftTarget;
    private InputDevice rightTarget;
    private InputDevice chosenTarget;
    public GameObject leftHandAnchor;
    public GameObject rightHandAnchor;
    private GameObject ball;
    private GameObject goal;
    public static bool canSpawnBall = true;
    public static bool ballHasSpawned = false;
    private bool canSpawnGoal = true;
    private bool goalWasSpawned = false;
    public GameObject ballPrefab;
    public GameObject goalPrefab;

    public Text debugText;


    // Get a reference to controller characteristics and use them to reference the left and right controllers if they are found
    void Start()
    {
        InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, leftDevices);

        InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, rightDevices);    

        if (leftDevices.Count > 0) {
            leftTarget = leftDevices[0];
        }
        if (rightDevices.Count > 0) {
            rightTarget = rightDevices[0];
        } 
    }


    // FixedUpdate is used here instead of Update because FixedUpdate triggers when the physics engine is triggered while Update triggers once a frame
    // Listens to trigger/grip input from controllers and spawns or releases balls or the goal
    void Update()
    {
        // Left grip pressed
        if (leftTarget.TryGetFeatureValue(CommonUsages.grip, out float leftGripValue) && leftGripValue > 0.1f) {
            if (canSpawnGoal) {
                if (!goalWasSpawned) {
                    goal = Instantiate(goalPrefab, leftHandAnchor.transform); // Spawn basketball goal
                    goalWasSpawned = true;
                }
                goal.transform.parent = leftHandAnchor.transform; // Make goal start following left controller movement
            }
        }

        // Left grip released 
        if (leftTarget.TryGetFeatureValue(CommonUsages.grip, out float leftGripVal) && leftGripVal < 0.05f) {
            if (goalWasSpawned) {
                goal.transform.parent = null; // Make the goal stop following left controller movement
            }
        }
        
        
        // Right hand trigger pressed
        if (rightTarget.TryGetFeatureValue(CommonUsages.trigger, out float rightTriggerValue) && rightTriggerValue > 0.1f) {            
                if (canSpawnBall) {
                    canSpawnBall = false;
                    SpawnBall(rightHandAnchor);
                }
        }

        // Right hand trigger released
        if (rightTarget.TryGetFeatureValue(CommonUsages.trigger, out float rightTriggerVal) && rightTriggerVal < 0.05f) {
            if (ballHasSpawned) {
                ThrowBall(rightTarget);
                ballHasSpawned = false;
            }
        }

        // Left hand trigger pressed
        if (leftTarget.TryGetFeatureValue(CommonUsages.trigger, out float leftTriggerValue) && leftTriggerValue > 0.1f) {
            debugText.text = "left pressed";
                if (canSpawnBall) {
                    canSpawnBall = false;
                    SpawnBall(leftHandAnchor);
                }
        }

        // Left hand trigger released
        if (leftTarget.TryGetFeatureValue(CommonUsages.trigger, out float leftTriggerVal) && leftTriggerVal < 0.05f) {
            if (ballHasSpawned) { 
                ThrowBall(leftTarget);
                ballHasSpawned = false;
            }
        }

    }


    // SpawnBall is called when a trigger is pressed
    void SpawnBall(GameObject handAnchor) {

        if (GameObject.FindWithTag("Ball") != null) { // if a ball exists
            // Destroy every other ball spawned so far
            foreach(GameObject ball in GameObject.FindGameObjectsWithTag("Ball")) {
                Destroy(ball);
            }
        }

        ball = Instantiate(ballPrefab, handAnchor.transform); // Spawn ball at hand
        ball.transform.parent = handAnchor.transform; // Make ball track the hand
        ball.GetComponent<Rigidbody>().useGravity = false;
        ballHasSpawned = true;
    }

    void ThrowBall(InputDevice handTarget) {
        ball.transform.parent = null; // Make the ball stop following controller movement
        ball.GetComponent<Rigidbody>().useGravity = true;
        if (handTarget.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 deviceVelocity)) {
            float forceMultiplier = 125; // This value just requires testing and tweaking            
            ball.GetComponent<Rigidbody>().AddRelativeForce(deviceVelocity * forceMultiplier);
        }
        canSpawnBall = true;
        ballHasSpawned = false;
    }
}