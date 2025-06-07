using UnityEngine;

/// <summary>
/// A ScriptableObject that holds the data for a single quiz item, 
/// pairing a name with its corresponding sprite.
/// </summary>
[CreateAssetMenu(fileName = "New Quiz Item", menuName = "Quiz/Quiz Item")]
public class QuizItem : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
}