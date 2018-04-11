/*********************************************************
 * CopyRight: tiaoshuidenong. 
 * Author: tiaoshuidenong.
 * Address: wuhan
 * Create: 2018/4/11 16:39:48
 * Modify: 2018/4/11 16:39:48
 * Blog: http://www.cnblogs.com/tiaoshuidenong/
 * Description: wcf interface
 *********************************************************/
using System.ServiceModel;

namespace WcfClient
{
    [ServiceContract]
    public interface IOperationInterface : IClientChannel
    {
        [OperationContract]
        string Call(int num1, int num2);
    }
}
