using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WindowsHostDistributer.Distributer
{
    // Client Delegates
    public delegate void ClientAuthorizedEvent();
    public delegate void HostsListUpdatedEvent(bool changed);

    // Server Delegates

    public static class EventCallbackSystem
    {
        class EventCallbackInformation
        {
            public object DelegateInstance { get; private set; }
            public MethodInfo InvocationMethod { get; private set; }

            public EventCallbackInformation(object delegateInstance, MethodInfo invocationMethod)
            {
                DelegateInstance = delegateInstance;
                InvocationMethod = invocationMethod;
            }
        }

        private static readonly Dictionary<string, List<EventCallbackInformation>> _eventCallbacks = new Dictionary<string, List<EventCallbackInformation>>();

        public static void RegisterCallback<T>(string name, T handler)
        {
            var invocationMethod = handler.GetType().GetMethod(
                "Invoke",
                BindingFlags.Public | BindingFlags.Instance);

            if (invocationMethod == null)
                throw new ArgumentException("The handler does not appear to be a valid delegate.");

            List<EventCallbackInformation> list;
            if (!_eventCallbacks.TryGetValue(name, out list))
                _eventCallbacks.Add(name, list = new List<EventCallbackInformation>());

            list.Add(new EventCallbackInformation(handler, invocationMethod));
        }

        public static void InvokeCallback(string name, params object[] args)
        {
            List<EventCallbackInformation> list;
            if (_eventCallbacks.TryGetValue(name, out list))
            {
                foreach (var item in list)
                {
                    item.InvocationMethod.Invoke(item.DelegateInstance, args);
                }
            }
        }
    }
}
