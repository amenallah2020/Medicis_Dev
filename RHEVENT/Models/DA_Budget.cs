using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace RHEVENT.Models
{
    public class DA_Budget : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        public string Réference { get; set; }

        [Required]
        [Display(Name = "Article")]
        public string Article { get; set; }

        [Display(Name = "Commentaire")]
        public string Description { get; set; }


        [Required]
        [Display(Name = "Prix Unitaire")]
        public float PrixUnitaire { get; set; }

        [Required]
        public int Quantité { get; set; }

        public float Total { get; set; }

        public string Fournisseur { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [Display(Name = "Date Réception Souhaitée")]
        public DateTime? Date_Recp_Souh { get; set; }

        //[DataType(DataType.Date)]
        //[Column(TypeName = "date")]
        [Display(Name = "Date Réception")]
        public string Date_Recp { get; set; }

        [Display(Name = "Type")]
        //[Required]
        public string Type { get; set; }


        [Display(Name = "Plafond Budget")]
        public string PlafondBudget { get; set; }


        //[Display(Name = "Plafond Budget")]
        //[Required]
        //public string PlafondBudget { get; set; }

        //public string AL { get; set; }
        [NotMapped]
        public List<DA_Budget> listesbudget { get; set; }

        [NotMapped]
        public List<DA_Fournisseurs> listesfournisseurs { get; set; }

        [NotMapped]
        public List<DA_Materiels> listeMateriels { get; set; }

        [NotMapped]
        public List<DA_Materiels> listeServices { get; set; }


        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            SqlConnection con = new SqlConnection(constr);

            con.Open();

            SqlDataAdapter da7 = new SqlDataAdapter("SELECT Date_reception FROM DA_Demande where Réference ='" + HttpContext.Current.Session["reff"].ToString() + "'", con);
            DataTable dt7 = new DataTable();
            da7.Fill(dt7);
            con.Close();
            DateTime daterecep = Convert.ToDateTime(dt7.Rows[0][0].ToString());

            List<ValidationResult> validationResult = new List<ValidationResult>();
            var validateName = Date_Recp_Souh <= daterecep;
            HttpContext.Current.Session["checkboxx"] = "0";
            if (validateName != true)
            {
                ValidationResult errorMessage = new ValidationResult
                ("La date de reception souhaitée de l'article doit etre inferieure ou égale à celle de reception souhaitée de la demande.", new[] { "Date_Recp_Souh" });
                validationResult.Add(errorMessage);
                HttpContext.Current.Session["checkboxx"] = "1";
                return validationResult;
            }

            else
            {
                return validationResult;
            }


        }
    }
}