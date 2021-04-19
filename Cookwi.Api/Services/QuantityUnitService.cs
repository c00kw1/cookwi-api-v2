using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cookwi.Api.Models.Recipes;
using Cookwi.Db;

namespace Cookwi.Api.Services
{
    public interface IQuantityUnitService
    {
        IEnumerable<QuantityUnitDto> GetAll();
    }

    public class QuantityUnitService : IQuantityUnitService
    {
        private CookwiContext _ctx;
        private IMapper _mapper;

        public QuantityUnitService(CookwiContext ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public IEnumerable<QuantityUnitDto> GetAll()
        {
            return _mapper.Map<QuantityUnitDto[]>(_ctx.QuantityUnits.ToArray());
        }
    }
}
