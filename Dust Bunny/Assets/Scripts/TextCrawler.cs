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

    private float elapsedLetters;
    private string currentText;
    private int lineIndex = -1;
    private TextMeshPro textController;
    private List<float> sizes;
    private int letterIndex;
    private bool started;
    public string sfx;
    private bool initalized = false;
    private bool enabled = true;
    // Start is called before the first frame update
    void Start(){
        Initalize();
    }

    public TextCrawler Initalize()
    {
        if (!initalized){
            textController = GameObject.Find("Bubble Text").GetComponent<TextMeshPro>();
            if (text == null) {
                text = new List<string>();
                text.Add(textController.text);
            }
            sizes = new List<float>();
            elapsedLetters = 0;
            letterIndex = 0;
            currentText = "";
            started = false;

            if (autoStart) Advance();

            initalized = true;
        }
        return this;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        if (enabled && !paused){
            float previousLetters = elapsedLetters;
            elapsedLetters += dt * speed;
            while (IsStarted() && !IsFinished() && !IsFinishedLine() && ((int) elapsedLetters > (int) previousLetters || speed == float.PositiveInfinity)){
                AddLetter();
                elapsedLetters -= speed;
            }
            UpdateSizes();
            if (IsStarted()) textController.text = GetFormattedString();
        }
    }

    public bool IsFinishedLine(){
        if (lineIndex >= text.Count) return true;
        return lineIndex >= 0 && letterIndex >= text[lineIndex].Length;
    }

    public bool IsFinished(){
        return IsFinishedLine() && lineIndex == text.Count-1; 
    }

    public bool IsStarted(){
        return lineIndex != -1 && started;
    }

    public void SetText(List<string> newText){
        text = newText;
        currentText = "";
        lineIndex = -1;
        sizes = new List<float>();
        letterIndex = 0;
        started = false;
    }

    public void SetText(string newText){
        List<string> l = new List<string>();
        l.Add(newText);
        SetText(l);
    }

    public void Advance(){
        currentText = "";
        lineIndex++;
        elapsedLetters = 0;
        letterIndex = 0;
        started = true;
        sizes = new List<float>();
    }

    public void FinishLine(){
        currentText = text[lineIndex];
        letterIndex = text[lineIndex].Length;
        sizes = new List<float>();
        for(int i = 0; i < letterIndex; i++){
            sizes.Add(textController.fontSize);
        }
    }

    private void UpdateSizes(){
        for(int i = 0; i < sizes.Count; i++){
            sizes[i] += Time.deltaTime * popinSpeed;
            sizes[i] = Mathf.Min(textController.fontSize, sizes[i]);
        }
    }

    private void AddLetter(){
        currentText += text[lineIndex][letterIndex];
        sizes.Add(1.0f);
        letterIndex++;
        //if (sfx != null) NoiseMachine.Play(sfx, transform.position, 1.0f, sfxSettings);
    }

    private string GetFormattedString(){
        string formattedString = "";

        //Size grow in effect
        for(int i = 0; i < currentText.Length; i++){
            int size = (int)sizes[i];
            string result = "";
            if (size != textController.fontSize){
                result += "<size=" + size.ToString() + ">" + currentText[i] + "</size>";
            }
            else{
                result = currentText[i].ToString();
            }
            formattedString += result;
        }

        return formattedString;
    }

    public void Disable(){
        enabled = false;
    }

    public void Enable(){
        enabled = true;
    }

    public void SetTextInstant(string newText){
        Disable();
        textController.text = newText;
        currentText = newText;
    }
}