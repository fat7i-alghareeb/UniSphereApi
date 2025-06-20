using UniSphere.Api.Controllers;

namespace UniSphere.Api.Entities;

public class MultilingualText
{
    public string? En { get; set; }
    public string? Ar { get; set; }
    

    
    public string GetTranslatedString( Languages lang)
    {
        return lang == Languages.En ? En ?? "" : Ar ?? "";
    }
} 
