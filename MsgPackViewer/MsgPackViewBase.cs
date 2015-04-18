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

        private static readonly byte[] TestData =
        {
            //0x83, 0xa7, 0x63, 0x6f, 0x6d, 0x70, 0x61, 0x63, 0x74, 0xc3, 0xa6, 0x73, 0x63, 0x68, 0x65, 0x6d, 0x61, 0x00, 0xa4, 0x74, 0x65, 0x73, 0x74, 0xce, 0x49, 0x96, 0x02, 0xd2
            0x84, 0xa7, 0x63, 0x6f, 0x6d, 0x70, 0x61, 0x63, 0x74, 0xc3, 0xa6, 0x73, 0x63, 0x68, 0x65, 0x6d, 0x61, 0x00, 0xa4, 0x6c, 0x69, 0x73, 0x74, 0x93, 0x01, 0x02, 0x03, 0xa5, 0x63, 0x68, 0x69, 0x6c, 0x64, 0x83, 0xa3, 0x6f, 0x6e, 0x65, 0x01, 0xa3, 0x74, 0x77, 0x6f, 0x02, 0xa4, 0x6e, 0x75, 0x6c, 0x6c, 0xc0
        };

        public byte[] body
        {
            get { return _body; }
            set
            {
                _body = value;
                //_viewer.DisplayData(value);

                _viewer.DisplayData(TestData);
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
