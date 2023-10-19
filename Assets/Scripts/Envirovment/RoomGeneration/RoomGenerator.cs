using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum TypeOfWall
{
    Corner,
    Straight,
    Full,
    None
}

public enum Direction
{
    Up, Down, Left, Right, None
}

public class RoomGenerator : MonoBehaviour
{
    //Procedular Room Generator. Generates a room basing on puting meshes into tiles.
    #region Singleton

    public static RoomGenerator instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;

    }

    #endregion

    public Transform Parent;
    public Tile[,] Tiles;
    public GameObject WallCornerPrefab;
    public GameObject wallPrefab; // Prefab œciany
    public GameObject floorPrefab; // Prefab pod³ogi
    public GameObject objectPrefab; // Prefab elementu na pod³odze
    public GameObject fullTileWall; // Prefab œciany zajmuj¹cej ca³y kafelek
    public GameObject StairwayPrefab; // Prefab schodów
    public GameObject PlayerSpawnPrefab; // Prefab schodów

    public List<GameObject> OnTileObjects; // Lista obiektów na kafelku

    public int roomWidth = 10; // Szerokoœæ pokoju w jednostkach Unity
    public int roomHeight = 10; // Wysokoœæ pokoju w jednostkach Unity

    public int roomSize = 40; // Wielkoœæ pokoju w kafelkach

    public int tileSize = 1; // Rozmiar jednego kafelka w jednostkach Unity

    public int spacing = 1; // Odstêp miêdzy kafelkami w jednostkach Unity

    public int seed; //Ziarno losowoœci dla generatora pokoju
    public float chanceToAddTile = 0.99f; //Szansa na (nie) wygenerowanie kafelka 


    public (bool[,], List<(int, int)>) GenerateBools(int width, int height)
    {
        bool[,] bools = new bool[width, height];
        //coordinates of the bools with true value
        List<(int, int)> coordinates = new List<(int, int)>();

        //var seed = (Time.deltaTime * Time.fixedTime + 0.000000000000000100f * Time.deltaTime).ToString();
        if (seed <= 0) seed = System.DateTime.Now.Millisecond + System.DateTime.Now.Second;

        UnityEngine.Random.InitState(seed);
        //first coordinate is spawn point
        var spawnpoint = (UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
        bools[spawnpoint.Item1, spawnpoint.Item2] = true;

        var exitpoint = (UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
        while (exitpoint.Item1 == spawnpoint.Item1 && exitpoint.Item2 == spawnpoint.Item2)
        {
            exitpoint = (UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
        }
        bools[exitpoint.Item1, exitpoint.Item2] = true;



        coordinates.Add(spawnpoint);
        coordinates.Add(exitpoint);
        //fill random bools
        for (int x = 0; x < bools.GetLength(0); x++)
        {
            for (int y = 0; y < bools.GetLength(1); y++)
            {
                bools[x, y] = UnityEngine.Random.value > chanceToAddTile;
            }
        }
        //add spawn point
        return (bools, coordinates);
    }

    public void GenerateRoom()
    {
        StartCoroutine(GenerateRoomCo());
    }
    public IEnumerator GenerateRoomCo()
    {
        /*bool[][] bools =
            {
            new bool[]{ false, true, true, true, false, false, false},
            new bool[]{ false, true, true, true, true,  false, false},
            new bool[]{ false, true, true, true, true,  false, false},
            new bool[]{ false, true, true, true, false, false, false},
            new bool[]{ true,  true, true, true, true,  false, false},
            new bool[]{ true,  true, true, true, true,  true, false},
            new bool[]{ true,  true, true, true, true,  true, true},
            new bool[]{ true,  true, true, true, true,  true, false},
            new bool[]{ true,  true, true, true, true,  false, false},
            new bool[]{ false, true, true, true, true,  false, false},
            new bool[]{ false, false,true, true, false, false, false},
        };*/

        var generator = new PBArrayGenerator();
        var (bools, points) = GenerateBools(roomSize, roomSize);

        bools = generator.ConnectTheDots(bools, 5);
        foreach (var point in points)
        {
            bools[point.Item1, point.Item2] = true;
            if (point.Item1 > 0 && point.Item2 > 0 && point.Item1 < bools.GetLength(0) && point.Item2 < bools.GetLength(1))
            {
                bools[point.Item1 - 1, point.Item2 - 1] = true;
                bools[point.Item1 + 1, point.Item2 + 1] = true;
                bools[point.Item1 - 1, point.Item2] = true;
                bools[point.Item1 + 1, point.Item2] = true;
                bools[point.Item1, point.Item2 - 1] = true;
                bools[point.Item1, point.Item2 + 1] = true;
            }

        }
        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
        GenerateTiles(bools, points[0], points[1]);
        // GenerateRoom(new(10, 10));
        yield return null;
    }

    /*void RescaleObjects()
    {
        //Rescale all objects in room to fit the tile size

        GameObject[] objects = { wallPrefab, floorPrefab, objectPrefab };
        foreach (var obj in objects)
        {
            //get the size of the object
            var renderer = obj.GetComponent<MeshRenderer>();
            var size = renderer.bounds.size;
            var maxSize = Mathf.Max(size.x, size.y, size.z);
            //scale the object
            var scaleFactor = tileSize / maxSize;
            obj.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);




        }
    }
*/
    void GenerateOnTileObjects(List<Tile> tiles, (int, int) spawnpoint)
    {
        //Generate player spawner on spawn point
    }

    void GenerateTiles(bool[,] bools, (int, int) spawnpoint, (int, int) exitpoint)
    {
        //Generates tiles basing on bools array, if bool is true, tile is generated 
        Tiles = new Tile[bools.GetLength(0), bools.GetLength(1)];
        for (int x = 0; x < bools.GetLength(0); x++)
        {
            for (int y = 0; y < bools.GetLength(1); y++)
            {
                var thisTile = bools[x, y];
                if (thisTile)
                {
                    //Check if there is a tile on the left, right, up or down
                    var previousTile = false;
                    if (x > 0)
                    {
                        previousTile = bools[x - 1, y];
                    }
                    var nextTile = false;
                    if (x < bools.GetLength(0) - 1)
                    {
                        nextTile = bools[x + 1, y];
                    }
                    var upTile = false;
                    if (y < bools.GetLength(1) - 1)
                    {
                        upTile = bools[x, y + 1];
                    }
                    var downTile = false;
                    if (y > 0)
                    {
                        downTile = bools[x, y - 1];
                    }
                    //Generuj pod³ogê jeœli zawiera przynajmniej 2 po³¹czenia
                    //Generate floor if it has at least 2 connections
                    var numberOfConnections = (previousTile ? 1 : 0) + (nextTile ? 1 : 0) + (upTile ? 1 : 0) + (downTile ? 1 : 0);
                    if (numberOfConnections > 1)
                    {
                        //Generate floor with Tile script and get it
                        Vector3 position = new Vector3(x * spacing, 0, y * spacing);

                        var tile = Instantiate(floorPrefab, position, Quaternion.identity, Parent);
                        var tileScript = tile.GetComponent<Tile>();
                        tileScript.x = x;
                        tileScript.y = y;
                        tileScript.TypeOfWall = TypeOfWall.None;
                        Tiles[x, y] = tileScript;
                        if (tileScript.x == spawnpoint.Item1 && tileScript.y == spawnpoint.Item2)
                        {
                            tileScript.onTileObjects.Add(Instantiate(PlayerSpawnPrefab, position, Quaternion.identity, tile.transform).GetComponent<OnTileObj>());
                            tileScript.onTileObjects[0].x = x;
                            tileScript.onTileObjects[0].y = y;
                            tileScript.onTileObjects[0].size = 1;
                            tileScript.isFree = false;
                            continue;
                        }
                        if (tileScript.x == exitpoint.Item1 && tileScript.y == exitpoint.Item2)
                        {
                            tileScript.onTileObjects.Add(Instantiate(StairwayPrefab, new Vector3(position.x, position.y - 4, position.z), Quaternion.identity, tile.transform).GetComponent<OnTileObj>());
                            tileScript.onTileObjects[0].x = x;
                            tileScript.onTileObjects[0].y = y;
                            tileScript.onTileObjects[0].size = 1;
                            tileScript.isFree = false;
                            foreach (var ren in tile.GetComponentsInChildren<MeshRenderer>())
                            {
                                if (ren.gameObject.name == "Plane") { ren.enabled = false; }
                                break;
                            }
                            //Calculate position of the wall
                            position = new Vector3(x * spacing, 0, y * spacing);

                        }

                            switch (previousTile, nextTile, upTile, downTile)
                            {
                                // Samotny kwadrat -> nie generuj œcian
                                default:
                                    break;
                                // Generuj lewy górny róg
                                case (false, true, false, true):
                                    tileScript.PlaceWall(WallCornerPrefab, 90, position, TypeOfWall.Corner);
                                    break;
                                // Generuj prawy górny róg
                                case (true, false, false, true):
                                    tileScript.PlaceWall(WallCornerPrefab, 180, position, TypeOfWall.Corner);
                                    break;
                                // Generuj lewy dolny róg -> domyœlny
                                case (false, true, true, false):
                                    tileScript.PlaceWall(WallCornerPrefab, 0, position, TypeOfWall.Corner);
                                    break;
                                // Generuj prawy dolny róg
                                case (true, false, true, false):
                                    tileScript.PlaceWall(WallCornerPrefab, 270, position, TypeOfWall.Corner);
                                    break;
                                // Generuj górny œrodek
                                case (true, true, false, true):
                                    tileScript.PlaceWall(wallPrefab, 0, position, TypeOfWall.Straight);
                                    break;
                                // Generuj dolny œrodek
                                case (true, true, true, false):
                                    tileScript.PlaceWall(wallPrefab, 0, position, TypeOfWall.Straight);
                                    break;
                                // Generuj lewy œrodek
                                case (false, true, true, true):
                                    tileScript.PlaceWall(wallPrefab, 90, position, TypeOfWall.Straight);

                                    break;
                                // Generuj prawy œrodek
                                case (true, false, true, true):
                                    tileScript.PlaceWall(wallPrefab, 90, position, TypeOfWall.Straight);
                                    break;
                                // Generuj œrodek
                                case (true, true, true, true):
                                    tileScript.ToCheck = true;
                                    break; //blyad

                            }

                        }
                    }
                }
            }



            //Method for getting info about neighbouring tiles
            (TypeOfWall, Direction, int) CheckTile(int x, int y, Direction direction)
            {

                if (x >= 0 && y >= 0 && x < Tiles.GetLength(0) && y < Tiles.GetLength(1))
                {
                    Tile tileToCheck = Tiles[x, y];
                    if (tileToCheck && tileToCheck.Wall/* && !tileToCheck.ToCheck*/ && !tileToCheck.secoundPhaseGeneration)
                    {
                        return (tileToCheck.TypeOfWall, direction, (int)tileToCheck.Wall.transform.rotation.eulerAngles.y);
                    }
                }
                return (TypeOfWall.None, Direction.None, 0);
            }



            //Generate additional corner walls for stairway-like walls
            (TypeOfWall, int) WallToAdd = new();
            foreach (var tile in Tiles)
            {

                if (tile && tile.TypeOfWall == TypeOfWall.None && tile.ToCheck)
                {
                    if (tile.x == 6 && tile.y == 31)
                    {
                        Debug.Log("asd");
                    }
                    //Check if there is a tile on the left, right, up or down
                    //if there is, check if it has a wall

                    (TypeOfWall, Direction, int)[] walls = new (TypeOfWall, Direction, int)[4];

                    walls[0] = CheckTile(tile.x - 1, tile.y, Direction.Left);
                    walls[1] = CheckTile(tile.x + 1, tile.y, Direction.Right);
                    walls[2] = CheckTile(tile.x, tile.y + 1, Direction.Up);
                    walls[3] = CheckTile(tile.x, tile.y - 1, Direction.Down);


                    //Check if neighbouring tiles have walls that can be connected with
                    //If they do, generate a wall

                    var directions = new List<Direction>();
                    foreach (var wall in walls)
                    {
                        if (wall.Item1 != TypeOfWall.None)
                        {
                            if (IsAbleToMakeConnection(wall.Item1, wall.Item2, wall.Item3))
                            {
                                directions.Add(wall.Item2);
                            }
                        }
                    }

                    if (directions.Count < 2)
                    {
                        tile.ToCheck = false;
                        tile.isFree = true;
                    }
                    else
                    {
                        if (directions.Count > 2)
                        {
                            WallToAdd = new(TypeOfWall.Full, 0);
                        }
                        else
                        {
                            switch (directions[0], directions[1])
                            {
                                case (Direction.Left, Direction.Right):
                                    WallToAdd = new(TypeOfWall.Straight, 0);
                                    break;
                                case (Direction.Up, Direction.Down):
                                    WallToAdd = new(TypeOfWall.Straight, 90);
                                    break;
                                case (Direction.Left, Direction.Up):
                                    WallToAdd = new(TypeOfWall.Corner, 270);
                                    break;
                                case (Direction.Left, Direction.Down):
                                    WallToAdd = new(TypeOfWall.Corner, 180);
                                    break;
                                case (Direction.Right, Direction.Up):
                                    WallToAdd = new(TypeOfWall.Corner, 0);
                                    break;
                                case (Direction.Right, Direction.Down):
                                    WallToAdd = new(TypeOfWall.Corner, 90);
                                    break;

                                case (Direction.Right, Direction.Left):
                                    WallToAdd = new(TypeOfWall.Straight, 0);
                                    break;
                                case (Direction.Down, Direction.Up):
                                    WallToAdd = new(TypeOfWall.Straight, 90);
                                    break;
                                case (Direction.Up, Direction.Left):
                                    WallToAdd = new(TypeOfWall.Corner, 270);
                                    break;
                                case (Direction.Down, Direction.Left):
                                    WallToAdd = new(TypeOfWall.Corner, 180);
                                    break;
                                case (Direction.Up, Direction.Right):
                                    WallToAdd = new(TypeOfWall.Corner, 0);
                                    break;
                                case (Direction.Down, Direction.Right):
                                    WallToAdd = new(TypeOfWall.Corner, 90);
                                    break;

                                default:
                                    break;
                            }




                            tile.ToCheck = false;
                            tile.secoundPhaseGeneration = true;
                            switch (WallToAdd.Item1)
                            {
                                case TypeOfWall.Corner:
                                    tile.PlaceWall(WallCornerPrefab, WallToAdd.Item2, new Vector3(tile.x * spacing, 0, tile.y * spacing), TypeOfWall.Corner);
                                    break;
                                case TypeOfWall.Straight:
                                    tile.PlaceWall(wallPrefab, WallToAdd.Item2, new Vector3(tile.x * spacing, 0, tile.y * spacing), TypeOfWall.Straight);
                                    break;
                                case TypeOfWall.Full:
                                    tile.PlaceWall(fullTileWall, WallToAdd.Item2, new Vector3(tile.x * spacing, 0, tile.y * spacing), TypeOfWall.Full);
                                    break;
                                case TypeOfWall.None:
                                    break;
                                default:
                                    break;

                            }
                        }
                    }
                }
            }


        }

        //is neighbour tile able to make a connection with this tile?
        bool IsAbleToMakeConnection(TypeOfWall type, Direction direction, int rotation)
        {
            switch (type, direction, rotation)
            {
                case (TypeOfWall.Corner, Direction.Left, 0):
                    return true;
                case (TypeOfWall.Corner, Direction.Left, 90):
                    return true;
                case (TypeOfWall.Corner, Direction.Up, 90):
                    return true;
                case (TypeOfWall.Corner, Direction.Up, 180):
                    return true;
                case (TypeOfWall.Corner, Direction.Right, 180):
                    return true;
                case (TypeOfWall.Corner, Direction.Right, 270):
                    return true;
                case (TypeOfWall.Corner, Direction.Down, 270):
                    return true;
                case (TypeOfWall.Corner, Direction.Down, 0):
                    return true;

                case (TypeOfWall.Straight, Direction.Left, 0):
                    return true;
                case (TypeOfWall.Straight, Direction.Up, 90):
                    return true;
                case (TypeOfWall.Straight, Direction.Right, 0):
                    return true;
                case (TypeOfWall.Straight, Direction.Down, 90):
                    return true;

                default:
                    if (type == TypeOfWall.Full && direction != Direction.None)
                    {
                        return true;
                    }
                    return false;
            }
        }
    }



