using System;

public interface ISavable
{
    public void Save(string directoryPath);

    public string Load(string directoryPath);
}