using System;

public interface ISavable
{
    public void Save(string directoryPath) => throw new NotImplementedException();

    public void Load() => throw new NotImplementedException();
}