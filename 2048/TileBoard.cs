using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TileBoard.cs
// 管理整個 2048 遊戲的邏輯，包括格子生成、合併邏輯與輸入控制
public class TileBoard : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;         // 預製的 tile 物件
    [SerializeField] private TileState[] tileStates;  // 所有 tile 的狀態（如數字與顏色）

    private TileGrid grid;       // 目前使用的格子網格（TileCell 容器）
    private List<Tile> tiles;    // 場上所有 tile 的清單
    private bool waiting;        // 是否正在等待合併動畫結束

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();   // 取得子物件中的 TileGrid
        tiles = new List<Tile>(16);                  // 初始化 tile 清單（最大 4x4）
    }

    public void ClearBoard()
    {
        // 清空所有格子與 tiles
        foreach (var cell in grid.cells) cell.tile = null;
        foreach (var tile in tiles) Destroy(tile.gameObject);
        tiles.Clear();
    }

    public void CreateTile()
    {
        // 建立新 tile 並加入隨機空格子
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0]);                      // 初始狀態通常是數字 2
        tile.Spawn(grid.GetRandomEmptyCell());             // 隨機生成
        tiles.Add(tile);
    }

    private void Update()
    {
        if (waiting) return; // 等待動畫時不接受輸入

        // 處理 WASD 或方向鍵移動輸入
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            Move(Vector2Int.up, 0, 1, 1, 1);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            Move(Vector2Int.left, 1, 1, 0, 1);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            Move(Vector2Int.down, 0, 1, grid.Height - 2, -1);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            Move(Vector2Int.right, grid.Width - 2, -1, 0, 1);
    }

    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        // 依照方向與順序遍歷所有格子
        for (int x = startX; x >= 0 && x < grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.Height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);
                if (cell.Occupied)
                    changed |= MoveTile(cell.tile, direction); // 若有變動，記錄
            }
        }

        if (changed) StartCoroutine(WaitForChanges());
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.Occupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    MergeTiles(tile, adjacent.tile);
                    return true;
                }
                break;
            }
            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell); // 平移但不合併
            return true;
        }

        return false;
    }

    private bool CanMerge(Tile a, Tile b)
    {
        // 判斷是否可以合併（數字相同且 b 尚未被合併）
        return a.state == b.state && !b.locked;
    }

    private void MergeTiles(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell); // 播放動畫並移除 a

        // 設定新狀態為下一級（如 2 → 4 → 8）
        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        TileState newState = tileStates[index];
        b.SetState(newState);

        GameManager.Instance.IncreaseScore(newState.number);
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
            if (state == tileStates[i]) return i;
        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        waiting = true;
        yield return new WaitForSeconds(0.1f); // 等待合併動畫
        waiting = false;

        foreach (var tile in tiles) tile.locked = false;

        if (tiles.Count != grid.Size) CreateTile(); // 生成新 tile

        if (CheckForGameOver()) GameManager.Instance.GameOver();
    }

    public bool CheckForGameOver()
    {
        if (tiles.Count != grid.Size) return false; // 尚有空格

        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if ((up != null && CanMerge(tile, up.tile)) ||
                (down != null && CanMerge(tile, down.tile)) ||
                (left != null && CanMerge(tile, left.tile)) ||
                (right != null && CanMerge(tile, right.tile)))
                return false;
        }

        return true;
    }

}
