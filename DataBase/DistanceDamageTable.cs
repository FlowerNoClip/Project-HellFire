using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;
using UnityEngine;
using static WeaponData;
using static DistanceDamageTable;
using System.Collections;

public class DistanceDamageTable : DBConnect
{
    public struct DistanceDamageInfo
    {
        public int index;
        public int min_Distance;
        public int max_Distance;
        public int head_dmg;
        public int body_dmg;
        public int leg_dmg;
    }


    public DistanceDamageInfo[] DistanceDamageInfoArray;

    public DistanceDamageInfo DistanceDamageInfos = new DistanceDamageInfo();
    protected override void Start()
    {
        base.Start();
        StartCoroutine(GetDatas());
    }

    private IEnumerator GetDatas()
    {
        yield return new WaitForSeconds(0.1f);
        GetData();
        yield return null;
    }
    private void GetData()
    {
        string query = "SELECT * FROM `distancedamageTable`";
        List<DistanceDamageInfo> GetData = new List<DistanceDamageInfo>();

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
                        DistanceDamageInfo distanceDamageInfo = new DistanceDamageInfo();
                        distanceDamageInfo.index = reader.GetInt32(1);
                        distanceDamageInfo.min_Distance = reader.GetInt32(2);
                        distanceDamageInfo.max_Distance = reader.GetInt32(4);
                        distanceDamageInfo.head_dmg = reader.GetInt32(5);
                        distanceDamageInfo.body_dmg = reader.GetInt32(6);
                        distanceDamageInfo.leg_dmg = reader.GetInt32(7);

                        // Add the populated WeaponDataInfos to your list
                        GetData.Add(distanceDamageInfo);




                        // Optionally, you can log the retrieved data for debugging
                        /*Debug.Log($"index: {distanceDamageInfo.index}, min_Distance: {distanceDamageInfo.min_Distance}, " +
                        $" max_Distance: {distanceDamageInfo.max_Distance}, " +
                        $"head_dmg: {distanceDamageInfo.head_dmg}, body_dmg: {distanceDamageInfo.body_dmg}, leg_dmg: {distanceDamageInfo.leg_dmg}");*/
                    }

                }
                DistanceDamageInfoArray = GetData.ToArray();
            }
        }
        catch (Exception e)
        {
            Debug.Log("Äõ¸® ¿À·ù: " + e.Message);
        }
    }
}

