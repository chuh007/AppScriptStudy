using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Network.Editor
{
    public class WebRequestAwaiter : INotifyCompletion
    {
        private UnityWebRequestAsyncOperation _asyncOperation;
        private Action _continuation;
        
        public bool IsCompleted => _asyncOperation.isDone;
        
        public WebRequestAwaiter(UnityWebRequestAsyncOperation asyncOperation)
        {
            _asyncOperation = asyncOperation;
            _continuation = null;
        }
        
        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
            _asyncOperation.completed += OnRequestCompleted;
        }

        private void OnRequestCompleted(AsyncOperation _)
        {
            _continuation?.Invoke();
        }
    }

    public static class WebRequestExtensions
    {
        // public static WebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOperation)
        // {
        //     return new WebRequestAwaiter(asyncOperation);
        // }
    }
}