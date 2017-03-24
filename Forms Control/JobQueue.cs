using System;
using System.Collections.Generic;
using System.Threading;

namespace Forms_Control
{
    /* A class that is capable of executing a queue of tasks */
    public class JobQueue
    {
        Queue<Thread> jobs;
        bool running;

        /* Constructor */
        public JobQueue() { jobs = new Queue<Thread>(); }

        /* Finishes the current job and then stops running */
        public void stop() { running = false; }

        /* Starts running all of the jobs in the job queue */
        public void start()
        {
            running = true;
            if (jobs.Count > 0)
            {
                if (jobs.Peek().ThreadState == ThreadState.Stopped)
                {
                    jobs.Peek().Start();
                }
            }
        }

        /* Adds a job to the queue */
        public void add(Action job)
        {
            // Add the job to the queue
            bool hasJobs = jobs.Count > 0;
            ThreadStart tasks = new ThreadStart(job);
            tasks += threadFinished;
            jobs.Enqueue(new Thread(tasks));

            // If the queue is running jobs and was empty before then run that job
            if (!hasJobs && running)
                jobs.Peek().Start();
        }

        /* Removes the job from the queue and starts the next one when the current job is finished */
        private void threadFinished()
        {
            jobs.Dequeue();
            if (running && jobs.Count > 0)
                jobs.Peek().Start();
        }
    }
}
