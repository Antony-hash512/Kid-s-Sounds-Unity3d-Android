using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public GameObject buttonPrefab; // Префаб кнопки
    public Transform buttonContainer; // Контейнер для кнопок
    private AudioSource audioSource;
    private AudioClip[] audioClips;
    private Sprite[] buttonSprites;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        LoadSoundsAndImages();
        CreateButtons();
    }

    void LoadSoundsAndImages()
    {
        string oggPath = Path.Combine(Application.dataPath, "Resources/ogg");
        string pngPath = Path.Combine(Application.dataPath, "Resources/png");
        
        string[] oggFiles = Directory.GetFiles(oggPath, "*.ogg");
        string[] pngFiles = Directory.GetFiles(pngPath, "*.png");
        
        audioClips = new AudioClip[oggFiles.Length];
        buttonSprites = new Sprite[pngFiles.Length];

        for (int i = 0; i < oggFiles.Length; i++)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(oggFiles[i]);
            string oggFilePath = "ogg/" + fileNameWithoutExtension;
            string pngFilePath = "png/" + fileNameWithoutExtension;
            
            audioClips[i] = Resources.Load<AudioClip>(oggFilePath);
            Texture2D texture = Resources.Load<Texture2D>(pngFilePath);
            if (texture != null)
            {
                Texture2D resizedTexture = ResizeTexture(texture, 256, 256);
                buttonSprites[i] = Sprite.Create(resizedTexture, new Rect(0, 0, resizedTexture.width, resizedTexture.height), new Vector2(0.5f, 0.5f));
            }
        }
    }

    void CreateButtons()
    {
        int columns = 4; // Количество столбцов
        float spacingX = 256f; // Расстояние между кнопками
        float spacingY = 256f; // Расстояние между кнопками
        float Yoffset = 256f;

        for (int i = 0; i < audioClips.Length; i++)
        {
            GameObject button = Instantiate(buttonPrefab, buttonContainer);
            button.GetComponentInChildren<Text>().text = audioClips[i].name;
            int clipIndex = i;
            button.GetComponent<Button>().onClick.AddListener(() => PlaySound(clipIndex));

            // Устанавливаем изображение на кнопке
            if (buttonSprites[i] != null)
            {
                button.GetComponent<Image>().sprite = buttonSprites[i];
            }

            // Задаем позицию кнопки
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            float x = -(columns * spacingX / 2) + (i % columns) * spacingX; // Вычисляем координату X
            float y = Yoffset - (i / columns) * spacingY; // Вычисляем координату Y (отрицательная для сдвига вниз)
            rectTransform.anchoredPosition = new Vector2(x, y);
        }

        // Устанавливаем размер контейнера Content, чтобы учесть все кнопки
        RectTransform contentRect = buttonContainer.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(columns * spacingX, (audioClips.Length / columns + 1) * spacingY);
    }

    public void PlaySound(int clipIndex)
    {
        if (clipIndex >= 0 && clipIndex < audioClips.Length)
        {
            audioSource.clip = audioClips[clipIndex];
            audioSource.Play();
        }
    }

    Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        RenderTexture rt = new RenderTexture(newWidth, newHeight, 24);
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D result = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);
        result.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        result.Apply();
        return result;
    }
}

