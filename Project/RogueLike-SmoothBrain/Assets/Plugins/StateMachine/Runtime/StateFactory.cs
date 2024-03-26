using System;
using System.Collections.Generic;
using System.Reflection;

namespace StateMachine
{
    public class StateFactory<T>
    {
        Dictionary<Type, BaseState<T>> states = new Dictionary<Type, BaseState<T>>();

        public StateFactory(T context)
        {
            Type stateType = typeof(BaseState<T>);
            Type[] allValidTypes = Assembly.GetAssembly(stateType).GetTypes();
            foreach (Type type in allValidTypes)
            {
                ConstructorInfo constructor = type.GetConstructor(new[] { typeof(T), typeof(StateFactory<T>) });
                if (constructor != null && type != typeof(BaseState<T>))
                {
                    states[type] = (BaseState<T>)constructor.Invoke(new object[] { context, this });
                }
            }
        }

        public BaseState<T> GetState<U>()
        {
            return GetState(typeof(U));
        }

        public BaseState<T> GetState(Type stateType)
        {
            if (states.ContainsKey(stateType))
                return states[stateType];
            else
                throw new ArgumentException($"{stateType.Name} is not a valid state Type.");
        }
    }
}
