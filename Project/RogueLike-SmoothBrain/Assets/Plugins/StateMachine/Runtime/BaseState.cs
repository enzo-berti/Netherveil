using UnityEngine;

namespace StateMachine
{
    public abstract class BaseState<T>
    {
        private T context;
        private StateFactory<T> factory;

        protected T Context => context;
        protected StateFactory<T> Factory => factory;

        public BaseState(T currentContext, StateFactory<T> currentFactory)
        {
            context = currentContext;
            factory = currentFactory;
        }

        protected abstract void EnterState();
        protected abstract void UpdateState();
        protected abstract void ExitState();
        protected abstract void CheckSwitchStates();

        public void Update()
        {
            CheckSwitchStates();
            UpdateState();
        }

        protected virtual void SwitchState(BaseState<T> newState)
        {
            // current state exits state
            ExitState();

            // new state enters state
            newState.EnterState();
        }
    }
}
