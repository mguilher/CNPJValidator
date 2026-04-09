

using CNPJValidator.Shared;

namespace CNPJValidator.Errors;

public static class Errors
{

    public static class Cnpj
    {
        public static readonly Error Empty = new(
            "Cnpj.Empty",
            "CNPJ is empty");
        public static readonly Error InvalidFormat = new(
            "Cnpj.InvalidFormat",
            "CNPJ format is invalid");
    }

   
}
