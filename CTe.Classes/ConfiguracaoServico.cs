using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CTe.Classes.Informacoes.Tipos;
using CTe.Classes.Servicos.Tipos;
using DFe.Classes.Entidades;
using DFe.Classes.Flags;
using DFe.Utils;
using DFe.Utils.Assinatura;

namespace CTe.Classes
{
    public sealed class ConfiguracaoServico : IDisposable
    {
        private static volatile ConfiguracaoServico _instancia;
        private static readonly object SyncRoot = new object();
        private string _diretorioSchemas;
        private bool _unZip = true;

        public ConfiguracaoServico()
        {
            ConfiguracaoCertificado = new ConfiguracaoCertificado();
            TipoEmissao = tpEmis.teNormal;
            IsValidaSchemas = true;
        }

        /// <summary>
        ///     Configurações relativas ao Certificado Digital
        /// </summary>
        public ConfiguracaoCertificado ConfiguracaoCertificado { get; set; }

        private X509Certificate2 _certificado = null;
        public X509Certificate2 X509Certificate2
        {
            get
            {
                if (this._certificado != null)
                    if (!this.ConfiguracaoCertificado.ManterDadosEmCache)
                        this._certificado.Reset();
                _certificado = ObterCertificado();
                return _certificado;
            }
        }

        private X509Certificate2 ObterCertificado()
        {
            return CertificadoDigital.ObterCertificado(ConfiguracaoCertificado);
        }

        public void Dispose()
        {
            if (!this.ConfiguracaoCertificado.ManterDadosEmCache && _certificado != null)
            {
                _certificado.Reset();
                _certificado = null;
            }
        }

        ~ConfiguracaoServico()
        {
            if (!this.ConfiguracaoCertificado.ManterDadosEmCache && _certificado != null)
            {
                _certificado.Reset();
                _certificado = null;
            }
        }

        /// <summary>
        ///     Tempo máximo de espera pela resposta do webservice, em milisegundos
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        ///     Estado de destino do webservice
        /// </summary>
        public Estado cUF { get; set; }

        /// <summary>
        ///     Tipo de ambiente do webservice (Produção, Homologação)
        /// </summary>
        public TipoAmbiente tpAmb { get; set; }

        /// <summary>
        ///     Versão do layout
        /// </summary>
        public versao VersaoLayout { get; set; }

        public versao ObterVersaoLayoutValida()
        {
            switch (VersaoLayout)
            {
                case versao.ve200:
                    return versao.ve200;
                case versao.ve300:
                    return versao.ve300;
                case versao.ve400:
                    return versao.ve400;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool IsAdicionaQrCode { get; set; }

        /// <summary>
        ///     Diretório onde estão aramazenados os schemas para validação
        /// </summary>
        public string DiretorioSchemas
        {
            get { return _diretorioSchemas; }
            set
            {
                if (!string.IsNullOrEmpty(value) && !Directory.Exists(value))
                    throw new Exception("Diretório " + value + " não encontrado!");
                _diretorioSchemas = value;
            }
        }

        /// <summary>
        ///     Informar se a biblioteca deve salvar o xml de envio e de retorno
        /// </summary>
        public bool IsSalvarXml { get; set; }

        /// <summary>
        ///     Diretório onde os xmls de envio/retorno devem ser salvos
        /// </summary>
        public string DiretorioSalvarXml { get; set; }
        
        /// <summary>
        /// Valor True, será descompactado os arquivos,
        /// Valor False, os valor não será descompactado e a classes não serão preenchidas
        /// </summary>
        public bool UnZip
        {
            get
            {
                return _unZip;
            }
            set
            {
                _unZip = value;
            }
        }

        /// <summary>
        ///     Instância do Singleton de ConfiguracaoServico
        /// </summary>
        public static ConfiguracaoServico Instancia
        {
            get
            {
                if (_instancia != null) return _instancia;
                lock (SyncRoot)
                {
                    if (_instancia != null) return _instancia;
                    _instancia = new ConfiguracaoServico();
                }

                return _instancia;
            }
        }

        public tpEmis TipoEmissao { get; set; }

        public bool IsValidaSchemas { get; set; }

        public bool NaoSalvarXml()
        {
            return !IsSalvarXml;
        }
    }
}