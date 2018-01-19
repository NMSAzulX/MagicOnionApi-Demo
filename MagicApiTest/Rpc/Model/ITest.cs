using MagicOnion;

namespace MagicApiTest.Rpc.Model
{
    
    public interface ITest: IService<ITest>
    {
        /// <summary>
        /// 获取一个活的学生
        /// </summary>
        /// <param name="sid">学生id</param>
        /// <returns>一个鲜活的学生</returns>
        UnaryResult<ReturnResult> GetStudent(int sid);
    }
    
    
}
