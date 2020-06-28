/*
 * @Author: fasthro
 * @Date: 2019-06-22 16:19:54
 * @Description: AssetBundle Asset
 */
using System;
using System.Collections;
using UFramework.Pool;
using UnityEngine;

namespace UFramework.ResLoader
{
    public class BundleAssetRes : Res, IPoolObject, IRunAsyncObject
    {
        private AssetBundleRequest m_request;
        private BundleRes m_bundleRes;

        /// <summary>
        /// 分配 AssetRes 对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BundleAssetRes Allocate(string resName)
        {
            var res = ObjectPool<BundleAssetRes>.Instance.Allocate();
            res.Initialize(resName);
            return res;
        }

        #region IPoolObject
        public bool isRecycled { get; set; }

        public void Recycle()
        {
            ObjectPool<BundleAssetRes>.Instance.Recycle(this);
        }

        public void OnRecycle()
        {
            ResPool.Recycle(resName);
            ResPool.Recycle(bundleName);
        }
        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        private void Initialize(string resName)
        {
            this.resName = resName;
            resStatus = ResStatus.Waiting;
            resType = ResType.AssetBundleAsset;
            assetObject = null;
            assetBundle = null;
        }

        /// <summary>
        /// 搜索bundle
        /// </summary>
        private bool SearchAssetBundle()
        {
            if (assetBundle == null)
            {
                m_bundleRes = ResPool.Allocate<BundleRes>(bundleName, ResType.AssetBundle);
                if (m_bundleRes != null)
                {
                    assetBundle = m_bundleRes.assetBundle;
                }
            }
            return assetBundle != null;
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        public override bool LoadSync()
        {
            if (resStatus == ResStatus.Ready)
                return true;

            if (!SearchAssetBundle())
            {
                resStatus = ResStatus.Failed;
                return false;
            }
            resStatus = ResStatus.Loading;
            assetObject = assetBundle.LoadAsset(assetName);
            if (assetObject == null)
            {
                resStatus = ResStatus.Failed;
                return false;
            }
            resStatus = ResStatus.Ready;
            return true;
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        public override void LoadAsync()
        {
            if (resStatus == ResStatus.Ready)
            {
                BroadcastEvent(true);
            }
            else if (resStatus == ResStatus.Waiting || resStatus == ResStatus.Failed)
            {
                if (!SearchAssetBundle())
                {
                    resStatus = ResStatus.Failed;
                    BroadcastEvent(false);
                }
                else
                {
                    resStatus = ResStatus.Loading;
                    RunAsync.Instance.Push(this);
                }
            }
        }

        /// <summary>
        /// 执行异步加载
        /// </summary>
        /// <param name="async"></param>
        public IEnumerator AsyncRun(IRunAsync async)
        {
            var request = assetBundle.LoadAssetAsync(assetName);

            m_request = request;
            yield return request;
            m_request = null;

            if (!request.isDone)
            {
                resStatus = ResStatus.Failed;
                async.OnRunAsync();
                BroadcastEvent(false);
                yield break;
            }

            assetObject = request.asset;

            if (assetObject == null)
            {
                resStatus = ResStatus.Failed;
                async.OnRunAsync();
                BroadcastEvent(false);
                yield break;
            }

            resStatus = ResStatus.Ready;
            async.OnRunAsync();
            BroadcastEvent(true);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        public override void Unload(bool unloadAllLoadedObjects = false)
        {
            Release();
        }

        /// <summary>
        /// 引用次数为0处理
        /// </summary>
        protected override void OnEmptyRef()
        {
            if (assetObject != null)
            {
                if (assetObject is GameObject) { }
                else Resources.UnloadAsset(assetObject);
            }

            if (m_bundleRes != null)
            {
                m_bundleRes.Unload();
                m_bundleRes = null;
            }

            assetBundle = null;
            assetObject = null;

            Recycle();
        }
    }
}