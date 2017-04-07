using System;
namespace CommercialLite
{
	public class Pessoa
	{
		public string Id { get; set; }
		public string IdLoja { get; set; }
		public string Codigo { get; set; }
		public string Nome { get; set; }
		public string Tp { get; set; }
		public string Doc { get; set; }
		public string IEstadual { get; set; }
		public string Telefone { get; set; }
		public string Email { get; set; }
		public bool Ativo { get; set; }
		public string Origem { get; set; }
		//public DateTime Cadastro { get; set; }
		public string IdCep { get; set; }
		public Cep Cep { get; set; }

		public Pessoa()
		{
		}
	}
}
