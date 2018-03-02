using System;
using Microsoft.AspNetCore.Mvc;

namespace MotorsportResultAPI.Domain.Workflow.Rally
{
    public class GetCompetitor : MotorsportResultAPI.Domain.Workflow.Rally.IGetCompetitor
    {
        private MotorsportResultAPI.Data.Rally.ICompetitorRepository c_competitorRepository;

        private readonly MotorsportResultAPI.Data.Rally.Mapper c_mapper;
        

        public GetCompetitor(
            MotorsportResultAPI.Data.Rally.ICompetitorRepository competitorRepository,
            MotorsportResultAPI.Data.Rally.Mapper mapper)
        {
            this.c_competitorRepository = competitorRepository;
            this.c_mapper = mapper;
        }


        public MotorsportResultAPI.Types.Domain.v1.Rally.Competitor Execute(
            string competitorId)
        {
            var _result = this.c_competitorRepository.FetchById(competitorId);

            return _result == null ? null : this.c_mapper.MapCompetitorToDomain(_result);
        }
    }
}