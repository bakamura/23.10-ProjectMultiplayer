using UnityEngine;
using Fusion;

public class RMarkable : MonoBehaviour
{

    private static RMarkable _markCurrent;
    private static Vector3 _markPos;

    public void Mark()
    {
        if (_markCurrent != null)
        {

        }
        _markCurrent = this;
        _markPos = transform.position;
    }

    public static void Recall()
    {
        if (_markCurrent)
        {
            _markCurrent.transform.position = _markPos;
            _markCurrent = null;
        }
        else Debug.Log("No Marked Object");
    }

}
// perguntar se ele vai conseguir chamar esse método do RPC na instancia especifica q foi chamada, sendo q ela foi ativada por um cliente q n tem StateAutority
//public class RMarkable : NetworkBehaviour
//{

//    [Networked] private NetworkBehaviourId _markCurrent { get; set; }
//    public RMarkable CurrentMarked => TryFind();
//    private static Vector3 _markPos;

//    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//    public void RpcMark()
//    {

//        if (_markCurrent != null)
//        {

//        }
//        _markCurrent = this;
//        _markPos = transform.position;
//    }
//    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//    public void RpcRecall()
//    {
//        RMarkable temp = CurrentMarked;
//        if (temp)
//        {
//            temp.transform.position = _markPos;
//            _markCurrent = default;
//        }
//        else Debug.Log("No Marked Object");
//    }

//    private RMarkable TryFind()
//    {
//        if (_markCurrent == default)
//        {
//            return null;
//        }

//        Runner.TryFindBehaviour(_markCurrent, out RMarkable obj);
//        return obj;
//    }

//}
