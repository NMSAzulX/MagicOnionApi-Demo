using MessagePack;

namespace MagicApiTest.Rpc.Model
{
    [MessagePackObject(true)]
    public struct Student
    {
        public string Name;
        public int Sid;
    }
}
