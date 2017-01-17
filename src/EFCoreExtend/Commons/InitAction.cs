using System;
using System.Threading;
using System.Threading.Tasks;

namespace EFCoreExtend.Commons
{
    /// <summary>
    /// 初始化Action
    /// </summary>
    public class InitAction
    {
        int _ival = 0;
        volatile bool _isDone = false;
        public bool IsDone
        {
            get { return _isDone; }
        }
        public event Action Action;

        public InitAction(Action action)
        {
            Action = action;
        }

        public void Release()
        {
            _isDone = false;
            Interlocked.CompareExchange(ref _ival, 0, _ival);
        }

        bool CanDo()
        {
            if (_ival <= 0)
            {
                if (Interlocked.Increment(ref _ival) <= 1)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 初始化事件的调用
        /// </summary>
        /// <param name="waitMillisecondsTimeout">指定等待事件完成的时间，默认为0不等待，-1为等待完成为止</param>
        public bool Invoke(int waitMillisecondsTimeout = 0)
        {
            if (CanDo())
            {
                Action();
                _isDone = true;
                return true;
            }
            else
            {
                return SpinWait.SpinUntil(() => _isDone, waitMillisecondsTimeout);
            }
        }

    }
}
