using System;
using System.Collections.Generic;
using System.IO;
using FullSerializer;
using UnityEngine;

[Serializable]
public enum Tile {
    NORMAL,
    PATH,
    START,
    FINISH,
}
[Serializable]
public class TowerData {
    public Vector2 position = new Vector2();
    public int upgradeLevel = 0;
    public TowerType type = TowerType.TURRET_1;
}
[Serializable]
public class WaveletData {
    public float timeOffset = 0f;
    public int enemiesCount = 10;
    public float spawnRate = 3f;
    public EnemyType enemyType = new EnemyType();
}
[Serializable]
public class WaveData {
    public List<WaveletData> wavelets = new List<WaveletData>();
}
[Serializable]
public class WavechainData {
    public List<WaveData> waves = new List<WaveData>();
}
[Serializable]
public class LevelData {
    public int id = -1;
    public string filename = "";
    public List<List<Tile>> grid = new List<List<Tile>>();
    public List<Vector2> waypoints = new List<Vector2>();
    public int width = 0;
    public int length = 0;

    public Vector2 startPos = new Vector2();
    public Vector2 finishPos = new Vector2();

    public List<TowerData> towers = new List<TowerData>();
    public WavechainData waves = new WavechainData();

    public int startingMoney = 450;

    public Tile At(Vector2 position) {
        return grid[(int)position.y][(int)position.x];
    }
}

public class LevelManager : MonoBehaviour {
    public enum SaveMode {
        SaveInitialLevelData,
        SaveCurrentLevelData,
    }

    //------------- Events -------------//
    public delegate void OnTowerPlaced();
    public delegate void OnLevelLoaded();

    public event OnTowerPlaced TowerPlaced;
    public event OnLevelLoaded LevelLoaded;

    private void TriggerTowerPlaced()
    {
        if (TowerPlaced != null) TowerPlaced();
    }
    private void TriggerLevelLoaded()
    {
        if (LevelLoaded != null) LevelLoaded();
    }
    //------------- Events -------------//

    private Vector3 brickSize;
    private Vector3 blockSize;
    public float spacing;

    [Header("Prefabs")]
    public GameObject Normal_Brick;
    public GameObject Path_Brick;
    public GameObject Start_Block;
    public GameObject Finish_Block;
    public GameObject Waypoint;
    public GameObject Wave_Spawner;

    [HideInInspector] public LevelData initialLevelData;
    [HideInInspector] public LevelData currentLevelData;

    [HideInInspector] public WaveSpawner waveSpawner;
    [HideInInspector] public GameObject waypointList;
    [HideInInspector] public GameObject towerList;

    [HideInInspector] public List<GameObject> placedTiles;
    private List<GameObject> placedNormal;
    private List<GameObject> placedPath;

    [HideInInspector] public Vector3 centerPoint;

    public void Awake() {
        brickSize   = Normal_Brick.GetComponent<Renderer>().bounds.size;
        blockSize   = Start_Block.GetComponent<Renderer>().bounds.size;
        waveSpawner = null;
        placedTiles = new List<GameObject>();
        placedNormal = new List<GameObject>();
        placedPath = new List<GameObject>();
    }

    //---------------- Level Load/Save ----------------//

