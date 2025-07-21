using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Test
{
    public class ThreadTest : MonoBehaviour
    {
        // private int _counter = 0;
        // private object _lock = new object();
        
        // 유니티에서 SetParent를 빈번히 하면 성능이 죽는다. 스레드 때문.
        private void Start()
        {
            WorkSequence();
            
            // Thread T = new Thread(WorkJob); // 괄호 안에 해야할 작업(Task)을 넣어야 해
            // T.Start();
            
            // Task.Run(WorkJob);
            // Task.Run(WorkJobDec);
            
            // T.IsBackground = true; // C#에서는 해야한다.
            // Debug.Log("Thread started, main thread continues...");
            // Debug.Log(Thread.CurrentThread.ManagedThreadId);
        }

        private async void WorkSequence()
        {
            await  WorkJob(1000); // yield return
            // Debug.Log($"Thread ID : {Thread.CurrentThread.ManagedThreadId}"); // 메인 나온다
            await WorkJob(2000);
            await WorkJob(3000);
        }
        
        private async Task WorkJob(int ms)
        {
            Debug.Log($"Thread ID : {Thread.CurrentThread.ManagedThreadId}, working for {ms} ms");
            await Task.Delay(ms);
            Debug.Log($"Thread ID : {Thread.CurrentThread.ManagedThreadId}, finished working");
        }

        // private void WorkJob()
        // {
        //     ulong i = 0;
        //     while (i < 300000L)
        //     {
        //         // Debug.Log($"Hello Thread! {i}, Thread ID: {Thread.CurrentThread.ManagedThreadId}");
        //         // Thread.Sleep(1000); // 1초마다 출력
        //         i++;
        //         lock (_lock)
        //         {
        //             // 깃발꽂기
        //             _counter++; // 어셈블리로는 3쥴
        //             // LDA ac;
        //             // INC
        //             // STA ac;
        //         }
        //     }
        //     Debug.Log("Incremented thread completed.");
        // }
        //
        // private void WorkJobDec()
        // {
        //     ulong i = 0;
        //     while (i < 300000L)
        //     {
        //         // Debug.Log($"Hello Thread! {i}, Thread ID: {Thread.CurrentThread.ManagedThreadId}");
        //         // Thread.Sleep(1000); // 1초마다 출력
        //         i++;
        //         lock (_lock)
        //         {
        //             _counter--; // 어셈블리로는 3쥴
        //         }
        //     }
        //     Debug.Log("Decrement thread completed.");
        // }
        
        private void Update()
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                Debug.Log($"Thread ID : {Thread.CurrentThread.ManagedThreadId}, A key was pressed, starting thread...");
                // Debug.Log(_counter);
            }
        }
    }
}