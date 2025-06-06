﻿using AutoMapper;
using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.ModelViews.RankModelViews;

namespace BeautySpa.Services.Mapper 
{
    public class RankMapping : Profile
    {
        public RankMapping()
        {
            CreateMap<Rank, GETRankModelView>().ReverseMap();
            CreateMap<POSTRankModelView, Rank>();
            CreateMap<PUTRankModelView, Rank>();
        }
    }
}
