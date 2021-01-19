using System;
using System.Collections;
using System.Data.Common;

public class CachedDailyMessageDataReader : DbDataReader
{
    private readonly int _id;
    private readonly string _message;
    private bool _read;

    public CachedDailyMessageDataReader(int id, string message)
    {
        _id = id;
        _message = message;
    }

    public override int FieldCount
        => throw new NotImplementedException();

    public override int RecordsAffected
        => 0;

    public override bool HasRows
        => throw new NotImplementedException();

    public override bool IsClosed
        => throw new NotImplementedException();

    public override int Depth
        => throw new NotImplementedException();

    public override bool Read()
        => _read = !_read;

    public override int GetInt32(int ordinal)
        => _id;

    public override bool IsDBNull(int ordinal)
        => false;

    public override string GetString(int ordinal)
        => _message;

    public override bool GetBoolean(int ordinal)
        => throw new NotImplementedException();

    public override byte GetByte(int ordinal)
        => throw new NotImplementedException();

    public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        => throw new NotImplementedException();

    public override char GetChar(int ordinal)
        => throw new NotImplementedException();

    public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        => throw new NotImplementedException();

    public override string GetDataTypeName(int ordinal)
        => throw new NotImplementedException();

    public override DateTime GetDateTime(int ordinal)
        => throw new NotImplementedException();

    public override decimal GetDecimal(int ordinal)
        => throw new NotImplementedException();

    public override double GetDouble(int ordinal)
        => throw new NotImplementedException();

    public override Type GetFieldType(int ordinal)
        => throw new NotImplementedException();

    public override float GetFloat(int ordinal)
        => throw new NotImplementedException();

    public override Guid GetGuid(int ordinal)
        => throw new NotImplementedException();

    public override short GetInt16(int ordinal)
        => throw new NotImplementedException();

    public override long GetInt64(int ordinal)
        => throw new NotImplementedException();

    public override string GetName(int ordinal)
        => throw new NotImplementedException();

    public override int GetOrdinal(string name)
        => throw new NotImplementedException();

    public override object GetValue(int ordinal)
        => throw new NotImplementedException();

    public override int GetValues(object[] values)
        => throw new NotImplementedException();

    public override object this[int ordinal]
        => throw new NotImplementedException();

    public override object this[string name]
        => throw new NotImplementedException();

    public override bool NextResult()
        => throw new NotImplementedException();

    public override IEnumerator GetEnumerator()
        => throw new NotImplementedException();
}
