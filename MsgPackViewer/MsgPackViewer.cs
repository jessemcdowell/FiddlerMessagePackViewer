using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MsgPackSlim;

namespace MsgPackViewer
{
    public partial class MsgPackViewer : UserControl
    {
        public MsgPackViewer()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            tree.Nodes.Clear();
        }

        public void DisplayData(byte[] data)
        {
            Clear();

            var root = CreateNode("Root", String.Format("({0} bytes)", data.LongLength));
            var parent = root;
            var remainingCountStack = new Stack<int>();
            var remainingCount = 0;

            using (var stream = new MemoryStream(data))
            using (var reader = new MsgPackReader(stream))
            {
                while (reader.ReadNext())
                {
                    var node = AddNode(parent, reader);
                    remainingCount--;

                    if (reader.IsArray || reader.IsMap)
                    {
                        remainingCountStack.Push(remainingCount);
                        remainingCount = reader.ChildObjectCount;
                        parent = node;
                    }

                    while (remainingCount == 0)
                    {
                        if (remainingCountStack.Count > 0)
                            break;
                        remainingCount = remainingCountStack.Pop();
                        parent = parent.Parent;
                    }
                }

                if (reader.ReadNext())
                {
                    root.Nodes.Add(CreateNode("content after the root node!", ""));
                }
            }

            tree.Nodes.Add(root);
            tree.ExpandAll();
        }

        private TreeNode CreateNode(string text, string toolTip)
        {
            return new TreeNode(text)
            {
                ToolTipText = toolTip
            };
        }

        private TreeNode AddNode(TreeNode parent, MsgPackReader reader)
        {
            var text = String.Format("{0} ({1})", reader.GetValue(), reader.Format.GetType().Name);
            var tooltip = String.Format("Format Byte: {0:x2} ({1} bytes)", reader.FormatByte, reader.TotalSize);
            var node = CreateNode(text, tooltip);
            parent.Nodes.Add(node);
            return node;
        }

        private string GetHex(byte[] data)
        {
            var output = new StringBuilder(data.Length*3 - 1);
            foreach (var b in data)
            {
                if (output.Length > 0)
                    output.Append(' ');
                output.Append(b.ToString("x2"));
            }
            return output.ToString();
        }
    }
}
