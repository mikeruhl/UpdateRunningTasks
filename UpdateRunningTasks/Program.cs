using System;
using System.Threading;
using System.Threading.Tasks;

namespace UpdateRunningTasks
{
    class Program
    {
        private static ConsoleColor _defaultColor;

        static async Task Main(string[] args)
        {
            _defaultColor = Console.ForegroundColor;
            var source = new CancellationTokenSource();
            var token = source.Token;
            var runner = new TaskRunner(LogMessage, token);
            runner.Start("This is a Task being repeated every 1 second", TimeSpan.FromSeconds(1));
            await Task.Delay(TimeSpan.FromSeconds(10), token);

            LogMessage("Now changing value", ConsoleColor.Cyan);

            runner.Update("This is a new task, now repeated every 2 seconds", TimeSpan.FromSeconds(2));

            await Task.Delay(TimeSpan.FromSeconds(10), token);
            
            runner.Stop();
            LogMessage("Task has been stopped", ConsoleColor.Cyan);
            await Task.Delay(TimeSpan.FromSeconds(5), token);
            LogMessage("And we waited 5 seconds, now we'll cancel the entire runner", ConsoleColor.Cyan);

            var newRunner = new TaskRunner(LogMessage, token);
            newRunner.Start("This runner will be stopped in 10 seconds", TimeSpan.FromSeconds(1));
            await Task.Delay(TimeSpan.FromSeconds(10), token);
            source.Cancel();
            Console.WriteLine("You should no longer get output");
            await Task.Delay(TimeSpan.FromSeconds(2), CancellationToken.None);
            Console.WriteLine("We waited 2 seconds and got no output.");

        }

        static void LogMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = _defaultColor;
            Console.Write($"{DateTime.Now.ToLongTimeString()}: ");
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = _defaultColor;
        }
    }
}
