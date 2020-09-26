using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BuzzOn.UI.Data;

namespace BuzzOn.UI
{
    public partial class FormPerfilSaldo : PhoneApplicationPage
    {
        protected PerfilSaldo PerfilSaldo { get; set; }

        public FormPerfilSaldo()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string idPerfil = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("idPerfil", out idPerfil))
            {
                var id = Convert.ToInt32(idPerfil);
                this.PerfilSaldo = BuzzOnConnection.Instance.Get<PerfilSaldo>(id);
                this.txtNome.Text = this.PerfilSaldo.Nome;
                this.txtCt.Text = this.PerfilSaldo.NrCartao.ToString();
                this.txtCpf.Text = this.PerfilSaldo.CPF;
            }
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var btnAdd = new ApplicationBarIconButton(new Uri("/Assets/icons/save.png", UriKind.Relative));
            btnAdd.Text = "Salvar";
            ApplicationBar.Buttons.Add(btnAdd);
            btnAdd.Click += btnAdd_Click;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (ValidarEntrada())
            {
                if (PerfilSaldo == null)
                {
                    PerfilSaldo = new PerfilSaldo { Nome = txtNome.Text, CPF = txtCpf.Text, NrCartao = Convert.ToInt64(txtCt.Text) };
                    BuzzOnConnection.Instance.Insert<PerfilSaldo>(PerfilSaldo);
                }
                else
                {
                    if (txtCpf.Text.Equals(PerfilSaldo.CPF) == false || txtCt.Text.Equals(PerfilSaldo.NrCartao.ToString()) ==  false)
                    {
                        var count = BuzzOnConnection.Instance.DeleteAll<HistoricoSaldo>(i => i.IdPerfilSaldo == PerfilSaldo.Id);
                    }

                    PerfilSaldo.Nome = txtNome.Text;
                    PerfilSaldo.CPF = txtCpf.Text;
                    PerfilSaldo.NrCartao = Convert.ToInt64(txtCt.Text);
                    PerfilSaldo.DtUltimaAtualizacao = null;
                    PerfilSaldo.UltimoSaldo = null;

                    BuzzOnConnection.Instance.Update<PerfilSaldo>(PerfilSaldo);
                    App.PerfisSaldoViewModel.CarregarColecaoDeDadosDaBase();
                }
                NavigationService.GoBack();
            }
        }

        private bool ValidarEntrada()
        {
            if (String.IsNullOrEmpty(txtNome.Text))
            {
                MessageBox.Show("Informe o nome do perfil...");
                return false;
            }

            if (String.IsNullOrEmpty(txtCpf.Text))
            {
                MessageBox.Show("Informe o CPF...");
                return false;
            }

            if (txtCpf.Text.Length < 11 || ValidaCPF(txtCpf.Text) == false)
            {
                MessageBox.Show("CPF inválido...");
                return false;
            }

            if (String.IsNullOrEmpty(txtCt.Text))
            {
                MessageBox.Show("Informe o número do cartão transporte...");
                return false;
            }

            var perfilExistente = BuzzOnConnection.Instance.Get<PerfilSaldo>(i => i.NrCartao == Convert.ToInt64(txtCt.Text) && i.CPF == txtCpf.Text);
            if (perfilExistente != null)
            {
                MessageBox.Show("Já existe um perfil com estes dados...");
                return false;
            }

            if (PerfilSaldo != null && (txtCpf.Text.Equals(PerfilSaldo.CPF) == false || txtCt.Text.Equals(PerfilSaldo.NrCartao.ToString()) == false))
            {
                var result = MessageBox.Show("Alterar o CPF ou Nº da Carteira URBS deste perfil irá fazer com que seu histórico seja apagado. Deseja continuar?", "Atenção", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        private bool ValidaCPF(string cpf)
        {
            var digito1 = cpf[0];
            var repetido = true;
            for (int i = 1; i < 11; i++)
            {
                if (digito1 != cpf[i])
                    repetido = false;
            }
            if (repetido)
                return false;

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;
            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }
    }
}