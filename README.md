# Nullable Tasklike

Quick and dirty monadic Nullable implementation for the C# Tasklike language proposal

# What?

Running this C# code:

```csharp
static void Main()
{
    var nullable = DoNullableThings();
    Console.WriteLine(nullable.HasValue);
    Console.ReadLine();
}

static async NullableTaskLike<int> DoNullableThings()
{
    var val1 = await GetValue();
    var val2 = await GetNoValue();

    var val3 = await GetValue();
    var val4 = await GetValue();
    var val5 = await GetValue();
    var val6 = await GetValue();
    var val7 = await GetValue();
    return val7;
}

static async NullableTaskLike<int> GetValue()
{
    Console.WriteLine("Returning 1");
    return 1;
}

static async NullableTaskLike<int> GetNoValue()
{
    Console.WriteLine("Returning none");
    return await new NullableTaskLike<int>();
}
```

produces this output:

```
Returning 1
Returning none
False
```

# Wait, what?

Using the arbitrary async return C# language proposal (located here: https://github.com/ljw1004/roslyn/blob/features/async-return/docs/specs/feature%20-%20arbitrary%20async%20returns.md), we can return any task-like type we want from async methods. Using this, we can leverage the compiler to write code that acts like do-notation in Haskell. This implementation of `Nullable` is akin to Haskell's `Maybe` type.

If a `NullableTaskLike` method returns a concrete value then execution of the calling method proceeds after an `await`. However, if the method does not return a value (well, returns an empty NullableTaskLike) then execution of the method short circuits.

We can rewrite the above code to compiling C# 6 as:

```
static Nullable<int> DoNullableThings()
{
    var val1 = GetValue();
    if (!val1.HasValue) return val1;
    
    var val2 = await GetNoValue();
    if (!val2.HasValue) return val2;

    
    ... etc ...
    
    var val7 = await GetValue();
    if (!val7.HasValue) return val7;
    
    return val7;
}
```

# Notes

In its current form, this is not compatible with asynchronous tasks.
