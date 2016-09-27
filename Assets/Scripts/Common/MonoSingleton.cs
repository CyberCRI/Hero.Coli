using UnityEngine;
 
 public class MonoSingleton<T> : MonoBehaviour where T: MonoSingleton<T> { 
     
     public static T instance;
     
     void Awake()
     {
         instance = this as T;
         AwakeEx();    
     }
     
     protected virtual void AwakeEx()
     {
     }
     
     protected virtual void OnDestroyEx()
     {
     }
     
     void OnDestroy()
     {
         OnDestroyEx();
         if(instance == this) instance = null;
     }
 }