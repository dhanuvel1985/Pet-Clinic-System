using AutoMapper;
using PetService.Application.Commands;
using PetService.Application.DTOs;
using PetService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Pet, PetDto>();
            CreateMap<CreatePetCommand, Pet>(); // if useful
        }
    }
}
