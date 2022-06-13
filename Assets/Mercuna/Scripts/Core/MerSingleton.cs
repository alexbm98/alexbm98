// Copyright (C) 2018 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using UnityEngine;

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// </summary>
namespace Mercuna
{
    public class MerSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T m_instance;

        private static object _lock = new object();

        public static T instance
        {
            get
            {
                lock (_lock)
                {
                    if (m_instance == null)
                    {
                        m_instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                " - there should never be more than 1 singleton!" +
                                " Reopening the scene might fix it.");
                            return m_instance;
                        }

                        if (m_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            m_instance = singleton.AddComponent<T>();
                            string name = typeof(T).ToString();
                            singleton.name = name.Substring(name.IndexOf('.') + 1);

                            Debug.Log("[Singleton] An instance of " + typeof(T) +
                                " is needed in the scene, so '" + singleton +
                                "' was created.");
                        }
                    }

                    return m_instance;
                }
            }
        }
    }
}
