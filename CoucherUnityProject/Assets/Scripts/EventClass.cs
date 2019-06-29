using System.Collections.Generic;
using System;

public class Event0
{
    public Dictionary<IListener, Action> ListenerDict { get; private set; }

    public Event0()
    {
        ListenerDict = new Dictionary<IListener, Action>();
    }

    public void AddListener(IListener listener, Action action)
    {
        if (ListenerDict.ContainsKey(listener) == false)
        {
            ListenerDict.Add(listener, action);
        }
    }

    public void RemoveListener(IListener listener)
    {
        if (ListenerDict.ContainsKey(listener) == true)
        {
            ListenerDict.Remove(listener);
        }
    }

    public void RaiseEvent()
    {
        foreach (Action action in ListenerDict.Values)
        {
            action.Invoke();
        }
    }
}

public class Event1<T>
{
    public Dictionary<IListener, Action<T>> ListenerDict { get; private set; }

    public Event1()
    {
        ListenerDict = new Dictionary<IListener, Action<T>>();
    }

    public void AddListener(IListener listener, Action<T> action)
    {
        if (ListenerDict.ContainsKey(listener) == false)
        {
            ListenerDict.Add(listener, action);
        }
    }

    public void RemoveListener(IListener listener)
    {
        if (ListenerDict.ContainsKey(listener) == true)
        {
            ListenerDict.Remove(listener);
        }
    }

    public void RaiseEvent(T data)
    {
        foreach (Action<T> action in ListenerDict.Values)
        {
            action.Invoke(data);
        }
    }
}

public interface IListener
{

}





