using UnityEngine;

public class Main : MonoBehaviour {

    MapGenerator mapGenerator;
    ContentManager contentManager;

    int[,] map;

    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 40)]
    public int randomWaterPercent;
    [Range(0, 40)]
    public int randomMountainPercent;

    [Range(0, 3)]
    public int nbrOfQuests;


    // Use this for initialization
    void Start () {
        mapGenerator = ScriptableObject.CreateInstance("MapGenerator") as MapGenerator;
        mapGenerator.Init(this.width, this.height, this.seed, this.useRandomSeed, 
                          this.randomWaterPercent, this.randomMountainPercent);
        map = mapGenerator.GenerateMap();

        contentManager = ScriptableObject.CreateInstance("ContentManager") as ContentManager;
        contentManager.Init(this.width, this.height, this.nbrOfQuests);
        contentManager.GenerateContent(map);
	}
	
	// Update is called once per frame.
	void Update () {

        // Generate a new map by clicking left mouse button.
        if (Input.GetMouseButtonDown(0)) {
            mapGenerator.Init(this.width, this.height, this.seed, this.useRandomSeed, 
                              this.randomWaterPercent, this.randomMountainPercent);
            UpdateMap();
            UpdateContent();
        }

        // Controller, checks for movement.
        // BAD! Should be in a separate controller!

        // Move upwards.
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            contentManager.MoveUp();
        }

        // Move downwards.
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            contentManager.MoveDown();
        }

        // Move left.
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            contentManager.MoveLeft();
        }

        // Move right.
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            contentManager.MoveRight();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            contentManager.checkQuestCollision();
        }
    }

    /*
     * Generates a new map.
     */
    void UpdateMap () {
        map = mapGenerator.GenerateMap();
    }

    /*
     * Generates new quests and gives player new location.
     */
    void UpdateContent () {
        contentManager.GenerateContent(map);
    }

    /*
     * Renders/draws the map and its content.
     */
    void OnDrawGizmos () {
        
        if (map != null) {
            // Render/draw the map.
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if (map[x, y] == 0) {
                        Gizmos.color = Color.blue;
                    }
                    else if (map[x, y] == 1) {
                        Gizmos.color = Color.yellow;
                    }
                    else if (map[x, y] == 2) {
                        Gizmos.color = Color.green;
                    }
                    else {
                        Gizmos.color = Color.gray;
                    }
                    Vector3 pos = new Vector3(-width / 2 + x + 0.5f, 0, -height / 2 + y + 0.5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }

            // Render Quest-objects and player.
            contentManager.OnDrawGizmos();
        }
    }

}
