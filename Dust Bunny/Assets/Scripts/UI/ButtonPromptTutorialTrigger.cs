using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpringCleaning.Player;
public class ButtonPromptTutorialTrigger : MonoBehaviour
{
    public GameObject promptPrefab;
    public ButtonPromptTutorial.Type type;
    public bool followPlayer = true;
    public bool showOnStart = false;
    public float showOnStartTimer;
    public GameObject playerIfShowOnStart;

    void Start()
    {

    }

    void Update()
    {
        showOnStartTimer -= Time.deltaTime;
        if (showOnStartTimer < 0 && showOnStart)
        {
            ShowPrompt(playerIfShowOnStart);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IPlayerController controller)) return;

        ShowPrompt(collision.gameObject);

    } // end OnTriggerStay2D

    public void ShowPrompt(GameObject player)
    {
        //We know we collided with a player, make a new prompt
        GameObject instance = Instantiate(promptPrefab);
        instance.GetComponent<ButtonPromptTutorial>().type = type;
        instance.GetComponent<ButtonPromptTutorial>().followPlayer = followPlayer;
        instance.GetComponent<ButtonPromptTutorial>().SetPlayerReference(player.GetComponent<IPlayerController>());
        instance.transform.position = transform.position;


        Destroy(gameObject);
    }
}
