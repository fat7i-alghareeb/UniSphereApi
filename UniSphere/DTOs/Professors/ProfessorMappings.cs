using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Professors;

public static class ProfessorMappings
{
    public static ProfessorUpdateDto ToUpdateDto(this Professor professor)
    {
        return new ProfessorUpdateDto
        {
            Gmail = professor.Gmail,
            FirstNameEn = professor.FirstName.En,
            FirstNameAr = professor.FirstName.Ar,
            LastNameEn = professor.LastName.En,
            LastNameAr = professor.LastName.Ar,
            BioEn = professor.Bio.En,
            BioAr = professor.Bio.Ar,
            Image = professor.Image
        };
    }

    public static void PatchFromDto(this Professor professor, ProfessorUpdateDto dto)
    {
        if (dto.Gmail is not null){
            professor.Gmail = dto.Gmail;
        } 
        if (dto.FirstNameEn is not null){
            professor.FirstName.En = dto.FirstNameEn;
        }
        if (dto.FirstNameAr is not null){
            professor.FirstName.Ar = dto.FirstNameAr;
        }
        if (dto.LastNameEn is not null){
            professor.LastName.En = dto.LastNameEn;
        }
        if (dto.LastNameAr is not null){
            professor.LastName.Ar = dto.LastNameAr;
        }
        if (dto.BioEn is not null){
            professor.Bio.En = dto.BioEn;
        }
        if (dto.BioAr is not null){
            professor.Bio.Ar = dto.BioAr;
        }
        if (dto.Image is not null){
            professor.Image = dto.Image;
        }
    }
} 