using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseManager : MonoBehaviour
{
    // layermask used to isolate raycasts against clickable layers
    public LayerMask clickableLayer;

    public Texture2D pointer;
    public Texture2D target;
    public Texture2D doorway;
    public Texture2D sword;

    public Events.EventVector3 OnClickEnvironment;
    public Events.EventGameObject OnClickAttackable;

    private bool _useDefaultCursor = false;

    private void Awake()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    void HandleGameStateChanged(GameManager.GameState currentState, 
        GameManager.GameState previousState)
    {
        _useDefaultCursor = (currentState != GameManager.GameState.RUNNING);
    }

    void Update()
    {
        if (_useDefaultCursor)
        {
            Cursor.SetCursor(pointer, Vector2.zero, CursorMode.Auto);
            return;
        }

        // Raycast into scene
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), 
            out hit, 50, clickableLayer.value))
        {
            //Override Cursor
            Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);

            bool door = false;
            
            if (hit.collider.gameObject.tag == "Doorway")
            {
                Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                door = true;
            }

            if (hit.collider.gameObject.tag == "Chest")
            {
                Cursor.SetCursor(pointer, new Vector2(16, 16), CursorMode.Auto);
            }

            bool isAttackable = hit.collider.GetComponent(typeof(IAttackable)) != null;
            if (isAttackable)
            {
                Cursor.SetCursor(sword, new Vector2(16, 16), CursorMode.Auto);
            }

            // If environment surface is clicked, invoke callbacks.
            if (Input.GetMouseButtonDown(0))
            {
                if (door)
                {
                    Transform doorway = hit.collider.gameObject.transform;
                    OnClickEnvironment.Invoke(doorway.position + doorway.forward * 10);
                }
                else if (isAttackable)
                {
                    GameObject attackable = hit.collider.gameObject;
                    OnClickAttackable.Invoke(attackable);
                }
                else
                {
                    OnClickEnvironment.Invoke(hit.point);
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                OnClickEnvironment.Invoke(hit.point);
            }
        }
        else
        {
            Cursor.SetCursor(pointer, Vector2.zero, CursorMode.Auto);
        }
    }
}


