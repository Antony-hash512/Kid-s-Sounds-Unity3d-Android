using UnityEngine;

public class ScreenResolution : MonoBehaviour
{
    void Start()
    {
        // Установите разрешение экрана (ширина, высота, полноэкранный режим)
        Screen.SetResolution(1920, 1080, true);
    }
}
