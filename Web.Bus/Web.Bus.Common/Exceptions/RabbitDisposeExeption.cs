namespace Web.Bus.Common.Exceptions;

public sealed class RabbitDisposeExeption
{
    public static void ThrowIfDisposed(bool disposed, string name = "RabbitClient")
    {
        if (disposed)
        {
            throw new ObjectDisposedException(name);
        }
    }
}
