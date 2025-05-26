using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TileBoard.cs
// �޲z��� 2048 �C�����޿�A�]�A��l�ͦ��B�X���޿�P��J����
public class TileBoard : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;         // �w�s�� tile ����
    [SerializeField] private TileState[] tileStates;  // �Ҧ� tile �����A�]�p�Ʀr�P�C��^

    private TileGrid grid;       // �ثe�ϥΪ���l����]TileCell �e���^
    private List<Tile> tiles;    // ���W�Ҧ� tile ���M��
    private bool waiting;        // �O�_���b���ݦX�ְʵe����

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();   // ���o�l���󤤪� TileGrid
        tiles = new List<Tile>(16);                  // ��l�� tile �M��]�̤j 4x4�^
    }

    public void ClearBoard()
    {
        // �M�ũҦ���l�P tiles
        foreach (var cell in grid.cells) cell.tile = null;
        foreach (var tile in tiles) Destroy(tile.gameObject);
        tiles.Clear();
    }

    public void CreateTile()
    {
        // �إ߷s tile �å[�J�H���Ů�l
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0]);                      // ��l���A�q�`�O�Ʀr 2
        tile.Spawn(grid.GetRandomEmptyCell());             // �H���ͦ�
        tiles.Add(tile);
    }

    private void Update()
    {
        if (waiting) return; // ���ݰʵe�ɤ�������J

        // �B�z WASD �Τ�V�䲾�ʿ�J
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

        // �̷Ӥ�V�P���ǹM���Ҧ���l
        for (int x = startX; x >= 0 && x < grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.Height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);
                if (cell.Occupied)
                    changed |= MoveTile(cell.tile, direction); // �Y���ܰʡA�O��
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
            tile.MoveTo(newCell); // ���������X��
            return true;
        }

        return false;
    }

    private bool CanMerge(Tile a, Tile b)
    {
        // �P�_�O�_�i�H�X�֡]�Ʀr�ۦP�B b �|���Q�X�֡^
        return a.state == b.state && !b.locked;
    }

    private void MergeTiles(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell); // ����ʵe�ò��� a

        // �]�w�s���A���U�@�š]�p 2 �� 4 �� 8�^
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
        yield return new WaitForSeconds(0.1f); // ���ݦX�ְʵe
        waiting = false;

        foreach (var tile in tiles) tile.locked = false;

        if (tiles.Count != grid.Size) CreateTile(); // �ͦ��s tile

        if (CheckForGameOver()) GameManager.Instance.GameOver();
    }

    public bool CheckForGameOver()
    {
        if (tiles.Count != grid.Size) return false; // �|���Ů�

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
