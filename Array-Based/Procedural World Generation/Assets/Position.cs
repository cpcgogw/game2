using UnityEngine;

public class Position : ScriptableObject {
    public int x, y;

    public void Init(int x, int y) {
        this.x = x;
        this.y = y;
    }
}