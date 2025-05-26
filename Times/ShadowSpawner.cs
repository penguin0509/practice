using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSpawner : MonoBehaviour
{
    public GameObject shadowPrefab;
    private int levelCount = 0;

    private Coroutine spawnRoutine;

    public void OnLevelCompleted()
    {
        levelCount++;

        // �}�l�C3��ͦ��v�l
        if (spawnRoutine == null)
        {
            spawnRoutine = StartCoroutine(SpawnShadows());
        }
    }

    IEnumerator SpawnShadows()
    {
        while (true)
        {
            float baseInterval = 3f;
            float extraInterval = Mathf.Max(0.5f, 3f - ((levelCount / 3) * 2f)); // �C3����2��

            yield return new WaitForSeconds(extraInterval);

            int index = PathRecorder.pathHistory.Count - 1;
            if (index >= 0)
            {
                GameObject ghost = Instantiate(shadowPrefab);
                ShadowGhost ghostScript = ghost.GetComponent<ShadowGhost>();
                ghostScript.SetPath(PathRecorder.pathHistory[index]);
            }
        }
    }
}
