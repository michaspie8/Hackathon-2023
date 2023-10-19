using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x, y; //Position of Tile in room
    /*
    public void PlaceWall(GameObject wall, Vector3 position, Quaternion rotation)
    {

    }*/
    public GameObject Wall;
    public TypeOfWall TypeOfWall = TypeOfWall.None;
    public bool ToCheck = false; //If the tile is to check for corner walls inside
    public bool isFree = false; //If the tile is free to place objects on (not a wall)
    public bool secoundPhaseGeneration = false; //If wall on tile is placed in secound phase of generation
    public List<OnTileObj> onTileObjects; //Objects placed on tile
    public List<GameObject> ObjectsSpawnedOnTile = new List<GameObject>(); //Objects spawned on tile

    /*public void AddOnTileObject(OnTileObj obj)
    {
        if (Wall == null)
        {
            if (onTileObjects == null)
            {
                onTileObjects = new List<OnTileObj>();
            }
            onTileObjects.Add(obj);
        }
        else{
            Debug.Log("Tr");
        }
    }*/

    public void PlaceWall(GameObject wall, int degrees, Vector3 position, TypeOfWall type)
    {
        GameObject obj;
        var parent = transform;
        switch (degrees)
        {
            case 0:
                obj = Instantiate(wall, position, Quaternion.identity, parent);
                break;
            case 90:
                obj = Instantiate(wall, position, Quaternion.Euler(0, 90, 0), parent);
                break;
            case 180:
                obj = Instantiate(wall, position, Quaternion.Euler(0, 180, 0), parent);
                break;
            case 270:
                obj = Instantiate(wall, position, Quaternion.Euler(0, 270, 0), parent);
                break;
            default:
                return;
        }
        if (obj != null)
        {
            Wall = obj;
            TypeOfWall = type;
        }
    }
}
