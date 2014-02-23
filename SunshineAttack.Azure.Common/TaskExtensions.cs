using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SunshineAttack.Azure.Common
{
    public static class TaskExtentions
    {

        private static Task _task;

        public static Task<T> Empty<T>(this Task task)
        {
            if (_task == null) _task = Task.FromResult(default(T));

            return (Task<T>)_task;
        }

        public static Task ForEachAsync<T>(this TaskFactory factory, IEnumerable<T> items,
                                           Func<T, int, Task> getProcessItemTask)
        {
            var tcs = new TaskCompletionSource<object>();

            IEnumerator<T> enumerator = items.GetEnumerator();
            var i = 0;

            Action<Task> continuationAction = null;
            continuationAction = ante =>
                {
                    if (ante.IsFaulted)
                        tcs.SetException(ante.Exception);
                    else if (ante.IsCanceled)
                        tcs.TrySetCanceled();
                    else
                        StartNextForEachIteration(factory, tcs, getProcessItemTask, enumerator, ref i,
                                                  continuationAction);
                };

            StartNextForEachIteration(factory, tcs, getProcessItemTask, enumerator, ref i, continuationAction);

            tcs.Task.ContinueWith(_ => enumerator.Dispose(), TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        private static void StartNextForEachIteration<T>(TaskFactory factory, TaskCompletionSource<object> tcs,
                                                         Func<T, int, Task> getProcessItemTask,
                                                         IEnumerator<T> enumerator, ref int i,
                                                         Action<Task> continuationAction)
        {
            bool moveNext;
            try
            {
                moveNext = enumerator.MoveNext();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
                return;
            }

            if (!moveNext)
            {
                tcs.SetResult(null);
                return;
            }

            Task iterationTask = null;
            try
            {
                iterationTask = getProcessItemTask(enumerator.Current, i);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            i++;

            if (iterationTask != null)
                iterationTask.ContinueWith(continuationAction, CancellationToken.None,
                                           TaskContinuationOptions.ExecuteSynchronously,
                                           factory.Scheduler ?? TaskScheduler.Default);
        }


 
    }


    public static class Empty<T>
    {
        public static Task<T> Task
        {
            get { return _task; }
        }

        private static readonly Task<T> _task = System.Threading.Tasks.Task.FromResult(default(T));
    }
}