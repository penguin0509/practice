using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRecorder : MonoBehaviour
{
    private List<Vector3> currentPath = new List<Vector3>();
    public static List<List<Vector3>> pathHistory = new List<List<Vector3>>();

    public void RecordPosition(Vector3 pos)
    {
        currentPath.Add(pos);
    }

    public void EndPath()
    {
        // 保存這次通關的路徑
        pathHistory.Add(new List<Vector3>(currentPath));
        currentPath.Clear();
    }
}
