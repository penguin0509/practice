using System.Collections;
using TMPro;
using UnityEngine;
// GameManager.cs
// �޲z��� 2048 �C���y�{�A�]�A���ơB���s�}�l�BGameOver ��ܵ��\��
[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // ��ҳ]�p�A�Ѩ�L�}���ޥ�

    [SerializeField] private TileBoard board;                // �s�� TileBoard�A�Ω󱱨�C���޿�
    [SerializeField] private CanvasGroup gameOver;           // Game Over �e���]�H�J����^
    [SerializeField] private TextMeshProUGUI scoreText;      // ��e���Ƥ�r
    [SerializeField] private TextMeshProUGUI hiscoreText;    // �̰����Ƥ�r

    public int score { get; private set; } = 0;              // ��e����

    private void Awake()
    {
        // �T�O�u���@�� GameManager ��Ҧs�b
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
        // ����Q�P���ɡA�M����ҹ��
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame(); // �C���}�l�ɦ۰ʱҰʷs�C��
    }

    public void NewGame()
    {
        // �����k�s
        SetScore(0);

        // ��ܾ��v�̰���
        hiscoreText.text = LoadHiscore().ToString();

        // ���� Game Over �e��
        gameOver.alpha = 0f;
        gameOver.interactable = false;

        // ���]�ѽL
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()
    {
        // ����ѽL�ާ@�A����� Game Over �e��
        board.enabled = false;
        gameOver.interactable = true;

        // �Ұʵe���H�J�ĪG
        StartCoroutine(Fade(gameOver, 1f, 1f));
    }

    // ����e���H�J�H�X�]�Ω� Game Over �e���^
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

    // �W�[����
    public void IncreaseScore(int points)
    {
        SetScore(score + points);
    }

    // �]�w���ƨç�s UI
    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();

        SaveHiscore(); // �����x�s�̰���
    }

    // �x�s�̰����]�Y���Ƥ񤧫e���^
    private void SaveHiscore()
    {
        int hiscore = LoadHiscore();

        if (score > hiscore)
        {
            PlayerPrefs.SetInt("hiscore", score);
        }
    }

    // Ū���̰���
    private int LoadHiscore()
    {
        return PlayerPrefs.GetInt("hiscore", 0);
    }

}
