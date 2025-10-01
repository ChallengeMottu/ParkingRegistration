namespace PulseSystem.Domain.Entities;

public class Address
{
    public string Street { get; set; }
    public string Complement { get; set; }
    public string Neighborhood { get; set; }
    public string Cep { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    
    public void ValidaCep(string cep)
    {
        if (string.IsNullOrEmpty(cep) || cep.Length != 8) throw new ArgumentException("CEP inválido");
    }
}