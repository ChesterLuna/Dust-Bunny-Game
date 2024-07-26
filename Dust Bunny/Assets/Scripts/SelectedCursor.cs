using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedCursor : MonoBehaviour
{
    public static SelectedCursor instance;

    [SerializeField] private GameObject cursor;
    [SerializeField] private float speed;
    [SerializeField] private Vector2 offset;


    private Canvas canvas;
    private Image image;

    private bool manualHide = false;

    private GameObject currentTarget;
    private GameObject lastSelectedObject;
    private GameObject lastHoveredObject;
    // Start is called before the first frame update
    void Start()
    {
        if(instance != null){
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        instance = this;

        canvas = GetComponent<Canvas>();
        image = GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject controllerSelected = EventSystem.current.currentSelectedGameObject;
        if(controllerSelected != lastSelectedObject){
            SetTarget(controllerSelected);
        }
        lastSelectedObject = controllerSelected;

        GameObject hoverSelected = PointerOverUIObject();
        if(hoverSelected != null && hoverSelected != lastHoveredObject){
            SetTarget(hoverSelected);
        }
        lastHoveredObject = hoverSelected;

        if (CheckIfValid()){
            ShowCursor(true);
            UpdatePosition();
        } else {
            ShowCursor(false);
        }
    }

    public void SetManualHide(bool newHide){
        manualHide = newHide;
    }

    bool CheckIfValid(){
        return currentTarget != null && !manualHide;
    }

    void ShowCursor(bool show){
        image.enabled = show;
    }

    void UpdatePosition(){
        Vector3 totalOffset = new Vector3(
            (offset.x + currentTarget.GetComponent<RectTransform>().rect.width/2) * transform.localScale.x, 
            (offset.y - currentTarget.GetComponent<RectTransform>().rect.height/2) * transform.localScale.y,
            0
        );

        cursor.transform.position = Vector3.Lerp(cursor.transform.position, currentTarget.transform.position + totalOffset, speed * Time.unscaledDeltaTime);
    }

    public void SetTarget(GameObject newtarget){
        currentTarget = newtarget;
    }

    public GameObject PointerOverUIObject(){
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        GameObject realTarget = null;
        for(int i = 0; i < results.Count; i++){
            if(results[i].gameObject.GetComponent<Selectable>() != null){
                return results[i].gameObject;
            }
        }
        return null;
    }
}
