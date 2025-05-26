using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowGhost : MonoBehaviour
{
    public float speed = 4f;
    private List<Vector3> path;
    private int index = 0;

    public void SetPath(List<Vector3> pathData)
    {
        path = pathData;
        transform.position = path[0];
        index = 1;
    }

    void Update()
    {
        if (path == null || index >= path.Count)
        {
            Destroy(gameObject); // ��F���I�۰ʮ���
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, path[index], speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, path[index]) < 0.05f)
        {
            index++;
        }
    }
}
