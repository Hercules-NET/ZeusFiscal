using System;

namespace NFe.Classes.Informacoes
{
    public class autXML
    {
        private const string ErroCpfCnpjPreenchidos = "Somente preencher um dos campos: CNPJ ou CPF, para um objeto do tipo autXML!";
        private string cnpj;
        private string cpf;

        /// <summary>
        ///     GA02 - CNPJ Autorizado
        /// </summary>
        public string CNPJ
        {
            get { return cnpj; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                if (!string.IsNullOrEmpty(cpf))
                    throw new ArgumentException(ErroCpfCnpjPreenchidos);

                cnpj = value;
            }
        }

        /// <summary>
        ///     GA03 - CPF Autorizado
        /// </summary>
        public string CPF
        {
            get { return cpf; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                if (!string.IsNullOrEmpty(cnpj))
                    throw new ArgumentException(ErroCpfCnpjPreenchidos);
    
                cpf = value;
            }
        }
    }
}