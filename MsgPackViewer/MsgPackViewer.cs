using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MsgPack;

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
            tree.Nodes.Add(root);

            using (var stream = new MemoryStream(data))
            using (var unpacker = Unpacker.Create(stream))
            {
                foreach (var child in unpacker)
                {
                    var typeName = "null";
                    if (child.UnderlyingType != null)
                        typeName = child.UnderlyingType.Name;
                    AddNode(root, GetValueText(child), typeName);
                }
            }

            tree.ExpandAll();
        }

        private TreeNode CreateNode(string text, string toolTip)
        {
            return new TreeNode(text)
            {
                ToolTipText = toolTip
            };
        }

        private TreeNode AddNode(TreeNode parent, string text, string toolTip)
        {
            var node = CreateNode(text, toolTip);
            parent.Nodes.Add(node);
            return node;
        }

        private string GetValueText(MessagePackObject node)
        {
            if (node.IsNil)
                return "null";
            if (node.IsArray)
                return "Array";
            if (node.IsMap)
                return "Map";
            if (node.IsDictionary)
                return "Dictionary";
            
            var type = node.UnderlyingType;

            if (type == typeof (Boolean))
                return node.AsBoolean().ToString();
            if (type == typeof (string))
                return "\"" + node.AsString() + "\"";
            if ((type == typeof(Byte)) ||
                (type == typeof(Int16)) ||
                (type == typeof(Int32)) ||
                (type == typeof(Int64)) ||
                (type == typeof(UInt16)) ||
                (type == typeof(UInt32)))
                return node.AsInt64().ToString(CultureInfo.InvariantCulture);
            if ((type == typeof(UInt64)))
                return node.AsUInt64().ToString(CultureInfo.InvariantCulture);
            if ((type == typeof(Single)) ||
                (type == typeof(Double)))
                return node.AsDouble().ToString(CultureInfo.InvariantCulture);

            if (node.IsRaw)
                return "Raw: " + GetHex(node.AsBinary());

            return "Unknown: " + type.Name;
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
