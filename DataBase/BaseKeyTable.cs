using UnityEngine;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class BaseKeyTable : DBConnect
{
    // 구조체 정의: 데이터베이스에서 가져온 원시 데이터 구조
    public struct BaseKeyData
    {
        public int index;
        public int actionName;
        public int actionDesc;
        public int baseKey;
    }

    // 구조체 정의: 변환된 문자열 정보 구조
    public struct BaseKeyDataString
    {
        public int index;
        public string actionName;
        public string actionDesc;
        public string baseKey;

        // 문자열로 변환하는 ToString() 메서드 재정의
        public override string ToString()
        {
            return $"ActionName: {actionName}, ActionDesc: {actionDesc}, BaseKey: {baseKey}";
        }
    }

    

    // 데이터 배열
    public BaseKeyData[] baseKeyDataArray;
    public BaseKeyDataString[] baseKeyDataStringArray;

    // 데이터 항목
    public BaseKeyData baseKeyDatas = new BaseKeyData();
    public BaseKeyDataString baseKeyDataString = new BaseKeyDataString();
    
    protected override void Start()
    {
        base.Start();
        if (baseKeyDataArray == null && ConnectionTest())
        {
            GetData(); // 데이터베이스에서 데이터 가져오기
            
        }
        if (baseKeyDataStringArray == null)
        {
            EqualData();
        }
    }
    
    private void EqualData()
    {
            List<BaseKeyDataString> GetData = new List<BaseKeyDataString>();

            // 데이터베이스 항목과 문자열 데이터를 비교하여 변환
            for (int j = 0; j < baseKeyDataArray.Length; j++)
            {
                BaseKeyDataString baseKeyDataString = new BaseKeyDataString();

                for (int i = 0; i < stringDataArray.Length; i++)
                {
                    if (stringDataArray[i].index == baseKeyDataArray[j].actionName)
                    {
                        baseKeyDataString.actionName = stringDataArray[i].stringDesc;
                    }
                    if (stringDataArray[i].index == baseKeyDataArray[j].actionDesc)
                    {
                        baseKeyDataString.actionDesc = stringDataArray[i].stringDesc;
                    }
                    if (stringDataArray[i].index == baseKeyDataArray[j].baseKey)
                    {
                        baseKeyDataString.baseKey = stringDataArray[i].stringInfo;
                    }
                }

                // 리스트에 변환된 데이터 추가
                GetData.Add(baseKeyDataString);
            }

            // 배열로 변환하여 저장
            baseKeyDataStringArray = GetData.ToArray();

            // 모든 데이터 출력 (디버그용)
            //PrintBaseKeyDataStringArray();
        
    }

    // 데이터 출력 메서드

    // 데이터베이스에서 데이터 가져오기
    public void GetData()
    {
        string query = "SELECT * FROM `basekeytable`";
        List<BaseKeyData> GetData = new List<BaseKeyData>();

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
                        baseKeyDatas.index = reader.GetInt32(0);
                        baseKeyDatas.actionName = reader.GetInt32(1);
                        baseKeyDatas.actionDesc = reader.GetInt32(2);
                        baseKeyDatas.baseKey = reader.GetInt32(3);
                        GetData.Add(baseKeyDatas);
                        //Debug.Log($"index: {baseKeyDatas.index}, actionName: {baseKeyDatas.actionName}, actionDesc: {baseKeyDatas.actionDesc}, baseKey: {baseKeyDatas.baseKey}");
                    }
                }
            }
            baseKeyDataArray = GetData.ToArray();
        }
        catch (Exception e)
        {
            Debug.Log("쿼리 오류: " + e.Message);
        }
    }
}
