/*
 * @Author: fasthro
 * @Date: 2020-09-18 14:44:45
 * @Description: web bundle
 */
using System.Collections;
using System.Collections.Generic;
using UFramework.Pool;
using UnityEngine;
using UnityEngine.Networking;

namespace UFramework.Assets
{
    public class WebBundleRequest : AssetRequest
    {
        public AssetBundle assetBundle { get { return asset as AssetBundle; } }
        private List<WebBundleRequest> dependencies = new List<WebBundleRequest>();
        private UnityWebRequest request;

        public override bool isAsset { get { return false; } }

        public static WebBundleRequest Allocate()
        {
            return ObjectPool<WebBundleRequest>.Instance.Allocate();
        }

        public override void Recycle()
        {
            ObjectPool<WebBundleRequest>.Instance.Recycle(this);
        }

        public override IEnumerator OnCoroutineTaskRun()
        {
            loadState = LoadState.LoadBundle;
            request = UnityWebRequestAssetBundle.GetAssetBundle(url);
            yield return request.SendWebRequest();
            if (loadState == LoadState.LoadBundle && request.isDone)
            {
                asset = DownloadHandlerAssetBundle.GetContent(request);
                loadState = LoadState.Loaded;
                OnAsyncCallback();
            }
        }

        public override void Load()
        {
            base.Load();
            if (loadState != LoadState.Init) return;

            var bundles = Asset.Instance.GetDependencies(url);
            for (int i = 0; i < bundles.Length; i++)
            {
                var bundle = Asset.Instance.GetBundle<WebBundleRequest>(bundles[i], true);
                bundle.AddCallback(OnBundleDone);
                bundle.Load();
                dependencies.Add(bundle);
            }
        }

        private void OnBundleDone(AssetRequest request)
        {
            request.RemoveCallback(OnBundleDone);
            StartCoroutine();
        }

        public override void Unload()
        {
            base.Unload();
            for (int i = 0; i < dependencies.Count; i++)
                dependencies[i].Unload();
        }

        protected override void OnReferenceEmpty()
        {
            dependencies.Clear();
            if (request != null)
            {
                request.Dispose();
                request = null;
            }
            if (assetBundle != null)
            {
                assetBundle.Unload(true);
                asset = null;
            }
            loadState = LoadState.Unload;
            base.OnReferenceEmpty();
        }
    }
}