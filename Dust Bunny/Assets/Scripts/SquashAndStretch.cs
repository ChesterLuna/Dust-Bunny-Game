using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{

    [System.Serializable]
    public class SquashAndStretchParameters{
        public string name;
        public float duration;
        public AnimationCurve xcurve;
        public AnimationCurve ycurve;
        public AnimationCurve zcurve;
        public Vector3 scaleMask;

        public float currentTime;
    }

    public List<SquashAndStretchParameters> effects;

    public List<SquashAndStretchParameters> currentEffects;

    // Start is called before the first frame update
    void Start()
    {
        currentEffects = new List<SquashAndStretchParameters>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessEffects();
    }

    void ProcessEffects(){
        Vector3 scaleMod = new Vector3(1, 1, 1);

        List<int> indexesToRemove = new List<int>();
        for(int i = 0; i < currentEffects.Count; i++){
            SquashAndStretchParameters param = currentEffects[i];
            float access = 0;
            if(param.currentTime > 0){
                access = param.currentTime / param.duration;
            }
            scaleMod.x += param.xcurve.Evaluate(access) * param.scaleMask.x;
            scaleMod.y += param.ycurve.Evaluate(access) * param.scaleMask.y;
            scaleMod.z += param.zcurve.Evaluate(access) * param.scaleMask.z;

            //Handle removal
            param.currentTime += Time.deltaTime;
            if(param.currentTime > param.duration){
                indexesToRemove.Add(i);
            }
        }

        // Remove expired
        for(int i = indexesToRemove.Count - 1; i >= 0; i--){
            currentEffects.RemoveAt(indexesToRemove[i]);
        }

        transform.localScale = scaleMod;
    }

    public void StartEffect(string name){
        SquashAndStretchParameters param = GetParameter(name);
        if(param == null) return;

        SquashAndStretchParameters newParam = CopyParameters(param);
        newParam.currentTime = 0;
        currentEffects.Add(newParam);
    }

    public SquashAndStretchParameters GetParameter(string name){
        for(int i = 0; i < effects.Count; i++){
            SquashAndStretchParameters param = effects[i];
            if(param.name == name){
                return param;
            }
        }
        return null;
    }

    public void SetParameter(SquashAndStretchParameters param){
        for(int i = 0; i < effects.Count; i++){
            SquashAndStretchParameters existingParam = effects[i];
            if(existingParam.name == param.name){
                effects[i] = param;
                return;
            }
        }

        effects.Add(param);
    }

    private SquashAndStretchParameters CopyParameters(SquashAndStretchParameters param){
        SquashAndStretchParameters newParam = new SquashAndStretchParameters();
        newParam.name = param.name;
        newParam.duration = param.duration;
        newParam.scaleMask = param.scaleMask;
        newParam.xcurve = param.xcurve;
        newParam.ycurve = param.ycurve;
        newParam.zcurve = param.zcurve;
        newParam.currentTime = param.currentTime;
        return newParam;
    }


}
