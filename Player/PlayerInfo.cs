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

    // ����ü ����: ��ȯ�� ���ڿ� ���� ����
    public struct PlayerInfoString
    {
        public int index;
        public string _class;
        public string class_Desc;

        // ���ڿ��� ��ȯ�ϴ� ToString() �޼��� ������
    }
    // ������ �迭
    public PlayerInfoIndex[] PlayerInfoArray;
    public PlayerInfoString[] PlayerInfoStringArray;

    // ������ �׸�
    public PlayerInfoIndex PlayerInfoDatas = new PlayerInfoIndex();
    public PlayerInfoString PlayerInfoStrings = new PlayerInfoString();

    protected override void Start()
    {
        base.Start();
        Debug.Log("asd");
        if (PlayerInfoArray == null && ConnectionTest())
        {
            GetData(); // �����ͺ��̽����� ������ ��������

        }
        if (PlayerInfoStringArray == null)
        {
            EqualData();
        }
    }

    private void EqualData()
    {
        List<PlayerInfoString> GetData = new List<PlayerInfoString>();

        // �����ͺ��̽� �׸�� ���ڿ� �����͸� ���Ͽ� ��ȯ
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

            // ����Ʈ�� ��ȯ�� ������ �߰�
            GetData.Add(playerInfoString);
        }

        // �迭�� ��ȯ�Ͽ� ����
        PlayerInfoStringArray = GetData.ToArray();

        // ��� ������ ��� (����׿�)
        //PrintBaseKeyDataStringArray();

    }

    // ������ ��� �޼���

    // �����ͺ��̽����� ������ ��������
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
            Debug.Log("���� ����: " + e.Message);
        }
    }
}
