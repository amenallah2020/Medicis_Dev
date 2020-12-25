using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RHEVENT.Models
{
    public class Dal
    {
        ApplicationDbContext bdd;
        public Dal()
        {
            bdd = new ApplicationDbContext();
        }
        public List<ApplicationUser> ObtientTousUtilisateurs()
        {
            return bdd.Users.ToList();
        }
        public List<Attestation> ObtientToutAttestations()
        {
            return bdd.Attestations.ToList();
        }
        public IEnumerable<SelectListItem> GetSignataires()
        {
            var signataires = bdd.Users.ToList().Select(x => new SelectListItem
            {
                Value = x.signataire.ToString(),
                Text = x.signataire.ToString()
            });
            return new SelectList(signataires, "Value", "Text");
        }


        public List<Attestation> ObtainUserAttestation(string UserId)
        {
            return bdd.Attestations.ToList();
        }

        public List<Autorisation> ObtainToutAutorisations()
        {
            return bdd.Autorisations.ToList();
        }
      
    }
}