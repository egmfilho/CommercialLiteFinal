using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{
	public class Pessoa
	{
		[JsonProperty("person_id")]
		public string Id { get; set; }

		[JsonProperty("person_code")]
		public string Codigo { get; set; }

		[JsonProperty("person_name")]
		public string Nome { get; set; }

		[JsonProperty("person_type")]
		public string Tipo { get; set; }

		[JsonProperty("person_cpf")]
		public string CPF { get; set; }

		[JsonProperty("person_cnpj")]
		public string CNPJ { get; set; }

		[JsonProperty("person_active")]
		public char Ativo { get; set; } = 'Y';

		[JsonProperty("person_address")]
		public List<Endereco> Enderecos;


		//public string Origem { get; set; }
		//public DateTime Cadastro { get; set; }
		//public string IdCep { get; set; }
		//public Cep Cep { get; set; }
	}
}
