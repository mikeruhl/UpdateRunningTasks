# UpdateRunningTasks
Possible Solution to https://www.reddit.com/r/dotnetcore/comments/b5n4xo/background_tasks_with_hosted_services/

## Overview
This console demo shows maintaining a task's state from within it's own class and manipulating it externally when necessary while allowing it to continue to run in the mean time.

## The flow:
The app first creates a `TaskRunner`.  This will create a repeating message to the console every 1 second and can run indefinitely.

After 10 seconds, we change the message and the interval that it echos.  Under the hood, this ends and creates a new task, but that functionality is hidden from the caller, instead they have a simple interface to use.

After another 10 seconds, we stop that runner to show the internal task is canceled and no longer outputs.

Now we wait 5 seconds to show the console is no longer outputting (internal task is canceled).

Next up we create a new task runner to show we can cancel the entire taskrunner, like in the event of the application starting a shutdown procedure.

After 10 seconds, we cancel the `newRunner` and show it no longer outputs by waiting 2 seconds.
