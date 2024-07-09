using UnityEngine;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class BaseKeyTable : DBConnect
{
    // ����ü ����: �����ͺ��̽����� ������ ���� ������ ����
    public struct BaseKeyData
    {
        public int index;
        public int actionName;
        public int actionDesc;
        public int baseKey;
    }

    // ����ü ����: ��ȯ�� ���ڿ� ���� ����
    public struct BaseKeyDataString
    {
        public int index;
        public string actionName;
        public string actionDesc;
        public string baseKey;

        // ���ڿ��� ��ȯ�ϴ� ToString() �޼��� ������
        public override string ToString()
        {
            return $"ActionName: {actionName}, ActionDesc: {actionDesc}, BaseKey: {baseKey}";
        }
    }

    

    // ������ �迭
    public BaseKeyData[] baseKeyDataArray;
    public BaseKeyDataString[] baseKeyDataStringArray;

    // ������ �׸�
    public BaseKeyData baseKeyDatas = new BaseKeyData();
    public BaseKeyDataString baseKeyDataString = new BaseKeyDataString();
    
    protected override void Start()
    {
        base.Start();
        if (baseKeyDataArray == null && ConnectionTest())
        {
            GetData(); // �����ͺ��̽����� ������ ��������
            
        }
        if (baseKeyDataStringArray == null)
        {
            EqualData();
        }
    }
    
    private void EqualData()
    {
            List<BaseKeyDataString> GetData = new List<BaseKeyDataString>();

            // �����ͺ��̽� �׸�� ���ڿ� �����͸� ���Ͽ� ��ȯ
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

                // ����Ʈ�� ��ȯ�� ������ �߰�
                GetData.Add(baseKeyDataString);
            }

            // �迭�� ��ȯ�Ͽ� ����
            baseKeyDataStringArray = GetData.ToArray();

            // ��� ������ ��� (����׿�)
            //PrintBaseKeyDataStringArray();
        
    }

    // ������ ��� �޼���

    // �����ͺ��̽����� ������ ��������
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
            Debug.Log("���� ����: " + e.Message);
        }
    }
}
