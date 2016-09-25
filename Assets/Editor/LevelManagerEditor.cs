using System.Collections.Generic;
using System.IO;
using FullSerializer;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
[CanEditMultipleObjects]
public class LevelManagerEditor : Editor {
    public string levelFilename = "";
    private LevelData levelData;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        LevelManager myScript = target as LevelManager;
        levelFilename = GUILayout.TextField(levelFilename);
        if (GUILayout.Button("Parse grid file `root/" + levelFilename + ".txt`")) {
            ParseGridFile();
        }
        if(GUILayout.Button("Transfer levelData to LevelManager")) {
            myScript.initialLevelData =  myScript.currentLevelData = levelData;
        }
        if (GUILayout.Button("Ask LevelManager to build")) {
            myScript.BuildLevel();
        }
        if (GUILayout.Button("Ask LevelManager to unbuild")) {
            myScript.UnbuildLevel();
        }
        if (GUILayout.Button("Save parsed file into `Resources/" + levelFilename + ".txt`" )) {
            SaveLevel();
        }
    }
    private void ParseGridFile() {
        levelData = new LevelData();
        StreamReader file = new System.IO.StreamReader(levelFilename + ".txt");
        Debug.Log("Parsing root\\" + levelFilename + ".txt");

        int y = 0;
        string line;
        while ((line = file.ReadLine()) != null)
        {
            levelData.grid.Add(ParseGridRow(line, y));
            y++;
        }
        file.Close();

        levelData.filename = levelFilename;
        levelData.width  = levelData.grid[0].Count;
        levelData.length = levelData.grid.Count;

        levelData.waypoints =  GetWaypoints();
    }
    private List<Tile> ParseGridRow(string line, int y) {
        List<Tile> row = new List<Tile>();
        for (int x = 0; x < line.Length; x++) {
            switch (line[x]) {
                case '.':
                    row.Add(Tile.NORMAL);
                    break;
                case 'P':
                    row.Add(Tile.PATH);
                    break;
                case 'S':
                    row.Add(Tile.START);
                    levelData.startPos = new Vector2(x, y);
                    break;
                case 'F':
                    row.Add(Tile.FINISH);
                    levelData.finishPos = new Vector2(x, y);
                    break;
                default: break;
            }
        }
        return row;
    }
    private List<Vector2> GetWaypoints() {
        List<Vector2> waypoints = new List<Vector2>();

        Vector2 currentPosition = levelData.startPos;
        Vector2 currentDirection = Vector2.right;
        Vector2 nextPosition = currentPosition + currentDirection;

        // find initial direction
        int count = 0;
        while (  levelData.At(nextPosition) != Tile.PATH
              && levelData.At(nextPosition) != Tile.FINISH
              && count < 3) {
            currentDirection = NextDirectionClockwise(currentDirection);
            nextPosition = currentPosition + currentDirection;
            count++;
        }

        while (levelData.At(currentPosition) != Tile.FINISH) {
            // if the next tile is a normal tile, that means that we hit a corner
            if (levelData.At(nextPosition) == Tile.NORMAL) {
                // therefore we should add a waypoint
                waypoints.Add(currentPosition);
                // and find the direction
                count = 0; // to prevent it from entering an infinite loop
                do {
                    currentDirection = NextDirectionClockwise(currentDirection);
                    nextPosition = currentPosition + currentDirection;
                    count++;
                } while (levelData.At(nextPosition) != Tile.PATH
                      && levelData.At(nextPosition) != Tile.FINISH
                      && count < 3);
            }
            currentPosition = nextPosition;
        }
        // once we are here, we are on the finish tile
        waypoints.Add(currentPosition);

        return waypoints;
    }
    private Vector2 NextDirectionClockwise(Vector2 currentDirection) {
        if (currentDirection == Vector2.right) return Vector2.up;
        if (currentDirection == Vector2.up) return Vector2.left;
        if (currentDirection == Vector2.left) return Vector2.down;
        if (currentDirection == Vector2.down) return Vector2.right;
        return Vector2.zero;
    }

    private void SaveLevel() {
        string pathToLevel = "Resources/" + levelData.filename + ".txt";
        Debug.Log("Saving into " + pathToLevel);
        StreamWriter writer = new StreamWriter(pathToLevel);

        fsData data;
        fsSerializer fs = new fsSerializer();
        fs.TrySerialize<LevelData>(levelData, out data);
        fsJsonPrinter.PrettyJson(data, writer);

        writer.Close();
    }
}