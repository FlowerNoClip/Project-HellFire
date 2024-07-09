using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;
using UnityEngine;
using static PlayerSkillInfo;
using static WeaponData;

public class ArmorData : DBConnect
{
    public struct ArmorDataInfo
    {
        public int index;
        public string armor_Name;
        public string armor_Desc;
        public int armor_Type;
        public int armor_Defense;
    }
    public ArmorDataInfo[] armorDataInfoArray;
    public ArmorDataInfo armorDataInfo = new ArmorDataInfo();
    protected override void Start()
    {
        base.Start();
            
            
        GetArmorData();
    }

    public void GetArmorData()
    {
        string query = "SELECT * FROM `armortable`";
        List<ArmorDataInfo> GetData = new List<ArmorDataInfo>();

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
                        armorDataInfo.index = reader.GetInt32(0);
                        armorDataInfo.armor_Name = reader.GetString(1);
                        armorDataInfo.armor_Desc = reader.GetString(2);
                        armorDataInfo.armor_Type = reader.GetInt32(3);
                        armorDataInfo.armor_Defense = reader.GetInt32(4);
                        GetData.Add(armorDataInfo);

                        //Debug.Log($"index: {armorDataInfo.index}, armor_Name: {armorDataInfo.armor_Name}, armor_Desc: {armorDataInfo.armor_Desc}, armor_Type: {armorDataInfo.armor_Type}, armor_Defense: {armorDataInfo.armor_Defense}");

                    }
                }
            }
            armorDataInfoArray = GetData.ToArray();
        }
        catch (Exception e)
        {
            Debug.Log("Äõ¸® ¿À·ù: " + e.Message);
        }
    }
 }
