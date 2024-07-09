using UnityEngine;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class DBConnect : MonoBehaviour
{
    // ������ ����ü ����
    public struct StringData
    {
        public int index;
        public string stringDesc;
        public string stringInfo;

        // Debug �� ToString() �޼��� �������̵�
        public override string ToString()
        {
            return $"index: {index}, stringDesc: {stringDesc}, stringInfo: {stringInfo}";
        }
    }

    // �����ͺ��̽� ���� ���ڿ�

    [HideInInspector]public string conStr = string.Format("Server={0};Database={1};Uid={2};Pwd={3};", "projectmwd.pro", "hellfire", "root", "Gnrhkdtkfkd!2");

    // ������ �迭
    public StringData[] stringDataArray;
    public StringData stringDatas = new StringData();

    protected virtual void Start()
    {
        // ������ �迭�� ��� �ְ� �����ͺ��̽� ���� �׽�Ʈ�� �����ϸ� ������ ����
        if (stringDataArray == null && ConnectionTest())
        {
            SaveData();
        }
        else
        {
            Debug.Log("DB ���� �׽�Ʈ ����.");
        }
    }

    // �����ͺ��̽� ���� �׽�Ʈ
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
            Debug.Log("���� ����: " + e.Message);
            return false;
        }
    }

    // ������ �����ͼ� �迭�� ����
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
            stringDataArray = stringDataList.ToArray(); // ����Ʈ�� �迭�� ��ȯ�Ͽ� ����
        }
        catch (Exception e)
        {
            Debug.Log("���� ����: " + e.Message);
        }
    }
}
