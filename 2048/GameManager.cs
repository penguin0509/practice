using System.Collections;
using TMPro;
using UnityEngine;
// GameManager.cs
// 管理整個 2048 遊戲流程，包括分數、重新開始、GameOver 顯示等功能
[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // 單例設計，供其他腳本引用

    [SerializeField] private TileBoard board;                // 連結 TileBoard，用於控制遊戲邏輯
    [SerializeField] private CanvasGroup gameOver;           // Game Over 畫面（淡入控制）
    [SerializeField] private TextMeshProUGUI scoreText;      // 當前分數文字
    [SerializeField] private TextMeshProUGUI hiscoreText;    // 最高分數文字

    public int score { get; private set; } = 0;              // 當前分數

    private void Awake()
    {
        // 確保只有一個 GameManager 實例存在
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        // 物件被銷毀時，清除單例實例
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame(); // 遊戲開始時自動啟動新遊戲
    }

    public void NewGame()
    {
        // 分數歸零
        SetScore(0);

        // 顯示歷史最高分
        hiscoreText.text = LoadHiscore().ToString();

        // 隱藏 Game Over 畫面
        gameOver.alpha = 0f;
        gameOver.interactable = false;

        // 重設棋盤
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()
    {
        // 停止棋盤操作，並顯示 Game Over 畫面
        board.enabled = false;
        gameOver.interactable = true;

        // 啟動畫面淡入效果
        StartCoroutine(Fade(gameOver, 1f, 1f));
    }

    // 控制畫面淡入淡出（用於 Game Over 畫面）
    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    // 增加分數
    public void IncreaseScore(int points)
    {
        SetScore(score + points);
    }

    // 設定分數並更新 UI
    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();

        SaveHiscore(); // 嘗試儲存最高分
    }

    // 儲存最高分（若分數比之前高）
    private void SaveHiscore()
    {
        int hiscore = LoadHiscore();

        if (score > hiscore)
        {
            PlayerPrefs.SetInt("hiscore", score);
        }
    }

    // 讀取最高分
    private int LoadHiscore()
    {
        return PlayerPrefs.GetInt("hiscore", 0);
    }

}
