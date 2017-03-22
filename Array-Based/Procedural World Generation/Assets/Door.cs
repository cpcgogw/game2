using UnityEngine;

public class Door : ScriptableObject {
    private bool locked;
    private Position pos;
    private Quest quest;

    /*
     * Constructor.
     */
    public void Init(int x, int y, Quest quest) {
        pos = ScriptableObject.CreateInstance("Position") as Position;
        pos.Init(x, y);

        this.quest = quest;
        this.LockDoor();
    }


    public bool IsLocked() {
        return this.locked;
    }

    public void LockDoor() {
        this.locked = true;
    }

    public void UnlockDoor() {
        this.locked = false;
    }

    public Position GetPosition() {
        return this.pos;
    }
}