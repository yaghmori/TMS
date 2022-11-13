namespace TMS.DataAccess.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> GetChildFlatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childrenSelector)
        {
            var queue = new Queue<T>(source);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;
                var children = childrenSelector(current);
                if (children == null) continue;
                foreach (var child in children) queue.Enqueue(child);
            }
        }

    }
}
