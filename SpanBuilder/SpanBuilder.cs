using System.Buffers;
using System.Collections.Immutable;

namespace SpanBuilder;

public class SpanBuilder<T>
{
    public int ExpansionPace { get; init; } = 50;
    static readonly ArrayPool<T> _pool = ArrayPool<T>.Shared;
    (int, int) _position = (-1, 0);
    readonly List<T[]> _borrowedMemory;

    public SpanBuilder()
    {
        _borrowedMemory = new List<T[]>(2);
    }

    public void AppendRange(Span<T> items) => AppendRange(new SpanCopier<T>(items));

    public void AppendRange(ReadOnlySpan<T> items) => AppendRange(new SpanCopier<T>(items));

    public void Append(T item)
    {
        var (arrIndex, index) = _position;

        var currentArray = arrIndex > -1 ? _borrowedMemory[arrIndex] : Array.Empty<T>();

        _position = (arrIndex, index + 1);
        if (currentArray.Length - 1 <= index)
        {
            currentArray = _pool.Rent(ExpansionPace * (_borrowedMemory.Count + 1));
            _borrowedMemory.Add(currentArray);
            currentArray[0] = item;
            _position = (arrIndex + 1, 1);
            return;
        }

        currentArray[index] = item;
    }

    public Span<T> Build()
    {
        var length = _position.Item2;
        for (int i = 0; i <= _position.Item1 - 1; i++)
        {
            var arr = _borrowedMemory[i];
            length += arr.Length;
        }

        Span<T> result = new T[length];

        var passedLength = 0;
        for (int i = 0; i <= _position.Item1 - 1; i++)
        {
            Span<T> arr = _borrowedMemory[i];
            arr.CopyTo(result[passedLength..]);
            passedLength += arr.Length;
        }

        var lastChunk = _borrowedMemory[_position.Item1].AsSpan();
        lastChunk[.._position.Item2].CopyTo(result[passedLength..]);

        Clear();

        return result;
    }

    void AppendRange(SpanCopier<T> values)
    {
        var (arrIndex, index) = _position;
        var currentArray = _borrowedMemory.Count > 0 ? _borrowedMemory[arrIndex] : Array.Empty<T>();
        var payloadLength = values.Length;
        int leftOfCurrent = currentArray.Length - index;

        int forCurrent = 0;
        if (leftOfCurrent > 0)
        {
            forCurrent = leftOfCurrent < payloadLength ? leftOfCurrent : payloadLength;
        }
        int forBorrowed = payloadLength - forCurrent;

        if (forCurrent > 0)
        {
            var currentSpan = currentArray.AsSpan();
            values.CopyTo(currentSpan[index..], 0, forCurrent);

            index += forCurrent;
        }

        if (forBorrowed > 0)
        {
            var newArray = _pool.Rent(ExpansionPace * (_borrowedMemory.Count + 1) + forBorrowed);
            _borrowedMemory.Add(newArray);

            Span<T> newSpan = newArray;
            values.CopyTo(newSpan, forCurrent);

            arrIndex++;
            index = forBorrowed;
        }

        _position = (arrIndex, index);
    }

    ~SpanBuilder()
    {
        Clear();
    }

    void Clear()
    {
        for (int i = 0; i < _borrowedMemory.Count; i++)
        {
            _pool.Return(_borrowedMemory[i]);
        }
        _borrowedMemory.Clear();
    }
}
