using System.Collections.Generic;
using N.GridSystems;
using UnityEngine;

public class ExampleGrid : MonoBehaviour
{
    public GameObject Tile;
    public GridType GridType;
    public PlaneType PlaneType;
    public Vector2Int GridSize;
    public Vector2 CellSize;
    public float Spacing;
    public bool draw500s;
    private GridSystem<GameObject> gridSystem;
    private List<GameObject> pool = new(); 
    void Start()
    {
        SpawnDemo();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            SpawnDemo();
    }

    void SpawnDemo()
    {
        gridSystem = GridSystem<GameObject>.GenerateGrid(GridType, PlaneType, GridSize, CellSize, Spacing, transform.position, draw500s);
        GenerateCell();
    }

    void GenerateCell()
    {
        for (int i = 0; i < pool.Count; i++)
            Destroy(pool[i].gameObject);

        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                GameObject cell = Instantiate(this.Tile, this.transform);
                cell.name = $"Cell ({x}x{y})";
                gridSystem.SetTileValue(x, y, cell);
                cell.transform.SetPositionAndRotation(gridSystem.GetCenterWorldPositionOfTile(x, y), Quaternion.identity);
                cell.gameObject.SetActive(true);
                pool.Add(cell);
            }
        }
    }
}
