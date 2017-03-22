using UnityEngine;
using System;

/*
 * Randomizes a map out of a chosen or pseudo-random seed. The user chooses how much (%)
 * water and mountain it should contain.
 * 
 * The generator is based on the Unity tutorial "Procedural Cave Generation tutorial",
 * https://unity3d.com/learn/tutorials/projects/procedural-cave-generation-tutorial
 * 
 */
public class MapGenerator : ScriptableObject {

    private int width;
    private int height;

    private string seed;
    private bool useRandomSeed;

    [Range(0, 40)]
    public int randomWaterPercent;
    [Range(0, 40)]
    public int randomMountainPercent;

    int[,] map;


    /*
     * Constructor.
     */
    public void Init (int w, int h, string s, bool useRndS, int rndWPercent, int rndMPercent) {
        Console.WriteLine("Initialize MapGenerator");

        this.width = w;
        this.height = h;
        this.seed = s;
        this.useRandomSeed = useRndS;
        this.randomWaterPercent = rndWPercent;
        this.randomMountainPercent = rndMPercent;
    }


    public int[,] GenerateMap() {
        map = new int[this.width, this.height];
        RandomFillMap();

        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }

        return map;
    }


    /*
     * Fills the map by pseudo-random assigning each tile a terrain.
     * Sand is not added in this step, but when the map has been smoothed.
     * 
     * 0 = Water
     * 1 = Sand
     * 2 = Grass
     * 3 = Mountain
     */
    void RandomFillMap () {
        if (useRandomSeed) {
            seed = Time.time.ToString();
            Debug.Log("MAP SEED: " + seed);
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                // Add grass around the map edges.
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
                    map[x, y] = 2;
                }
                else if (pseudoRandom.Next(0, 80) < randomWaterPercent) {
                    map[x, y] = 0;
                }
                else if (pseudoRandom.Next(0, 80) < randomMountainPercent) {
                    map[x, y] = 3;
                }
                else {
                    map[x, y] = 2;
                }
            }
        }
    }


    /*
     * Cellular automata that smoothens the map.
     * Adds sand around water last.
     */
    void SmoothMap () {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int neighbourWaterTiles = GetSurroundingWaterCount(x, y);
                int neighbourGrassTiles = GetSurroundingGrassCount(x, y);
                int neighbourMountainTiles = GetSurroundingMountainCount(x, y);

                if (neighbourWaterTiles > 3) {
                    map[x, y] = 0;
                }
                else if (neighbourMountainTiles > 3) {
                    map[x, y] = 3;
                }
                else if (neighbourGrassTiles > 4) {
                    map[x, y] = 2;
                }

                // Remove small mountain blocks, replace with grass.
                if (map[x, y] == 3 && neighbourMountainTiles < 2) {
                    map[x, y] = 2;
                }

                // Remove small water blocks, replace with grass.
                if (map[x, y] == 0 && neighbourWaterTiles < 2) {
                    map[x, y] = 2;
                }

                // Fill sand around water.
                if ((map[x, y] == 2 || map[x, y] == 3) && neighbourWaterTiles > 1) {
                    map[x, y] = 1;
                }
            }
        }

        for (int i = 0; i < 3; i++) {
            AddSand();
        }

    }


    /*
     * Adds sand around water.
     */
    void AddSand () {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int neighbourSandTiles = GetSurroundingSandCount(x, y);

                if ((map[x, y] == 2 || map[x, y] == 3) && neighbourSandTiles > 4) {
                    map[x, y] = 1;
                }
            }
        }
    }


    /*
     * Get the amount of mountain tiles (id 3) around a given tile.
     */
    int GetSurroundingMountainCount (int gridX, int gridY) {
        int mountainCount = 0;

        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {

                // Are we looking at a tile inside the map?
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
                    // Is the neighbouring tile actually the tile itself?
                    if (neighbourX != gridX || neighbourY != gridY) {
                        if (map[neighbourX, neighbourY] == 3) {
                            mountainCount++;
                        }
                    }
                }

            }
        }

        return mountainCount;
    }


    /*
     * Get the amount of water tiles (id 0) around a given tile.
     */
    int GetSurroundingWaterCount (int gridX, int gridY) {
        int waterCount = 0;

        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {

                // Are we looking at a tile inside the map?
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
                    // Is the neighbouring tile actually the tile itself?
                    if (neighbourX != gridX || neighbourY != gridY) {
                        if (map[neighbourX, neighbourY] == 0) {
                            waterCount++;
                        }
                    }
                }

            }
        }

        return waterCount;
    }


    /*
     * Get the amount of grass tiles (id 2) around a given tile.
     */
    int GetSurroundingGrassCount (int gridX, int gridY) {
        int grassCount = 0;

        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {

                // Are we looking at a tile inside the map?
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
                    // Is the neighbouring tile actually the tile itself?
                    if (neighbourX != gridX || neighbourY != gridY) {
                        if (map[neighbourX, neighbourY] == 2) {
                            grassCount++;
                        }
                    }
                }

            }
        }

        return grassCount;
    }


    /*
     * Get the amount of sand tiles (id 1) around a given tile.
     */
    int GetSurroundingSandCount (int gridX, int gridY) {
        int sandCount = 0;

        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {

                // Are we looking at a tile inside the map?
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
                    // Is the neighbouring tile actually the tile itself?
                    if (neighbourX != gridX || neighbourY != gridY) {
                        if (map[neighbourX, neighbourY] == 1) {
                            sandCount++;
                        }
                    }
                }

            }
        }

        return sandCount;
    }

}