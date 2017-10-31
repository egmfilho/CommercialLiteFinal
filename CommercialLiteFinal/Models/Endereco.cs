using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{
	public class Bairro
	{
		[JsonProperty("district_id")]
		public string Id
		{
			get;
			set;
		}

		[JsonProperty("district_code")]
		public string Codigo
		{
			get;
			set;
		}

		[JsonProperty("district_name")]
		public string Nome
		{
			get;
			set;
		}
	}

	public class Cidade
	{
		[JsonProperty("city_id")]
		public string Id
		{
			get;
			set;
		}

		[JsonProperty("uf_id")]
		public string UF
		{
			get;
			set;
		}

		[JsonProperty("city_code")]
		public string Codigo
		{
			get;
			set;
		}

		[JsonProperty("city_name")]
		public string Nome
		{
			get;
			set;
		}

		[JsonProperty("city_ibge")]
		public string IBGE
		{
			get;
			set;
		}

		[JsonProperty("city_ddd")]
		public string DDD
		{
			get;
			set;
		}
	}

	public class CEP
	{
		[JsonProperty("cep_code")]
		public string Codigo
		{
			get;
			set;
		}

		[JsonProperty("uf_id")]
		public string UF
		{
			get;
			set;
		}

		[JsonProperty("city_id")]
		public string IdCidade
		{
			get;
			set;
		}

		[JsonProperty("district_id")]
		public string IdBairro
		{
			get;
			set;
		}

		[JsonProperty("public_place")]
		public string Logradouro
		{
			get;
			set;
		}

		[JsonProperty("public_place_type")]
		public string Tipo
		{
			get;
			set;
		}

		[JsonProperty("district")]
		public Bairro Bairro
		{
			get;
			set;
		}

		[JsonProperty("city")]
		public Cidade Cidade
		{
			get;
			set;
		}
	}

	public class Endereco
	{
		[JsonProperty("person_id")]
		public string IdPessoa
		{
			get;
			set;
		}

		[JsonProperty("uf_id")]
		public string UF
		{
			get;
			set;
		}

		[JsonProperty("city_id")]
		public string IdCidade
		{
			get;
			set;
		}

		[JsonProperty("city")]
		public Cidade Cidade
		{
			get;
			set;
		}

		[JsonProperty("district_id")]
		public string IdBairro
		{
			get;
			set;
		}

		[JsonProperty("district")]
		public Bairro Bairro
		{
			get;
			set;
		}

		[JsonProperty("person_address_cep")]
		public string CEP
		{
			get;
			set;
		}

		[JsonProperty("person_address_code")]
		public string Codigo
		{
			get;
			set;
		}

		[JsonProperty("person_address_active")]
		public char Ativo
		{
			get;
			set;
		} = 'Y';

		[JsonProperty("person_address_main")]
		public char Principal
		{
			get;
			set;
		} = 'N';

		[JsonProperty("person_address_delivery")]
		public char Entrega
		{
			get;
			set;
		} = 'Y';

		[JsonProperty("person_address_ie")]
		public string InscricaoEstadual
		{
			get;
			set;
		}

		[JsonProperty("person_address_type")]
		public string Tipo
		{
			get;
			set;
		}

		[JsonProperty("person_address_public_place")]
		public string Logradouro
		{
			get;
			set;
		}

		[JsonProperty("person_address_number")]
		public string Numero
		{
			get;
			set;
		}

		[JsonProperty("person_address_note")]
		public string Observacoes
		{
			get;
			set;
		}

		[JsonProperty("person_address_reference")]
		public string Referencia
		{
			get;
			set;
		}

		[JsonProperty("icms_type")]
		public string TipoICMS
		{
			get;
			set;
		}

		[JsonProperty("person_address_contact")]
		public List<Contato> Contatos
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2}, {3} - {4} {5}", Tipo, Logradouro, Numero, Bairro.Nome, Cidade.Nome, UF);
		}
	}
}
