namespace Delivery.Core.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException() : base() { }

    public NotFoundException(string mensaje) : base(mensaje) { }

    public NotFoundException(string mensaje, Exception inner) : base(mensaje, inner) { }
}
