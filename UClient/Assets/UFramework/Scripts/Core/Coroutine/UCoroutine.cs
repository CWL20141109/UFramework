/*
 * @Author: fasthro
 * @Date: 2019-10-24 17:08:12
 * @Description: 协同包装 (支持停止，暂停，取消暂停)，扩展了Unity自带的协同程序
 * 注释:停止之后不能再做其他操作
 */
using System.Collections;
using UFramework.Messenger;
using UnityEngine;

namespace UFramework.Coroutine
{
    public class UCoroutine
    {
        public bool running { get; private set; }
        public bool paused { get; private set; }
        public bool stopped { get; private set; }

        private UCallback<bool> m_completeCallback;
        private IEnumerator m_routine;

        static UCoroutineHelper helper;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="autoStart"></param>
        /// <param name="completeCallback"></param>
        public UCoroutine(IEnumerator routine, bool autoStart, UCallback<bool> completeCallback = null)
        {
            this.m_routine = routine;
            this.m_completeCallback = completeCallback;
            if (autoStart)
            {
                Start();
            }
        }

        public void Start()
        {
            running = true;
            if (helper == null)
            {
                helper = (new GameObject("UCoroutineHelper")).AddComponent<UCoroutineHelper>();
            }
            helper.StartCoroutine(CallWrapper());
        }

        public void Pause() { paused = true; }

        public void Unpause() { paused = false; }

        public void Stop()
        {
            stopped = true;
            running = false;
        }

        IEnumerator CallWrapper()
        {
            yield return null;
            IEnumerator e = this.m_routine;
            while (running)
            {
                if (paused)
                    yield return null;
                else
                {
                    if (e != null && e.MoveNext()) yield return e.Current;
                    else running = false;
                }
            }
            m_completeCallback.InvokeGracefully(stopped);
        }
    }
}