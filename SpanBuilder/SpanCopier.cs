using System.ComponentModel;

namespace SpanBuilder;

ref struct SpanCopier<T>
{
    readonly ReadOnlySpan<T> _rSpan;

    public int Length { get; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Provide an argument.", true)]
    public SpanCopier() { }

    public SpanCopier(ReadOnlySpan<T> rSpan)
    {
        _rSpan = rSpan;
        Length = rSpan.Length;
    }

    readonly Span<T> _span;

    public SpanCopier(Span<T> span)
    {
        _span = span;
        Length = span.Length;
    }

    public void CopyTo(Span<T> target, int from = 0, int count = -1)
    {
        if (_rSpan.Length > 0)
        {
            var source = _rSpan;
            if (count < 1)
            {
                source[from..].CopyTo(target);
            }
            else
            {
                source[from..(from + count)].CopyTo(target);
            }
        }
        if (_span.Length > 0)
        {
            var source = _span;
            if (count < 1)
            {
                source[from..].CopyTo(target);
            }
            else
            {
                source[from..(from + count)].CopyTo(target);
            }
        }
    }

}
