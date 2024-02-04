using System.Collections.Generic;

//used to serialize list of lists, so that you can have List<NestedList<T>> and be serialized in inspector.
[System.Serializable]
public class NestedList<T>
{
    public List<T> data;
}
