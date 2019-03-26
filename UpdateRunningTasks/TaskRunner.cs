using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UpdateRunningTasks
{
    public class TaskRunner
    {
        private readonly Action<string, ConsoleColor> _outputAction;
        private CancellationToken _currentToken;
        private CancellationTokenSource _cts;
        private readonly CancellationToken _originalToken;
        private Task _currentTask;
        private bool _started;
        public TaskRunner(Action<string, ConsoleColor> outputAction, CancellationToken cancellationToken)
        {
            _outputAction = outputAction;
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _originalToken = cancellationToken;
        }
        public void Start(string initialValue, TimeSpan echoInterval)
        {
            if(string.IsNullOrEmpty(initialValue))
                throw new ArgumentNullException(nameof(initialValue));
            if(echoInterval == null)
                throw new ArgumentNullException(nameof(echoInterval));
            if(_started)
                throw new InvalidOperationException("TaskRunner already started");
           CreateNewTask(initialValue, echoInterval);
           _started = true;
        }

        public void Stop()
        {
            if(!_started)
                throw new InvalidOperationException("TaskRunner has not been started");
            _cts.Cancel();
        }

        private void CreateNewTask(string newValue, TimeSpan echoInterval)
        {
            if(_currentTask != null && !_currentTask.IsCanceled)
                _cts.Cancel();
            _currentToken = new CancellationToken();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(_originalToken, _currentToken);
            _currentTask = Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (!_cts.IsCancellationRequested)
                    {
                        _outputAction(newValue, ConsoleColor.White);
                        await Task.Delay(echoInterval, _cts.Token);
                    }
                }
                catch (TaskCanceledException)
                {
                    _outputAction($"Ending task for {newValue}", ConsoleColor.Green);
                }

                
            }, _cts.Token);
        }

        public void Update(string newEchoValue, TimeSpan echoInterval)
        {
            if(string.IsNullOrEmpty(newEchoValue))
                throw new ArgumentNullException(nameof(newEchoValue));
            if(echoInterval == null)
                throw new ArgumentNullException(nameof(echoInterval));
            CreateNewTask(newEchoValue, echoInterval);
        }
    }
}
