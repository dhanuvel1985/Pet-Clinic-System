using AutoMapper;
using ConsultationService.Application.Commands;
using ConsultationService.Application.DTOs;
using ConsultationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultationService.Application.Mappings
{
    public class ConsultationProfile : Profile
    {
        public ConsultationProfile()
        {
            CreateMap<CreateConsultationCommand, Consultation>();
            CreateMap<Consultation, ConsultationDto>();
        }
    }
}
