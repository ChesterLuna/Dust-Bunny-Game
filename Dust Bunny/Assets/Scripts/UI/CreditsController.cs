using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour
{
    [SerializeField] private string _nextLevelName;
    [SerializeField] private Vector3 _nextLevelSpawnLocation;
    [SerializeField] private Animator _transition;
    [SerializeField] private float _transitionTime = 1f;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetTrigger("start");
        
    }

    public void OnEnd(){
        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
        levelLoader.StartLoadLevel(_nextLevelName, _transition, _transitionTime);
    }
}
