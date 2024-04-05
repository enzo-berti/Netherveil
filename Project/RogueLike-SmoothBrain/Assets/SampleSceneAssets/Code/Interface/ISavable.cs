using System;

public interface ISavable
{
    public void Save();

    public static string Load(string data) => throw new NotImplementedException();
}