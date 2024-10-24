public class AlimentDto
{
    public string nom_francais { get; set; } = string.Empty;
    public string groupe { get; set; } = string.Empty;
    public string ciqual_code { get; set; } = string.Empty;

    public ImpactEnvironnementalAlimentDto impact_environnemental { get; set; } = new ImpactEnvironnementalAlimentDto();
 }

public class ImpactEnvironnementalAlimentDto
{
    public ChangementClimatiqueDto changement_climatique { get; set; } = new ChangementClimatiqueDto();
    public EpuisementEauDto epuisement_eau { get; set; } = new EpuisementEauDto();
}

public class ChangementClimatiqueDto {
    public double synthese { get; set; }
    public string unite { get; set; } = string.Empty;
}

public class EpuisementEauDto {
    public double synthese { get; set; }
    public string unite { get; set; } = string.Empty;
}