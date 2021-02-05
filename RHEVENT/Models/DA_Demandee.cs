using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RHEVENT.Models
{
    public class DA_Demandee
    {
        public DA_Demande dadem { get; set; }
        public DA_Budget dabudg { get; set; }
        public DA_Materiels_Dem damateriels { get; set; }
        public DA_ProduitsDem dabproduits { get; set; }
        
       
        public List<DA_Materiels_Dem> listemateriels { get; set; }
        
        public List<DA_Budget> listesbudget { get; set; }
        
        public List<DA_ProduitsDem> listeproduits { get; set; }

        [NotMapped]
        public List<DA_Fournisseurs> listefournisseurs { get; set; }

        [NotMapped]
        public List<DA_Labo> listelabo { get; set; }

        [NotMapped]
        public List<MotifsRejet> listeMotifs { get; set; }

    }
}