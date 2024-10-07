using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{

    private TextMeshProUGUI text;
    private static string TEXT_HEADER = "Score: ";

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        UpdateScore(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScore(float score)
    {
        text.SetText($"{TEXT_HEADER}{score}");
    }
}
