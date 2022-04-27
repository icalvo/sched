namespace Scheduler.Options;

public abstract class Option
{
    public static readonly NoneOption None = new();

    public abstract bool IsSome { get; }
    public bool IsNone => !IsSome;

    public static Option<T> Some<T>(T i) => Option<T>.Some(i);
}

public abstract class Option<T> : Option
{
    private static readonly Option<T> NoneT = new NoneTyped();
    

    public static Option<T> Some(T i)
    {
        return new SomeTyped(i);
    }    

    public static implicit operator Option<T>(T value) =>
        new SomeTyped(value);

    public static implicit operator Option<T>(NoneOption _) => NoneT;

    public abstract Option<TResult> Map<TResult>(Func<T, TResult> map);
    public abstract Option<TResult> MapOptional<TResult>(Func<T, Option<TResult>> map);
    public abstract T Reduce(T whenNone);
    public abstract T Reduce(Func<T> whenNone);

    public TResult MapReduce<TResult>(Func<T, TResult> map, TResult whenNone)
        => Map(map).Reduce(whenNone);
    public TResult MapReduce<TResult>(Func<T, TResult> map, Func<TResult> whenNone)
        => Map(map).Reduce(whenNone);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Option<T>)obj);
    }

    private bool Equals(Option<T> other)
    {
        return MapReduce(
            x => other.MapReduce(y => Equals(x, y), false),
            other.IsNone);
    }

    public override int GetHashCode()
    {
        return Map(x => x?.GetHashCode() ?? 0).Reduce(0);
    }

    private sealed class SomeTyped : Option<T>
    {
        public T Content { get; }

        public SomeTyped(T value)
        {
            Content = value;
        }

        public static implicit operator T(SomeTyped some) =>
            some.Content;

    
        public override bool IsSome => true;

        public override Option<TResult> Map<TResult>(Func<T, TResult> map) =>
            map(Content);

        public override Option<TResult> MapOptional<TResult>(Func<T, Option<TResult>> map) =>
            map(Content);

        public override T Reduce(T whenNone) =>
            Content;

        public override T Reduce(Func<T> whenNone) =>
            Content;

        public override string ToString()
        {
            return $"Some {Content}";
        }
    }
    private class NoneTyped : Option<T>
    {
        public override bool IsSome => false;

        public override Option<TResult> Map<TResult>(Func<T, TResult> map) => None;

        public override Option<TResult> MapOptional<TResult>(Func<T, Option<TResult>> map) => None;

        public override T Reduce(T whenNone) => whenNone;

        public override T Reduce(Func<T> whenNone) => whenNone();

        public override bool Equals(object? obj) => obj is Option { IsSome: false };

        public override int GetHashCode() => 0;
    }
}