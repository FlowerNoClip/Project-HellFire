using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ThrowableData : DBConnect
{
    public struct ThrowableDataInfo
    {
        public int index;
        public string throwables_Name;
        public string throwables_Desc;
        public int effect_Duration;
        public float radius;
        public float mounting_Time;
        public float equip_Run_SPD;
        public int throwables_Ban_Time;

/*        public override string ToString()
        {
            return $"index: {index}, throwables_Name: {throwables_Name}, throwables_Desc: {throwables_Desc}, " +
                   $"effect_Duration: {effect_Duration}, radius: {radius}, " +
                   $"mounting_Time: {mounting_Time}, equip_Run_SPD: {equip_Run_SPD}, " +
                   $"throwables_Ban_Time: {throwables_Ban_Time}";
        }*/
    }
    public ThrowableDataInfo[] throwableDataInfoArray;
    public ThrowableDataInfo throwableDataInfo = new ThrowableDataInfo();

/*    private void PrintAllSkillData()
    {
        foreach (var data in throwableDataInfoArray)
        {
            Debug.Log(data.ToString());
        }
    }*/
    protected override void Start()
    {
        base.Start();


        GetThrowableData();
    }

    public void GetThrowableData()
    {
        string query = "SELECT * FROM `throwabletable`";
        List<ThrowableDataInfo> GetData = new List<ThrowableDataInfo>();

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
                        throwableDataInfo.index = reader.GetInt32(0);
                        throwableDataInfo.throwables_Name = reader.GetString(1);
                        throwableDataInfo.throwables_Desc = reader.GetString(2);
                        throwableDataInfo.effect_Duration = reader.GetInt32(3);
                        throwableDataInfo.radius = reader.GetFloat(4);
                        throwableDataInfo.mounting_Time = reader.GetFloat(5);
                        throwableDataInfo.equip_Run_SPD = reader.GetFloat(6);
                        throwableDataInfo.throwables_Ban_Time = reader.GetInt32(7);
                        GetData.Add(throwableDataInfo);

                        /*Debug.Log($"index: {throwableDataInfo.index}, throwables_Name: {throwableDataInfo.throwables_Name}, throwables_Desc: {throwableDataInfo.throwables_Desc}, effect_Duration: {throwableDataInfo.effect_Duration}, radius: {throwableDataInfo.radius}, mounting_Time: {throwableDataInfo.mounting_Time}, equip_Run_SPD: {throwableDataInfo.equip_Run_SPD}, throwables_Ban_Time: {throwableDataInfo.throwables_Ban_Time}");*/
                    }
                }
            }
            throwableDataInfoArray = GetData.ToArray();
        }
        catch (Exception e)
        {
            Debug.Log("Äõ¸® ¿À·ù: " + e.Message);
        }
    }
}
