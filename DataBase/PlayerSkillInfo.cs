using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Mathematics;

public class PlayerSkillInfo : DBConnect
{
    public int selectclass = 1;

    [HideInInspector] public int[] skills;
    [HideInInspector] public int[] prices;
    
    public struct ClassData
    {
        public int index;
        public int class_name;
        public int class_desc;
        public int skill_1;
        public int skill_2;
        public int skill_3;
        public int skill_4;
        public int skill_ult;
    }
    public struct ClassDataString
    {
        public int index;
        public string class_name;
        public string class_desc;

        public override string ToString()
        {
            return $"Index: {index}, Class Name: {class_name}, Class Description: {class_desc}";
        }
    }
    public struct SkillData
    {
        public int index;       
        public int skill_Type;  //1: 공격 2: 방어 3: 궁극기 / 공격, 방어스킬 교차 구매 불가
        public int skill_class; //1: 진화 2: 일반 3: 궁극기
        public int skill_name;
        public int skill_desc;
        public int evo_skill_effect;
        public int base_evo_skill_lv;
        public int max_evo_skill_lv;
        public int evo_skill_use_stack;
        public float mounting_time;
        public float casting_time;
        public int ult_point;
        public int ult_point_source;
        public int ult_point_gain;
        public int skill_ban_time;
    }
    public struct SkillDataString
    {
        public int index;
        public string skill_name;
        public string skill_desc;
        public string evo_skill_effect;
        public string ult_point_source;
        public override string ToString()
        {
            return $"SkillIndex : {index}, Skill Name: {skill_name}, Skill Description: {skill_desc}, Evo Skill Effect: {evo_skill_effect}, Ult Point Source: {ult_point_source}";
        }
    }

    // clss 데이터 배열
    public ClassData[] classDataArray;
    public ClassDataString[] classDataStringArray;
    public ClassData classDatas = new ClassData();
    public ClassDataString classDataString = new ClassDataString();
    // skill 데이터 배열
    public SkillData[] skillDataArray;
    public SkillDataString[] skillDataStringArray;
    public SkillData skillDatas = new SkillData();
    public SkillDataString skillDataString = new SkillDataString();

    private void Awake()
    {
        skills = new int[5];
        prices = new int[5];
    }
    protected override void Start()
    {
        base.Start();
        
        if (skillDataArray == null && ConnectionTest())
        {
            GetSkillData(); // 데이터베이스에서 데이터 가져오기
            GetClassData();
            SelectGetSkill(selectclass);
        }
    }


    public void SelectGetSkill(int index)
    {
        for (int i = 0; i < classDataStringArray.Length; i++)
        {
            if (classDataStringArray[i].index == index)
            {
                for (int j = 0; j < skillDataArray.Length; j++)
                {
                    if (skillDataArray[j].index == classDataArray[i].skill_1)
                    {
                        // skills 배열의 i 인덱스에 해당하는 위치에 값을 할당
                        skills[0] = skillDataArray[j].skill_name;
                        prices[0] = skillDataArray[j].index;
                    }
                    if (skillDataArray[j].index == classDataArray[i].skill_2)
                    {
                        // skills 배열의 i 인덱스에 해당하는 위치에 값을 할당
                        skills[1] = skillDataArray[j].skill_name;
                        prices[1] = skillDataArray[j].index;
                    }
                    if (skillDataArray[j].index == classDataArray[i].skill_3)
                    {
                        // skills 배열의 i 인덱스에 해당하는 위치에 값을 할당
                        skills[2] = skillDataArray[j].skill_name;
                        prices[2] = skillDataArray[j].index;
                    }
                    if (skillDataArray[j].index == classDataArray[i].skill_4)
                    {
                        // skills 배열의 i 인덱스에 해당하는 위치에 값을 할당
                        skills[3] = skillDataArray[j].skill_name;
                        prices[3] = skillDataArray[j].index;
                    }
                    if (skillDataArray[j].index == classDataArray[i].skill_ult)
                    {
                        // skills 배열의 i 인덱스에 해당하는 위치에 값을 할당
                        skills[4] = skillDataArray[j].skill_name;
                    }
                }
            }
        }
    }
    
    // 모든 데이터를 출력하는 메서드 (디버그용)
    private void PrintAllSkillData()
    {
        foreach (var skillData in skillDataStringArray)
        {
            Debug.Log(skillData.ToString());
        }
    }

    private void PrintAllClassData()
    {
        foreach (var classData in classDataStringArray)
        {
            Debug.Log(classData.ToString());
        }
    }


