using Grpc.Core;
using MagicApiTest.Rpc.Model;
using MagicOnion;
using MagicOnion.Server;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MagicApiTest.Rpc.Core
{
    public class Test :ServiceBase<ITest>, ITest
    {
        public UnaryResult<ReturnResult> GetStudent(int sid)
        {
            //GetStudentBySidFromDatabase(sid)
            //GetModel
            Student student;
            student.Name = "Test_小明";
            student.Sid = sid;

            //FillResult
            ReturnResult result = new ReturnResult() {
                Data = student,
                Status = 0
            };

            //Return
            return UnaryResult(result);
        }
    }
}
