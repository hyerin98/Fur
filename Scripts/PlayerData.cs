namespace IMFINE.Utils.JoyStream.Communicator
{
    [System.Serializable]
    public class PlayerData
    {
        public string color_id; // 컬러 아이디
        public string conn_id; // 유저 고유 아이디
        public int player_index;

        // 생성자에서 color_id와 conn_id에 기본값 설정
        public PlayerData(string connID = "", int playerIndex = 0, string colorId = "")
        {
            conn_id = connID;
            player_index = playerIndex;
            color_id = colorId;
        }

        public override string ToString()
        {
            return $"conn_id: {conn_id}, player_index: {player_index}, color_id: {color_id}";
        }
    }
}