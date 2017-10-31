using System;
using Newtonsoft.Json;

namespace CommercialLiteFinal
{
	public class Contato
	{
		[JsonProperty("person_address_code")]
		public string CdEndereco
		{
			get;
			set;
		}

		[JsonProperty("person_address_contact_type_id")]
		public string IdTipo
		{
			get;
			set;
		}

		[JsonProperty("person_address_contact_main")]
		public char Principal
		{
			get;
			set;
		} = 'N';

		[JsonProperty("person_address_contact_name")]
		public string Nome
		{
			get;
			set;
		}

		[JsonProperty("person_address_contact_note")]
		public string Observacoes
		{
			get;
			set;
		}

		[JsonProperty("person_address_contact_label")]
		public string Label
		{
			get;
			set;
		}

		[JsonProperty("person_address_contact_value")]
		public string Valor
		{
			get;
			set;
		}
	}
}
