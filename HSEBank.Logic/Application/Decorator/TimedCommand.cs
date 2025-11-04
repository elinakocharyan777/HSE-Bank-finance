using System.Diagnostics;
using HSEBank.Logic.Application.Interfaces;

namespace HSEBank.Logic.Application.Decorators;

public class TimedCommand<TResult> : ICommand<TResult>
{
    private readonly ICommand<TResult> _inner;
    private readonly Action<TimeSpan> _onCompleted;

    public TimedCommand(ICommand<TResult> inner, Action<TimeSpan>? onCompleted = null)
    {
        _inner = inner;
        _onCompleted = onCompleted ?? (_ => { });
    }

    public TResult Execute()
    {
        var sw = Stopwatch.StartNew();
        try { return _inner.Execute(); }
        finally { sw.Stop(); _onCompleted(sw.Elapsed); }
    }
}