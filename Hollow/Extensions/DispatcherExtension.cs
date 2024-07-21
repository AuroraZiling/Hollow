using System;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace Hollow.Extensions;

public static class DispatcherExtension
{
    /// <summary>
    /// 在指定的DispatcherFrame上运行一个Task，等待其完成，避免阻塞UI线程
    /// </summary>
    /// <param name="task"></param>
    /// <param name="dispatcher"></param>
    /// <returns></returns>
    /// <exception cref="AggregateException"></exception>
    public static void WaitOnDispatcherFrame(this Task task, Dispatcher? dispatcher = null)
    {
        var frame = new DispatcherFrame();
        AggregateException? capturedException = null;

        task.ContinueWith(t =>
            {
                capturedException = t.Exception;
                frame.Continue = false; // 结束消息循环
            },
            TaskContinuationOptions.AttachedToParent);

        dispatcher ??= Dispatcher.UIThread;
        dispatcher.PushFrame(frame);

        if (capturedException != null)
        {
            throw capturedException;
        }
    }
}