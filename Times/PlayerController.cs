using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public PathRecorder recorder;

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(x, y, 0).normalized;

        transform.position += direction * speed * Time.deltaTime;

        // 記錄當前位置
        recorder.RecordPosition(transform.position);
    }
}
