using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecayMask : MonoBehaviour
{
    private float timer;
    private float shrinkTime;
    private float defaultScaleX;
    private float defaultScaleY;
    private void Awake() 
    {
        timer = 0;
        defaultScaleX = transform.localScale.x;
        defaultScaleY = transform.localScale.y;
        shrinkTime = 1;
    }
    public void setShrinkTime(float time)
    {
        shrinkTime = time;
    }
    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= shrinkTime)
        {
            timer = 0;
            Destroy(gameObject);
        }

        float tmp = 1 - timer / shrinkTime;
        transform.localScale = new Vector3(defaultScaleX * tmp, defaultScaleY * tmp, 1);
    }
}
