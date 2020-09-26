using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BuzzOn.UI.Model;
using BuzzOn.UI.Data;
using System.Net.NetworkInformation;
using BuzzOn.UI.Resources;
using Newtonsoft.Json.Linq;
using System.Text;

namespace BuzzOn.UI
{
    public partial class HistoricoSaldoURBS : PhoneApplicationPage
    {
        private PerfilSaldoModel Perfil;

        public HistoricoSaldoURBS()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
            ApplicationBar.IsVisible = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string idPerfil = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("idPerfil", out idPerfil))
            {
                var id = Convert.ToInt32(idPerfil);
                var perfilSelecionado = BuzzOnConnection.Instance.Get<PerfilSaldo>(id);
                this.Perfil = new PerfilSaldoModel(perfilSelecionado);

                this.DataContext = this.Perfil;
            }
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBarIconButton appBarButtonUpdate = new ApplicationBarIconButton(new Uri("/Assets/icons/sync.png", UriKind.Relative));
            appBarButtonUpdate.Text = "Atualizar";
            appBarButtonUpdate.Click += appBarButtonUpdate_Click;

            ApplicationBarIconButton appBarButtonInfo = new ApplicationBarIconButton(new Uri("/Assets/icons/questionmark.png", UriKind.Relative));
            appBarButtonInfo.Text = "Informações";
            appBarButtonInfo.Click += (sender, evt) =>
            {
                StringBuilder sbInfo = new StringBuilder();
                sbInfo.AppendLine("\n- Segundo a URBS, a atualização dessas informações pode acontecer em até 48 horas.\n");
                sbInfo.AppendLine("- Cada consulta apresenta o extrato das últimas 12 utilizações do seu cartão. Caso queira um extrato completo, procure um dos Postos de Atendimento da URBS.\n");
                sbInfo.AppendLine("- Seguindo a nossa Política de Privacidade, nós não mantemos seus dados armazenados conosco. Todos os dados informados aqui, permanecem no seu aparelho.\n");
                sbInfo.AppendLine("Caso tenha alguma dúvida, entre em contato conosco pelo e-mail contato@buzzon.com.br, ou deixe uma mensagem pelo formulário Fale Conosco.\n");

                MessageBox.Show(sbInfo.ToString(), "Informações importantes", MessageBoxButton.OK);
            };

            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = true;
            ApplicationBar.Buttons.Add(appBarButtonUpdate);
            ApplicationBar.Buttons.Add(appBarButtonInfo);
        }

        protected void appBarButtonUpdate_Click(object sender, EventArgs e)
        {
            ApplicationBar.IsVisible = false;
            ucLoading.Show("Por favor, aguarde...\n\nEsta consulta demora alguns segundos.");
            bool estaConectado = NetworkInterface.GetIsNetworkAvailable();
            if (estaConectado)
            {
                var perfilOriginal = BuzzOnConnection.Instance.Get<PerfilSaldo>(i => i.Id == this.Perfil.Id);
                String Url = WebApiResources.Saldo.Replace("{ct}", perfilOriginal.NrCartao.ToString()).Replace("{cpf}", perfilOriginal.CPF.ToString()).Replace("{token}", App.UserId);

                WebClient clientAtualizacaoItinerario = new WebClient();
                clientAtualizacaoItinerario.Encoding = Encoding.UTF8;
                clientAtualizacaoItinerario.DownloadStringCompleted += (snd, evt) =>
                {
                    if (evt.Error != null)
                    {
                        ucLoading.Hide();
                        ucToastMessage.ShowToast("Desculpe, não foi possível completar esta solicitação.\nVerifique seus dados e tente novamente.");
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(evt.Result) == false)
                        {
                            try
                            {
                                var jSaldo = Newtonsoft.Json.JsonConvert.DeserializeObject<ViewModels.HistoricoSaldoViewModel>(evt.Result);

                                if ("NOP".Equals(jSaldo.result.ToUpper()))
                                {
                                    ucLoading.Hide();
                                    ucToastMessage.ShowToast(jSaldo.msg);
                                }
                                else if ("OK".Equals(jSaldo.result.ToUpper()))
                                {
                                    this.Perfil.LimparHistorico();
                                    var historico = jSaldo.extrato.Select(i => new HistoricoSaldo { DtConsumo = Convert.ToDateTime(i.data), IdPerfilSaldo = this.Perfil.Id, Local = i.local, ValorConsumo = Convert.ToDecimal(i.valor) }).ToList();
                                    historico.ForEach(i => this.Perfil.AddHistorico(i));

                                    var ultimoSaldo = jSaldo.extrato.OrderByDescending(i => i.data).First().saldo;
                                    this.Perfil.UltimoSaldo = "Último Saldo: R$ " + ultimoSaldo;
                                    this.Perfil.DtUltimaAtualizacao = "Atualização: " + DateTime.Now.ToString("dd/MM/yyyy");

                                    var conn = BuzzOnConnection.Instance;
                                    var pSaldo = conn.Get<PerfilSaldo>(i => i.Id == this.Perfil.Id);
                                    pSaldo.DtUltimaAtualizacao = DateTime.Now;
                                    pSaldo.UltimoSaldo = Convert.ToDecimal(ultimoSaldo);
                                    conn.Update<PerfilSaldo>(pSaldo);

                                    conn.DeleteAll<HistoricoSaldo>(i => i.IdPerfilSaldo == pSaldo.Id);
                                    conn.InsertAll(historico);
                                }
                                else
                                {
                                    throw new Exception();
                                }

                                ucLoading.Hide();
                            }
                            catch (Exception)
                            {
                                ucLoading.Hide();
                                ucToastMessage.ShowToast("Desculpe, não foi possível completar esta solicitação.");
                            }
                        }
                        else
                        {
                            ucLoading.Hide();
                            ucToastMessage.ShowToast("Ocorreu um problema com a sua solicitação.\nPor favor, tente novamente.");
                        }
                    }
                    ApplicationBar.IsVisible = true;
                };
                clientAtualizacaoItinerario.DownloadStringAsync(new Uri(Url));
            }
            else
            {
                ucLoading.Hide();
                ucToastMessage.ShowToast("Você precisa estar conectado à internet para executar esta operação...", 4);
            }
        }

        private void LongListSelector_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationBar.IsVisible = true;
            ucLoading.Hide();
        }

        private void LongListSelector_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (this.Perfil.Historico.Any())
            {
                txtVazio.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}