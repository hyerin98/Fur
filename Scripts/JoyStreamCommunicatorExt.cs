using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMFINE.Utils.JoyStream.Communicator.ext
{
    public static class JoyStreamCommunicatorExt
    {
        public static void LogDetailedState(this JoyStreamCommunicator communicator)
        {
            communicator.MessageReceived += (conn_id, key, value) =>
            {
                TraceBox.Log($"Detailed Log - Connection ID: {conn_id}, Key: {key}, Value: {value}");
            };
        }

        public static void RegisterPlayerActivity(this JoyStreamCommunicator communicator)
    {
        communicator.KeyUp += (conn_id, key_code) =>
        {
            // 키 업 이벤트를 구체적으로 처리하는 로직
            TraceBox.Log($"키 업 이벤트 발생 - {conn_id}: 키 코드 {key_code}");
        };
    }
    }
}
