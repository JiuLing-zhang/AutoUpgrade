﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace JiuLing.AutoUpgrade.Net
{
    /// <summary>
    /// 网络请求工具
    /// </summary>
    internal class HttpClientHelper
    {
        private static HttpClient _httpClient;
        public static HttpClient HttpClient => _httpClient;
        /// <summary>
        /// 初始化
        /// </summary>
        public HttpClientHelper()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// 发送一个GET请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>返回服务器请求得到的字符串</returns>
        public async Task<string> GetReadString(string url, TimeSpan timeout)
        {
            _httpClient.Timeout = timeout;
            return await _httpClient.GetStringAsync(url);
        }

        /// <summary>
        /// 以byte数组的形式下载文件
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="progress">下载的进度（范围0-1）</param>
        /// <param name="bufferSize">下载时缓冲区的字节大小</param>
        /// <returns>返回服务器请求得到的字节数组</returns>
        public async Task<byte[]> GetFileByteArray(string url, IProgress<float> progress, int bufferSize = 8192)
        {
            using (var responseMessage = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                progress?.Report(0);
                var content = responseMessage.Content;
                long contentLength = content.Headers.ContentLength ?? throw new Exception(AutoUpgrade.Properties.Resources.UnknownFileSize);
                using (var responseStream = await content.ReadAsStreamAsync())
                {
                    var buffer = new byte[bufferSize];
                    int length;
                    long downloadLength = 0;
                    var bytes = new byte[contentLength];
                    while ((length = await responseStream.ReadAsync(buffer, 0, bufferSize)) > 0)
                    {
                        Array.Copy(buffer, 0, bytes, downloadLength, length);
                        downloadLength += length;
                        progress?.Report((float)downloadLength / contentLength);
                    }
                    progress?.Report(1);
                    return bytes.ToArray();
                }
            }
        }
    }
}
