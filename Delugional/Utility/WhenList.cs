using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delugional.Utility
{
    public class WhenList<T> : IWhenList<T>
    {
        private readonly IList<T> backingList;

        private readonly IList<TaskCompletionSource<T>> completionSources = new List<TaskCompletionSource<T>>();

        public WhenList()
            : this(Enumerable.Empty<T>())
        {
        }

        public WhenList(IEnumerable<T> source)
        {
            backingList = new List<T>(source);
        }

        public int Count => backingList.Count;

        public bool IsReadOnly => backingList.IsReadOnly;

        public T this[int index]
        {
            get { return backingList[index]; }
            set
            {
                backingList[index] = value;
                ProcessConditions(value);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return backingList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            backingList.Add(item);
            ProcessConditions(item);
        }

        public void Clear()
        {
            backingList.Clear();
        }

        public bool Contains(T item)
        {
            return backingList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            backingList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return backingList.Remove(item);
        }

        public int IndexOf(T item)
        {
            return backingList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            backingList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            backingList.RemoveAt(index);
        }

        public Task<T> GetWhen(Func<T, bool> condition)
        {
            // We may already have the item
            T item = backingList.FirstOrDefault(condition);
            if (item != null)
                return Task.FromResult(item);

            // Create a completion source that we can use to
            // set the tasks result
            var tcs = new TaskCompletionSource<T>(condition);
            tcs.Task.ContinueWith(t =>
            {
                completionSources.Remove(tcs);
                return t.Result;
            });

            completionSources.Add(tcs);

            return tcs.Task;
        }

        private void ProcessConditions(T item)
        {
            TaskCompletionSource<T>[] sourcesThatMeetCondition = completionSources.Where(cs =>
            {
                var condition = (Func<T, bool>)cs.Task.AsyncState;
                return condition(item);
            }).ToArray();

            foreach (TaskCompletionSource<T> completionSource in sourcesThatMeetCondition)
            {
                completionSource.TrySetResult(item);
            }
        }
    }

    public interface IWhenList<T> : IList<T>
    {
        Task<T> GetWhen(Func<T, bool> condition);
    }

    public static class WhenExtension
    {
        public static WhenList<T> ToWhenList<T>(this IEnumerable<T> source, Func<T, bool> condition)
        {
            return new WhenList<T>(source);
        }
    }
}