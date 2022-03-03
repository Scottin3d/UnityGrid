using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlacedObjectTypeSO : ScriptableObject {

    /// <summary>
    /// Get the next direction (ClockWise).
    /// </summary>
    /// <param name="dir">The current direction.</param>
    /// <returns>The next <c>Dir</c> clockwise 90 degrees.</returns>
    public static Dir GetNextDir(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down:      return Dir.Left;
            case Dir.Left:      return Dir.Up;
            case Dir.Up:        return Dir.Right;
            case Dir.Right:     return Dir.Down;
        }
    }

    public enum Dir {
        Down,
        Left,
        Up,
        Right,
    }

    public string nameString;
    public Transform prefab;
    public Transform visual;
    public int width;
    public int height;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public int GetRotationAngle(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down:  return 0;
            case Dir.Left:  return 90;
            case Dir.Up:    return 180;
            case Dir.Right: return 270;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public Vector2Int GetRotationOffset(Dir dir) {
        switch (dir) {
            default:
            case Dir.Down:  return new Vector2Int(0, 0);
            case Dir.Left:  return new Vector2Int(0, width);
            case Dir.Up:    return new Vector2Int(width, height);
            case Dir.Right: return new Vector2Int(height, 0);
        }
    }

    /// <summary>
    /// Return a list of occupied grid by the a <c>PlaceObjectTypeSO</c>.
    /// </summary>
    /// <param name="origin">The grid tile coordinate.</param>
    /// <param name="dir">The direction the <c>PlaceObjectTypeSO</c> is facing.</param>
    /// <returns> a <c>List</c> of grid coordinates.</returns>
    public List<Vector2Int> GetGridPositionList(Vector2Int origin, Dir dir) {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch (dir) {
            default:
            case Dir.Down:
            case Dir.Up:
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        gridPositionList.Add(origin + new Vector2Int(x, y));
                    }
                }
                break;
            case Dir.Left:
            case Dir.Right:
                for (int x = 0; x < height; x++) {
                    for (int y = 0; y < width; y++) {
                        gridPositionList.Add(origin + new Vector2Int(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }

}
