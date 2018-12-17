using UnityEngine;

public class Exciter : MonoBehaviour 
{
     RectTransform rectTransform;

     void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

     void OnEnable()
    {
        rectTransform.localScale = Vector3.zero;
    }

    public void Stop()
    {
        gameObject.SetActive(false);
    }

}
