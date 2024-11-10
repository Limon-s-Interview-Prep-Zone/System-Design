# OS
Operating system is a software that manages a computer's hardwares and software resources, providing an environmnent for application to run.

1. **Karnel:** The core component that manages `CPU, memory and decice operation`. Ex: a `manager` of a theater who handles lighting, sound, and stage management.

2. **Process:** A process is an isolated and independent execution unit with its `own memory space, resources and ststems states`.
   1. Ex: A web browser, A word processor.

3. **Thread:** A thread is a smaller unit within in a `process`, which can have `multiples threads`, all sharing `same memory space` but performing different `task simultaneously`.
   1. In a browser, each tab might be a thread.
4. **Multitasking:** Multitasking is a system running multiple processes or programs at same time.
   1. p1: writing document
   2. p2: running a video player
   3. p3: browsing social media
5. **Virtual Memory:** Extends physical memory onto the hard drive to handle more data.
   1. If a machine has `4gb ram` but runs a program require `6GB`, virtual memory allows the program to continue running by using hard drive space.
6. **Cache memory:** A small, fast type of memory that stores frequently accessed data.

---

## Thread Life Cycle
1. **Creation:** The thread is created but not yet started. `thread()`
2. **Runable:** The thread is in the Runnable state once the `start()` method is called and is waiting for CPU time.
3. **Running:** The machine is now performing its assigned task.
   1. `sleep()`
   2. `wait()`
   3. `join()`: wait untill all threads has been completed.
4. **Blocking/waiting:** The thread is temporarily inactive because it is waiting for some condition to be met `(such as waiting for input, waiting for a lock, or waiting for a response from another system)`. During this time, the thread is not `consuming CPU resources`.
   1. **Timed Waiting:** The machine is paused for a specific time.
5. **Termination/Dead:** The machine has finished its job and is turned off permanently.

## Multi-Threading thread-off:
### Advantages
1. **Concurrency**: Multithreading allows multiple tasks (or threads) to run concurrently within a `single process`.
2. **Parallelism**: On multicore systems, multithreading enables `parallel execution`, where multiple threads run simultaneously on **different CPU cores**.
3. **Responsiveness and Low Latency:** Multithreading improves application responsiveness by allowing time-consuming tasks to run in the background, without freezing the user interface (UI). This reduces latency, as threads can process user inputs or system events immediately.
4. **Efficient Resource Utilization:** Multithreading optimizes the use of system resources, especially when a task involves waiting for external input/output (I/O) operations like file reading or network access.
5.  **Asynchronous Processing:** Multithreading enables `asynchronous task execution`, where certain tasks can run in the background without blocking the `main process`.
6.  **Thread Pool:** Thread pooling is an important feature of multithreading that allows efficient `reuse of threads`. Instead of creating and destroying threads repeatedly (which is costly in terms of performance), a pool of pre-created threads is maintained, and tasks are assigned to them as needed.
7.  **Synchronization:** Multithreading provides mechanisms to control access to `shared resources` through synchronization. This prevents `race conditions`, where `multiple threads` try to `modify` the same data simultaneously, ensuring consistency and correctness in the program.
---
### Disadvantages:
1. ***Race condition:*** When multiple threads try to access and modify shared data simultaneously, leading to inconsistent results.
   * Thread A: Reads the account balance (let’s say $100) and plans to add $50.
   * Thread B: Simultaneously reads the balance (still $100) and plans to subtract $30.<br>
  
    `If both threads read the balance before either one writes back the updated amount, both think the account has $100.` So:

   * Thread A adds $50 and writes $150.
   * Thread B subtracts $30 and writes $70
2. ***Deadlock:*** A situation where two or more threads wait for each other to `release resources`, causing an infinite block.
   1. Imagine two threads, `Thread A and Thread B`, need two resources, `Resource 1 and Resource 2`, to complete their tasks.
      - Thread A locks `Resource 1` and waits for `Resource 2` to become available.
      - Thread B locks `Resource 2` and waits for `Resource 1` to become available.
3. **Livelock:** A livelock occurs when threads remain active, but they can’t `make progress `because they `continuously respond to each other`. Unlike a deadlock, where threads are stuck waiting, in a livelock, the threads are still running, but they are too busy adjusting their actions to a constantly changing state.
   1. Two threads are attempting to avoid a deadlock by releasing locks when they detect that another thread holds the needed resource.
      - Thread `A holds Lock 1` and tries to acquire `Lock 2`, but notices that Thread B is holding it, so Thread A releases `Lock 1 and waits`.
      - Thread B holds Lock 2 and tries to acquire Lock 1, but sees that `Thread A is holding it`, so Thread B releases `Lock 2`.
