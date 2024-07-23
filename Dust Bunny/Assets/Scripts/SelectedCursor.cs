using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedCursor : MonoBehaviour
{
    [SerializeField] private GameObject cursor;
    [SerializeField] private float speed;
    [SerializeField] private Vector2 offset;

    private Canvas canvas;

    private GameObject currentTarget;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        canvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        currentTarget = EventSystem.current.currentSelectedGameObject;


        if (CheckIfValid()){
            ShowCursor(true);
            UpdatePosition();
        } else {
            ShowCursor(false);
        }
    }

    bool CheckIfValid(){
        return currentTarget != null;
    }

    void ShowCursor(bool show){
        canvas.enabled = show;
    }

    void UpdatePosition(){
        Vector3 totalOffset = new Vector3(
            offset.x + currentTarget.GetComponent<RectTransform>().rect.width/2, 
            offset.y + currentTarget.GetComponent<RectTransform>().rect.height/2,
            0
        );

        cursor.transform.position = Vector3.Lerp(cursor.transform.position, currentTarget.transform.position + totalOffset, speed * Time.unscaledDeltaTime);
    }
}
