using System.Threading;

namespace SemaphoreHomeWork;

public class ThreadData
{
    public Semaphore Semaphore { get; set; }
    public string Name { get; set; }

    public ThreadData(Semaphore semaphore, string name)
    {
        Semaphore = semaphore;
        Name = name;
    }
}