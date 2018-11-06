using UnityEngine;
using UnityEngine.UI;

public class GameStatsUI : MonoBehaviour
{
    public Text DistanceText;
    public Text AirTimeText;

    void Start()
    {
        DistanceText.color = Color.yellow;
        DistanceText.text = "";
        AirTimeText.text = "";
    }

    void Update()
    {
        var stats = ObjectLocator.Stats;
        if(stats == null) return;

        if(stats.Fallen)
        {
            DistanceText.text = "YOU BAILED!";
            DistanceText.color = Color.red;
            AirTimeText.text = "";
            return;
        }

        if(stats.CurrentDistance > 0f)
        {
            DistanceText.text = "Distance: " + (int)stats.CurrentDistance + " m";
            if(stats.FinalDistance > 0f)
            {
                DistanceText.text = "Distance: " + (int)stats.FinalDistance + " m";
                DistanceText.color = Color.yellow;
                AirTimeText.text = "Air Time: " + System.Math.Round(stats.AirTime, 2).ToString();
                AirTimeText.color = Color.yellow;
            }
        }
    }
}