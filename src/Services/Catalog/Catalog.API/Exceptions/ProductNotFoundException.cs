using BuildingBlocks.Exceptions;

namespace Catalog.API.Exceptions;

    public class ProductNotFoundException : NotFoundException
{
    public ProductNotFoundException(object id)
       : base("Product", id)
    {
    }
}

