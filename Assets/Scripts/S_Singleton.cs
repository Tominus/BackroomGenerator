using UnityEngine;

public class S_Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;

    public static T Instance => instance;

    protected virtual void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this as T;
    }
}