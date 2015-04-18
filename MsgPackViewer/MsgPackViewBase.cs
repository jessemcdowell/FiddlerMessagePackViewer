using System.Windows.Forms;
using Fiddler;

namespace MsgPackViewer
{
    public abstract class MsgPackViewBase : Inspector2
    {
        private readonly MsgPackViewer _viewer = new MsgPackViewer();
        private byte[] _body;

        public override void AddToTab(TabPage o)
        {
            o.Text = "MsgPack";
            o.Controls.Add(_viewer);
            _viewer.Dock = DockStyle.Fill;
        }

        public override int GetOrder()
        {
            return 1000;
        }

        public void Clear()
        {
            _viewer.Clear();
        }

        public byte[] body
        {
            get { return _body; }
            set
            {
                _body = value;
                _viewer.DisplayData(value);
            }
        }

        public bool bDirty
        {
            get { return false; }
        }

        public bool bReadOnly
        {
            get { return true; }
            set { }
        }
    }
}
