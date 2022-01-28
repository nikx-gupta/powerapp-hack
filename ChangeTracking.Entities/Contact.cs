using System;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;

namespace ChangeTracking.Entities
{
    [SqlTable(Name = "ccwis.Person")]
    [DataverseTable(Name = "contacts")]
    public class Contact : BaseEntity, ISqlTrackingEntity
    {
        [JsonProperty("ccwis_ssn")]
        public string SSN { get; set; }

        [JsonProperty("lastname")]
        public string SurName { get; set; }

        [JsonProperty("firstname")]
        public string GivenName { get; set; }

        [JsonProperty("middlename")]
        public string MiddleName { get; set; }

        [JsonProperty("namesuffix")]
        public string NameSuffix { get; set; }

        [JsonProperty("birthdate")]
        public string BirthDate { get; set; }

        [JsonProperty("ccwis_facs_id_nbr")]
        public string FACS_ID_NBR { get; set; }

        [JsonProperty("ccwis_sex")]
        public string Sex { get; set; }

        [JsonProperty("ccwis_ethnicity")]
        public string EthnicityCode { get; set; }

        [JsonProperty("ccwis_emailaddresstypeprimary")]
        public string EmailAddressPrimary { get; set; }

        [JsonProperty("emailaddress1")]
        [Write(false)]
        public string PrimaryEmail => EmailAddressPrimary;

        [JsonProperty("ccwis_emailaddresstypesecondary")]
        public string EmailAddressSecondary { get; set; }

        [JsonProperty("emailaddress2")]
        [Write(false)]
        public string SecondaryEmail => EmailAddressSecondary;

        [JsonProperty("modifiedon")]
        [SqlChangeTrackingModifiedDate]
        public DateTime ModifiedOn { get; set; }

        [Write(false)]
        public DateTime LastModifiedDate => ModifiedOn;
    }

    //public class PersonRace
    //{
    //    [JsonProperty("ccwis_ssn")]
    //    public string SSN { get; set; }

    //    [JsonProperty("lastname")]
    //    public string SurName { get; set; }

    //    [JsonProperty("firstname")]
    //    public string GivenName { get; set; }

    //    [JsonProperty("middlename")]
    //    public string MiddleName { get; set; }

    //    [JsonProperty("namesuffix")]
    //    public string NameSuffix { get; set; }

    //    [JsonProperty("birthdate")]
    //    public string BirthDate { get; set; }

    //    [JsonProperty("ccwis_facs_id_nbr")]
    //    public string FACS_ID_NBR { get; set; }

    //    [JsonProperty("ccwis_sex")]
    //    public string Sex { get; set; }

    //    [JsonProperty("ccwis_ethnicity")]
    //    public string EthnicityCode { get; set; }
    //}
}
