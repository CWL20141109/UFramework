/*
 * @Author: fasthro
 * @Date: 2020-09-23 11:01:36
 * @Description: downloadhandler
 */
using System;
using System.IO;
using UFramework.Messenger;
using UFramework.Pool;
using UnityEngine.Networking;

namespace UFramework.Tools
{
    public class DownloadHandler : DownloadHandlerScript, IPoolObject
    {
        /// <summary>
        /// 下载链接
        /// </summary>
        /// <value></value>
        public string url { get; protected set; }

        /// <summary>
        /// 本地保存路径
        /// </summary>
        /// <value></value>
        public string localPath { get; protected set; }

        /// <summary>
        /// 下载状态
        /// </summary>
        /// <value></value>
        public DownloadState downloadState { get; protected set; }

        /// <summary>
        /// 文件总长度
        /// </summary>
        /// <value></value>
        public long totalLength { get; protected set; }

        /// <summary>
        /// 已下载长度
        /// </summary>
        /// <value></value>
        public long receiveLength { get; protected set; }

        /// <summary>
        /// 下载进度
        /// </summary>
        /// <value></value>
        public float progress
        {
            get
            {
                if (totalLength > 0)
                    return (float)receiveLength / (float)totalLength;
                else if (downloadState == DownloadState.Donwloaded) return 1f;
                else return 0f;
            }
        }

        /// <summary>
        /// 文件总长度
        /// 如果有值就需要进行文件长度验证
        /// </summary>
        protected long _verifyTotalLength;

        /// <summary>
        /// 文件哈希
        /// 如果有值就需要进行文件哈希验证
        /// </summary>
        protected string _verifyHash;

        protected string _tempLocalPath;
        protected FileStream _stream;
        protected long _contentLength;

        protected UnityWebRequest _request;


        protected UCallback _onCompleted;
        protected UCallback<float> _onProgress;
        protected UCallback _onCancelled;
        protected UCallback<string> _onFailed;

        protected bool _isContinue;

        #region pool

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">下载链接</param>
        /// <param name="local">本地保存路径</param>
        /// <param name="onCompleted">完成事件</param>
        /// <param name="onProgress">下载进度事件</param>
        /// <param name="onCancelled">取消下载事件</param>
        /// <param name="onFailed">下载失败事件</param>
        /// <param name="length">文件长度(大于0时下载文件需要进行长度验证)</param>
        /// <param name="hash">文件hash(有值下载文件时需要进行hash验证)</param>
        /// <param name="hash">是否断点续传</param>
        /// <returns></returns>
        public static DownloadHandler Allocate(string url, string local,
         UCallback onCompleted = null, UCallback<float> onProgress = null, UCallback onCancelled = null, UCallback<string> onFailed = null,
          long length = 0, string hash = null, bool isContinue = true)
        {
            return ObjectPool<DownloadHandler>.Instance.Allocate().Builder(url, local, onCompleted, onProgress, onCancelled, onFailed, length, hash, isContinue);
        }

        public bool isRecycled { get; set; }

        public void Recycle()
        {
            ObjectPool<DownloadHandler>.Instance.Recycle(this);
        }