4. ***Starvation:*** Starvation occurs when a thread is perpetually denied access to resources because other threads are constantly prioritized over it.
   - `High-Priority Thread:` Continuously gets CPU time for execution.
   - `Low-Priority Thread:` Keeps waiting indefinitely because the high-priority thread is constantly running.
5. ***Priority Inversion:*** Priority inversion occurs when a `low-priority thread` holds a resource that a `higher-priority thread needs`. While the higher-priority thread waits for the resource, medium-priority threads may continue to run, preventing the low-priority thread from completing its task and releasing the resource.
   - Low-Priority Thread locks a resource (e.g., a mutex).
   - High-Priority Thread needs the same resource and is blocked, waiting for the low-priority thread to release it.
   - Medium-Priority Thread doesn't need the resource but keeps getting CPU time, preventing the low-priority thread from finishing and releasing the resource.
6. ***Context Switching Overhead:*** Each time the operating system switches between threads, it incurs context switching overhead. This involves saving the current thread’s state (registers, program counter, etc.) and loading the next thread’s state. While necessary, frequent context switching can reduce performance.
7. ***Thread Pool Exhaustion*** In systems that use a thread pool, if all threads in the pool are busy, new tasks may have to `wait indefinitely`, leading to thread pool exhaustion. This can cause the system to become unresponsive or degrade performance if the pool size is not properly managed.
   - New requests are queued, and users experience delays or timeouts.
   - If tasks are slow to complete, it can lead to thread pool exhaustion, where no threads are available to handle new tasks.
---

## Why do we need Synchronization?
In multi-threated programming, multiple threads often share shared resources such as `variable, objects and collections`. If two threads try to `read/write` the same resouces simultaneously, it leads to `race condition`, resulting in `Data coruption`.

Consider two threads `Thread1` and `Thread2`, both try to increment a shared counter without synchronization with initial value of `counter=5`.
```bash
counter++
```
1. `Thread1` reads the value `counter=5`
2. `Thread2` also reads the value `counter=5`
3. `Thread1` increments the value `counter=6`
4. `Thread2` also increments the value `counter=6`

With synchonization:
1. `Thread1` reads the value `counter=5` and `lock` this.
2. `Thread2` tries to read the value but has to wait.
3. `Thread1` increments the value `counter=6`
4. `Thread2` get the current value of `counter=6` then increments to `7`

---
### Synchronization Technique for c#
1. `lock:` Lock a section of code, which ensures that only one thread can enter the critical section at a time.

2. `Monitor:` Provides fine-grained control for lacking, with explicit lock management using `Monitor.Enter` and `Monitor.Exit`

3. `Mutex:` A system-wide synchronization technique or A Mutex is a synchronization primitive that can be used to manage access to a resource `across multiple processes`, not just multiple threads. `mutex.WaitOne()` and ` mutex.ReleaseMutex()`.
    ```bash
        private static readonly object _monitorLock = new object();

        public void ExampleMethod()
        {
            Monitor.Enter(_monitorLock);
            try
            {
                // Critical section of code
            }
            finally
            {
                Monitor.Exit(_monitorLock);
            }
        }

    ```

4. `Semaphore and SemaphoreSlim:` A Semaphore `restricts the number of threads` that can access a particular resource or code block at the same time. `SemaphoreSlim` is a lightweight version of Semaphore used for synchronization within the same process.
   ```bash
   SemaphoreSlim semaphoreSlim = new SemaphoreSlim(3);
   await semaphoreSlim.WaitAsync();
    try
    {
        // Critical section of code
    }
    finally
    {
        semaphoreSlim.Release();
    }
   ```

5. `AutoResetEvent and ManualResetEvent:` AutoResetEvent used for signaling beteen threads. Blocks a thread untill the `Set` method is called from another thread. After a single thread is released, it automatically resets to the non=signal state.

6. *`Thread Safe Collections:`* This collections are designed for multi-thread scenarios and eliminate the need for manual synchronization.
   1. *`ConcurrentDictionary<TKey, TValue>`*: A thread-safe version of `Dictionary<TKey, TValue>`.
   2. *`ConcurrentQueue<T>`*: A thread-safe queue that allows multiple threads to `enqueue` and `dequeue` items.
   3. *`ConcurrentStack<T>`*: A thread-safe version of `Stack<T>`.
   4. *`ConcurrentBag<T>`*: A thread-safe, unordered collection for fast insertations and removals.