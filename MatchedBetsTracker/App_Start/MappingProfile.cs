using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using MatchedBetsTracker.Dtos;
using MatchedBetsTracker.Models;

namespace MatchedBetsTracker.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Mapper.CreateMap<Transaction, TransactionDto>();
            Mapper.CreateMap<TransactionDto, Transaction>();
        }
    }
}