using System.Collections.Generic;
using UnityEngine;

public class ContentManager : ScriptableObject {

    private int width;
    private int height;

    private int playerX;
    private int playerY;

    private int nbrOfQuests;
    public List<Quest> quests;

    private int[,] map;
    private UnityEngine.Object[,] content;
    public List<Door> doorList;

    System.Random randomize;


    /*
     * Constructor.
     * Defines width and height of map.
     * Defines the number of quests that should be generated.
     */
    public void Init (int w, int h, int nbrOfQ) {
        this.width = w;
        this.height = h;
        this.nbrOfQuests = nbrOfQ;

        this.quests = new List<Quest>();
        this.doorList = new List<Door>();
        this.content = new UnityEngine.Object[this.width, this.height];
        this.randomize = new System.Random();
    }


    /*
     * Creates quests and quest points.
     * Gives the player a pseudo-random position.
     */
    public void GenerateContent (int[,] map) {
        quests.Clear();
        doorList.Clear();

        this.map = map;
        SpawnPlayer(this.map);
        PopulateQuests();
    }
    

    /*
     * Creates and returns a quest of given type.
     */
    public Quest CreateQuest (Quest.QuestType type) {
        // Randomize locations.
        int x1 = randomize.Next(0, this.width);
        int y1 = randomize.Next(0, this.height);

        // While the new location has mountain terrain, randomize new location.
        while (map[x1, y1] == 3) {
            x1 = randomize.Next(0, this.width);
            y1 = randomize.Next(0, this.height);
        }

        int x2 = randomize.Next(0, this.width);
        int y2 = randomize.Next(0, this.height);

        // While the new location has mountain terrain, randomize new location.
        while (map[x2, y2] == 3) {
            x2 = randomize.Next(0, this.width);
            y2 = randomize.Next(0, this.height);
        }

        Position p1 = ScriptableObject.CreateInstance("Position") as Position;
        p1.Init(x1, y1);
        Position p2 = ScriptableObject.CreateInstance("Position") as Position;
        p2.Init(x2, y2);

        Quest quest = ScriptableObject.CreateInstance("Quest") as Quest;
        quest.Init(p1, p2, type);

        content[x1, y1] = quest;
        content[x2, y2] = quest;

        // If it is a LOCK-quest, create a door on the start position.
        if (type == Quest.QuestType.LOCK) {
            Door d = ScriptableObject.CreateInstance("Door") as Door;
            d.Init(x1, y1, quest);
            content[x1, y1] = d;
            doorList.Add(d);
        }

        return quest;
    }


    /*
    * Creates the desired amount of quests, with random types.
    */
    void PopulateQuests () {
        for (int i = 0; i < nbrOfQuests; i++) {
            Quest.QuestType type;

            // Randomize QuestType
            int val = randomize.Next(0, 2);
            if (val == 0) {
                type = Quest.QuestType.LOCK;
            }
            else {
                type = Quest.QuestType.KILL;
            }

            quests.Add(CreateQuest(type));
        }
    }


    /*
     * Removes a quest. This is typically done when the player completes it.
     * Variable q is the quest to be removed, variable i represents its position
     * in the quest list.
     * 
     * The door does not get removed (although the quest's start point does), it
     * only gets unlocked.
     */
    void RemoveQuest (Quest q) {
        if (content[q.GetStartPos().x, q.GetStartPos().y].GetType() == typeof(Door)) {
            ((Door)content[q.GetStartPos().x, q.GetStartPos().y]).UnlockDoor();
        }

        content[q.GetStartPos().x, q.GetStartPos().y] = null;
        content[q.GetGoalPos().x, q.GetGoalPos().y] = null;
        this.quests.Remove(q);
    }


    /*
     * Give player pseudo-random position on grass.
     */
    void SpawnPlayer (int[,] map) {
        int x = randomize.Next(0, this.width);
        int y = randomize.Next(0, this.height);

        while (map[x, y] != 2) {
            x = randomize.Next(0, this.width);
            y = randomize.Next(0, this.height);
        }

        playerX = x;
        playerY = y;
    }


    // Controller, moves player.
    // BAD! Should be in a separate controller!
    public void MoveUp () {
        if (playerY < this.height-1 && map[playerX, playerY+1] != 3) {
            foreach (Door d in doorList) {
                if (playerX == d.GetPosition().x && playerY+1 == d.GetPosition().y) {
                    if (d.IsLocked()) {
                        Debug.Log("Collision with locked door!");
                        return;
                    }
                }
            }
            playerY++;
        }
    }


    public void MoveDown () {
        if (playerY > 0 && map[playerX, playerY-1] != 3) {
            foreach (Door d in doorList) {
                if (playerX == d.GetPosition().x && playerY - 1 == d.GetPosition().y) {
                    if (d.IsLocked()) {
                        Debug.Log("Collision with locked door!");
                        return;
                    }
                }
            }
            playerY--;
        }
    }


    public void MoveRight () {
        if (playerX < this.width- 1 && map[playerX+1, playerY] != 3) {
            foreach (Door d in doorList) {
                if (playerX + 1 == d.GetPosition().x && playerY == d.GetPosition().y) {
                    if (d.IsLocked()) {
                        Debug.Log("Collision with locked door!");
                        return;
                    }
                }
            }
            playerX++;
        }
    }


    public void MoveLeft () {
        if (playerX > 0 && map[playerX-1, playerY] != 3) {
            foreach (Door d in doorList) {
                if (playerX - 1 == d.GetPosition().x && playerY == d.GetPosition().y) {
                    if (d.IsLocked()) {
                        Debug.Log("Collision with locked door!");
                        return;
                    }
                }
            }
            playerX--;
        }
    }

    public void checkQuestCollision () {
        var i = 0;
        foreach (Quest q in quests) {
            if (content[playerX, playerY] == q) {

                if (playerX == q.GetStartPos().x && playerY == q.GetStartPos().y) {
                    if (!q.IsAccepted()) {
                        q.AcceptQuest();
                        Debug.Log("Quest accepted!");
                    }
                }
                else if (q.IsAccepted()) {
                    q.CompleteQuest();
                    this.RemoveQuest(q);
                    Debug.Log("Quest completed!");
                }
                break;
            }

            i++;
        }
    }


    /*
     * Draws player and quest points.
     */
    public void OnDrawGizmos () {
        // Render quests.
        for (int i = 0; i < quests.Count; i++) {
            Quest q = quests[i];

            // Draw/render start of quest.
            Gizmos.color = Color.white;
            Vector3 questPos = new Vector3(-width / 2 + q.GetStartPos().x + 0.5f, 0, -height / 2 + q.GetStartPos().y + 0.5f);
            Gizmos.DrawSphere(questPos, 0.5f);

            // Draw/render goal of quest if the quest has been accepted.
            if (q.IsAccepted()) {
                Gizmos.color = q.GetGoalColor();
                questPos = new Vector3(-width / 2 + q.GetGoalPos().x + 0.5f, 0, -height / 2 + q.GetGoalPos().y + 0.5f);
                Gizmos.DrawSphere(questPos, 0.5f);
            }
        }

        // Render player.
        Gizmos.color = Color.black;
        Vector3 playerPos = new Vector3(-width / 2 + playerX + 0.5f, 0, -height / 2 + playerY + 0.5f);
        Gizmos.DrawSphere(playerPos, 0.5f);
    }
}
