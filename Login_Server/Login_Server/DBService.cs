using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Login_Server
{
    class DBService
    {
        private MySqlConnection conn;
        private string strConn;

        public DBService()
        {
            strConn = "Server=localhost;Database=playerlist;Uid=root;Pwd=skaksdmlskf;SslMode=none;";
            conn = new MySqlConnection(strConn);
        }

        // Connect
        public void Connect()
        {
            conn.Open();
        }

        // Close
        public void Close()
        {
            conn.Close();
        }

        // 상황에 따라서 코드값을 주고 그것을 실행시켜주면 좋을 듯? 
        public bool Login(string id, string pw, out string token, out string port)
        {
            string query;
            token = null;
            port = null;
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;

            // DB연결
            Connect();

            // ID, PW 확인
            {
                query = "SELECT * FROM player_list WHERE user_id=\'" + id + "\';";

                cmd.CommandText = query;
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read() == false)
                {
                    Log("아이디가 존재하지 않습니다.");
                    return false;
                }

                if ((string)reader["user_pw"] != pw)
                {
                    Log("비밀번호가 맞지 않습니다.");
                    return false;
                }

                reader.Close();
            }

            // ID, PW 확인 후, 메인 서버 접근을 위한 토큰값 저장.
            {
                query = "UPDATE player_list SET token=\'" + "20180824" + "\' WHERE user_id=\'" + id + "\'";
                cmd.CommandText = query;

                int row = cmd.ExecuteNonQuery();

                if (row != 1)
                    Log("토큰이 정상적으로 등록되지 않았습니다.");
                else
                    Log("토큰이 정상적으로 등록되었습니다.");

                Log("정상적으로 로그인토큰이 활성화 되었습니다.");
            }

            token = "20180824";
            port = "9000";

            // DB 종료
            Close();

            return true;
        }

        private void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
