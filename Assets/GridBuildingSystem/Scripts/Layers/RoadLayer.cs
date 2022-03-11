using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGrid;


public class RoadLayer : MonoBehaviour
{
    int gridSize;
    int numberOfCells;
    private readonly float opacity = 1f;
    private Color[] BRGBW;
    [SerializeField] private bool buildRoadOn;

    [SerializeField] GameObject roadBuilderPrefab;
    public TileObjectObjectPool tilePool;
    

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Vector3 clickPosition;
    [SerializeField] private int clickX = -1;
    [SerializeField] private int clickZ = -1;
    [SerializeField] private int mouseX = -1;
    [SerializeField] private int mouseZ = -1;
    [SerializeField] private int xDif;
    [SerializeField] private int zDif;


    [SerializeField] Vector3 bPos;

    [SerializeField] private PlacedObjectTypeSO placedObject;
    List<TileObject> roadTilesToBuild;

    GridXZ<RoadTileObject> grid;
    TileObject a;
    TileObject b;
    TileObject c;

    public RoadLayer(int _gridSize, int _numberOfCells) {
        gridSize = LayerManager.singleton.TerrainSize;
        numberOfCells = LayerManager.singleton.NumberOfCells;
        BRGBW = Resources.Load<Texture2D>("Texture/BRGBWStrip").GetPixels();

    }

    // Start is called before the first frame update
    void Start()
    {
        roadTilesToBuild = new List<TileObject>();
        a = tilePool.GetPrefabInstance();
        a.name = "Tile A";
        b = tilePool.GetPrefabInstance();
        b.name = "Tile B";
        c = tilePool.GetPrefabInstance();
        c.name = "Tile C";

        int width = LayerManager.singleton.Width;
        int height = LayerManager.singleton.Height;
        float cellSize = LayerManager.singleton.CellSize;


        grid = new GridXZ<RoadTileObject>(Mathf.FloorToInt(width / cellSize),
                            Mathf.FloorToInt(height / cellSize), cellSize, Vector3.zero,
                            (GridXZ<RoadTileObject> g, int x, int z) => new RoadTileObject(g, x, z));
        buildRoadOn = false;
        float s = LayerManager.singleton.CellSize;

        StartCoroutine(BuildRoad());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            buildRoadOn = !buildRoadOn;
            roadBuilderPrefab.SetActive(buildRoadOn);
        }

