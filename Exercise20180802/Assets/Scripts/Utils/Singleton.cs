using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* whatever is inside of the <> is the variable name for the type that is going
 * to be used inside of the class
   where T : Singleton<T> means:
   the type that is passed in is an object that is meant to extend the singleton
   of that same type.
   This prevent us from creating singletons of arbitrary objects that does not 
   extend the class Singleton.
 */
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    //An accessor in C# is a method that looks like a variable that contains
    //a getter and a setter.
    public static T Instance { get; private set; }

    //where or not an instance has already been set.
    public static bool IsInitialized {
        get { return Instance != null; }
    }

    //virtual means the function can be overridden.
    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("[Singleton] Trying to instantiate a second " +
                "instance of a singleton class.");
        }
        else
        {

            //(T): we cast this to the T;
            //ensure the object is of type T that's being passed in
            Instance = (T) this;
        }

    }

    void Start()
    {

    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
