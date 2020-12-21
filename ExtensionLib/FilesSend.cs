using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ExtensionLib
{
    public class FilesSend
    {
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
    }
}
