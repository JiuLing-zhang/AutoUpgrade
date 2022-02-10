﻿using System.IO.Compression;
using System.Text;

namespace JiuLing.AutoUpgrade.Templates
{
    internal abstract class UpgradeAbstract
    {
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="downloadUrl">更新包的下载地址</param>
        /// <param name="appPath">程序运行路径</param>
        /// <param name="updatePackPath">更新包的路径</param>
        /// <param name="tempZipDirectory">更新文件的临时解压路径</param>
        public async Task Update(string downloadUrl, string appPath, string updatePackPath, string tempZipDirectory, IProgress<float> progress = null!)
        {
            await DownloadApp(downloadUrl, updatePackPath, progress);
            PublishZipFile(updatePackPath, tempZipDirectory);
            CopyFiles(tempZipDirectory, appPath);
            ClearFileCache(updatePackPath, tempZipDirectory);
        }

        public abstract Task DownloadApp(string downloadUrl, string updatePackPath, IProgress<float> progress);

        private static void PublishZipFile(string filePath, string dstPath)
        {
            ZipFile.ExtractToDirectory(filePath, dstPath, Encoding.GetEncoding("GBK"), true);
        }

        private void CopyFiles(string sourcePath, string destinationPath)
        {
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            var files = Directory.GetFiles(sourcePath);
            foreach (var file in files)
            {
                var fi = new FileInfo(file);
                File.Copy(file, Path.Combine(destinationPath, fi.Name), true);
            }

            var directories = Directory.GetDirectories(sourcePath);
            foreach (var directory in directories)
            {
                var di = new DirectoryInfo(directory);
                CopyFiles(directory, Path.Combine(destinationPath, di.Name));
            }
        }

        private static void ClearFileCache(string updatePackPath, string tempZipDirectory)
        {
            File.Delete(updatePackPath);
            Directory.Delete(tempZipDirectory, true);
        }
    }
}
