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

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        LoadSounds();
        CreateButtons();
    }

    void LoadSounds()
    {
        string path = Path.Combine(Application.dataPath, "Resources/ogg");
        string[] files = Directory.GetFiles(path, "*.ogg");
        audioClips = new AudioClip[files.Length];

        for (int i = 0; i < files.Length; i++)
        {
            string filePath = "ogg/" + Path.GetFileNameWithoutExtension(files[i]);
            audioClips[i] = Resources.Load<AudioClip>(filePath);
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

            // Задаем позицию кнопки
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            float x = -(columns * spacingX/2) + (i % columns) * spacingX; // Вычисляем координату X
            float y = Yoffset- (i / columns) * spacingY; // Вычисляем координату Y (отрицательная для сдвига вниз)
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
}
