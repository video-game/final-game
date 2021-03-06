﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMB<T> : MonoBehaviour {

    //Singleton
    protected static SingletonMB<T> instance = null;
    protected static T component;
    public static T Instance {
        get {
            if(component == null || EqualityComparer<T>.Default.Equals(component, default(T)) && instance != null)
            {
                component = instance.GetComponent<T>();
            }
            return component;
        }
    }

    //Virtual method, that can be implemented
    //The purpose of this is to copy any neccasery information over to a new scene.
    //Some singletons might not have any values that need copying.
    public virtual void CopyValues(T copy) { }

    protected void Awake()
    {
        //If another instance exists, copy it's values
        //This is used when moving between scenes.
        //moving between scenes, destroys the items, so no need to force destroy here.
        if (instance != null && instance != this)
        {
            this.CopyValues(Instance);
            component = default(T);
        }

        instance = this;
    }


}
