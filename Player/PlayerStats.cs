using FishNet.Serializing;
using UnityEngine;
public struct PlayerStats
{
    public int kills;
    public int deaths;
    public PlayerStats(int kills, int deaths)
    {
        this.kills = kills;
        this.deaths = deaths;
    }
    public static void Write(Writer writer, PlayerStats value)
    {
        writer.WriteInt32(value.kills);
        writer.WriteInt32(value.deaths);
    }
    public static PlayerStats Read(Reader reader)
    {
        return new PlayerStats(reader.ReadInt32(), reader.ReadInt32());
    }
}