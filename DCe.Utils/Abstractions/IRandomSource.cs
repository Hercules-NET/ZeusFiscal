using System;
namespace DCe.Utils.Abstractions;

/// <summary>
/// Fonte do código numérico aleatório que compõe a chave de acesso. O XSD exige 6 dígitos
/// aleatórios gerados pelo emitente; injetar permite congelar o valor em teste.
/// </summary>
public interface IRandomSource
{
    int ProximoCodigoDocumento();
}

public sealed class SecureRandomSource : IRandomSource
{
    public int ProximoCodigoDocumento()
    {
        // Amostragem por rejeição evita viés e funciona também no .NET Framework.
        using (var gerador = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            var bytes = new byte[4];
            uint valor;
            do { gerador.GetBytes(bytes); valor = BitConverter.ToUInt32(bytes, 0); }
            while (valor >= uint.MaxValue - (uint.MaxValue % 1_000_000));
            return (int)(valor % 1_000_000);
        }
    }
}
