using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lady
{

    internal class Listener
    {
        public static Dictionary<object, List<Listener>> dict = new Dictionary<object, List<Listener>>();

        public static List<Listener> Retrieve(object target)
        {
            return dict.ContainsKey(target) ? dict[target] : null;
        }

        public static List<Listener> Ensure(object target)
        {
            List<Listener> listeners = Retrieve(target);

            if (listeners == null)
                dict[target] = listeners = new List<Listener>();

            return listeners;
        }

        public static void Clear(object target)
        {
            if (dict[target] != null)
            {
                dict[target].Clear();
                dict.Remove(target);
            }
        }

        public static string Info()
        {
            int listenerCounter = 0;

            foreach (var entry in dict)
                listenerCounter += entry.Value.Count;

            return string.Format("Lady.Listener: {0} targets, {1} listeners", dict.Count, listenerCounter);
        }


        static int counter = 0;

        public readonly int id;
        public readonly object target;
        public readonly string type;
        public readonly object key;
        public readonly Action<Event> callback;

        public Listener(object target, string type, Action<Event> callback, object key = null)
        {
            id = counter++;

            this.target = target;
            this.type = type;
            this.callback = callback;
            this.key = key;

            Debug.Log(ToString());
        }

        public bool Match(string type = "*", object key = null, Action<Event> callback = null)
        {
            return (type == "*" || type == this.type) 
                && (key == null || key == this.key)
                && (callback == null || callback == this.callback);
        }

        public override string ToString()
        {
            return string.Format("Listener#{0}[type: {1}, key: {2}]", id, type, key);
        }

    }

    public class Event
    {
        public static string Info()
        {
            return Listener.Info();
        }

        public static void On(object target, string type, Action<Event> callback, object key = null)
        {
            Listener.Ensure(target).Add(new Listener(target, type, callback, key));
        }

        public static void Off(object target, string type = "*", Action<Event> callback = null, object key = null)
        {
            List<Listener> listeners = Listener.Retrieve(target);

            if (listeners == null)
                return;

            foreach (Listener listener in new List<Listener>(listeners))
            {
                if (listener.Match(type, key, callback))
                    listeners.Remove(listener);
            }

            if (listeners.Count == 0)
                Listener.Clear(target);

            Debug.Log("Off\n" + Listener.Info());
        }

        public static void Dispatch(Event @event)
        {
            List<Listener> listeners = Listener.Retrieve(@event.currentTarget);

            if (listeners != null)
            {
                foreach (Listener listener in listeners)
                {
                    if (listener.Match(@event.type))
                        listener.callback(@event);
                }
            }

            @event.DoPropagation();
        }

        public static void Dispatch(object target, string type, bool cancelable = true, PropagationCallback propagation = null)
        {
            Dispatch(new Event(target, type, cancelable, propagation));
        }



        public delegate object[] PropagationCallback(object gameObject);

        public readonly object target;
        public readonly object currentTarget;
        public readonly string type;
        public readonly bool cancelable;
        public readonly PropagationCallback propagation;

        public bool Canceled { get; private set; }

        public Event(object target, string type, bool cancelable = true, PropagationCallback propagation = null, object currentTarget = null) 
        {
            this.target = target;
            this.currentTarget = currentTarget ?? target;
            this.type = type;
            this.cancelable = cancelable;
            this.propagation = propagation;
        }

        public void Cancel()
        {
            if (cancelable)
                Canceled = true;
        }

        private void DoPropagation()
        {
            if (propagation == null)
                return;

            foreach(object newCurrentTarget in propagation(currentTarget))
            {
                Debug.LogFormat("propagation {0}\n{1}", type, newCurrentTarget);

                Event currentEvent = new Event(target, type, cancelable, propagation, newCurrentTarget);

                Dispatch(currentEvent);
            }
        }
    }
}
