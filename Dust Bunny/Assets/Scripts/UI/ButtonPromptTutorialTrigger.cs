using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPromptTutorialTrigger : MonoBehaviour
{
    public GameObject promptPrefab;
    public ButtonPromptTutorial.Type type;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IPlayerController controller)) return;
        
        //We know we collided with a player, make a new prompt
        GameObject instance = Instantiate(promptPrefab);
        instance.GetComponent<ButtonPromptTutorial>().type = type;
        instance.GetComponent<ButtonPromptTutorial>().SetPlayerReference(controller);
        instance.transform.position = transform.position;


        Destroy(gameObject);

    } // end OnTriggerStay2D
}
