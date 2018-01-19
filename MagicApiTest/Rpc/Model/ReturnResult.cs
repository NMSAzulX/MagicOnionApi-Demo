using MessagePack;

namespace MagicApiTest.Rpc.Model
{
    [MessagePackObject(true)]
    public struct ReturnResult
    {
        public string Msg;
        public int Status;
        public object Data;
    }
}
