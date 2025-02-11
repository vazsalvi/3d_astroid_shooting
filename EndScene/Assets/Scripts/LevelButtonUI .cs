using UnityEngine;
using UnityEngine.UI;

public class LevelButtonUI : MonoBehaviour
{
    public Sprite defaultSprite;
    public Sprite completedSprite;
    public Text buttonText;

    public void SetCompleted()
    {
        GetComponent<Image>().sprite = completedSprite;
        buttonText.text = "Completed";
        buttonText.color = Color.yellow; // Optional: Change text color
    }

    public void SetDefault(int levelNumber)
    {
        GetComponent<Image>().sprite = defaultSprite;
        buttonText.text = "Level " + levelNumber;
        buttonText.color = Color.white; // Reset text color
    }
}
