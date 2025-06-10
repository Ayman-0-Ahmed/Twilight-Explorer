using UnityEngine;

public class SunManager : MonoBehaviour {
    [SerializeField] float cycleMinutes = 6f; // Duration of a full day cycle in minutes
    [SerializeField, Range(0f, 24f)] float TimeOfDay;
    void Start() {
    }

    void Update() {
        if (Application.isPlaying){
            float sunAngle = TimeOfDay / 24f * 360f;
            transform.rotation = Quaternion.Euler(sunAngle-90f, 90f, 0f);
            TimeOfDay += Time.deltaTime / 60f * (24f / cycleMinutes);
            TimeOfDay %= 24f;
        }                
    }
}
