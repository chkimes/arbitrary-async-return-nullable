using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NullableTaskLike
{
    [Tasklike(typeof(ListTaskMethodBuilder<>))]
    public class ListTaskLike<TResult> : List<TResult>
    {
        public bool IsCompleted => true;
        public bool IsCompletedSuccessfully => true;
        public bool IsFaulted => false;
        public bool IsCanceled => false;

        public ListTaskAwaiter<TResult> GetAwaiter() => new ListTaskAwaiter<TResult>(this);
        public TResult Result => this.FirstOrDefault();
    }

    public static class ListTaskLike
    {
        public static Yielder<T> Yield<T>(T value) => new Yielder<T>(value);
    }

    public struct ListTaskAwaiter<TResult> : ICriticalNotifyCompletion
    {
        private readonly ListTaskLike<TResult> _list;
        internal ListTaskAwaiter(ListTaskLike<TResult> list) { _list = list; }

        public bool IsCompleted => _list.IsCompleted;
        public TResult GetResult() => _list.Result;

        public void OnCompleted(Action continuation) => continuation();
        public void UnsafeOnCompleted(Action continuation) => continuation();
    }

    public class Yielder<TResult> : ICriticalNotifyCompletion
    {
        internal TResult Value;
        internal Yielder(TResult value) { Value = value; }

        public bool IsCompleted => false;
        public Yielder<TResult> GetAwaiter() => this;
        public void OnCompleted(Action continuation) => continuation();
        public void UnsafeOnCompleted(Action continuation) => continuation();
        public void GetResult() { }
    }

    class ListTaskMethodBuilder<TResult>
    {
        internal ListTaskLike<TResult> _list = new ListTaskLike<TResult>(); 

        public void SetResult(TResult result) => _list.Add(result);
        public ListTaskLike<TResult> Task => _list;
        public static ListTaskMethodBuilder<TResult> Create() => new ListTaskMethodBuilder<TResult>();
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine => stateMachine.MoveNext();

        public void SetStateMachine(IAsyncStateMachine stateMachine) { }
        public void SetException(Exception exception) { }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            if (awaiter is Yielder<TResult>)
            {
                _list.Add(((Yielder<TResult>)(object)awaiter).Value);
                stateMachine.MoveNext();
            }
        }
    }
}
