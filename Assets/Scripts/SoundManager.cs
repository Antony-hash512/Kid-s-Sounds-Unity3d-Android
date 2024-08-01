using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public GameObject buttonPrefab; // Префаб кнопки
    public Transform buttonContainer; // Контейнер для кнопок
    public float moveSpeed = 100f; // Скорость движения кнопок
    public float resetYPosition = 800f; // Фиксированная координата Y за пределами экрана сверху

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
        // Используйте Resources.Load для загрузки файлов из папки Resources
        string[] audioFileNames = { "ma" }; // Укажите имена ваших файлов без расширения

        audioClips = new AudioClip[audioFileNames.Length];
        buttonSprites = new Sprite[audioFileNames.Length];

        for (int i = 0; i < audioFileNames.Length; i++)
        {
            string oggFilePath = "ogg/" + audioFileNames[i];
            string pngFilePath = "png/" + audioFileNames[i];

            audioClips[i] = Resources.Load<AudioClip>(oggFilePath);
            if (audioClips[i] == null)
            {
                Debug.LogError("Failed to load audio clip: " + oggFilePath);
                continue; // Пропускаем итерацию, если аудиоклип не загружен
            }

            Texture2D texture = Resources.Load<Texture2D>(pngFilePath);
            if (texture != null)
            {
                Texture2D resizedTexture = ResizeTexture(texture, 256, 256);
                buttonSprites[i] = Sprite.Create(resizedTexture, new Rect(0, 0, resizedTexture.width, resizedTexture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("Failed to load or resize texture: " + pngFilePath);
                continue; // Пропускаем итерацию, если текстура не загружена или не читаема
            }
        }
    }

    void CreateButtons()
    {
        int columns = 4; // Количество столбцов
        float spacingX = 260f; // Расстояние между кнопками
        float spacingY = 260f; // Расстояние между кнопками
        float Yoffset = 200f;
        float Xoffset = 150f;

        for (int i = 0; i < audioClips.Length; i++)
        {
            if (audioClips[i] == null || buttonSprites[i] == null)
            {
                Debug.LogError("Skipping button creation for index: " + i + " due to missing audio or sprite.");
                continue; // Пропускаем итерацию, если аудиоклип или спрайт не загружены
            }

            GameObject button = Instantiate(buttonPrefab, buttonContainer);
            if (button == null)
            {
                Debug.LogError("Failed to instantiate button prefab.");
                continue; // Пропускаем итерацию, если кнопка не была создана
            }

            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText == null)
            {
                Debug.LogError("Button prefab is missing Text component.");
                continue; // Пропускаем итерацию, если у кнопки нет компонента Text
            }

            buttonText.text = audioClips[i].name;
            int clipIndex = i;
            Button uiButton = button.GetComponent<Button>();
            if (uiButton == null)
            {
                Debug.LogError("Button prefab is missing Button component.");
                continue; // Пропускаем итерацию, если у кнопки нет компонента Button
            }

            uiButton.onClick.AddListener(() => PlaySound(clipIndex));

            // Устанавливаем изображение на кнопке
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = buttonSprites[i];
            }
            else
            {
                Debug.LogError("Button prefab is missing Image component.");
            }

            // Задаем размер кнопки
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(256, 256);

                // Задаем начальную позицию кнопки
                float x = Xoffset - (columns * spacingX / 2) + (i % columns) * spacingX; // Вычисляем координату X
                float y = Yoffset - (i / columns) * spacingY; // Вычисляем координату Y (отрицательная для сдвига вниз)
                rectTransform.anchoredPosition = new Vector2(x, y);

                // Запускаем корутину для плавного движения кнопки вверх
                StartCoroutine(MoveButtonUp(rectTransform, spacingY * (audioClips.Length / columns)));
            }
            else
            {
                Debug.LogError("Button prefab is missing RectTransform component.");
            }
        }

        // Устанавливаем размер контейнера Content, чтобы учесть все кнопки
        RectTransform contentRect = buttonContainer.GetComponent<RectTransform>();
        if (contentRect != null)
        {
            contentRect.sizeDelta = new Vector2(columns * spacingX, (audioClips.Length / columns) * spacingY);
        }
        else
        {
            Debug.LogError("Button container is missing RectTransform component.");
        }
    }

    public void PlaySound(int clipIndex)
    {
        if (clipIndex >= 0 && clipIndex < audioClips.Length && audioClips[clipIndex] != null)
        {
            audioSource.clip = audioClips[clipIndex];
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Invalid clip index or audio clip is null: " + clipIndex);
        }
    }

    IEnumerator MoveButtonUp(RectTransform buttonRect, float totalHeight)
    {
        while (true)
        {
            buttonRect.anchoredPosition += new Vector2(0, moveSpeed * Time.deltaTime);

            if (buttonRect.anchoredPosition.y >= resetYPosition)
            {
                buttonRect.anchoredPosition -= new Vector2(0, totalHeight);
            }

            //yield return null;
            yield return new WaitForSeconds(0.0001f); // Пауза между кадрами, чтобы контролировать скорость обновления
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

