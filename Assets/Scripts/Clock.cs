using UnityEngine;
using System.Collections;

public class Clock : MonoBehaviour {

    [SerializeField] public GameObject hud;

    public int minutes = 0;
    public int hour = 0;
    
    public float clockSpeed = 1.0f;     // 1.0f = realtime, < 1.0f = slower, > 1.0f = faster
    
    int seconds;
    float msecs;
    GameObject pointerSeconds;
    GameObject pointerMinutes;
    GameObject pointerHours;

    void Start() {
        pointerSeconds = transform.Find("rotation_axis_pointer_seconds").gameObject;
        pointerMinutes = transform.Find("rotation_axis_pointer_minutes").gameObject;
        pointerHours   = transform.Find("rotation_axis_pointer_hour").gameObject;

        msecs = 0.0f;
        seconds = 0;
    }

    void Update() {
        if (!MainMenu.Pause) {
            msecs += Time.deltaTime * clockSpeed;
            if (msecs >= 1.0f) {
                msecs -= 1.0f;
                seconds--;
                if (seconds <= -60) {
                    seconds = 0;
                    minutes--;
                    if (minutes < -60) {
                        minutes = 0;
                        hour--;
                        if (hour <= -24)
                            hour = 0;
                    }
                }
            }


            float rotationSeconds = (360.0f / 60.0f) * seconds;
            float rotationMinutes = (360.0f / 60.0f) * minutes;
            float rotationHours = ((360.0f / 12.0f) * hour) + ((360.0f / (60.0f * 12.0f)) * minutes);

            pointerSeconds.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotationSeconds);
            pointerMinutes.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotationMinutes);
            pointerHours.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotationHours);

            if (hour == 0 && minutes == 0 && seconds == 0) {
                transform.parent.parent.gameObject.SetActive(false);
                MainMenu.Pause = false;
                MainMenu.Interactable = true;
                if (hud) {
                    hud.GetComponent<HUD>().CloseTimer();
                } else {
                    Debug.LogError("hud not found");
                }
            }
        }
    }

}
