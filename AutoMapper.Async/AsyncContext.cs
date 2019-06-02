using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AutoMapper
{
    internal class AsyncContext
    {
        private readonly List<Task> _tasks;
        private readonly int _threadId;

        public AsyncContext(CancellationToken token)
        {
            _threadId = Thread.CurrentThread.ManagedThreadId;
            _tasks = new List<Task>();

            Token = token;
        }

        public CancellationToken Token { get; }

        public void Add(Task task)
        {
            VerifyThread();
            _tasks.Add(task);
        }

        public Task WhenAllAsync() => Task.WhenAll(_tasks);

        private void VerifyThread()
        {
            if (_threadId != Thread.CurrentThread.ManagedThreadId)
            {
                throw new InvalidOperationException("Must be run on the same thread");
            }
        }
    }
}