        if (Input.GetMouseButtonDown(0)) {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 999f, layerMask)) {
                buildRoadOn = true;
                clickPosition = hit.point;
                LayerManager.singleton.GetXZ(hit.point, out clickX, out clickZ);
                Vector3 start = LayerManager.singleton.GetWorldPosition(clickX, clickZ);
                a.transform.position = start + new Vector3(0, 0.1f, 0);
                a.gameObject.SetActive(true);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            buildRoadOn = false;
            roadBuilderPrefab.SetActive(buildRoadOn);
            clickPosition = Vector3.zero;
            a.ReturnToPool();
            b.ReturnToPool();
            c.ReturnToPool();
        }
    }

    private void CreateRoadTile(Vector3 pos) {
        GameObject obj = Instantiate(roadBuilderPrefab, pos, Quaternion.identity);
        obj.SetActive(true);
        //roadTilesToBuild.Add(obj);
    }
    private void ClearRoadTiles(List<TileObject> list) {
        foreach (TileObject tile in list.ToArray()) {
            tile.ReturnToPool();
        }
    }

    IEnumerator BuildRoad() {
        Vector3 cPos = Vector3.zero;
        while (true) {
            if (buildRoadOn && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 999f, layerMask))
            {
                // create anchor
                Vector3 currentMousePosition = hit.point;
                LayerManager.singleton.GetXZ(currentMousePosition, out int mouseX, out int mouseZ);

                xDif = clickX - mouseX;
                zDif = clickZ - mouseZ;


                a.GetComponentInChildren<MeshRenderer>().material.SetFloat("Index", 1);
                if (xDif != 0 && zDif != 0)
                {

                    if (Mathf.Abs(xDif) >= Mathf.Abs(zDif))
                    {
                        bPos = LayerManager.singleton.GetWorldPosition(clickX, mouseZ);
                        b.GetComponentInChildren<MeshRenderer>().material.SetFloat("Index", 3);
                        c.GetComponentInChildren<MeshRenderer>().material.SetFloat("Index", 1);
                    }
                    else
                    {
                        bPos = LayerManager.singleton.GetWorldPosition(mouseX, clickZ);
                        b.GetComponentInChildren<MeshRenderer>().material.SetFloat("Index", 3);
                        c.GetComponentInChildren<MeshRenderer>().material.SetFloat("Index", 1);
                    }

                    cPos = LayerManager.singleton.GetWorldPosition(mouseX, mouseZ);
                    b.gameObject.SetActive(true);
                    c.gameObject.SetActive(true);

                }
                else if (xDif == 0 && zDif != 0)
                {
                    bPos = LayerManager.singleton.GetWorldPosition(clickX, clickZ);
                    b.gameObject.SetActive(false);
                    cPos = LayerManager.singleton.GetWorldPosition(mouseX, mouseZ);
                    c.gameObject.SetActive(true);
                    c.GetComponentInChildren<MeshRenderer>().material.SetFloat("Index", 1);

                }
                else if (xDif !=0 && zDif == 0)
                {
                    bPos = LayerManager.singleton.GetWorldPosition(clickX, clickZ);
                    b.gameObject.SetActive(false);
                    cPos = LayerManager.singleton.GetWorldPosition(mouseX, mouseZ);
                    c.gameObject.SetActive(true);
                    c.GetComponentInChildren<MeshRenderer>().material.SetFloat("Index", 1);

                }
                else {
                    a.GetComponentInChildren<MeshRenderer>().material.SetFloat("Index", 0);
                    b.gameObject.SetActive(false);
                    c.gameObject.SetActive(false);

                }


                b.transform.position = bPos + new Vector3(0, 0.1f, 0);
                c.transform.position = cPos + new Vector3(0, 0.1f, 0);
                Debug.DrawLine(clickPosition, bPos, Color.green, 1f / 30);
                Debug.DrawLine(bPos, currentMousePosition, Color.green, 1f / 30);
            }
            yield return new WaitForSeconds(1f / 30);
        }
    }

    IEnumerator MouseUpdate()
    {
        while (true)
        {
            if (buildRoadOn)
            {

                Vector3 mousePos = CodeMonkey.Utils.UtilsClass.GetMouseWorldPosition();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    LayerManager.singleton.GetXZ(hit.point, out int x, out int z);
                    Debug.Log("Raycast hit at: " + hit.point + " at gridcell (" + x + ", " + z + ")");
                    roadBuilderPrefab.transform.position = LayerManager.singleton.GetWorldPosition(x, z) + new Vector3(0f, LayerManager.layerData[x,z].elevation, 0f);

                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}

[System.Serializable]
public class RoadTileObject {
    private GridXZ<RoadTileObject> grid;

    public int x;
    public int Z;
    public Material material;
    public GameObject prefab;

    public RoadTileObject(GridXZ<RoadTileObject> _g, int _x, int _z) {
        grid = _g;
        x = _x;
        Z = _z;
    }

    public void SetTile(GameObject _prefab) {
        prefab = _prefab;
        material = prefab.GetComponent<MeshRenderer>().material;
    }

    public void ClearTile() {
        prefab = null;
        material = null;
    }

    public bool CanBuild() {
        return prefab == null;
    }

    public GameObject GetTileGameObject() {
        return prefab;
    }

    public static Dir GetNextDir(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return Dir.Left;
            case Dir.Left: return Dir.Up;
            case Dir.Up: return Dir.Right;
            case Dir.Right: return Dir.Down;
        }
    }

    public enum Dir
    {
        Down,
        Left,
        Up,
        Right,
    }

    public int GetRotationAngle(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return 0;
            case Dir.Left: return 90;
            case Dir.Up: return 180;
            case Dir.Right: return 270;
        }
    }
    public static Vector2Int GetRotationOffset(Dir dir)
    {
        switch (dir)
        {
            default:
            case Dir.Down: return new Vector2Int(0, 0);
            case Dir.Left: return new Vector2Int(0, 1);
            case Dir.Up: return new Vector2Int(1, 1);
            case Dir.Right: return new Vector2Int(1, 0);
        }
    }
}
