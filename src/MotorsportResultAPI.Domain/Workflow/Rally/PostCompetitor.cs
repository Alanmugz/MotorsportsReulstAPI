using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MotorsportResultAPI.Domain.Workflow.Rally
{
    public class PostCompetitor : MotorsportResultAPI.Domain.Workflow.Rally.IPostCompetitor
    {
        private MotorsportResultAPI.Data.Rally.ICompetitorRepository c_competitorRepository;

        private readonly MotorsportResultAPI.Data.Rally.Mapper c_mapper;
        

        public PostCompetitor(
            MotorsportResultAPI.Data.Rally.ICompetitorRepository competitorRepository,
            MotorsportResultAPI.Data.Rally.Mapper mapper)
        {
            this.c_competitorRepository = competitorRepository;
            this.c_mapper = mapper;
        }


        public (MotorsportResultAPI.Types.Domain.v1.Rally.Competitor, MotorsportResultAPI.Types.Enumeration.Results) Execute(
            MotorsportResultAPI.Types.Domain.v1.Rally.Competitor subject)
        {
			var _competitor = this.c_competitorRepository.FetchById(subject.Id);

			if (_competitor == null)
			{
				var _dataCompetitor = this.c_mapper.MapCompetitorToData(subject);
				this.c_competitorRepository.Save(_dataCompetitor);
				
				return (subject, MotorsportResultAPI.Types.Enumeration.Results.Created);
			}

			return (subject, MotorsportResultAPI.Types.Enumeration.Results.AlreadyExists);
        }
    }
}