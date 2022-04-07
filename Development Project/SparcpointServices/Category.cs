using DBCore;
using SparcpointServices.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SparcpointServices
{
    public class Category : ICategory
    {
        private readonly SparcpointDbContext _sparcpointDbContext;
        public Category(SparcpointDbContext sparcpointDbContext)
        {
            _sparcpointDbContext = sparcpointDbContext;
        }
        public void Add(Domain.Entities.Category category)
        {
            throw new NotImplementedException();
        }
    }
}
