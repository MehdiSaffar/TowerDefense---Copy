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
        StreamReader file = new StreamReader(levelFilename + ".txt");
        Debug.Log("Parsing " + levelFilename + ".txt");

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

        SetWaypoints(levelData.startPos, Vector2.right);
    }
    private List<Tile> ParseGridRow(string line, int y) {
        List<Tile> row = new List<Tile>();
        for (int x = 0; x < line.Length; x++) {
            switch (line[x]) {
                case '-':
                    row.Add(Tile.EMPTY);
                    break;
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

    private void SetWaypoints(Vector2 currentPos, Vector2 currentDir)
    {
        if (levelData.At(currentPos) == Tile.FINISH)
        {
            levelData.waypoints.Add(currentPos);
            return;
        }
        Vector2 nextPos = currentPos + currentDir;
        if(levelData.At(nextPos) != Tile.PATH)
        {
            levelData.waypoints.Add(currentPos);
            Vector2 right = NextDirectionClockwise(currentDir);
            Vector2 left = -right;
            if(levelData.At(currentPos + right) != Tile.NORMAL && levelData.At(currentPos + right) != Tile.EMPTY) SetWaypoints(currentPos + right, right);
            if(levelData.At(currentPos + left ) != Tile.NORMAL && levelData.At(currentPos + left) != Tile.EMPTY) SetWaypoints(currentPos + left , left);
            return;
        }
        SetWaypoints(nextPos, currentDir);
    }
     private Vector2 NextDirectionClockwise(Vector2 currentDirection) {
        if (currentDirection == Vector2.right) return Vector2.down;
        if (currentDirection == Vector2.up) return Vector2.right;
        if (currentDirection == Vector2.left) return Vector2.up;
        if (currentDirection == Vector2.down) return Vector2.left;
        return Vector2.zero;
    }

    private void SaveLevel() {
        string pathToLevel = "Assets/Resources/" + levelFilename + ".txt";
        Debug.Log("Saving into " + pathToLevel);
        StreamWriter writer = new StreamWriter(pathToLevel);

        fsData data;
        fsSerializer fs = new fsSerializer();
        fs.TrySerialize<LevelData>(levelData, out data);
        fsJsonPrinter.PrettyJson(data, writer);

        writer.Close();
    }
}