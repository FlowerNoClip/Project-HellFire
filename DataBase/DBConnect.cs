using UnityEngine;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class DBConnect : MonoBehaviour
{
    // 데이터 구조체 정의
    public struct StringData
    {
        public int index;
        public string stringDesc;
        public string stringInfo;

        // Debug 용 ToString() 메서드 오버라이드
        public override string ToString()
        {
            return $"index: {index}, stringDesc: {stringDesc}, stringInfo: {stringInfo}";
        }
    }

    // 데이터베이스 연결 문자열

    [HideInInspector]public string conStr = string.Format("Server={0};Database={1};Uid={2};Pwd={3};", "projectmwd.pro", "hellfire", "root", "Gnrhkdtkfkd!2");

    // 데이터 배열
    public StringData[] stringDataArray;
    public StringData stringDatas = new StringData();

    protected virtual void Start()
    {
        // 데이터 배열이 비어 있고 데이터베이스 연결 테스트가 성공하면 데이터 저장
        if (stringDataArray == null && ConnectionTest())
        {
            SaveData();
        }
        else
        {
            Debug.Log("DB 연결 테스트 실패.");
        }
    }

    // 데이터베이스 연결 테스트
    public bool ConnectionTest()
    {
        try
        {
            using (MySqlConnection conn = new MySqlConnection(conStr))
            {
                conn.Open();
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.Log("연결 오류: " + e.Message);
            return false;
        }
    }

    // 데이터 가져와서 배열에 저장
    public void SaveData()
    {
        string query = "SELECT * FROM `stringtable`";
        List<StringData> stringDataList = new List<StringData>();

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
                        stringDatas.index = reader.GetInt32(0);
                        stringDatas.stringDesc = reader.GetString(1);
                        stringDatas.stringInfo = reader.GetString(2);
                        stringDataList.Add(stringDatas);
                        //Debug.Log("index: " + stringDatas.index + ", stringDesc: " + stringDatas.stringDesc + ", stringInfo: " + stringDatas.stringInfo);
                    }
                }
            }
            stringDataArray = stringDataList.ToArray(); // 리스트를 배열로 변환하여 저장
        }
        catch (Exception e)
        {
            Debug.Log("쿼리 오류: " + e.Message);
        }
    }
}
