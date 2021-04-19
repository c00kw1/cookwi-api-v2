using System.Collections.Generic;
using Cookwi.Common.Models;

namespace Cookwi.Api.Models.Tribes
{
    public class TribeResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LogoPath { get; set; }
        public List<TribeMemberDto> Members { get; set; }
        public List<string> Recipes { get; set; }

        public TribeResponse()
        {
            Name = "";
            LogoPath = "";
            Members = new List<TribeMemberDto>();
            Recipes = new List<string>();
        }
    }

    public class TribeMemberDto
    {
        public string Id { get; set; }
        public Gender Gender { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public TribeAccess Access { get; set; }
    }
}
