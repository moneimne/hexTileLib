using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float minHeight;
    public float maxHeight;
    public float angle;
    public float speed;

    float screenThreshold = 0.99f;
    float mapHalfWidth = 20.0f;
    float mapHalfHeight = 20.0f;
    
	void Start () {
        transform.position = new Vector3(0.0f, maxHeight, 0.0f);
        transform.rotation = Quaternion.Euler(new Vector3(angle, 0.0f, 0.0f));
	}

    void goToPosition(float x, float z) {
        transform.position = new Vector3(x, transform.position.y, z);
    }

    void moveInDirection(Vector3 direction) {
        transform.Translate(direction * speed, Space.World);
        float xPosition = Mathf.Clamp(transform.position.x, -mapHalfWidth, mapHalfWidth);
        float zPosition = Mathf.Clamp(transform.position.z, -mapHalfHeight, mapHalfHeight);
        transform.position = new Vector3(xPosition, transform.position.y, zPosition);
    }
	
	void FixedUpdate () {
        // Camera panning
        if (Input.mousePosition.x < Screen.width * (1.0f - screenThreshold)) {
            moveInDirection(-Vector3.right);
        }
        else if (Input.mousePosition.x > Screen.width * screenThreshold) {
            moveInDirection(Vector3.right);
        }
        if (Input.mousePosition.y < Screen.height * (1.0f - screenThreshold)) {
            moveInDirection(-Vector3.forward);
        }
        else if (Input.mousePosition.y > Screen.height * screenThreshold) {
            moveInDirection(Vector3.forward);
        }

        // Camera zooming
        if (Input.mouseScrollDelta.y > 0.0f && transform.position.y > minHeight) {
            moveInDirection(transform.forward);
        }
        else if (Input.mouseScrollDelta.y < 0.0f && transform.position.y < maxHeight) {
            moveInDirection(-transform.forward);
        }
	}
}
