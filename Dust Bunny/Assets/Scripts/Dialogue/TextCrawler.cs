using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextCrawler : MonoBehaviour
{
    public List<string> text; //Text to display, separated by line
    public float speed; //speed in letters per second
    public float popinSpeed; //speed letters grow after appearing

    public bool autoStart = false; //makes text automatically scroll when box is initalized

    public bool paused = false;
    public TextMeshPro textController;

    public GameObject voicePrefab;

    private float elapsedLetters;
    private string currentText;
    private int lineIndex = -1;

    private List<float> sizes;
    private int letterIndex;
    private bool started;
    private bool initalized = false;
    private bool isEnabled = true;
    private AudioSource[] speechSources;
    private SpeechVoice voice;

    // Start is called before the first frame update
    void Start()
    {
        Initalize();
    }

    public TextCrawler Initalize()
    {
        if (!initalized)
        {
            //If unassigned, try to find text controller on this object first. If it fails, try to find it on any child.
            if (textController == null)
            {
                textController = GetComponent<TextMeshPro>();
                Debug.Log("Text mesh was not assigned to a text crawler. Using text mesh from parent.");
            }
            if (textController == null)
            {
                textController = GetComponentInChildren<TextMeshPro>();
                Debug.Log("Text mesh was not assigned to a text crawler. Using text mesh from parent.");
            }
            speechSources = GetComponents<AudioSource>();
            if (voicePrefab != null)
            {
                voice = voicePrefab.GetComponent<SpeechVoice>();
            }

            if (text == null)
            {
                text = new List<string>();
                text.Add(textController.text);
            }
            sizes = new List<float>();
            elapsedLetters = 0;
            letterIndex = 0;
            currentText = "";
            started = false;

            if (autoStart) Advance();
            else textController.text = "";

            initalized = true;
        }
        return this;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        if (isEnabled && !paused)
        {
            float previousLetters = elapsedLetters;
            elapsedLetters += dt * speed;
            while (IsStarted() && !IsFinished() && !IsFinishedLine() && ((int)elapsedLetters > (int)previousLetters || speed == float.PositiveInfinity))
            {
                AddLetter();
                elapsedLetters -= speed;
            }
            UpdateSizes();
            if (IsStarted() && !IsFinishedLineAndSizes())
            {
                textController.text = GetFormattedString();
            }
        }
    }

    public bool IsFinishedLine()
    {
        if (lineIndex >= text.Count) return true;
        return lineIndex >= 0 && letterIndex >= text[lineIndex].Length;
    }

    public bool IsFinishedLineAndSizes()
    {
        return IsFinishedLine() && !textController.text.Contains("<size=");
    }

    public bool IsFinished()
    {
        return IsFinishedLine() && lineIndex == text.Count - 1;
    }

    public bool IsStarted()
    {
        return lineIndex != -1 && started;
    }

    public void SetText(List<string> newText)
    {
        text = newText;
        currentText = "";
        lineIndex = -1;
        sizes = new List<float>();
        letterIndex = 0;
        started = false;
    }

    public void SetText(string newText)
    {
        List<string> l = new List<string>();
        l.Add(newText);
        SetText(l);
    }

    public void Advance()
    {
        currentText = "";
        lineIndex++;
        elapsedLetters = 0;
        letterIndex = 0;
        started = true;
        sizes = new List<float>();
    }

    public void FinishLine()
    {
        currentText = text[lineIndex];
        letterIndex = text[lineIndex].Length;
        sizes = new List<float>();
        for (int i = 0; i < letterIndex; i++)
        {
            sizes.Add(textController.fontSize);
        }
    }

    private void UpdateSizes()
    {
        for (int i = 0; i < sizes.Count; i++)
        {
            Debug.Log(textController.fontSize);
            sizes[i] += Time.deltaTime * popinSpeed;
            sizes[i] = Mathf.Min(textController.fontSize, sizes[i]);
            Debug.Log(sizes[i]);
        }
    }

    private void AddLetter()
    {
        char nextLetter = text[lineIndex][letterIndex];
        currentText += nextLetter;
        sizes.Add(1.0f);
        letterIndex++;

        //Find available audio source
        AudioSource openSource = null;
        for (int i = 0; i < speechSources.Length; i++)
        {
            if (!speechSources[i].isPlaying)
            {
                openSource = speechSources[i];
                break;
            }
        }

        // Play speech sfx
        if (openSource != null && voice != null)
        {
            openSource.clip = voice.GetLetterClip(nextLetter);
            openSource.Play();
        }
    }

    private string GetFormattedString()
    {
        string formattedString = "";

        //Size grow in effect
        for (int i = 0; i < currentText.Length; i++)
        {
            int size = (int)sizes[i];
            string result = "";
            if (size != textController.fontSize)
            {
                result += "<size=" + size.ToString() + ">" + currentText[i] + "</size>";
            }
            else
            {
                result = currentText[i].ToString();
            }
            formattedString += result;
        }

        return formattedString;
    }

    public void Disable()
    {
        isEnabled = false;
    }

    public void Enable()
    {
        isEnabled = true;
    }

    public void SetTextInstant(string newText)
    {
        Disable();
        textController.text = newText;
        currentText = newText;
    }
}