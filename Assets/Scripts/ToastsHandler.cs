using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ToastsHandler : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private String toastsParentName;
    [SerializeField] private float durationSeconds;

    public static ToastsHandler Instance { get; private set; }
    
    private List<Label> _toastList;
    private Queue<ToastData> _dataQueue;
    
    private void Start()
    {
        var root = document.rootVisualElement;
        var toostsParent = root.Q(toastsParentName);
        var toastList = toostsParent.Children().ToList();
        
        _toastList = toastList.Select(ve =>
        {
            var children = ve.Children().ToList();
            return children.First() as Label;
        }).ToList();
        _toastList.ForEach(toast => toast.text = "");

        _dataQueue = new Queue<ToastData>(_toastList.Count);

        Instance = this;
    }

    public void CreateToastMessage(String message)
    {
        if (_dataQueue.Count >= _toastList.Count)
        {
            var oldData = _dataQueue.Dequeue();
            StopCoroutine(oldData.Coroutine);
        }

        var newData = new ToastData(message, StartCoroutine(RemoveDelay(durationSeconds)));
        _dataQueue.Enqueue(newData);
        
        UpdateToasts();
    }

    private IEnumerator RemoveDelay(float waitDuration)
    {
        yield return new WaitForSeconds(waitDuration);

        _dataQueue.Dequeue();
        
        UpdateToasts();
    }

    private void UpdateToasts()
    {
        var toastListEnumerator = _toastList.GetEnumerator();

        foreach (var data in _dataQueue)
        {
            if (!toastListEnumerator.MoveNext())
            {
                return;
            }

            toastListEnumerator.Current.text = data.Message;
        }

        while (toastListEnumerator.MoveNext())
        {
            toastListEnumerator.Current.text = "";
        }
    }

    private struct ToastData
    {
        public readonly String Message;
        public readonly Coroutine Coroutine;

        public ToastData(String message, Coroutine coroutine)
        {
            Message = message;
            Coroutine = coroutine;
        }
    }
}
