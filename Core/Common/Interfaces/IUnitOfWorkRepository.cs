using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IUnitOfWorkRepository
    {
        IVillaRepository Villa { get; }
        IAmenityRepository Amenity { get; }
        IVillaNumberRepository VillaNumber { get; }
        void Save();
    }
}
