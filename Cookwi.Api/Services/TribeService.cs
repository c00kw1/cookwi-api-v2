using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cookwi.Api.Models.Tribes;
using Cookwi.Common.Exceptions;
using Cookwi.Db;
using Microsoft.EntityFrameworkCore;

namespace Cookwi.Api.Services
{
    public interface ITribeService
    {
        IEnumerable<TribeResponse> GetAll(int userId);
    }

    public class TribeService : ITribeService
    {
        private CookwiContext _ctx;
        private IMapper _mapper;

        public TribeService(CookwiContext ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public IEnumerable<TribeResponse> GetAll(int userId)
        {
            var account = _ctx.Accounts.Include(a => a.Tribes).FirstOrDefault(a => a.Id == userId);

            if (account == null) throw new NotFoundException($"Account with id {userId} cannot be found");

            return _mapper.Map<List<TribeResponse>>(account.Tribes);
        }
    }
}
