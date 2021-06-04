using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitHubApiApp.Models
{
    public static class FilesManager
    {
        public static ArrayList Files { get; set; } = new ArrayList();
        public static List<ArrayList> AllFiles { get; set; } = new List<ArrayList>();
        public static bool canBack { get; set; } = false;

        public static List<string> Extension { get; set; } = new List<string>()
        {
            "jpg",
            "png",
            "gif",
            "jpeg",
            "ico",
            "bmp",
        };

        public static List<TreeNode> GetAllNodes(this TreeView _self)
        {
            List<TreeNode> result = new List<TreeNode>();
            foreach (TreeNode child in _self.Nodes)
            {
                result.AddRange(child.GetAllNodes());
            }
            return result;
        }

        public static List<TreeNode> GetAllNodes(this TreeNode _self)
        {
            List<TreeNode> result = new List<TreeNode>();
            result.Add(_self);
            foreach (TreeNode child in _self.Nodes)
            {
                result.AddRange(child.GetAllNodes());
            }
            return result;
        }

        public static List<string> Split(string str)
        {
            char[] separators = new char[] { ',', '.', '\\', ';', '/', ':' };

            List<string> arr = new List<string>();

            string buf = "";
            for (int i = 0; i < str.Length; i++)
            {
                buf += str[i];
                if (separators.Any(t => t == str[i]))
                {
                    arr.Add(buf);
                    buf = "";
                }
            }
            return arr;
        }
    }
}