    public void GetSkillData()
    {
        string query = "SELECT * FROM `skilltable`";
        List<SkillData> GetData = new List<SkillData>();

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
                        skillDatas.index = reader.GetInt32(0);
                        skillDatas.skill_Type = reader.GetInt32(1);
                        skillDatas.skill_class = reader.GetInt32(2);
                        skillDatas.skill_name = reader.GetInt32(3);
                        skillDatas.skill_desc = reader.GetInt32(4);
                        skillDatas.evo_skill_effect = reader.GetInt32(5);
                        skillDatas.base_evo_skill_lv = reader.GetInt32(6);
                        skillDatas.max_evo_skill_lv = reader.GetInt32(7);
                        skillDatas.evo_skill_use_stack = reader.GetInt32(8);
                        skillDatas.mounting_time = reader.GetFloat(9);
                        skillDatas.casting_time = reader.GetFloat(10);
                        skillDatas.ult_point = reader.GetInt32(11);
                        skillDatas.ult_point_source = reader.GetInt32(12);
                        skillDatas.ult_point_gain = reader.GetInt32(13);
                        skillDatas.skill_ban_time = reader.GetInt32(14);

                        GetData.Add(skillDatas);

                        //Debug.Log($"index: {skillDatas.index}, skill_Type: {skillDatas.skill_Type}, skill_class: {skillDatas.skill_class}, skill_name: {skillDatas.skill_name}, skill_desc: {skillDatas.skill_desc}, evo_skill_effect: {skillDatas.evo_skill_effect}, base_evo_skill_lv: {skillDatas.base_evo_skill_lv}, max_evo_skill_lv: {skillDatas.max_evo_skill_lv}, evo_skill_use_stack: {skillDatas.evo_skill_use_stack}, mounting_time: {skillDatas.mounting_time}, casting_time: {skillDatas.casting_time}, ult_point: {skillDatas.ult_point}, ult_point_source: {skillDatas.ult_point_source}, ult_point_gain: {skillDatas.ult_point_gain}, skill_ban_time: {skillDatas.skill_ban_time}");
                    }
                }
            }
            skillDataArray = GetData.ToArray();
        }
        catch (Exception e)
        {
            Debug.Log("쿼리 오류: " + e.Message);
        }

        List<SkillDataString> GetDatas = new List<SkillDataString>();

        // 데이터베이스 항목과 문자열 데이터를 비교하여 변환
        for (int j = 0; j < skillDataArray.Length; j++)
        {
            SkillDataString skillDataString = new SkillDataString();

            for (int i = 0; i < stringDataArray.Length; i++)
            {
                if (stringDataArray[i].index == skillDataArray[j].skill_name)
                {
                    skillDataString.skill_name = stringDataArray[i].stringInfo;
                    skillDataString.index = skillDataArray[j].index;
                }
                if (stringDataArray[i].index == skillDataArray[j].skill_desc)
                {
                    skillDataString.skill_desc = stringDataArray[i].stringInfo;
                    
                }
                if (stringDataArray[i].index == skillDataArray[j].evo_skill_effect)
                {
                    skillDataString.evo_skill_effect = stringDataArray[i].stringInfo;
                    
                }
                if (stringDataArray[i].index == skillDataArray[j].ult_point_source)
                {
                    skillDataString.ult_point_source = stringDataArray[i].stringInfo;
                    
                }
            }

            // 리스트에 변환된 데이터 추가
            GetDatas.Add(skillDataString);
        }

        // 배열로 변환하여 저장
        skillDataStringArray = GetDatas.ToArray();

        //PrintAllSkillData();
    }
    public void GetClassData()
    {
        string query = "SELECT * FROM `charactertable`";
        List<ClassData> GetData = new List<ClassData>();

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
                        classDatas.index = reader.GetInt32(0);
                        classDatas.class_name = reader.GetInt32(1);
                        classDatas.class_desc = reader.GetInt32(2);
                        classDatas.skill_1 = reader.GetInt32(14);
                        classDatas.skill_2 = reader.GetInt32(15);
                        classDatas.skill_3 = reader.GetInt32(16);
                        classDatas.skill_4 = reader.GetInt32(17);
                        classDatas.skill_ult = reader.GetInt32(18);
                        GetData.Add(classDatas);
/*                        Debug.Log($"index: {classDatas.index}, class_name: {classDatas.class_name}, class_desc: {classDatas.class_desc}, " +
                      $"skill_1: {classDatas.skill_1}, skill_2: {classDatas.skill_2}, skill_3: {classDatas.skill_3}, " +
                      $"skill_4: {classDatas.skill_4}, skill_ult: {classDatas.skill_ult}");*/
                    }
                }
            }
            classDataArray = GetData.ToArray();
        }
        catch (Exception e)
        {
            Debug.Log("쿼리 오류: " + e.Message);
        }

        List<ClassDataString> GetDatas = new List<ClassDataString>();

        // 데이터베이스 항목과 문자열 데이터를 비교하여 변환
        for (int j = 0; j < classDataArray.Length; j++)
        {
            //Debug.Log("asd");
            ClassDataString classDataString = new ClassDataString();

            for (int i = 0; i < stringDataArray.Length; i++)
            {
                if (stringDataArray[i].index == classDataArray[j].class_name)
                {
                    classDataString.class_name = stringDataArray[i].stringInfo;
                    classDataString.index = classDataArray[j].index;
                }
                if (stringDataArray[i].index == classDataArray[j].class_desc)
                {
                    classDataString.class_desc = stringDataArray[i].stringInfo;

                }
            }
            // 리스트에 변환된 데이터 추가
            GetDatas.Add(classDataString);
        }

        // 배열로 변환하여 저장
        classDataStringArray = GetDatas.ToArray();

        //PrintAllClassData();
    }
}
