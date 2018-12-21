using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {

    private bool goUp;
    [SerializeField] public float speed = 1;
    [SerializeField] public float rotationSpeed = 1;
    private Color initialColor;
    private Vector3 screenPoint;
    private Vector3 offset;
    private bool dragging = false;
    
    // Use this for initialization
    void Start () {
        (gameObject.GetComponent("Halo") as Behaviour).enabled = false;
        StartCoroutine(SwitchDirection());
    }
	
	// Update is called once per frame
	void Update () {
        if (!dragging) {
            if (goUp) {
                transform.position = transform.position + new Vector3(0, speed * Time.deltaTime, 0);
            } else {
                transform.position = transform.position - new Vector3(0, speed * Time.deltaTime, 0);
            }
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    IEnumerator SwitchDirection() {
        while (gameObject.activeSelf) {
            yield return new WaitForSeconds(0.5f);
            goUp = !goUp;
        }
    }

    void OnMouseEnter() {
        if (!MainMenu.Pause && MainMenu.Interactable) {
            (gameObject.GetComponent("Halo") as Behaviour).enabled = true;
        }
    }

    void OnMouseExit() {
        (gameObject.GetComponent("Halo") as Behaviour).enabled = false;
    }

    void OnMouseDown() {
        if (!MainMenu.Pause && MainMenu.Interactable) {
            (gameObject.GetComponent("Halo") as Behaviour).enabled = false;
            screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            transform.position += new Vector3(0, 1, 0);
        }
    }

    void OnMouseUp() {
        if (!MainMenu.Pause && MainMenu.Interactable && dragging) {
           transform.position -= new Vector3(0, 1, 0);
            dragging = false;
        }
    }

    void OnMouseDrag() {
        if (!MainMenu.Pause && MainMenu.Interactable) {
            dragging = true;
            screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            transform.position = new Vector3(curPosition.x, transform.position.y, curPosition.z);
        }
    }
    
}