    public void CopyLevelsToDisk(int firstLevel, int lastLevel) {
        for (int i = firstLevel; i <= lastLevel; i++) {
            string filename = "Level" + i;
            //Debug.Log("Trying to load " + filename + " from the Resources folder");
            TextAsset fileContent = Resources.Load(filename) as TextAsset;
            if(fileContent == null) {
                Debug.LogError("Could not load level " + filename + ".txt from resources");
                return;
            }
            string pathToLevel = Application.persistentDataPath + "/" + filename + ".dat";
            StreamWriter writer = new StreamWriter(pathToLevel);
            writer.Write(fileContent.text);
            writer.Close();
            Debug.Log("Successfully moved level " + pathToLevel + " out of ressources and into " + pathToLevel);
        }
    }
    public void ReloadLevel() {
        UnbuildLevel();

        fsData data;
        fsSerializer fs = new fsSerializer();
        fs.TrySerialize(initialLevelData, out data);
        fs.TryDeserialize(data, ref currentLevelData);

        centerPoint = Vector3.zero;

        TriggerLevelLoaded();

        BuildLevel();
        Debug.Log("Successfully reloaded level " + initialLevelData.filename);
    }
    public void LoadLevel(string filename) {
        if (currentLevelData != null && filename == currentLevelData.filename) {
            Debug.Log("Level `" + filename + "` is already loaded");
            return;
        }
        string pathToLevel = Application.persistentDataPath + "/" + filename;
        if (!File.Exists(pathToLevel)) {
            Debug.LogError("The level `" + pathToLevel + "` you are trying to load cannot be found.");
            return;
        }
        UnbuildLevel();
        StreamReader reader = new StreamReader(pathToLevel);
        fsData data = fsJsonParser.Parse(reader.ReadToEnd());
        fsSerializer fs = new fsSerializer();
        fs.TryDeserialize(data, ref currentLevelData);
        fs.TryDeserialize(data, ref initialLevelData);
        reader.Close();

        centerPoint = Vector3.zero;
        TriggerLevelLoaded();
    }
    public void SaveLevel(string filename, SaveMode saveMode = SaveMode.SaveCurrentLevelData) {
        string pathToLevel = Application.persistentDataPath + "/" + filename + ".dat";
        StreamWriter writer = new StreamWriter(pathToLevel);

        fsData data;    
        fsSerializer fs = new fsSerializer();
        if (saveMode == SaveMode.SaveInitialLevelData) {
            fs.TrySerialize(initialLevelData, out data);
            fsJsonPrinter.PrettyJson(data, writer);
        } else if (saveMode == SaveMode.SaveCurrentLevelData) {
            fs.TrySerialize(currentLevelData, out data);
            fsJsonPrinter.PrettyJson(data, writer);
        }
        Debug.Log("Saved level " + filename + " to " + pathToLevel);
        writer.Close();
    }
    public void SaveLevelFrom(LevelData levelData) {
        string pathToLevel = Application.persistentDataPath + "/" + levelData.filename + ".dat";
        StreamWriter writer = new StreamWriter(pathToLevel);

        fsData data;
        fsSerializer fs = new fsSerializer();
        fs.TrySerialize(levelData, out data);
        fsJsonPrinter.PrettyJson(data, writer);

        writer.Close();
    }

    private Vector2 NextDirectionClockwise(Vector2 currentDirection) {
        if (currentDirection == Vector2.right) return Vector2.up;
        if (currentDirection == Vector2.up) return Vector2.left;
        if (currentDirection == Vector2.left) return Vector2.down;
        if (currentDirection == Vector2.down) return Vector2.right;
        return Vector2.zero;
    }
    public void SetWaypoints() {
        // TODO: Implement automatic waypoints algorithm
        Vector2 currentPosition = currentLevelData.startPos;
        Vector2 currentDirection = Vector2.right;
        Vector2 nextPosition = currentPosition + currentDirection;

        // find initial direction
        int count = 0;
        while (  currentLevelData.At(nextPosition) != Tile.PATH
              && currentLevelData.At(nextPosition) != Tile.FINISH
              && count < 3) {
            currentDirection = NextDirectionClockwise(currentDirection);
            nextPosition = currentPosition + currentDirection;
            count++;
        }

        while (currentLevelData.At(currentPosition) != Tile.FINISH) {
            // if the next tile is a normal tile, that means that we hit a corner
            if (currentLevelData.At(nextPosition) == Tile.NORMAL) {
                // therefore we should add a waypoint
                currentLevelData.waypoints.Add(currentPosition);
                // and find the direction
                count = 0; // to prevent it from entering an infinite loop
                do {
                    currentDirection = NextDirectionClockwise(currentDirection);
                    nextPosition = currentPosition + currentDirection;
                    count++;
                } while (currentLevelData.At(nextPosition) != Tile.PATH
                      && currentLevelData.At(nextPosition) != Tile.FINISH
                      && count < 3);
            }
            currentPosition = nextPosition;
        }
        // once we are here, we are on the finish tile
        currentLevelData.waypoints.Add(currentPosition);
    }