        public void OnRecycle()
        {
            downloadState = DownloadState.Init;
            _onCompleted = null;
            _onProgress = null;
            _onCancelled = null;
            _onFailed = null;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">下载链接</param>
        /// <param name="local">本地保存路径</param>
        /// <param name="onCompleted">完成事件</param>
        /// <param name="onProgress">下载进度事件</param>
        /// <param name="onCancelled">取消下载事件</param>
        /// <param name="onFailed">下载失败事件</param>
        /// <param name="length">文件长度(大于0时下载文件需要进行长度验证)</param>
        /// <param name="hash">文件hash(有值下载文件时需要进行hash验证)</param>
        /// <param name="hash">是否断点续传</param>
        /// <returns></returns>
        private DownloadHandler Builder(string url, string local,
         UCallback onCompleted = null, UCallback<float> onProgress = null, UCallback onCancelled = null, UCallback<string> onFailed = null,
          long length = 0, string hash = null, bool isContinue = true)
        {
            this.url = url;
            this.localPath = local;
            this._onCompleted = onCompleted;
            this._onProgress = onProgress;
            this._onCancelled = onCancelled;
            this._onFailed = onFailed;
            this._verifyTotalLength = length;
            this._verifyHash = hash;
            this._isContinue = isContinue;

            this.downloadState = DownloadState.Init;

            this._tempLocalPath = local + ".crdownload";
            if (!isContinue)
                IOPath.FileDelete(_tempLocalPath);
            this._stream = new FileStream(_tempLocalPath, FileMode.Append, FileAccess.Write);
            this.receiveLength = _stream.Length;
            this.totalLength = this.receiveLength;

            return this;
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        /// <returns></returns>
        public DownloadHandler Download()
        {
            if (downloadState == DownloadState.Init)
            {
                downloadState = DownloadState.Donwloading;

                _request = UnityWebRequest.Get(url);
                _request.disposeDownloadHandlerOnDispose = true;
                _request.SetRequestHeader("Range", string.Format("bytes={0}-", receiveLength));
                _request.downloadHandler = this;
                _request.SendWebRequest();
            }
            return this;
        }

        /// <summary>
        /// 取消下载
        /// </summary>
        public void Cancel()
        {
            if (downloadState == DownloadState.Donwloading)
            {
                Release();
            }
            IOPath.FileDelete(_tempLocalPath);
            _onCancelled.InvokeGracefully();
        }

        /// <summary>
        /// 下载文件头信息
        /// </summary>
        /// <param name="contentLength">下载内容总长度</param>
        protected override void ReceiveContentLengthHeader(ulong contentLength)
        {
            _contentLength = (long)contentLength;
            totalLength = receiveLength + _contentLength;
        }

        /// <summary>
        /// 接受数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="dataLength">数据长度</param>
        /// <returns></returns>
        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || data.Length == 0)
                return false;

            receiveLength += dataLength;
            _contentLength -= dataLength;
            _stream.Write(data, 0, dataLength);

            _onProgress.InvokeGracefully(progress);

            return true;
        }

        /// <summary>
        /// 数据接收完成
        /// </summary>
        protected override void CompleteContent()
        {
            Release();

            string error = string.Empty;
            if (IOPath.FileExists(_tempLocalPath))
            {
                if (IOPath.FileExists(localPath))
                    IOPath.FileDelete(localPath);
                File.Move(_tempLocalPath, localPath);

                using (var fs = File.OpenRead(localPath))
                {
                    if (fs.Length != _verifyTotalLength)
                    {
                        error = "下载文件长度异常:" + fs.Length;
                    }
                    const StringComparison compare = StringComparison.OrdinalIgnoreCase;
                    if (!_verifyHash.Equals(UHash.GetCRC32Hash(fs), compare))
                    {
                        error = "下载文件哈希异常:" + _verifyHash;
                    }
                }
            }
            else
            {
                error = "文件不存在";
            }

            if (string.IsNullOrEmpty(error))
                _onCompleted.InvokeGracefully();
            else ProError(error);
        }

        /// <summary>
        /// Update
        /// </summary>
        public void OnUpdate()
        {
            if (downloadState != DownloadState.Donwloading)
                return;

            if (_request.isNetworkError)
            {
                ProError(_request.error);
                return;
            }

            if (_request.isDone)
            {
                downloadState = DownloadState.Donwloaded;
                _onCompleted.InvokeGracefully();
            }
        }

        protected void ProError(string error)
        {
            Release();
            IOPath.FileDelete(_tempLocalPath);
            _onFailed.InvokeGracefully(error);
        }

        public void Release()
        {
            downloadState = DownloadState.Donwloaded;

            if (_stream != null)
            {
                _stream.Close();
                _stream.Dispose();
            }

            if (_request != null)
            {
                _request.Abort();
                _request.Dispose();

            }
            _request = null;
            Dispose();
        }
    }
}