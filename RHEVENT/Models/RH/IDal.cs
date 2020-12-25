using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RHEVENT.Models
{
    interface IDal
    {
        List<ApplicationUser> ObtientTousUtilisateurs();
        List<Attestation> ObtientToutAttestations();
        List<Attestation> ObtainUserAttestation();
        List<Autorisation> ObtainToutAutorisations();
    }
}
