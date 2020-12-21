using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Extensions
{
    public class FilesSend
    {
        private int maxbytes;
        private int total;
        private int copied;

        public List<FileInfo> FilesInfoList { get; set; }
        public TreeNode TreeNode { get; set; }

        public FilesSend(DirectoryInfo directoryInfo)
        {
            FilesInfoList = new List<FileInfo>();

            TreeNode = new TreeNode(directoryInfo.Name);
        }

        public TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            foreach (var directory in directoryInfo.GetDirectories())
                TreeNode.Nodes.Add(CreateDirectoryNode(directory));

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                TreeNode.Nodes.Add(new TreeNode(file.Name));
                FilesInfoList.Add(file);
            }

            return TreeNode;
        }

        /*
        private void Copy(FileInfo fileSource, string targetDirectory)
        {
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            //Gets size of all files present in source folder.
            GetSize(fileSource, diTarget);
            maxbytes = maxbytes / 1024;

            CopyAll(fileSource, diTarget);
        }

        private void CopyAll(FileInfo fi, DirectoryInfo target)
        {
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);

            total += (int)fi.Length;

            copied += (int)fi.Length;
            copied /= 1024;

        }

        private void GetSize(FileInfo fi, DirectoryInfo target)
        {
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            maxbytes += (int)fi.Length;//Size of File
        }
        */
    }
}
