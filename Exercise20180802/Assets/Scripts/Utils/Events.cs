using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Events {

    //Param: Incoming GameState, previous GameState
    [System.Serializable] public class EventGameState : UnityEvent
        <GameManager.GameState, GameManager.GameState> { }

    //param: true for FadeOut, false for FadeIn
    [System.Serializable] public class EventFadeComplete : UnityEvent<bool> { }

    //Send the clicked point to the NavMesh Agent so it can move to that point 
    [System.Serializable] public class EventVector3 : UnityEvent<Vector3> { }

    [System.Serializable] public class EventGameObject: UnityEvent<GameObject> { }

    [System.Serializable] public class EventMobDeath: UnityEvent<MobType,Vector3>{ }

    [System.Serializable] public class EventIntegerEvent: UnityEvent<int>{ }
}

public class EventTriggerExample : EventTrigger {
    public override void OnPointerClick(PointerEventData data)
    {
        Debug.Log("OnPointerClick called.");
    }
}
