namespace Bomber.Domain.Common.Interfaces;

public interface IIterator<T>
{
    bool HasNext();

    T Next();

    void Reset();
}
