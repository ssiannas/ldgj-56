using UnityEngine;
using TMPro;

public class PlaceHolderTextController : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        string userName = LeaderBoardController.Instance.GetUserName();
        if (userName != null && userName != "") { GetComponent<TextMeshProUGUI>().text = userName; } 
    }
}
