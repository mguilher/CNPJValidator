using CNPJValidator.Primitives;
using CNPJValidator.Shared;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using static CNPJValidator.Errors.Errors;

namespace CNPJValidator
{
    public sealed class Cnpj : ValueObject
    {
        private Cnpj(string value) => Value = value;

        public string Value { get; }

        public static Result<Cnpj> Create(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
            {
                return Result.Failure<Cnpj>(Errors.Errors.Cnpj.Empty);
            }

            if (!CnpjValidation.IsValidCNPJ(cnpj))
            {
                return Result.Failure<Cnpj>(Errors.Errors.Cnpj.InvalidFormat);
            }

            return Result.Success(new Cnpj(cnpj));
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
