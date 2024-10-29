                    },

                    VeicTracao = new MDFeVeicTracao
                    {
                        Placa = "KKK9888",
                        RENAVAM = "888888888",
                        UF = Estado.GO,
                        Tara = 222,
                        CapM3 = 222,
                        CapKG = 22,
                        Condutor = new List<MDFeCondutor>
                        {
                            new MDFeCondutor
                            {
                                CPF = "11392381754",
                                XNome = "Ricardão"
                            }
                        },
                        TpRod = MDFeTpRod.Outros,
                        TpCar = MDFeTpCar.NaoAplicavel
                    },

                    lacRodo = new List<MDFeLacre>
                    {
                        new MDFeLacre
                        {
                            NLacre = "lacre01"
                        }
                    }
                };
            }

            #endregion modal

            #region infMunDescarga
            mdfe.InfMDFe.InfDoc.InfMunDescarga = new List<MDFeInfMunDescarga>
            {
                new MDFeInfMunDescarga
                {
                    XMunDescarga = "CUIABA",
                    CMunDescarga = "5103403",
                    InfCTe = new List<MDFeInfCTe>
                    {
                        new MDFeInfCTe
                        {
                            ChCTe = "52161021351378000100577500000000191194518006"
                        }
                    }
                }
            };


            if (MDFeConfiguracao.VersaoWebService.VersaoLayout == VersaoServico.Versao300)
            {
                mdfe.InfMDFe.InfDoc.InfMunDescarga[0].InfCTe[0].Peri = new List<MDFePeri>
                {
                    new MDFePeri
                    {
                        NONU = "1111",
                        QTotProd = "quantidade 20"
                    }
                };
            }

            #endregion infMunDescarga

            #region seg

            if (MDFeConfiguracao.VersaoWebService.VersaoLayout == VersaoServico.Versao300)
            {
                mdfe.InfMDFe.Seg = new List<MDFeSeg>();

                mdfe.InfMDFe.Seg.Add(new MDFeSeg
                {
                    InfResp = new MDFeInfResp
                    {
                        CNPJ = "21025760000123",
                        RespSeg = MDFeRespSeg.EmitenteDoMDFe
                    },
                    InfSeg = new MDFeInfSeg
                    {
                        CNPJ = "21025760000123",
                        XSeg = "aaaaaaaaaa"
                    },
                    NApol = "aaaaaaaaaa",
                    NAver = new List<string>
                        {
                            "aaaaaaaa"
                        }
                });
            }

            #endregion

            #region Produto Predominante

            if (MDFeConfiguracao.VersaoWebService.VersaoLayout == VersaoServico.Versao300)
            {
                mdfe.InfMDFe.prodPred = new prodPred
                {
                    tpCarga = tpCarga.CargaGeral,
                    xProd = "aaaaaaaaaaaaaaaaaaaaa",
                    infLotacao = new infLotacao
                    {
                        infLocalCarrega = new infLocalCarrega
                        {
                            CEP = "75950000"
                        },
                        infLocalDescarrega = new infLocalDescarrega
                        {
                            CEP = "75950000"
                        }
                    }
                };
            }

            #endregion

            #region Totais (tot)
            mdfe.InfMDFe.Tot.QCTe = 1;
            mdfe.InfMDFe.Tot.vCarga = 500.00m;
            mdfe.InfMDFe.Tot.CUnid = MDFeCUnid.KG;
            mdfe.InfMDFe.Tot.QCarga = 100.0000m;
            #endregion Totais (tot)

            #region informações adicionais (infAdic)
            mdfe.InfMDFe.InfAdic = new MDFeInfAdic
            {
                InfCpl = "aaaaaaaaaaaaaaaa"
            };
            #endregion

            #region dados responsavel tecnico 

            mdfe.InfMDFe.infRespTec = new infRespTec
            {
                CNPJ = "21025760000123",
                email = "robertoalvespereira18@gmail.com",
                fone = "64981081602",
                xContato = "roberto alves"
            };
            #endregion  

            var servicoRecepcao = new ServicoMDFeRecepcao();

            var retornoEnvio = servicoRecepcao.MDFeRecepcaoSinc(mdfe);

            OnSucessoSync(new RetornoEEnvio(retornoEnvio));

            config.ConfigWebService.Numeracao++;
            new ConfiguracaoDao().SalvarConfiguracao(config);
        }

        private static int GetRandom()
        {
            var rand = new Random();
            return rand.Next(11111111, 99999999);
        }

        public void BuscarDiretorioSchema()
        {
            var dlg = new FolderBrowserDialog();
            dlg.ShowDialog();
            DiretorioSchemas = dlg.SelectedPath;
        }

        public void GerarESalvar()
        {
            var config = new ConfiguracaoDao().BuscarConfiguracao();
            CarregarConfiguracoesMDFe(config);
            var mdfe = new MDFeEletronico();

            #region (ide)
            mdfe.InfMDFe.Ide.CUF = config.ConfigWebService.UfEmitente;
            mdfe.InfMDFe.Ide.TpAmb = config.ConfigWebService.Ambiente;
            mdfe.InfMDFe.Ide.TpEmit = MDFeTipoEmitente.PrestadorServicoDeTransporte;
            mdfe.InfMDFe.Ide.Mod = ModeloDocumento.MDFe;
            mdfe.InfMDFe.Ide.Serie = 751;
            mdfe.InfMDFe.Ide.NMDF = ++config.ConfigWebService.Numeracao;
            mdfe.InfMDFe.Ide.CMDF = GetRandom();
            mdfe.InfMDFe.Ide.Modal = MDFeModal.Rodoviario;
            mdfe.InfMDFe.Ide.DhEmi = DateTime.Now;
            mdfe.InfMDFe.Ide.TpEmis = MDFeTipoEmissao.Normal;
            mdfe.InfMDFe.Ide.ProcEmi = MDFeIdentificacaoProcessoEmissao.EmissaoComAplicativoContribuinte;
            mdfe.InfMDFe.Ide.VerProc = "versao28383";
            mdfe.InfMDFe.Ide.UFIni = Estado.GO;
            mdfe.InfMDFe.Ide.UFFim = Estado.MT;


            mdfe.InfMDFe.Ide.InfMunCarrega.Add(new MDFeInfMunCarrega
            {
                CMunCarrega = "5211701",
                XMunCarrega = "JANDAIA"
            });

            mdfe.InfMDFe.Ide.InfMunCarrega.Add(new MDFeInfMunCarrega
            {
                CMunCarrega = "5209952",
                XMunCarrega = "INDIARA"
            });

            mdfe.InfMDFe.Ide.InfMunCarrega.Add(new MDFeInfMunCarrega
            {
                CMunCarrega = "5200134",
                XMunCarrega = "ACREUNA"
            });

            #endregion (ide)

            #region dados emitente (emit)
            mdfe.InfMDFe.Emit.CNPJ = config.Empresa.Cnpj;
            mdfe.InfMDFe.Emit.IE = config.Empresa.InscricaoEstadual;
            mdfe.InfMDFe.Emit.XNome = config.Empresa.Nome;
            mdfe.InfMDFe.Emit.XFant = config.Empresa.NomeFantasia;

            mdfe.InfMDFe.Emit.EnderEmit.XLgr = config.Empresa.Logradouro;
            mdfe.InfMDFe.Emit.EnderEmit.Nro = config.Empresa.Numero;
            mdfe.InfMDFe.Emit.EnderEmit.XCpl = config.Empresa.Complemento;
            mdfe.InfMDFe.Emit.EnderEmit.XBairro = config.Empresa.Bairro;
            mdfe.InfMDFe.Emit.EnderEmit.CMun = config.Empresa.CodigoIbgeMunicipio;
            mdfe.InfMDFe.Emit.EnderEmit.XMun = config.Empresa.NomeMunicipio;
            mdfe.InfMDFe.Emit.EnderEmit.CEP = long.Parse(config.Empresa.Cep);
            mdfe.InfMDFe.Emit.EnderEmit.UF = config.Empresa.SiglaUf;
            mdfe.InfMDFe.Emit.EnderEmit.Fone = config.Empresa.Telefone;
            mdfe.InfMDFe.Emit.EnderEmit.Email = config.Empresa.Email;
            #endregion dados emitente (emit)

            #region modal
            if (MDFeConfiguracao.VersaoWebService.VersaoLayout == VersaoServico.Versao100)
            {
                mdfe.InfMDFe.InfModal.VersaoModal = MDFeVersaoModal.Versao100;
                mdfe.InfMDFe.InfModal.Modal = new MDFeRodo
                {
                    RNTRC = config.Empresa.RNTRC,
                    VeicTracao = new MDFeVeicTracao
                    {
                        Placa = "KKK9888",
                        RENAVAM = "888888888",
                        UF = Estado.GO,
                        Tara = 222,
                        CapM3 = 222,
                        CapKG = 22,
                        Condutor = new List<MDFeCondutor>
                    {
                        new MDFeCondutor
                        {
                            CPF = "11392381754",
                            XNome = "Ricardão"
                        }
                    },
                        TpRod = MDFeTpRod.Outros,
                        TpCar = MDFeTpCar.NaoAplicavel
                    }
                };
            }


            if (MDFeConfiguracao.VersaoWebService.VersaoLayout == VersaoServico.Versao300)
            {
                mdfe.InfMDFe.InfModal.VersaoModal = MDFeVersaoModal.Versao300;
                mdfe.InfMDFe.InfModal.Modal = new MDFeRodo
                {
                    infANTT = new MDFeInfANTT
                    {
                        RNTRC = config.Empresa.RNTRC,

                        // não é obrigatorio
                        infCIOT = new List<infCIOT>
                        {
                            new infCIOT
                            {
                                CIOT = "123456789123",
                                CNPJ = "21025760000123"
                            }
                        },
                        valePed = new MDFeValePed
                        {
                            Disp = new List<MDFeDisp>
                                    {
                                        new MDFeDisp
                                        {
                                            CNPJForn = "21025760000123",
                                            CNPJPg = "21025760000123",
                                            NCompra = "838388383",
                                            vValePed = 100.33m
                                        }
                                    }
                        }
                    },

                    VeicTracao = new MDFeVeicTracao
                    {
                        Placa = "KKK9888",
                        RENAVAM = "888888888",
                        UF = Estado.GO,
                        Tara = 222,
                        CapM3 = 222,
                        CapKG = 22,
                        Condutor = new List<MDFeCondutor>
                        {
                            new MDFeCondutor
                            {
                                CPF = "11392381754",
                                XNome = "Ricardão"
                            }
                        },
                        TpRod = MDFeTpRod.Outros,
                        TpCar = MDFeTpCar.NaoAplicavel
                    },

                    lacRodo = new List<MDFeLacre>
                    {
                        new MDFeLacre
                        {
                            NLacre = "lacre01"
                        }
                    }

                };
            }

            #endregion modal

            #region infMunDescarga
            mdfe.InfMDFe.InfDoc.InfMunDescarga = new List<MDFeInfMunDescarga>
            {
                new MDFeInfMunDescarga
                {
                    XMunDescarga = "CUIABA",
                    CMunDescarga = "5103403",
                    InfCTe = new List<MDFeInfCTe>
                    {
                        new MDFeInfCTe
                        {
                            ChCTe = "52161021351378000100577500000000191194518006"
                        }
                    }
                }
            };


            if (MDFeConfiguracao.VersaoWebService.VersaoLayout == VersaoServico.Versao300)
            {
                mdfe.InfMDFe.InfDoc.InfMunDescarga[0].InfCTe[0].Peri = new List<MDFePeri>
                {
                    new MDFePeri
                    {
                        NONU = "1111",
                        QTotProd = "quantidade 20"
                    }
                };
            }

            #endregion infMunDescarga

            #region seg

            if (MDFeConfiguracao.VersaoWebService.VersaoLayout == VersaoServico.Versao300)
            {
                mdfe.InfMDFe.Seg = new List<MDFeSeg>();

                mdfe.InfMDFe.Seg.Add(new MDFeSeg
                {
                    InfResp = new MDFeInfResp
                    {
                        CNPJ = "21025760000123",
                        RespSeg = MDFeRespSeg.EmitenteDoMDFe
                    },
                    InfSeg = new MDFeInfSeg
                    {
                        CNPJ = "21025760000123",
                        XSeg = "aaaaaaaaaa"
                    },
                    NApol = "aaaaaaaaaa",
                    NAver = new List<string>
                        {
                            "aaaaaaaa"
                        }
                });
            }

            #endregion

            #region Totais (tot)
            mdfe.InfMDFe.Tot.QCTe = 1;
            mdfe.InfMDFe.Tot.vCarga = 500.00m;
            mdfe.InfMDFe.Tot.CUnid = MDFeCUnid.KG;
            mdfe.InfMDFe.Tot.QCarga = 100.0000m;
            #endregion Totais (tot)

            #region informações adicionais (infAdic)
            mdfe.InfMDFe.InfAdic = new MDFeInfAdic
            {
                InfCpl = "aaaaaaaaaaaaaaaa"
            };
            #endregion

            mdfe = mdfe.Assina();
            mdfe = mdfe.Valida();

            mdfe.SalvarXmlEmDisco();
        }

        public void BuscarDiretorioSalvarXml()
        {
            var dlg = new FolderBrowserDialog();
            dlg.ShowDialog();
            DiretorioSalvarXml = dlg.SelectedPath;
        }

        public void ConsultaPorRecibo()
        {
            var config = new ConfiguracaoDao().BuscarConfiguracao();
            CarregarConfiguracoesMDFe(config);

            var recibo = InputBoxTuche("Digite o recibo");

            if (string.IsNullOrEmpty(recibo))
            {
                MessageBoxTuche("Recibo inválido");
                return;
            }

            var servicoRecibo = new ServicoMDFeRetRecepcao();
            var retorno = servicoRecibo.MDFeRetRecepcao(recibo);

            OnSucessoSync(new RetornoEEnvio(retorno));
        }

        public void ConsultaPorProtocolo()
        {
            var porChave = MessageBoxConfirmTuche("Sim = Por chave\nNão = Por arquivo xml");
            var chave = string.Empty;


            if (porChave == DialogResult.Yes)
            {
                chave = InputBoxTuche("Digite a chave de acesso da MDF-e");
            }

            if (porChave == DialogResult.No)
            {
                chave = BuscarChaveMDFe();
            }

            if (string.IsNullOrEmpty(chave))
            {
                MessageBoxTuche("Ops.. Não a oque fazer sem uma chave de acesso");
                return;
            }

            var config = new ConfiguracaoDao().BuscarConfiguracao();
            CarregarConfiguracoesMDFe(config);

            var servicoConsultaProtocolo = new ServicoMDFeConsultaProtocolo();
            var retorno = servicoConsultaProtocolo.MDFeConsultaProtocolo(chave);


            OnSucessoSync(new RetornoEEnvio(retorno));
        }

        private string BuscarChaveMDFe()
        {
            var chave = string.Empty;
            var caminhoArquivoXml = BuscarArquivoXmlMDFe();

            if (caminhoArquivoXml != null)
            {
                try
                {
                    var enviMDFe = MDFeEnviMDFe.LoadXmlArquivo(caminhoArquivoXml);

                    chave = enviMDFe.MDFe.Chave();
                }
                catch
                {
                    try
                    {
                        chave = MDFeEletronico.LoadXmlArquivo(caminhoArquivoXml).Chave();
                    }
                    catch
                    {
                        var proc = FuncoesXml.ArquivoXmlParaClasse<MDFeProcMDFe>(caminhoArquivoXml);
                        chave = proc.MDFe.Chave();
                    }
                }

            }
            return chave;
        }

        public string BuscarArquivoXmlMDFe()
        {
            var janelaArquivo = new OpenFileDialog
            {
                Filter = "XML(*.xml)|*.xml"
            };

            if (janelaArquivo.ShowDialog() == true)
            {
                var caminhoXml = janelaArquivo.FileName;

                if (caminhoXml == null) return string.Empty;

                return caminhoXml;
            }
            return string.Empty;
        }

        private static string InputBoxTuche(string titulo)
        {
            var inputBox = new InputBoxWindow
            {
                TxtValor = { Text = string.Empty },
                TxtDescricao = { Text = titulo }
            };
            inputBox.ShowDialog();

            var valor = inputBox.TxtValor.Text;

            return valor;
        }

        private static DialogResult MessageBoxConfirmTuche(string mensagem)
        {
            return MessageBox.Show(mensagem, @"MDF-e", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        private static void MessageBoxTuche(string mensagem, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            MessageBox.Show(mensagem, @"MDF-e", MessageBoxButtons.OK, icon);
        }

        public void ConsultaStatusServico()
        {
            var config = new ConfiguracaoDao().BuscarConfiguracao();
            CarregarConfiguracoesMDFe(config);

            var servicoStatusServico = new ServicoMDFeStatusServico();
            var retorno = servicoStatusServico.MDFeStatusServico();

            OnSucessoSync(new RetornoEEnvio(retorno));

        }

        public void ConsultaNaoEncerrados()
        {
            var config = new ConfiguracaoDao().BuscarConfiguracao();
            CarregarConfiguracoesMDFe(config);

            var servicoConsultaNaoEncerrados = new ServicoMDFeConsultaNaoEncerrados();
            var retorno = servicoConsultaNaoEncerrados.MDFeConsultaNaoEncerrados(config.Empresa.Cnpj);

            OnSucessoSync(new RetornoEEnvio(retorno));
        }

        public void EventoIncluirCondutor()
        {
            var config = new ConfiguracaoDao().BuscarConfiguracao();
            CarregarConfiguracoesMDFe(config);


            var evento = new ServicoMDFeEvento();

            MDFeEletronico mdfe;
            var caminhoXml = BuscarArquivoXmlMDFe();

            try
            {
                var enviMDFe = MDFeEnviMDFe.LoadXmlArquivo(caminhoXml);

                mdfe = enviMDFe.MDFe;
            }
            catch
            {
                try
                {
                    mdfe = MDFeEletronico.LoadXmlArquivo(caminhoXml);
                }
                catch
                {
                    var proc = FuncoesXml.ArquivoXmlParaClasse<MDFeProcMDFe>(caminhoXml);
                    mdfe = proc.MDFe;
                }
            }

            var nomeCondutor = InputBoxTuche("Nome condutor");
            var cpfCondutor = InputBoxTuche("Cpf condutor");

            if (string.IsNullOrEmpty(nomeCondutor))
            {
                MessageBoxTuche("Nome do condutor não pode ser vazio ou nulo");
                return;
            }

            if (string.IsNullOrEmpty(cpfCondutor))
            {
                MessageBoxTuche("CPF do condutor não pode ser vazio ou nulo");
                return;
            }

            var retorno = evento.MDFeEventoIncluirCondutor(mdfe, 1, nomeCondutor, cpfCondutor);

            OnSucessoSync(new RetornoEEnvio(retorno));
        }

        public void EventoEncerramento()
        {
            var config = new ConfiguracaoDao().BuscarConfiguracao();
            CarregarConfiguracoesMDFe(config);

            MDFeEletronico mdfe;
            var caminhoXml = BuscarArquivoXmlMDFe();

            try
            {
                var enviMDFe = MDFeEnviMDFe.LoadXmlArquivo(caminhoXml);

                mdfe = enviMDFe.MDFe;
            }
            catch
            {
                try
                {
                    mdfe = MDFeEletronico.LoadXmlArquivo(caminhoXml);
                }
                catch
                {
                    var proc = FuncoesXml.ArquivoXmlParaClasse<MDFeProcMDFe>(caminhoXml);
                    mdfe = proc.MDFe;
                }
            }

            var evento = new ServicoMDFeEvento();

            var protocolo = InputBoxTuche("Digite um protocolo");

            if (string.IsNullOrEmpty(protocolo))
            {
                MessageBoxTuche("O protocolo não pode ser vazio ou nulo");
                return;
            }

            var retorno = evento.MDFeEventoEncerramentoMDFeEventoEncerramento(mdfe, 1, protocolo);

            OnSucessoSync(new RetornoEEnvio(retorno));
        }

        public void EventoCancelar()
        {
            var config = new ConfiguracaoDao().BuscarConfiguracao();
            CarregarConfiguracoesMDFe(config);

            var evento = new ServicoMDFeEvento();

            MDFeEletronico mdfe;
            var caminhoXml = BuscarArquivoXmlMDFe();

            try
            {
                var enviMDFe = MDFeEnviMDFe.LoadXmlArquivo(caminhoXml);

                mdfe = enviMDFe.MDFe;
            }
            catch
            {
                try
                {
                    mdfe = MDFeEletronico.LoadXmlArquivo(caminhoXml);
                }
                catch
                {
                    var proc = FuncoesXml.ArquivoXmlParaClasse<MDFeProcMDFe>(caminhoXml);
                    mdfe = proc.MDFe;
                }
            }

            var protocolo = InputBoxTuche("Digite um protocolo");

            if (string.IsNullOrEmpty(protocolo))
            {
                MessageBoxTuche("O protocolo não pode ser vazio ou nulo");
                return;
            }

            var justificativa = InputBoxTuche("Digite uma justificativa (minimo 15 digitos)");

            if (string.IsNullOrEmpty(justificativa))
            {
                MessageBoxTuche("A justificativa não pode ser vazio ou nulo");
                return;
            }

            var retorno = evento.MDFeEventoCancelar(mdfe, 1, protocolo, justificativa);

            OnSucessoSync(new RetornoEEnvio(retorno));
        }

        private static void CarregarConfiguracoesMDFe(Configuracao config)
        {
            var configuracaoCertificado = new ConfiguracaoCertificado
            {
                TipoCertificado = TipoCertificado.A1Repositorio,
                Serial = config.CertificadoDigital.NumeroDeSerie,
                Senha = config.CertificadoDigital.Senha,
                //Arquivo = config.CertificadoDigital.CaminhoArquivo,
                ManterDadosEmCache = config.CertificadoDigital.ManterEmCache,
            };

            MDFeConfiguracao.ConfiguracaoCertificado = configuracaoCertificado;
            MDFeConfiguracao.CaminhoSchemas = config.ConfigWebService.CaminhoSchemas;
            MDFeConfiguracao.CaminhoSalvarXml = config.DiretorioSalvarXml;
            MDFeConfiguracao.IsSalvarXml = config.IsSalvarXml;

            MDFeConfiguracao.VersaoWebService.VersaoLayout = config.ConfigWebService.VersaoLayout;

            MDFeConfiguracao.VersaoWebService.TipoAmbiente = config.ConfigWebService.Ambiente;
            MDFeConfiguracao.VersaoWebService.UfEmitente = config.ConfigWebService.UfEmitente;
            MDFeConfiguracao.VersaoWebService.TimeOut = config.ConfigWebService.TimeOut;
            MDFeConfiguracao.IsAdicionaQrCode = true;
        }

        protected virtual void OnSucessoSync(RetornoEEnvio e)
        {
            if (SucessoSync == null) return;

            SucessoSync.Invoke(this, e);
        }

        public void EventoIncluirDFe()
        {
            var config = new ConfiguracaoDao().BuscarConfiguracao();
            CarregarConfiguracoesMDFe(config);

            var evento = new ServicoMDFeEvento();
            MDFeEletronico mdfe;
            var caminhoXml = BuscarArquivoXmlMDFe();
            try
            {
                var enviMDFe = MDFeEnviMDFe.LoadXmlArquivo(caminhoXml);
                mdfe = enviMDFe.MDFe;
            }
            catch
            {
                try
                {
                    mdfe = MDFeEletronico.LoadXmlArquivo(caminhoXml);
                }
                catch
                {
                    var proc = FuncoesXml.ArquivoXmlParaClasse<MDFeProcMDFe>(caminhoXml);
                    mdfe = proc.MDFe;
                }
            }
            var protocolo = InputBoxTuche("Protocolo");
            var codigoMunicipioCarregamento = InputBoxTuche("Código do Município de Carregamento");
            var nomeMunicipioCarregamento = InputBoxTuche("Nome do Município de Carregamento");
            var cmunDescarga = InputBoxTuche("Código do Município de Descarga");
            var xmunDescarga = InputBoxTuche("Nome do Município de Descarga");
            var chNFe = InputBoxTuche("Chave da NFe");
            if (string.IsNullOrEmpty(codigoMunicipioCarregamento))
            {
                MessageBoxTuche("Código do Município de Carregamento não pode ser vazio ou nulo");
                return;
            }
            if (string.IsNullOrEmpty(nomeMunicipioCarregamento))
            {
                MessageBoxTuche("Nome do Município de Carregamento não pode ser vazio ou nulo");
                return;
            }
            if (string.IsNullOrEmpty(cmunDescarga))
            {
                MessageBoxTuche("Nome do Município de Descarga não pode ser vazio ou nulo");
                return;
            }
            if (string.IsNullOrEmpty(xmunDescarga))
            {
                MessageBoxTuche("Nome do Município de Descarga não pode ser vazio ou nulo");
                return;
            }
            if (string.IsNullOrEmpty(chNFe))
            {
                MessageBoxTuche("Chave NFe não pode ser vazio ou nulo");
                return;
            }
            var informacoesDocumentos = new List<MDFeInfDocInc>
            {
                new MDFeInfDocInc
                {
                    CMunDescarga = cmunDescarga,
                    XMunDescarga = xmunDescarga,
                    ChNFe = chNFe
                }
            };
            var retorno = evento.MDFeEventoIncluirDFe(mdfe, 1, protocolo, codigoMunicipioCarregamento, nomeMunicipioCarregamento, informacoesDocumentos);
            OnSucessoSync(new RetornoEEnvio(retorno));
        }
    }
}