using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum TypeOfWall
{
    Corner,
    Straight,
    Full,
    None
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


    public int roomWidth = 10; // Szerokoœæ pokoju w jednostkach Unity
    public int roomHeight = 10; // Wysokoœæ pokoju w jednostkach Unity


    public int tileSize = 1; // Rozmiar jednego kafelka w jednostkach Unity

    public int spacing = 1; // Odstêp miêdzy kafelkami w jednostkach Unity
    public (bool[,], List<(int,int)>) GenerateBools(int width, int height)
    {
        bool[,] bools = new bool[width, height];
        //coordinates of the bools with true value
        List<(int, int)> coordinates = new List<(int, int)>();

        var seed = (Time.deltaTime * Time.fixedTime + 0.000000000000000100f * Time.deltaTime).ToString();
        /*try
        {
            UnityEngine.Random.InitState(Math.Abs(int.TryParse(seed.Remove(seed.IndexOf("."), 1).Substring(0, 10), out int a) ? a : 1));
        }
        catch
        {
            try
            {
                UnityEngine.Random.InitState(Math.Abs(int.Parse(seed.ToString())));
            }
            catch
            {
                UnityEngine.Random.InitState(634);
            }

        }*/

        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond + System.DateTime.Now.Second);
        var spawnpoint = (UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
        bools[spawnpoint.Item1, spawnpoint.Item2] = true;
        coordinates.Add(spawnpoint);
        //fill random bools
        for (int x = 0; x < bools.GetLength(0); x++)
        {
            for (int y = 0; y < bools.GetLength(1); y++)
            {
                bools[x, y] = UnityEngine.Random.value > 0.99f;
                coordinates.Add((x, y));
                
            }
        }

        //add spawn point
        return (bools,coordinates);
    }

    
    void Start()
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
        var (bools, points) = GenerateBools(80, 80);
        
        bools = generator.ConnectTheDots(bools, 5);
        GenerateTiles(bools);
        // GenerateRoom(new(10, 10));
    }


    void RescaleObjects()
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
    void GenerateTiles(bool[,] bools)
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

                        //Calculate position of the wall
                        position = new Vector3(x * spacing, 0, y * spacing);



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
                                tileScript.PlaceWall(WallCornerPrefab, -90, position, TypeOfWall.Corner);
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
        //Generate additional corner walls for stairway-like walls
        List<(Tile, int)> WallsToAdd = new();
        foreach (var tile in Tiles)
        {
            if (tile && tile.TypeOfWall == TypeOfWall.None && tile.ToCheck)
            {
                //Check if there is a tile on the left, right, up or down
                //if there is, check if it has a wall
                TypeOfWall[] walls = new TypeOfWall[4]; Tile leftTile = Tiles[tile.x - 1, tile.y];
                if (tile.x > 0)
                {

                    if (leftTile)
                    {
                        walls[0] = leftTile.TypeOfWall;
                    }
                    else
                    {
                        walls[0] = TypeOfWall.None;
                    }
                }
                Tile rightTile = Tiles[tile.x + 1, tile.y];
                if (tile.x < Tiles.GetLength(0) - 1)
                {

                    if (rightTile)
                    {
                        walls[1] = rightTile.TypeOfWall;
                    }
                    else
                    {
                        walls[1] = TypeOfWall.None;
                    }
                }
                Tile upTile = Tiles[tile.x, tile.y + 1];
                if (tile.y < Tiles.GetLength(1) - 1)
                {

                    if (upTile)
                    {
                        walls[2] = upTile.TypeOfWall;
                    }
                    else
                    {
                        walls[2] = TypeOfWall.None;
                    }
                }
                Tile downTile = Tiles[tile.x, tile.y - 1];
                if (tile.y > 0)
                {

                    if (downTile)
                    {
                        walls[3] = downTile.TypeOfWall;
                    }
                    else
                    {
                        walls[3] = TypeOfWall.None;
                    }
                }
                //Generate wall based on the walls around the tile
                //- Add wall to the list of walls so it can be generated after all tiles are checked
                switch (walls[0], walls[1], walls[2], walls[3])
                {
                    default:
                        //if there is no option to make a corner wall, make wall that will occupy the space of whole tile 
                        //tile.PlaceMainCornerWall(WallCornerPrefab, 0, new Vector3(tile.x * spacing, 0, tile.y * spacing));
                        break;

                    case (TypeOfWall.Corner or TypeOfWall.Straight, TypeOfWall.None, TypeOfWall.Corner or TypeOfWall.Straight, TypeOfWall.None):

                        //If there is a wall on the left or up that we cannot make connection with, don't generate a wall
                        if (walls[0] == TypeOfWall.Straight)
                        {
                            if (leftTile.Wall.transform.rotation.y == 90)
                            {
                                break;
                            }

                        }
                        if (walls[3] == TypeOfWall.Straight)
                        {
                            if (upTile.Wall.transform.rotation.y == 0)
                            {
                                break;
                            }
                        }
                        WallsToAdd.Add((tile, -90));
                        break;
                    case (TypeOfWall.Corner or TypeOfWall.Straight, TypeOfWall.None, TypeOfWall.None, TypeOfWall.Corner or TypeOfWall.Straight):

                        //If there is a wall on the left or down that we cannot make connection with, don't generate a wall
                        if (walls[0] == TypeOfWall.Straight)
                        {
                            if (leftTile.Wall.transform.rotation.y == 90)
                            {
                                break;
                            }
                        }
                        if (walls[3] == TypeOfWall.Straight)
                        {
                            if (downTile.Wall.transform.rotation.y == 0)
                            {
                                break;
                            }
                        }
                        WallsToAdd.Add((tile, 180));

                        break;
                    case (TypeOfWall.None, TypeOfWall.Corner or TypeOfWall.Straight, TypeOfWall.Corner or TypeOfWall.Straight, TypeOfWall.None):
                        //If there is a wall on the right or up that we cannot make connection with, don't generate a wall
                        if (walls[1] == TypeOfWall.Straight)
                        {
                            if (rightTile.Wall.transform.rotation.y == 90)
                            {
                                break;
                            }
                        }
                        if (walls[2] == TypeOfWall.Straight)
                        {
                            if (upTile.Wall.transform.rotation.y == 0)
                            {
                                break;
                            }
                        }
                        WallsToAdd.Add((tile, 0));
                        break;
                    case (TypeOfWall.None, TypeOfWall.Corner or TypeOfWall.Straight, TypeOfWall.None, TypeOfWall.Corner or TypeOfWall.Straight):
                        //If there is a wall on the right or down that we cannot make connection with, don't generate a wall
                        if (walls[1] == TypeOfWall.Straight)
                        {
                            if (rightTile.Wall.transform.rotation.y == 90)
                            {
                                break;
                            }
                        }
                        if (walls[3] == TypeOfWall.Straight)
                        {
                            if (downTile.Wall.transform.rotation.y == 0)
                            {
                                break;
                            }
                        }
                        WallsToAdd.Add((tile, 90));
                        break;


                }

            }
        }
        //Generate walls from the list
        foreach ((Tile tile, int rotation) in WallsToAdd)
        {
            tile.PlaceWall(WallCornerPrefab, rotation, new Vector3(tile.x * spacing, 0, tile.y * spacing), TypeOfWall.Corner);
        }
    }


    /*
        void GenerateRoom(Vector2 roomPos)
        {
            // Generowanie pod³ogi
            for (int x = 0; x < roomWidth; x++)
            {
                for (int z = 0; z < roomHeight; z++)
                {
                    // Tworzenie pod³ogi
                    GameObject floor = Instantiate(floorPrefab, new Vector3(x * spacing, 0, z * spacing), Quaternion.identity);
                    floor.transform.parent = transform;
                }
            }

            // Generowanie œcian
            for (int x = 0; x < roomWidth; x++)
            {
                // Œciana pó³nocna
                GameObject northWall = Instantiate(wallPrefab, new Vector3(x * spacing, 0, roomHeight * spacing), Quaternion.identity);
                northWall.transform.parent = transform;

                // Œciana po³udniowa
                GameObject southWall = Instantiate(wallPrefab, new Vector3(x * spacing, 0, -1 * spacing), Quaternion.identity);
                southWall.transform.parent = transform;
            }

            for (int z = 0; z < roomHeight; z++)
            {
                // Œciana wschodnia
                GameObject eastWall = Instantiate(wallPrefab, new Vector3(roomWidth * spacing, 0, z * spacing), Quaternion.Euler(0, 90, 0));
                eastWall.transform.parent = transform;

                // Œciana zachodnia
                GameObject westWall = Instantiate(wallPrefab, new Vector3(-1 * spacing, 0, z * spacing), Quaternion.Euler(0, 90, 0));
                westWall.transform.parent = transform;
            }

            // Generowanie elementów na pod³odze
            for (int x = 0; x < roomWidth; x++)
            {
                for (int z = 0; z < roomHeight; z++)
                {
                    // Tworzenie elementu na pod³odze
                    GameObject floorObject = Instantiate(objectPrefab, new Vector3(x * spacing, 0, z * spacing), Quaternion.identity);
                    floorObject.transform.parent = transform;
                }
            }
        }*/
}


