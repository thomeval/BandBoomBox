using System.Collections.Generic;
using Unity.Netcode;

public class SectionResultDto : INetworkSerializable
{
    public ulong NetId;
    public int PlayerSlot;
    public double SectionAccuracy;
    public SectionJudgeResult JudgeResult;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref NetId);
        serializer.SerializeValue(ref PlayerSlot);
        serializer.SerializeValue(ref SectionAccuracy);
        serializer.SerializeValue(ref JudgeResult);
    }
}

public class SectionResultSetDto : INetworkSerializable
{
    public SectionResultDto[] SectionResults;
    public int SectionIndex;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref SectionResults);
    }
}