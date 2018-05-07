using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class MapGenerator : MonoBehaviour {
    public Vector2 MaxMapSize;
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform mavMeshFloor;
    public Transform navMeshMaskPrefab;
    public float tileSize;
    [Range(0,1)]
    public float outlinePercent;
    List<Coord> allTileCoords;
    Queue<Coord> shuffleTileCoords;
    public System.Random prg;

    Map currentMap;
    public Map[] maps;
    public int mapIndex;
       
    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        prg = new System.Random(currentMap.seed);

        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, 0.05f, currentMap.mapSize.y * tileSize);

        allTileCoords = new List<Coord>();

        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Coord newCoord = new Coord(x, y);
                allTileCoords.Add(newCoord);
            }
        }

        shuffleTileCoords = new Queue<Coord>(Utility.ShuffleArray<Coord>(allTileCoords.ToArray(), currentMap.seed));
        currentMap.mapCentre = new Coord((int)currentMap.mapSize.x / 2, (int)currentMap.mapSize.y / 2);


        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Vector3 pos = CoordToPos(x,y);
                Transform newTile = Instantiate(tilePrefab, pos, Quaternion.Euler(Vector3.right * 90),mapHolder) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent)*tileSize;
            }
        }

        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        int obstacleCount = (int) (currentMap.mapSize.x*currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != currentMap.mapCentre &&MapIsFullyAccesible(obstacleMap, currentObstacleCount))
            {
                float randomHeight = Mathf.Lerp(currentMap.minMapHeight, currentMap.maxMapHeight, (float)prg.NextDouble());
                Vector3 pos = CoordToPos(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, pos + Vector3.up * randomHeight, Quaternion.identity, mapHolder) as Transform;
                newObstacle.localScale = new Vector3((1 - outlinePercent),randomHeight, (1 - outlinePercent))*tileSize;

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMat = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMat.color = Color.Lerp(currentMap.frontColor, currentMap.backColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMat;

            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }

            
        }
        mavMeshFloor.localScale = new Vector3(MaxMapSize.x, MaxMapSize.y)*tileSize;

        Transform leftMask = Instantiate(navMeshMaskPrefab, (MaxMapSize.x + currentMap.mapSize.x) / 4 * Vector3.left*tileSize, Quaternion.identity);
        leftMask.parent = mapHolder;
        leftMask.localScale = new Vector3((MaxMapSize.x - currentMap.mapSize.x) / 2,1, MaxMapSize.y) * tileSize;

        Transform rightMask = Instantiate(navMeshMaskPrefab, (MaxMapSize.x + currentMap.mapSize.x) / 4 * Vector3.right * tileSize, Quaternion.identity);
        rightMask.parent = mapHolder;
        rightMask.localScale = new Vector3((MaxMapSize.x - currentMap.mapSize.x) / 2, 1, MaxMapSize.y) * tileSize;

        Transform topMask = Instantiate(navMeshMaskPrefab, (MaxMapSize.y + currentMap.mapSize.y) / 4 * Vector3.forward * tileSize, Quaternion.identity);
        topMask.parent = mapHolder;
        topMask.localScale = new Vector3(currentMap.mapSize.x,1,(MaxMapSize.y - currentMap.mapSize.y) / 2) * tileSize;

        Transform bottomMask = Instantiate(navMeshMaskPrefab, (MaxMapSize.y + currentMap.mapSize.y) / 4 * Vector3.back * tileSize, Quaternion.identity);
        bottomMask.parent = mapHolder;
        bottomMask.localScale = new Vector3(currentMap.mapSize.x, 1, (MaxMapSize.y - currentMap.mapSize.y) / 2) * tileSize;
    }

    public bool MapIsFullyAccesible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queu = new Queue<Coord>();
        queu.Enqueue(currentMap.mapCentre);
        mapFlags[currentMap.mapCentre.x, currentMap.mapCentre.y] = true;
        int accesibleTileCount = 1;

        while (queu.Count>0)
        {
            Coord tile = queu.Dequeue();
            for (int x = -1; x <=1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 || y == 0 )
                    {
                        int neighbourX = tile.x + x;
                        int neighbourY = tile.y + y;
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queu.Enqueue(new Coord(neighbourX, neighbourY));
                                accesibleTileCount++;

                            }
                        }
                    }
                    
                }
            }
        }
        int correctAccesibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return accesibleTileCount == correctAccesibleTileCount;
    }

    public Vector3 CoordToPos(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2 + .5f + x, 0, -currentMap.mapSize.y / 2 + .5f + y)*tileSize;
    }
        
    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffleTileCoords.Dequeue();
        shuffleTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    [System.Serializable]
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator == (Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }
        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1==c2);
        }

    }

    [System.Serializable]
    public class Map
    {
        public Vector2 mapSize;
                
        [Range(0, 1)]
        public float obstaclePercent;

        public Coord mapCentre;
        public int seed = 20;

        public float maxMapHeight;
        public float minMapHeight;
        public Color frontColor;
        public Color backColor;


    }

}
