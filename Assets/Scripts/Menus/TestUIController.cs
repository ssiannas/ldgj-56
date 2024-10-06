using UnityEngine;

namespace Menus
{
    public class TestUIController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Application.platform != RuntimePlatform.WindowsEditor) return;

            // 7 => Start Menu
            // 8 => Pause Menu
            // 9 => Tutorial
            // 0 => Hide Menus
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                Managers.UIManager.Instance.ShowStartMenu();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                Managers.UIManager.Instance.ShowPauseMenu();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                Managers.UIManager.Instance.ShowTutorialMenu();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Managers.UIManager.Instance.HideMenu();
            }
        }
    }
}