using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingText : MonoBehaviour 
{
    //control how many seconds the text scrolls before detroy it.
    public float Duration = 1f;
    public float Speed;

    TextMesh textMesh;
    float startTime;

    //Use awake instead of start since getting reference to one of the components.
	void Awake () 
    {
        textMesh = GetComponent<TextMesh>();

        //Time.time tells how many seconds have passed since the game started.
        //This can keep track of how long the text has been scrolling.
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(Time.time - startTime < Duration)
        {
            //Scroll up
            transform.LookAt(Camera.main.transform);
            transform.Translate(Vector3.up * Speed * Time.deltaTime);
        }
        else
        {
            //Destroy
            Destroy(gameObject);
        }
	}

    public void SetText(string text)
    {
        textMesh.text = text;    
    }

    public void SetColor(Color color)
    {
        textMesh.color = color;
    }
}
