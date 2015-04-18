using Fiddler;

namespace MsgPackViewer
{
    public class MsgPackRequestView : MsgPackViewBase, IRequestInspector2
    {
        public HTTPRequestHeaders headers { get; set; }
    }
}
