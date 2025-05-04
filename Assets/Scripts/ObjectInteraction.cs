using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Events;

public class ObjectInteraction : MonoBehaviour
{
    public UnityEvent onInteract;

    private WebManager _webManager;

    void Awake()
    {
        _webManager = GameObject.FindWithTag("Web").GetComponent<WebManager>();
        if (_webManager == null)
        {
            Debug.LogWarning("This scene has not contain Web Manager");
        }

    }

    public void ObjectInteract()
    {
        onInteract.Invoke();
    }

    public void LLMResponse()
    {
        StartCoroutine(_webManager.GetResponse((response) => {
            Debug.Log($"{response}");
        }));
    }

    public void DisableObejct()
    {
        gameObject.SetActive(false);
    }
  
}
