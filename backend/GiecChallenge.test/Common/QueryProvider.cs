using System.Linq.Expressions;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

#nullable disable

namespace GiecChallenge.Tests
{
    public static class ICollectionExtensions
    {
        public static IQueryable<T> AsAsyncQueryable<T>(this ICollection<T> source) =>
            new AsyncQueryable<T>(source.AsQueryable());
    }

    internal class AsyncQueryable<T> : IAsyncEnumerable<T>, IQueryable<T>
    {
        private IQueryable<T> Source;

        public AsyncQueryable(IQueryable<T> source)
        {
            Source = source;
        }

        public Type ElementType => typeof(T);

        public Expression Expression => Source.Expression;

        public IQueryProvider Provider => new AsyncQueryProvider<T>(Source.Provider);

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new AsyncEnumeratorWrapper<T>(Source.GetEnumerator());
        }

        public IEnumerator<T> GetEnumerator() => Source.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal class AsyncQueryProvider<T> : IQueryProvider, IAsyncQueryProvider
    {
        private readonly IQueryProvider Source;

        public AsyncQueryProvider(IQueryProvider source)
        {
            Source = source;
        }

        public IQueryable CreateQuery(Expression expression) =>
            Source.CreateQuery(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
            new AsyncQueryable<TElement>(Source.CreateQuery<TElement>(expression));

        public object Execute(Expression expression) => Execute<T>(expression);

        public TResult Execute<TResult>(Expression expression) =>
            Source.Execute<TResult>(expression);

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                                .GetMethod(
                                    name: nameof(IQueryProvider.Execute),
                                    genericParameterCount: 1,
                                    types: new[] {typeof(Expression)})
                                .MakeGenericMethod(expectedResultType)
                                .Invoke(this, new[] {expression});

            return (TResult) typeof(Task).GetMethod(nameof(Task.FromResult))
                                        ?.MakeGenericMethod(expectedResultType)
                                        .Invoke(null, new[] {executionResult});
        }

        TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) =>
            ExecuteAsync<TResult>(expression, cancellationToken);
    }

    internal class AsyncEnumeratorWrapper<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> Source;

        public AsyncEnumeratorWrapper(IEnumerator<T> source)
        {
            Source = source;
        }

        public T Current => Source.Current;

        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(Source.MoveNext());
        }
    }
}