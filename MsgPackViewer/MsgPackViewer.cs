using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MsgPackSlim;
using MsgPackSlim.Formats;

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

            TreeNode root = null;
            TreeNode parent = null;
            var remainingCountStack = new Stack<int>();
            var remainingCount = 1;

            using (var stream = new MemoryStream(data))
            using (var reader = new MsgPackReader(stream))
            {
                try
                {
                    while (reader.ReadNext())
                    {
                        var node = CreateNode(reader);
                        if (parent != null)
                            parent.Nodes.Add(node);
                        else
                        {
                            root = node;
                            parent = node;
                        }
                        remainingCount--;

                        if (reader.IsArray || reader.IsMap)
                        {
                            remainingCountStack.Push(remainingCount);
                            remainingCount = reader.ChildObjectCount;
                            parent = node;
                        }

                        while ((remainingCount == 0) && (remainingCountStack.Count > 0))
                        {
                            remainingCount = remainingCountStack.Pop();
                            parent = parent.Parent;
                        }
                        if ((remainingCount == 0) && (remainingCountStack.Count == 0))
                            break;
                    }

                    if (reader.ReadNext())
                        throw new Exception("content after the root node");
                }
                catch (Exception ex)
                {
                    var message = String.Format("Error at byte {0}: {1}", stream.Position, ex.Message);
                    var node = CreateNode(message, ex.ToString());
                    if (root == null)
                        root = node;
                    else
                        root.Nodes.Add(node);
                }
            }

            if (root != null)
            {
                tree.Nodes.Add(root);
                tree.ExpandAll();
                tree.SelectedNode = root;
            }
        }

        private TreeNode CreateNode(string text, string toolTip)
        {
            return new TreeNode(text)
            {
                ToolTipText = toolTip
            };
        }

        private TreeNode CreateNode(MsgPackReader reader)
        {
            var formatName = GetFormatName(reader.Format);

            var text = GetNodeText(reader);

            var tooltip = String.Format("{0} ({1:x2}) ({2} bytes)", formatName, reader.FormatByte, reader.TotalSize);
            if (reader.IsExtended)
                tooltip += String.Format(" (Extended Type {0})", reader.ExtendedType);
            else if ((reader.ContentSize > 0) && (reader.ContentSize <= 32) && !reader.IsBinary)
                tooltip += " " + GetHex(reader.ContentBytes);

            return CreateNode(text, tooltip);
        }

        private static string GetNodeText(MsgPackReader reader)
        {
            if (reader.IsArray)
                return String.Format("Array ({0} items)", reader.ChildObjectCount);
            if (reader.IsMap)
                return String.Format("Map ({0} items)", reader.ChildObjectCount/2);

            var value = reader.GetValue();
            if (value == null)
                return "null";

            var stringValue = value as string;
            if (stringValue != null)
                return "\"" + stringValue.Replace("\"", "\\\"").Replace("\0", "\\0") + "\"";

            var byteValue = value as byte[];
            if (byteValue != null)
                return GetHex(byteValue);

            return value.ToString();
        }

        private string GetFormatName(IMsgPackFormat format)
        {
            var name = format.GetType().Name;
            if (name.EndsWith("Format"))
                return name.Substring(0, name.Length - 6);
            return name;
        }

        private static string GetHex(byte[] data)
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

        private void tree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            e.CancelEdit = true;
        }
    }
}
