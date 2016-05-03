using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NullableTaskLike
{

    [Tasklike(typeof(NullableTaskMethodBuilder<>))]
    public struct NullableTaskLike<TResult>
    {
        // A ValueTask holds *either* a value _result, *or* a task _task. Not both.
        // The idea is that if it's constructed just with the value, it avoids the heap allocation of a Task.
        internal readonly TResult _result;
        internal readonly bool _hasResult;

        public NullableTaskLike(TResult result)
        {
            _result = result;
            _hasResult = true;
        }

        public bool HasValue => _hasResult;
        public bool Equals(NullableTaskLike<TResult> other) => !_hasResult && !other._hasResult || _hasResult && other._hasResult && _result.Equals(other._result);
        public override bool Equals(object obj) => obj is NullableTaskLike<TResult> && Equals((NullableTaskLike<TResult>)obj);
        public static bool operator ==(NullableTaskLike<TResult> left, NullableTaskLike<TResult> right) => left.Equals(right);
        public static bool operator !=(NullableTaskLike<TResult> left, NullableTaskLike<TResult> right) => !left.Equals(right);
        public override string ToString() => _hasResult ? _result.ToString() : "";
        public bool IsCompleted => true;
        public bool IsCompletedSuccessfully => true;
        public bool IsFaulted => false;
        public bool IsCanceled => false;

        public NullableTaskAwaiter<TResult> GetAwaiter() => new NullableTaskAwaiter<TResult>(this);

        public TResult Result
        {
            get
            {
                if (!_hasResult) throw new Exception();
                return _result;
            }
        }
    }

    public struct NullableTaskAwaiter<TResult> : ICriticalNotifyCompletion
    {
        private readonly NullableTaskLike<TResult> _value;

        internal NullableTaskAwaiter(NullableTaskLike<TResult> value)
        {
            _value = value;
        }

        public bool IsCompleted => _value.IsCompleted;
        public TResult GetResult() => _value.Result;

        public void OnCompleted(Action continuation)
        {
            if (_value._hasResult) continuation();
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            if (_value._hasResult) continuation();
        }
    }

    struct NullableTaskMethodBuilder<TResult>
    {
        internal TResult _result;
        internal bool GotResult;

        public void SetResult(TResult result)
        {
            _result = result;
            GotResult = true;
        }

        public NullableTaskLike<TResult> Task => GotResult ? new NullableTaskLike<TResult>(_result) : new NullableTaskLike<TResult>();
        public static NullableTaskMethodBuilder<TResult> Create() => new NullableTaskMethodBuilder<TResult>();
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
        }
    }
}
