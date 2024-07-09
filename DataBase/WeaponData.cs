using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;
using UnityEngine;
using static PlayerSkillInfo;

public class WeaponData : DBConnect
{
    public struct WeaponDataInfo
    {
        public int index;
        public string weapon_Name;
        public string weapon_Desc;
        public int weapon_Type;
        public int weapon_Class;
        public int fire_Type;
        public int aiming_Mode;
        public float mounting_Time;
        public float equip_Run_SPD1;
        public float equip_Run_SPD2;
        public int wall_Penetration;
        public float first_Shot_Spread1;
        public float first_Shot_Spread2;
        public float reload_SPD;
        public float fire_Rate1;
        public float fire_Rate2;
        public int loaded_Ammo_Count;
        public int spare_Ammo_Count;
    }

    public WeaponDataInfo[] weaponDataInfoArray;
    public WeaponDataInfo weaponDataInfos = new WeaponDataInfo();


    protected override void Start()
    {
        base.Start();

        if (ConnectionTest())
        {
            GetWeaponData();
        }
        else
        {
            Debug.Log("DB 연결 테스트 실패.");
        }

    }

    public void GetWeaponData()
    {
        string query = "SELECT * FROM `weapontable`";
        List<WeaponDataInfo> GetData = new List<WeaponDataInfo>();

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
                        weaponDataInfos.index = reader.GetInt32(0);
                        weaponDataInfos.weapon_Name = reader.GetString(1);
                        weaponDataInfos.weapon_Desc = reader.GetString(2);
                        weaponDataInfos.weapon_Type = reader.GetInt32(3);
                        weaponDataInfos.weapon_Class = reader.GetInt32(4);
                        weaponDataInfos.fire_Type = reader.GetInt32(5);
                        weaponDataInfos.aiming_Mode = reader.GetInt32(6);
                        weaponDataInfos.mounting_Time = reader.GetFloat(7);
                        weaponDataInfos.equip_Run_SPD1 = reader.GetFloat(8);
                        weaponDataInfos.equip_Run_SPD2 = reader.GetFloat(9);
                        weaponDataInfos.wall_Penetration = reader.GetInt32(10);
                        weaponDataInfos.first_Shot_Spread1 = reader.GetFloat(11);
                        weaponDataInfos.first_Shot_Spread2 = reader.GetFloat(12);
                        weaponDataInfos.reload_SPD = reader.GetFloat(13);
                        weaponDataInfos.fire_Rate1 = reader.GetFloat(14);
                        weaponDataInfos.fire_Rate2 = reader.GetFloat(15);
                        weaponDataInfos.loaded_Ammo_Count = reader.GetInt32(16);
                        weaponDataInfos.spare_Ammo_Count = reader.GetInt32(17);

                        // Add the populated WeaponDataInfos to your list
                        GetData.Add(weaponDataInfos);

                        // Optionally, you can log the retrieved data for debugging
/*                        Debug.Log($"index: {weaponDataInfos.index}, weapon_Name: {weaponDataInfos.weapon_Name}, weapon_Desc: {weaponDataInfos.weapon_Desc}, " +
                                  $"weapon_Type: {weaponDataInfos.weapon_Type}, weapon_Class: {weaponDataInfos.weapon_Class}, fire_Type: {weaponDataInfos.fire_Type}, " +
                                  $"aiming_Mode: {weaponDataInfos.aiming_Mode}, mounting_Time: {weaponDataInfos.mounting_Time}, equip_Run_SPD1: {weaponDataInfos.equip_Run_SPD1}, " +
                                  $"equip_Run_SPD2: {weaponDataInfos.equip_Run_SPD2}, wall_Penetration: {weaponDataInfos.wall_Penetration}, " +
                                  $"first_Shot_Spread1: {weaponDataInfos.first_Shot_Spread1}, first_Shot_Spread2: {weaponDataInfos.first_Shot_Spread2}, " +
                                  $"reload_SPD: {weaponDataInfos.reload_SPD}, fire_Rate1: {weaponDataInfos.fire_Rate1}, fire_Rate2: {weaponDataInfos.fire_Rate2}, " +
                                  $"loaded_Ammo_Count: {weaponDataInfos.loaded_Ammo_Count}, spare_Ammo_Count: {weaponDataInfos.spare_Ammo_Count}");*/
                    }
                }
            }
            weaponDataInfoArray = GetData.ToArray();
        }
        catch (Exception e)
        {
            Debug.Log("쿼리 오류: " + e.Message);
        }
    }
}

