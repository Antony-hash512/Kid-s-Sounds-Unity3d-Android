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
        for (int i = 0; i < audioClips.Length; i++)
        {
            GameObject button = Instantiate(buttonPrefab, buttonContainer);
            button.GetComponentInChildren<Text>().text = audioClips[i].name;
            int clipIndex = i;
            button.GetComponent<Button>().onClick.AddListener(() => PlaySound(clipIndex));

            // Задаем позицию кнопки
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -i * 50); // Шаг 50 по оси Y
        }
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