    //---------------- Level Build/Unbuild ----------------//
    public void BuildLevel() {
        if (currentLevelData == null) {
            Debug.LogError("Cannot build level, no level loaded");
            return;
        }
        GameManager.Player.Money = currentLevelData.startingMoney;
        // Building tiles and setting center point
        centerPoint = Vector3.zero;
        for (int x = 0; x < currentLevelData.width; x++)
            for (int y = 0; y < currentLevelData.length; y++)
                AddTile(x, y, currentLevelData.grid[y][x]);

        GameObject PlacedNormals = new GameObject("_Placed Normals");
        PlacedNormals.transform.parent = transform;
        PlacedNormals.AddComponent<MeshFilter>();
        PlacedNormals.AddComponent<MeshRenderer>();

        GameObject PlacedPaths = new GameObject("_Placed Paths");
        PlacedPaths.transform.parent = transform;
        PlacedPaths.AddComponent<MeshFilter>();
        PlacedPaths.AddComponent<MeshRenderer>();


        Combine(placedNormal, PlacedNormals);
        Combine(placedPath, PlacedPaths);

        Camera.main.GetComponent<CameraController>().target = centerPoint;
        // Placing waypoints
        if (waypointList == null) {
            waypointList = new GameObject("Waypoint List");
            waypointList.transform.parent = transform;
            waypointList.transform.localPosition = Vector3.zero;
        }
        for (int i = 0; i < currentLevelData.waypoints.Count; i++) {
            GameObject waypoint = Instantiate<GameObject>(Waypoint);
            waypoint.name = "Waypoint " + i;
            waypoint.transform.position = new Vector3(
                currentLevelData.waypoints[i].x * (brickSize.x + spacing),
                brickSize.y,
                currentLevelData.waypoints[i].y * (brickSize.z + spacing)
                );
            waypoint.transform.parent = waypointList.transform;
        }

        // Adding towers
        if (towerList == null) {
            towerList = new GameObject("Tower List");
            towerList.transform.parent = GameManager.EntitiesList.transform;
            towerList.transform.localPosition = Vector3.zero;
        }
        for (int i = 0; i < currentLevelData.towers.Count; i++) {
            Tower newTower = (Instantiate(GameManager.TowerManager.tower[currentLevelData.towers[i].type][0].prefab) as GameObject).GetComponent<Tower>();
            newTower.data.position = currentLevelData.towers[i].position;
            CenterOnTile(newTower.data.position, newTower);
            newTower.transform.parent = towerList.transform;
        }
    }
    public void UnbuildLevel() {
        // destroying bricks, blocks, waypoints, and wave spawner (which in turn destroys all enemies)
        Transform[] tiles = GetComponentsInChildren<Transform>(true);
        for (int i = 1; i < tiles.Length; i++)
            Destroy(tiles[i].gameObject);

        // destroying towers
        if (towerList != null)
        {
            Transform[] towers = towerList.GetComponentsInChildren<Transform>(true);
            for (int i = 1; i < towers.Length; i++) Destroy(towers[i].gameObject);
            Destroy(towerList.gameObject);
            towerList = null;
        }

        currentLevelData.towers.Clear();
        placedTiles.Clear();
        placedNormal.Clear();
        placedPath.Clear();

        centerPoint = Vector3.zero;
    }
    private void AddTile(int x, int y, Tile tile) {

        Vector3 tilePos = new Vector3(
            x * (brickSize.x + spacing),
            0,
            y * (brickSize.z + spacing));
        Vector3 blockPos = new Vector3(
            x * (blockSize.x + spacing),
            blockSize.y / 2f,
            y * (blockSize.z + spacing));

        switch (tile) {
            case Tile.NORMAL: {
                    GameObject brick = Instantiate(Normal_Brick, tilePos, Quaternion.identity) as GameObject;
                    brick.GetComponent<Renderer>().sharedMaterial = Normal_Brick.GetComponent<Renderer>().sharedMaterial;
                    brick.GetComponent<Brick>().internalPos = new Vector2(x, y);
                    brick.transform.parent = transform;
                    brick.name = Normal_Brick.name;
                    placedTiles.Add(brick);
                    placedNormal.Add(brick);
                }
                break;

            case Tile.PATH: {
                    GameObject brick = Instantiate(Path_Brick, tilePos, Quaternion.identity) as GameObject;
                    brick.GetComponent<Renderer>().sharedMaterial = Path_Brick.GetComponent<Renderer>().sharedMaterial;
                    brick.GetComponent<Brick>().internalPos = new Vector2(x, y);
                    brick.transform.parent = transform;
                    brick.name = Path_Brick.name;
                    placedTiles.Add(brick);
                    placedPath.Add(brick);
                }
                break;

            case Tile.START: {
                    GameObject block = Instantiate(Start_Block, blockPos, Quaternion.identity) as GameObject;
                    block.transform.parent = transform;
                    block.name = Start_Block.name;
                    placedTiles.Add(block);

                    GameObject waveSpawnerObj = Instantiate(Wave_Spawner, tilePos, Quaternion.identity) as GameObject;
                    waveSpawnerObj.transform.parent = transform;
                    waveSpawnerObj.name = Wave_Spawner.name;
                    waveSpawner = waveSpawnerObj.GetComponent<WaveSpawner>();
                    waveSpawner.waveData = currentLevelData.waves;
                }
                break;

            case Tile.FINISH: {
                    GameObject block = Instantiate(Finish_Block, blockPos, Quaternion.identity) as GameObject;
                    block.transform.parent = transform;
                    block.name = transform.name;
                    placedTiles.Add(block);
                }
                break;
        }
        centerPoint += tilePos / (currentLevelData.width * currentLevelData.length);
    }
    private void Combine(List<GameObject> gos, GameObject parent)
    {
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        foreach (var go in gos)
        {
            meshFilters.Add(go.GetComponent<MeshFilter>());
            Destroy(go.GetComponent<MeshFilter>());
            Destroy(go.GetComponent<MeshRenderer>());
        }
        CombineInstance[] combine = new CombineInstance[meshFilters.Count];
        {
            int i = 0;
            while (i < meshFilters.Count)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                i++;
            }
        }
        parent.GetComponent<MeshFilter>().mesh = new Mesh();
        parent.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        parent.GetComponent<Renderer>().sharedMaterial = gos[0].GetComponent<Renderer>().sharedMaterial;
    }

    //---------------- Tower place/detach ----------------//
    public bool IsAvailable(Vector2 tilePos)
    {
        if (isSmoothFading) return false;
        if (currentLevelData.grid[(int)tilePos.y][(int)tilePos.x] != Tile.NORMAL) return false;
        if (currentLevelData.towers != null)
            if (currentLevelData.towers.Exists(towerInfo => towerInfo.position == tilePos)) return false;
        return true;
    }
    public void CenterOnTile(Vector2 tilePos, Tower tower) {
        tower.transform.position = new Vector3(
            tilePos.x * (brickSize.x + spacing),
            brickSize.y / 2f,
            tilePos.y * (brickSize.z + spacing));
        //tower.towerInfo.position = tilePos;
    }
    public void AddTowerOnTile(Vector2 tilePos, Tower tower) {
        TriggerTowerPlaced();

        CenterOnTile(tilePos, tower);
        tower.transform.parent = towerList.transform;
        tower.data.position = tilePos;
        currentLevelData.towers.Add(tower.data);
        //Debug.Log("You successfully placed  " + tower.data.type + " !");
    }
    public void DetachTowerFromTile(Tower tower)
    {
        int existingTowerIndex = currentLevelData.towers.FindIndex(towerInfo => towerInfo.position == tower.data.position);
        if(existingTowerIndex == -1)
        {
            Debug.LogError("You are trying to remove a non-existing tower at " + tower.data.position);
            return;
        }
        currentLevelData.towers.RemoveAt(existingTowerIndex);
        //Debug.Log("You just detached tower " + tower.name + " from tile " + tower.data.position);
    }

    public Vector3[] GetUseableWaypoints() {
        Vector2[] tempWaypoints = currentLevelData.waypoints.ToArray();
        List<Vector3> waypoints = new List<Vector3>();
        for (int i = 0; i < tempWaypoints.Length; i++) {
            waypoints.Add(new Vector3(
                tempWaypoints[i].x * (brickSize.x + spacing),
                brickSize.y,
                tempWaypoints[i].y * (brickSize.z + spacing)));
        }
        return waypoints.ToArray();
    }


    //---------------- Fade effect ----------------//
    public float smoothFadeSpeed;
    private bool isSmoothFading = false;
    private float targetHeight;

    public void SmoothFade() {
        targetHeight = transform.position.y;
        Vector3 displacement = Vector3.up * 50f;
        towerList.transform.parent = transform;
        transform.position += displacement;
        for (int i = 0; i < placedTiles.Count; i++) {
            Renderer renderer = placedTiles[i].GetComponent<Renderer>();
            renderer.material.color = new Color(
                renderer.material.color.r,
                renderer.material.color.g,
                renderer.material.color.b,
                0);
        }
        isSmoothFading = true;
    }
    public void Update() {
        if (isSmoothFading) {
            transform.position = new Vector3(
                transform.position.x,
                Mathf.Lerp(transform.position.y,
                           targetHeight,
                           Time.deltaTime * smoothFadeSpeed),
                transform.position.z);
            for (int i = 0; i < placedTiles.Count; i++) {
                Renderer renderer = placedTiles[i].GetComponent<Renderer>();
                renderer.material.color = Color.Lerp(
                    renderer.material.color,
                    new Color(
                        renderer.material.color.r,
                        renderer.material.color.g,
                        renderer.material.color.b,
                        1),
                    Time.deltaTime * smoothFadeSpeed);
            }
            if ((transform.position.y - targetHeight) <= 0.1f) {
                transform.position = new Vector3(
                    transform.position.x,
                    targetHeight,
                    transform.position.z);
                towerList.transform.parent = GameManager.EntitiesList.transform;
                for (int i = 0; i < placedTiles.Count; i++) {
                    Renderer renderer = placedTiles[i].GetComponent<Renderer>();
                    renderer.material.color = new Color(
                        renderer.material.color.r,
                        renderer.material.color.g,
                        renderer.material.color.b,
                        1);
                }
                isSmoothFading = false;
            }
        }
    }
}