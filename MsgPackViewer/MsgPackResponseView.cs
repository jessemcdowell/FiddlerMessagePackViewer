using Fiddler;

namespace MsgPackViewer
{
    public class MsgPackResponseView : MsgPackViewBase, IResponseInspector2
    {
        public HTTPResponseHeaders headers { get; set; }
    }
}
