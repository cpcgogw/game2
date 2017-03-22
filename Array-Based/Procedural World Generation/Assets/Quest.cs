using UnityEngine;

/*
 * Class for describing a Quest object. 
 * It contains a start and goal position, information wether it is 
 * accepted and/or completed, and what type of Quest it is. 
 * 
 * @author Jenny Orell
 */
public class Quest : ScriptableObject {

    private Position startPos, goalPos;
    private bool accepted, completed;
    private QuestType type;

    private Color goalColor;


    public enum QuestType { LOCK, KILL };

    /*
     * "Constructor".
     * Sets start and goal position, and type of quest.
     * If the quest is of type 'LOCK', it is also accepted.
     */
    public void Init (Position start, Position goal, QuestType type) {

        this.startPos = start;
        this.goalPos = goal;

        if (type == QuestType.LOCK) {
            this.AcceptQuest();
            goalColor = Color.magenta;
        }
        else {
            goalColor = Color.red;
            this.accepted = false;
        }
    }



    // ===== SETTERS =====

    public void SetStartPos (Position p) {
        this.startPos = p;
    }

    public void SetGoalPos (Position p) {
        this.goalPos = p;
    }

    public void SetQuestType (QuestType t) {
        this.type = t;
    }

    public void AcceptQuest () {
        this.accepted = true;
    }

    public void CompleteQuest () {
        this.completed = true;
    }


    // ===== GETTERS =====

    public Position GetStartPos () {
        return this.startPos;
    }

    public Position GetGoalPos () {
        return this.goalPos;
    }

    public QuestType GetQuestType () {
        return this.type;
    }

    public bool IsAccepted () {
        return this.accepted;
    }

    public bool IsCompleted () {
        return this.completed;
    }

    public Color GetGoalColor () {
        return this.goalColor;
    }
}