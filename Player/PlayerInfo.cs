using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;
using UnityEngine;
using static BaseKeyTable;

public class PlayerInfo : DBConnect
{
    public struct PlayerInfoIndex
    {
        public int index;
        public int _class;
        public int class_Desc;
    }

    // 구조체 정의: 변환된 문자열 정보 구조
    public struct PlayerInfoString
    {
        public int index;
        public string _class;
        public string class_Desc;

        // 문자열로 변환하는 ToString() 메서드 재정의
    }
    // 데이터 배열
    public PlayerInfoIndex[] PlayerInfoArray;
    public PlayerInfoString[] PlayerInfoStringArray;

    // 데이터 항목
    public PlayerInfoIndex PlayerInfoDatas = new PlayerInfoIndex();
    public PlayerInfoString PlayerInfoStrings = new PlayerInfoString();

    protected override void Start()
    {
        base.Start();
        Debug.Log("asd");
        if (PlayerInfoArray == null && ConnectionTest())
        {
            GetData(); // 데이터베이스에서 데이터 가져오기

        }
        if (PlayerInfoStringArray == null)
        {
            EqualData();
        }
    }

    private void EqualData()
    {
        List<PlayerInfoString> GetData = new List<PlayerInfoString>();

        // 데이터베이스 항목과 문자열 데이터를 비교하여 변환
        for (int j = 0; j < PlayerInfoArray.Length; j++)
        {
            PlayerInfoString playerInfoString = new PlayerInfoString();

            for (int i = 0; i < stringDataArray.Length; i++)
            {
                if (stringDataArray[i].index == PlayerInfoArray[j]._class)
                {
                    playerInfoString._class = stringDataArray[i].stringDesc;
                }
                if (stringDataArray[i].index == PlayerInfoArray[j].class_Desc)
                {
                    playerInfoString.class_Desc = stringDataArray[i].stringDesc;
                }
            }

            // 리스트에 변환된 데이터 추가
            GetData.Add(playerInfoString);
        }

        // 배열로 변환하여 저장
        PlayerInfoStringArray = GetData.ToArray();

        // 모든 데이터 출력 (디버그용)
        //PrintBaseKeyDataStringArray();

    }

    // 데이터 출력 메서드

    // 데이터베이스에서 데이터 가져오기
    public void GetData()
    {
        string query = "SELECT * FROM `charactertable`";
        List<PlayerInfoIndex> GetData = new List<PlayerInfoIndex>();

        try
        {
            using (MySqlConnection conn = new MySqlConnection(conStr))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PlayerInfoDatas.index = reader.GetInt32(0);
                        PlayerInfoDatas._class = reader.GetInt32(1);
                        PlayerInfoDatas.class_Desc = reader.GetInt32(2);
                        GetData.Add(PlayerInfoDatas);
                    }
                }
            }
            PlayerInfoArray = GetData.ToArray();
        }
        catch (Exception e)
        {
            Debug.Log("쿼리 오류: " + e.Message);
        }
    }
}
