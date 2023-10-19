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
            case -90:
                obj = Instantiate(wall, position, Quaternion.Euler(0, -90, 0), parent);
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
